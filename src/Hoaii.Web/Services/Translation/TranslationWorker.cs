using System.Text;
using System.Text.Json;

namespace Hoaii.Web.Services.Translation;

/// <summary>
/// Worker nền: rút chuỗi tiếng Việt từ hàng đợi của <see cref="GeminiTranslator"/>, gom thành
/// lô rồi nhờ Gemini dịch một lần cho cả lô.
///
/// Gom lô không chỉ để tiết kiệm số lần gọi. Dịch cả cụm cùng lúc thì model nhìn được
/// ngữ cảnh chung nên thuật ngữ đồng nhất hơn là dịch lẻ từng chuỗi một.
/// </summary>
public sealed class TranslationWorker : BackgroundService
{
    private const string GeminiEndpointPrefix =
        "https://generativelanguage.googleapis.com/v1beta/models/";

    private const int BatchSize = 40;

    // Chờ một nhịp sau khi có chuỗi đầu tiên, cho những chuỗi còn lại của cùng trang đó kịp
    // rơi vào hàng đợi, để cả trang được dịch trong cùng một lần gọi.
    private static readonly TimeSpan GatherWindow = TimeSpan.FromSeconds(2);
    private static readonly TimeSpan BackoffAfterError = TimeSpan.FromSeconds(30);

    private readonly GeminiTranslator _translator;
    private readonly IHttpClientFactory _httpFactory;
    private readonly IConfiguration _config;
    private readonly ILogger<TranslationWorker> _log;

    public TranslationWorker(
        GeminiTranslator translator,
        IHttpClientFactory httpFactory,
        IConfiguration config,
        ILogger<TranslationWorker> log)
    {
        _translator = translator;
        _httpFactory = httpFactory;
        _config = config;
        _log = log;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var apiKey = _config["Gemini:ApiKey"];
        if (string.IsNullOrWhiteSpace(apiKey))
        {
            // Không có khóa thì im lặng bỏ qua chứ không ném lỗi: máy dev và CI không cần
            // khóa Gemini vẫn phải chạy được web, chỉ là nút EN sẽ hiển thị tiếng Việt.
            _log.LogWarning(
                "Chưa cấu hình Gemini:ApiKey — chế độ tiếng Anh sẽ hiển thị nguyên tiếng Việt. "
                + "Đặt khóa bằng: dotnet user-secrets set \"Gemini:ApiKey\" \"...\"");
            return;
        }

        var model = _config["Gemini:Model"];
        if (string.IsNullOrWhiteSpace(model)) model = "gemini-2.0-flash";

        while (!stoppingToken.IsCancellationRequested)
        {
            List<string> batch;
            try
            {
                batch = await DrainAsync(stoppingToken);
            }
            catch (OperationCanceledException)
            {
                break;
            }

            if (batch.Count == 0) continue;

            try
            {
                var translated = await CallGeminiAsync(batch, apiKey, model, stoppingToken);
                if (translated.Count > 0)
                {
                    await _translator.StoreAsync(translated, stoppingToken);
                    _log.LogInformation("Đã dịch {Done}/{Total} chuỗi sang tiếng Anh.",
                        translated.Count, batch.Count);
                }
                else
                {
                    _translator.Release(batch);
                }
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                break;
            }
            catch (Exception ex)
            {
                // Thả lô ra để lần render sau xếp hàng lại, rồi nghỉ một lát — không thì gặp
                // lỗi hạn mức sẽ quay vòng gọi liên tục.
                _log.LogError(ex, "Gọi Gemini thất bại cho lô {Count} chuỗi.", batch.Count);
                _translator.Release(batch);
                try { await Task.Delay(BackoffAfterError, stoppingToken); }
                catch (OperationCanceledException) { break; }
            }
        }
    }

    private async Task<List<string>> DrainAsync(CancellationToken ct)
    {
        var first = await _translator.Pending.ReadAsync(ct);
        var batch = new List<string> { first };

        await Task.Delay(GatherWindow, ct);

        while (batch.Count < BatchSize && _translator.Pending.TryRead(out var next))
        {
            batch.Add(next);
        }

        return batch;
    }

    private async Task<Dictionary<string, string>> CallGeminiAsync(
        List<string> batch, string apiKey, string model, CancellationToken ct)
    {
        var prompt = BuildPrompt(batch);

        var payload = new
        {
            contents = new[]
            {
                new { parts = new[] { new { text = prompt } } },
            },
            generationConfig = new
            {
                // Nhiệt độ 0: cùng một chuỗi phải luôn ra cùng một bản dịch.
                temperature = 0.0,
                responseMimeType = "application/json",
            },
        };

        var client = _httpFactory.CreateClient();
        client.Timeout = TimeSpan.FromSeconds(90);

        // Nối chuỗi thường chứ không dùng chuỗi nội suy: URL này chỉ có đúng một chỗ thay đổi,
        // và viết nội suy đã từng làm lọt cặp ngoặc nhọn thừa vào giữa URL.
        var url = GeminiEndpointPrefix + model + ":generateContent";

        using var request = new HttpRequestMessage(HttpMethod.Post, url)
        {
            Content = new StringContent(
                JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json"),
        };
        // Khóa đi qua header chứ không nhét vào query string, để nó không rơi vào log truy cập.
        request.Headers.Add("x-goog-api-key", apiKey);

        using var response = await client.SendAsync(request, ct);
        var body = await response.Content.ReadAsStringAsync(ct);

        if (!response.IsSuccessStatusCode)
        {
            throw new HttpRequestException(
                $"Gemini trả về {(int)response.StatusCode}: {Truncate(body, 500)}");
        }

        return ParseResponse(body, batch);
    }

    private static string BuildPrompt(List<string> batch)
    {
        var sb = new StringBuilder();
        sb.AppendLine("You are translating UI text for a Vietnamese premium gift-hamper store.");
        sb.AppendLine();
        sb.AppendLine("Rules:");
        sb.AppendLine("- Translate each Vietnamese string into natural, concise English.");
        sb.AppendLine("- Keep the tone warm and premium; this is retail copy, not documentation.");
        sb.AppendLine("- Do NOT translate the brand name \"Hoài\". Leave it exactly as written.");
        sb.AppendLine("- Keep proper nouns for Vietnamese festivals recognisable, e.g.");
        sb.AppendLine("  \"Quà Tết\" -> \"Lunar New Year Gifts\",");
        sb.AppendLine("  \"Quà Trung Thu\" -> \"Mid-Autumn Gifts\".");
        sb.AppendLine("- Preserve any placeholder such as {0} or {name} exactly.");
        sb.AppendLine("- Preserve leading/trailing punctuation and casing style (ALL CAPS stays ALL CAPS).");
        sb.AppendLine("- Do not add explanations, quotes or commentary.");
        sb.AppendLine();
        sb.AppendLine("Return ONLY a JSON array of strings, same length and same order as the input.");
        sb.AppendLine();
        sb.AppendLine("Input:");
        sb.AppendLine(JsonSerializer.Serialize(batch, new JsonSerializerOptions
        {
            Encoder = System.Text.Encodings.Web.JavaScriptEncoder.Create(
                System.Text.Unicode.UnicodeRanges.All),
        }));

        return sb.ToString();
    }

    private Dictionary<string, string> ParseResponse(string body, List<string> batch)
    {
        var result = new Dictionary<string, string>(StringComparer.Ordinal);

        using var document = JsonDocument.Parse(body);
        if (!document.RootElement.TryGetProperty("candidates", out var candidates)
            || candidates.GetArrayLength() == 0)
        {
            _log.LogWarning("Gemini trả về phản hồi không có candidates: {Body}", Truncate(body, 500));
            return result;
        }

        var text = candidates[0]
            .GetProperty("content")
            .GetProperty("parts")[0]
            .GetProperty("text")
            .GetString();

        if (string.IsNullOrWhiteSpace(text)) return result;

        string[]? translations;
        try
        {
            translations = JsonSerializer.Deserialize<string[]>(StripCodeFence(text));
        }
        catch (JsonException ex)
        {
            _log.LogWarning(ex, "Không đọc được mảng JSON Gemini trả về: {Text}", Truncate(text, 500));
            return result;
        }

        if (translations is null) return result;

        // Model đôi khi trả thiếu hoặc thừa phần tử. Ghép theo chỉ số và bỏ phần lệch, còn hơn
        // vứt toàn bộ lô; những chuỗi không được ghép sẽ tự xếp hàng lại ở lần render sau.
        if (translations.Length != batch.Count)
        {
            _log.LogWarning(
                "Gemini trả về {Got} bản dịch cho {Want} chuỗi — chỉ nhận phần khớp được.",
                translations.Length, batch.Count);
        }

        var count = Math.Min(translations.Length, batch.Count);
        for (var i = 0; i < count; i++)
        {
            var english = translations[i];
            if (string.IsNullOrWhiteSpace(english)) continue;
            result[batch[i]] = english.Trim();
        }

        return result;
    }

    /// <summary>Gỡ rào ```json nếu model vẫn bọc dù đã yêu cầu JSON thuần.</summary>
    private static string StripCodeFence(string text)
    {
        var trimmed = text.Trim();
        if (!trimmed.StartsWith("```", StringComparison.Ordinal)) return trimmed;

        var firstLineBreak = trimmed.IndexOf('\n');
        if (firstLineBreak < 0) return trimmed;

        trimmed = trimmed[(firstLineBreak + 1)..];
        var closing = trimmed.LastIndexOf("```", StringComparison.Ordinal);
        return closing >= 0 ? trimmed[..closing].Trim() : trimmed.Trim();
    }

    private static string Truncate(string value, int max) =>
        value.Length <= max ? value : value[..max] + "...";
}
