using System.Collections.Concurrent;
using System.Globalization;
using System.Text.Json;
using System.Threading.Channels;

namespace Hoaii.Web.Services.Translation;

/// <summary>
/// Tra cứu tiếng Việt -> tiếng Anh. Cache nằm trên đĩa, được <see cref="TranslationWorker"/>
/// bổ sung dần ở nền bằng Gemini.
///
/// Điểm mấu chốt: <see cref="Localize"/> KHÔNG BAO GIỜ gọi mạng và không bao giờ chặn
/// luồng. Gặp chuỗi chưa dịch thì nó trả nguyên tiếng Việt rồi đẩy chuỗi đó vào hàng
/// đợi; lượt xem sau mới có tiếng Anh. Đây là đánh đổi có chủ ý: thà chậm một nhịp
/// còn hơn để mỗi lượt xem trang phụ thuộc vào một lần gọi API bên ngoài.
///
/// Cache là một file JSON trong App_Data chứ chưa phải bảng trong DB, vì thêm bảng là
/// phải chạy EF migration. Khi nào cần cho admin sửa tay từng bản dịch thì chuyển sang DB.
/// </summary>
public sealed class GeminiTranslator
{
    public const string VietnameseCulture = "vi";
    public const string EnglishCulture = "en";

    private readonly ConcurrentDictionary<string, string> _cache = new(StringComparer.Ordinal);
    private readonly ConcurrentDictionary<string, byte> _queued = new(StringComparer.Ordinal);
    private readonly Channel<string> _pending = Channel.CreateUnbounded<string>();
    private readonly SemaphoreSlim _writeLock = new(1, 1);
    private readonly string _cachePath;
    private readonly ILogger<GeminiTranslator> _log;

    public GeminiTranslator(IWebHostEnvironment env, ILogger<GeminiTranslator> log)
    {
        _log = log;
        var dir = Path.Combine(env.ContentRootPath, "App_Data");
        Directory.CreateDirectory(dir);
        _cachePath = Path.Combine(dir, "translations.en.json");
        LoadFromDisk();
    }

    /// <summary>Luồng hiện tại đang phục vụ tiếng Anh hay không.</summary>
    public static bool IsEnglish => string.Equals(
        CultureInfo.CurrentUICulture.TwoLetterISOLanguageName,
        EnglishCulture,
        StringComparison.OrdinalIgnoreCase);

    public ChannelReader<string> Pending => _pending.Reader;

    public int CachedCount => _cache.Count;

    /// <summary>
    /// Trả về bản tiếng Anh nếu đang ở chế độ tiếng Anh và cache đã có; ngược lại trả
    /// nguyên văn tiếng Việt.
    /// </summary>
    public string Localize(string? text)
    {
        if (string.IsNullOrWhiteSpace(text)) return text ?? string.Empty;
        if (!IsEnglish) return text;

        var key = text.Trim();
        if (_cache.TryGetValue(key, out var english) && !string.IsNullOrWhiteSpace(english))
        {
            return english;
        }

        // TryAdd trả false nếu chuỗi đã nằm trong hàng đợi — tránh nhét cùng một chuỗi hàng
        // chục lần khi nó xuất hiện lặp lại trên một trang (ví dụ tên danh mục trong mega menu).
        if (_queued.TryAdd(key, 0))
        {
            _pending.Writer.TryWrite(key);
        }

        return text;
    }

    /// <summary>Ghi các cặp vừa dịch xong vào cache và lưu xuống đĩa.</summary>
    public async Task StoreAsync(IReadOnlyDictionary<string, string> pairs, CancellationToken ct)
    {
        if (pairs.Count == 0) return;

        foreach (var pair in pairs)
        {
            if (string.IsNullOrWhiteSpace(pair.Value)) continue;
            _cache[pair.Key] = pair.Value;
            _queued.TryRemove(pair.Key, out _);
        }

        await SaveToDiskAsync(ct);
    }

    /// <summary>
    /// Thả các chuỗi dịch hỏng ra khỏi hàng đợi để lần render sau còn thử lại được.
    /// </summary>
    public void Release(IEnumerable<string> keys)
    {
        foreach (var key in keys) _queued.TryRemove(key, out _);
    }

    private void LoadFromDisk()
    {
        if (!File.Exists(_cachePath)) return;

        try
        {
            var json = File.ReadAllText(_cachePath);
            var stored = JsonSerializer.Deserialize<Dictionary<string, string>>(json);
            if (stored is null) return;

            foreach (var pair in stored) _cache[pair.Key] = pair.Value;
            _log.LogInformation("Đã nạp {Count} bản dịch tiếng Anh từ cache.", _cache.Count);
        }
        catch (Exception ex)
        {
            // Cache hỏng không được phép làm app không khởi động được — cùng lắm là dịch lại.
            _log.LogWarning(ex, "Không đọc được cache bản dịch tại {Path}, bỏ qua.", _cachePath);
        }
    }

    private async Task SaveToDiskAsync(CancellationToken ct)
    {
        await _writeLock.WaitAsync(ct);
        try
        {
            var snapshot = _cache.ToDictionary(p => p.Key, p => p.Value, StringComparer.Ordinal);
            var json = JsonSerializer.Serialize(snapshot, new JsonSerializerOptions
            {
                WriteIndented = true,
                // Không escape tiếng Việt thành \u1ee7... để còn mở file ra đọc và sửa tay được.
                Encoder = System.Text.Encodings.Web.JavaScriptEncoder.Create(
                    System.Text.Unicode.UnicodeRanges.All),
            });

            // Ghi ra file tạm rồi thay thế, để tắt app giữa chừng không để lại file JSON cụt.
            var temp = _cachePath + ".tmp";
            await File.WriteAllTextAsync(temp, json, ct);
            File.Move(temp, _cachePath, overwrite: true);
        }
        catch (Exception ex)
        {
            _log.LogWarning(ex, "Không ghi được cache bản dịch xuống {Path}.", _cachePath);
        }
        finally
        {
            _writeLock.Release();
        }
    }
}
