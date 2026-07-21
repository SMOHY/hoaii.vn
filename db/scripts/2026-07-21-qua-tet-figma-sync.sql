/*
  Đồng bộ danh mục Quà tết với Figma node 722:25252 (lưới "danh sách sản phẩm").

  Vì sao có file này: dữ liệu sản phẩm giờ do admin quản lý chứ không seed bằng migration nữa,
  nên đợt sửa này chạy thẳng trên DB. Script được giữ lại để chạy lại được trên máy khác
  (SQL Server của bên vận hành) mà không phải dò lại từ đầu. Chạy được nhiều lần.

  Figma vẽ đúng 7 sản phẩm, tất cả 899.000, theo thứ tự:
    1 Mã đáo thành công   2 Phụng hoa trình tường   3 Ngũ quả nhân gian   4 Dân gian họa kỳ
    5 Điện biên hòa ca    6 Việt Nam hào ca         7 Việt Nam hoa thị

  Đối chiếu với DB trước khi chạy:
    - 5/7 đã có nhưng đang ẩn (IsActive = 0) nên trang danh mục không hiện.
    - "Mã đáo thành công" có Price = 0 → sẽ hiện "0 VNĐ".
    - "Dân gian họa kỳ" và "Điện biên hòa ca" chưa hề tồn tại.
    - Tên trong DB lệch Figma ở 3 chỗ (Ngũ quả / Việt Nam Hạo Ca / Việt Nam Hoa Thị).
    - Ảnh trong DB là ảnh mẫu chụp người thật, còn Figma dùng ảnh tĩnh chụp sản phẩm trên set.
      Ảnh cũ KHÔNG bị xoá — chỉ đẩy xuống thành ảnh phụ, ảnh Figma lên làm ảnh bìa.

  Nguồn ảnh: tải từ chính Figma (fill gốc của từng card, bản 4096px), vì bộ ảnh chụp Tết không
  nằm trong file Excel media được cung cấp — file đó là "MEDIA TRUNG THU 2026".
*/

SET XACT_ABORT ON;
BEGIN TRANSACTION;

DECLARE @tet INT = (SELECT Id FROM Categories WHERE Slug = 'qua-tet');
IF @tet IS NULL THROW 50000, N'Không tìm thấy danh mục qua-tet.', 1;

/* ---- 1. Tên và giá theo đúng Figma ---------------------------------------------------- */
UPDATE Products SET Name = N'Ngũ quả nhân gian' WHERE Slug = 'ngu-qua'            AND Name <> N'Ngũ quả nhân gian';
UPDATE Products SET Name = N'Việt Nam hào ca'   WHERE Slug = 'viet-nam-hao-ca'    AND Name <> N'Việt Nam hào ca';
UPDATE Products SET Name = N'Việt Nam hoa thị'  WHERE Slug = 'viet-nam-hoa-thi'   AND Name <> N'Việt Nam hoa thị';
UPDATE Products SET Price = 899000              WHERE Slug = 'ma-dao-thanh-cong'  AND Price = 0;

/* ---- 2. Hai sản phẩm Figma có mà DB chưa có ------------------------------------------- */
/* Description để NULL cho khớp 5 sản phẩm Tết còn lại — chúng cũng đang NULL. Cần copy thật
   thì nhập trong admin, không bịa ở đây. */
INSERT INTO Products (Name, Slug, Description, Price, CompareAtPrice, Badge, IsFeatured, CategoryId, CreatedAt, IsActive, SortOrder)
SELECT N'Dân gian họa kỳ', 'dan-gian-hoa-ky', NULL, 899000, NULL, 0, 0, @tet, SYSUTCDATETIME(), 1, 4
WHERE NOT EXISTS (SELECT 1 FROM Products WHERE Slug = 'dan-gian-hoa-ky');

INSERT INTO Products (Name, Slug, Description, Price, CompareAtPrice, Badge, IsFeatured, CategoryId, CreatedAt, IsActive, SortOrder)
SELECT N'Điện biên hòa ca', 'dien-bien-hoa-ca', NULL, 899000, NULL, 0, 0, @tet, SYSUTCDATETIME(), 1, 5
WHERE NOT EXISTS (SELECT 1 FROM Products WHERE Slug = 'dien-bien-hoa-ca');

/* ---- 3. Ảnh bìa theo Figma ------------------------------------------------------------ */
/* Ảnh cũ tụt xuống một bậc để ảnh Figma nằm ở SortOrder 0; chạy lại lần hai không nhân đôi. */
DECLARE @covers TABLE (Slug NVARCHAR(200), Url NVARCHAR(500));
INSERT INTO @covers VALUES
  ('ma-dao-thanh-cong',     '/images/products/tet/ma-dao-thanh-cong.jpg'),
  ('phung-hoa-trinh-tuong', '/images/products/tet/phung-hoa-trinh-tuong.jpg'),
  ('ngu-qua',               '/images/products/tet/ngu-qua-nhan-gian.jpg'),
  ('dan-gian-hoa-ky',       '/images/products/tet/dan-gian-hoa-ky.jpg'),
  ('dien-bien-hoa-ca',      '/images/products/tet/dien-bien-hoa-ca.jpg'),
  ('viet-nam-hao-ca',       '/images/products/tet/viet-nam-hao-ca.jpg'),
  ('viet-nam-hoa-thi',      '/images/products/tet/viet-nam-hoa-thi.jpg');

UPDATE i SET i.SortOrder = i.SortOrder + 1
FROM ProductImages i
JOIN Products p ON p.Id = i.ProductId
JOIN @covers c ON c.Slug = p.Slug
WHERE NOT EXISTS (SELECT 1 FROM ProductImages x WHERE x.ProductId = p.Id AND x.Url = c.Url);

INSERT INTO ProductImages (ProductId, Url, SortOrder)
SELECT p.Id, c.Url, 0
FROM Products p
JOIN @covers c ON c.Slug = p.Slug
WHERE NOT EXISTS (SELECT 1 FROM ProductImages x WHERE x.ProductId = p.Id AND x.Url = c.Url);

/* ---- 4. Hiện lên và xếp đúng thứ tự Figma --------------------------------------------- */
/* Không tắt sản phẩm nào: Thiên điểu lạc hồng và Tinh hoa bắc bộ vẫn bán, chỉ xếp sau 7 sản
   phẩm Figma vẽ. */
DECLARE @order TABLE (Slug NVARCHAR(200), Ord INT);
INSERT INTO @order VALUES
  ('ma-dao-thanh-cong', 1), ('phung-hoa-trinh-tuong', 2), ('ngu-qua', 3),
  ('dan-gian-hoa-ky', 4), ('dien-bien-hoa-ca', 5),
  ('viet-nam-hao-ca', 6), ('viet-nam-hoa-thi', 7),
  ('thien-dieu-lac-hong', 8), ('tinh-hoa-bac-bo', 9);

UPDATE p SET p.IsActive = 1, p.SortOrder = o.Ord, p.UpdatedAt = SYSUTCDATETIME()
FROM Products p
JOIN @order o ON o.Slug = p.Slug
WHERE p.CategoryId = @tet
  AND (p.IsActive = 0 OR p.SortOrder <> o.Ord);

COMMIT TRANSACTION;

SELECT p.SortOrder, p.Name, p.Price, p.IsActive,
       (SELECT TOP 1 Url FROM ProductImages i WHERE i.ProductId = p.Id ORDER BY i.SortOrder) AS Cover
FROM Products p
WHERE p.CategoryId = @tet
ORDER BY p.SortOrder;
