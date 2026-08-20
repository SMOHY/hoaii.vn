namespace Hoaii.Domain.Entities;

/// <summary>
/// Tài liệu PDF cho đại lý / cộng tác viên (chính sách hoa hồng theo mùa, biểu mẫu...).
/// Tách khỏi MediaAsset vì đây là tệp để TẢI VỀ, có tiêu đề và mùa vụ riêng, không phải ảnh
/// để chèn vào nội dung.
/// </summary>
public class PartnerDocument
{
    public int Id { get; set; }

    public required string Title { get; set; }

    /// <summary>Đường dẫn công khai, ví dụ /uploads/tai-lieu/2026/{guid}.pdf.</summary>
    public required string FileUrl { get; set; }

    /// <summary>Tên tệp gốc, để người tải về nhận được đúng tên chứ không phải chuỗi guid.</summary>
    public required string FileName { get; set; }

    /// <summary>Nhãn mùa vụ tự do: "Trung thu 2026", "Tết 2027". Rỗng thì không hiện nhãn.</summary>
    public string? Season { get; set; }

    public long SizeBytes { get; set; }

    /// <summary>Ẩn khỏi trang Đối tác mà không phải xoá tệp — mùa cũ thường cần giữ lại.</summary>
    public bool IsPublished { get; set; } = true;

    public int SortOrder { get; set; }

    public DateTime CreatedAt { get; set; }
}
