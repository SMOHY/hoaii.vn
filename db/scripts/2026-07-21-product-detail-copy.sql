/*
  Nội dung trang chi tiết cho dòng Tinh Hoa Bắc Bộ (B18). Chạy được nhiều lần.

  Bối cảnh: 45/45 sản phẩm đang bán đều để trống Description (thành phần), StoryBody (câu chuyện)
  và FeatureBody (kích thước hộp) — các cột này thêm ở đợt CMS nhưng chưa ai nhập. Hệ quả là mọi
  trang chi tiết hiện cùng một đoạn mặc định. Xem WF-030.

  Figma chỉ dựng trang chi tiết cho đúng một sản phẩm — Tinh Hoa Bắc Bộ (node 826:14920) — nên chỉ
  sản phẩm này có nội dung chính thức để nhập. Nguồn:
     câu chuyện  826:13879
     thành phần  826:13864   (Figma gõ nhầm tiêu đề "THÀN PHẦN", web hiện đúng "THÀNH PHẦN")
     kích thước  826:13881

  44 sản phẩm còn lại vẫn trống — phải nhập trong admin, không bịa ở đây.
*/

SET XACT_ABORT ON;
BEGIN TRANSACTION;

DECLARE @story NVARCHAR(MAX) = N'“Tinh Hoa Bắc Bộ” được khơi gợi cảm hứng từ hào quang của triều đại Hậu Lê - thời kỳ mà đất nước Đại Việt ta bước vào giai đoạn thịnh trị bậc nhất, phát triển rực rỡ về kinh tế, văn hóa, giáo dục và cả nghệ thuật. Phần quai xách mô phỏng dáng hình mái “Điện Kính Thiên” - linh hồn của Hoàng thành Thăng Long, đây là trung tâm chính trị, nơi diễn ra các buổi thiết triều và quyết định những đại sự trọng yếu của quốc gia. Trên thân hộp, từng nét họa được chọn lọc tinh túy, khắc họa trọn vẹn thể cách của buổi chầu trong điện, không khí khoa cử tuyển hiền tài, hơi thở tôn nghiêm của các nghi lễ, và âm hưởng nhã nhạc cung đình... Mỗi chi tiết là một dấu ấn lịch sử, vừa lưu giữ hồn cốt triều đại hưng thịnh, vừa vang vọng vẻ đẹp trường tồn của văn hóa Bắc Bộ. Một sản phẩm của HOÀI x CƠM LỆ';

/* Câu chuyện là về thiết kế hộp nên áp cho cả ba sản phẩm cùng dòng. */
UPDATE Products
SET StoryTitle = N'Câu chuyện sản phẩm', StoryBody = @story, UpdatedAt = SYSUTCDATETIME()
WHERE Slug IN ('tinh-hoa-bac-bo','hop-banh-tinh-hoa-bac-bo-4','hop-banh-tinh-hoa-bac-bo-6')
  AND (StoryBody IS NULL OR StoryBody <> @story);

/* Danh sách bánh trong Figma là của hộp 4 bánh. Sản phẩm "tinh-hoa-bac-bo" có biến thể
   4 Bánh/6 Bánh và Figma dựng đúng trang đó, nên áp cho cả nó và hộp 4 bánh. Hộp 6 bánh có thành
   phần khác nên để trống chờ nhập. */
UPDATE Products
SET Description = N'Bánh nướng Thập cẩm xá xíu đặc biệt (150g), Bánh nướng Trà xanh lava (150g), Bánh nướng Sen nhuyễn trứng muối (150g), Bánh nướng Đậu xanh long nhãn (150g)',
    UpdatedAt = SYSUTCDATETIME()
WHERE Slug IN ('tinh-hoa-bac-bo','hop-banh-tinh-hoa-bac-bo-4')
  AND (Description IS NULL OR LEN(Description) = 0);

/* Kích thước hộp — trước đây được hard-code trong controller và in ra cho cả 45 sản phẩm. Giờ là
   dữ liệu, chỉ thuộc về sản phẩm thật sự có số đo này. */
UPDATE Products
SET FeatureTitle = N'Đặc điểm',
    FeatureBody = N'KÍCH THƯỚC:' + CHAR(10)
        + N'Hộp cứng: 48x15.7x6cm' + CHAR(10)
        + N'Hộp con: 9.5x9.5x5cm' + CHAR(10)
        + N'Quai xách: 38x15.7x6.2cm' + CHAR(10)
        + N'Túi đựng: 49x17x7cm',
    UpdatedAt = SYSUTCDATETIME()
WHERE Slug IN ('tinh-hoa-bac-bo','hop-banh-tinh-hoa-bac-bo-4')
  AND (FeatureBody IS NULL OR LEN(FeatureBody) = 0);

COMMIT TRANSACTION;

SELECT Slug,
       CASE WHEN LEN(ISNULL(Description,'')) > 0 THEN N'có' ELSE N'trống' END AS ThanhPhan,
       CASE WHEN LEN(ISNULL(StoryBody,''))   > 0 THEN N'có' ELSE N'trống' END AS CauChuyen,
       CASE WHEN LEN(ISNULL(FeatureBody,'')) > 0 THEN N'có' ELSE N'trống' END AS KichThuoc
FROM Products
WHERE Slug LIKE '%tinh-hoa-bac-bo%'
ORDER BY Slug;
