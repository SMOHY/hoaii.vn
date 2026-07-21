/*
  Đồng bộ danh mục Quà trung thu với Figma node 758:11370 (desktop) / 1068:28892 (mobile).
  Chạy được nhiều lần.

  Đối chiếu trước khi chạy:

  1. Ảnh 6 sản phẩm trong DB bị gán lệch đúng một bậc — lỗi seed:
       Việt Nam Hoa Thị   -> thien-dieu-lac-hong.jpg
       Thiên Điểu Lạc Hồng-> tinh-hoa-bac-bo.jpg
       Ngũ Quả            -> phung-hoa-trinh-tuong.jpg
       Phụng Hoa Trình Tường -> ngu-qua.jpg
     và hai hộp Tinh Hoa Bắc Bộ còn đang đeo ảnh /images/placeholders/featured-*.jpg.
     Ảnh mới tải thẳng từ fill gốc của từng card trong Figma.

  2. Cả 6 sản phẩm đang IsActive = 0 nên trang danh mục ra trạng thái rỗng. Figma vẽ trang này
     có sản phẩm, và đây là chính 6 sản phẩm của danh mục chứ không phải hàng mượn từ nơi khác,
     nên bật lên. Xem WF-002 trong warringFaild.md.

  3. Copy hero/campaign lấy từ Figma: eyebrow 758:11375, kicker 758:11395,
     campaign eyebrow 758:11430, campaign body 758:11431.

  Lưu ý: Figma vẽ lưới Trung thu bằng 5 card mượn tên + ảnh của trang Quà tết, giá đồng loạt
  899.000. Đó là nội dung mock — DB có 6 sản phẩm bánh trung thu thật, giá 595k-899k. Dùng dữ
  liệu thật, giữ layout Figma. Xem WF-003.
*/

SET XACT_ABORT ON;
BEGIN TRANSACTION;

DECLARE @mid INT = (SELECT Id FROM Categories WHERE Slug = 'qua-trung-thu');
IF @mid IS NULL THROW 50000, N'Không tìm thấy danh mục qua-trung-thu.', 1;

/* ---- 1. Copy hero + campaign theo Figma ------------------------------------------------ */
UPDATE Categories SET
    HeroEyebrow  = N'Quà trung thu đặc sắc',
    HeroKicker   = N'Bộ quà 4 hộp bánh',
    PromoEyebrow = N'HOÀI x Cơm Lệ x Thuỷ Tạ',
    PromoTitle   = N'“Tinh Hoa Bắc Bộ” được khơi gợi cảm hứng từ hào quang của triều đại Hậu Lê - thời kỳ mà đất nước Đại Việt ta bước vào giai đoạn thịnh trị bậc nhất, phát triển rực rỡ về kinh tế, văn hóa, giáo dục và cả nghệ thuật.',
    PromoCtaText = N'Mua ngay',
    PromoCtaUrl  = '/danh-muc/qua-trung-thu',
    PromoImageUrl= '/images/categories/mid-autumn/campaign/tinh-hoa-bac-bo-campaign.jpg'
WHERE Id = @mid;

/* Quà tết cũng cần kicker riêng, trước đây bị hard-code trong Razor (node 1519:34009). */
UPDATE Categories SET HeroKicker = N'Bộ quà 6 hộp' WHERE Slug = 'qua-tet' AND HeroKicker IS NULL;

/* ---- 2. Sửa ảnh bị gán lệch ------------------------------------------------------------ */
DECLARE @covers TABLE (Slug NVARCHAR(200), Url NVARCHAR(500));
INSERT INTO @covers VALUES
  ('hop-banh-tinh-hoa-bac-bo-4',     '/images/categories/mid-autumn/products/tinh-hoa-bac-bo.jpg'),
  ('hop-banh-tinh-hoa-bac-bo-6',     '/images/categories/mid-autumn/products/tinh-hoa-bac-bo.jpg'),
  ('hop-banh-thien-dieu-lac-hong',   '/images/categories/mid-autumn/products/thien-dieu-lac-hong.jpg'),
  ('hop-banh-ngu-qua',               '/images/categories/mid-autumn/products/ngu-qua.jpg'),
  /* Ảnh phụng hoa của Figma trùng byte 100% với bản đã lưu cho Quà tết, nên dùng lại file đó
     thay vì tạo bản sao. */
  ('hop-banh-phung-hoa-trinh-tuong', '/images/products/tet/phung-hoa-trinh-tuong.jpg'),
  /* Figma không vẽ card Việt Nam Hoa Thị ở trang Trung thu; dùng ảnh cùng dòng sản phẩm đã có.
     Xem WF-004. */
  ('hop-banh-viet-nam-hoa-thi',      '/images/products/tet/viet-nam-hoa-thi.jpg');

/* Ảnh cũ sai thì thay hẳn ảnh bìa, không giữ làm ảnh phụ — chúng là ảnh của sản phẩm khác. */
UPDATE i SET i.Url = c.Url
FROM ProductImages i
JOIN Products p ON p.Id = i.ProductId
JOIN @covers c ON c.Slug = p.Slug
WHERE p.CategoryId = @mid AND i.SortOrder = 0 AND i.Url <> c.Url;

INSERT INTO ProductImages (ProductId, Url, SortOrder)
SELECT p.Id, c.Url, 0
FROM Products p
JOIN @covers c ON c.Slug = p.Slug
WHERE p.CategoryId = @mid
  AND NOT EXISTS (SELECT 1 FROM ProductImages x WHERE x.ProductId = p.Id AND x.SortOrder = 0);

/* ---- 3. Thứ tự theo Figma và bật lên --------------------------------------------------- */
DECLARE @order TABLE (Slug NVARCHAR(200), Ord INT);
INSERT INTO @order VALUES
  ('hop-banh-tinh-hoa-bac-bo-4', 1), ('hop-banh-tinh-hoa-bac-bo-6', 2),
  ('hop-banh-thien-dieu-lac-hong', 3), ('hop-banh-phung-hoa-trinh-tuong', 4),
  ('hop-banh-ngu-qua', 5), ('hop-banh-viet-nam-hoa-thi', 6);

UPDATE p SET p.IsActive = 1, p.SortOrder = o.Ord, p.UpdatedAt = SYSUTCDATETIME()
FROM Products p
JOIN @order o ON o.Slug = p.Slug
WHERE p.CategoryId = @mid AND (p.IsActive = 0 OR p.SortOrder <> o.Ord);

COMMIT TRANSACTION;

SELECT p.SortOrder, p.Name, p.Price, p.IsActive,
       (SELECT TOP 1 Url FROM ProductImages i WHERE i.ProductId = p.Id ORDER BY i.SortOrder) AS Cover
FROM Products p WHERE p.CategoryId = @mid ORDER BY p.SortOrder;
