namespace Hoaii.Domain.Entities;

/// <summary>
/// Ảnh phụ của một ô "Dịch vụ theo yêu cầu" ở trang chủ. Ảnh chính vẫn là
/// <see cref="HomeServiceTab.PanelImageUrl"/>; các ảnh ở đây nối tiếp phía sau và được chạy
/// luân phiên trong đúng khung ảnh đang có, nên bố cục trang không đổi khi chưa ai thêm ảnh nào.
/// </summary>
public class HomeServiceImage
{
    public int Id { get; set; }

    public int HomeServiceTabId { get; set; }
    public HomeServiceTab HomeServiceTab { get; set; } = null!;

    public required string Url { get; set; }

    /// <summary>Điểm neo dạng "50% 30%" — giữ phần quan trọng khi ảnh bị cắt. Rỗng = giữa ảnh.</summary>
    public string? Focal { get; set; }

    public int SortOrder { get; set; }
}
