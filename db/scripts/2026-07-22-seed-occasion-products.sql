/*
  Sản phẩm TẠM cho 5 danh mục dịp, để hai trang landing không còn ô trống.

  Bối cảnh: Valentine, Quốc tế Phụ nữ, Giáng sinh, Quà tặng người ấy, Quà tặng bố mẹ đều có
  **0 sản phẩm**, nên 10 ô card trên /qua-theo-dip và /qua-tang-ca-nhan để trắng. Bộ ảnh khách
  cung cấp (file MEDIA TRUNG THU 2026) chỉ có ảnh Trung thu; Figma cũng chỉ vẽ khối xám cho các
  section này. Người dùng đã đồng ý dùng dữ liệu tạm để trang nhìn đủ như thiết kế, và sẽ tự
  cập nhật trong admin sau khi bàn giao.

  ⚠️ ĐÂY LÀ DỮ LIỆU TẠM — tên, giá và ảnh đều KHÔNG phải hàng thật:
     - Ảnh mượn từ bộ chụp Trung thu đã import.
     - Giá đặt trong khoảng giá của các sản phẩm thật đang bán (680.000 – 1.450.000).
     - Không nhập Thành phần / Câu chuyện / Kích thước: bịa thành phần cho sản phẩm ăn được là
       thứ không được phép, nên các khối đó tự ẩn cho tới khi có nội dung thật.

  Nhận diện để thay sau: mọi sản phẩm ở đây có slug bắt đầu bằng `tam-`.
  Xoá sạch: DELETE FROM Products WHERE Slug LIKE 'tam-%';

  Chạy được nhiều lần.
*/

SET XACT_ABORT ON;
BEGIN TRANSACTION;

/* Ảnh lấy từ thư viện thật, mỗi sản phẩm một ảnh khác nhau. */
DECLARE @seed TABLE (
    Slug NVARCHAR(200), Name NVARCHAR(400), CatSlug NVARCHAR(200),
    Price DECIMAL(18,2), Ord INT, ImgRank INT
);
INSERT INTO @seed VALUES
  ('tam-valentine-ngot-ngao',   N'Set quà Valentine – Ngọt ngào',      'ngay-le-tinh-yeu',    899000,  1, 1),
  ('tam-valentine-hen-uoc',     N'Set quà Valentine – Hẹn ước',        'ngay-le-tinh-yeu',   1250000,  2, 2),
  ('tam-83-doa-xuan',           N'Set quà 8/3 – Đóa xuân',             'ngay-quoc-te-phu-nu', 899000,  1, 3),
  ('tam-83-diu-dang',           N'Set quà 8/3 – Dịu dàng',             'ngay-quoc-te-phu-nu',1100000,  2, 4),
  ('tam-noel-dem-an-lanh',      N'Set quà Giáng sinh – Đêm an lành',   'qua-giang-sinh',      899000,  1, 5),
  ('tam-noel-mua-sum-vay',      N'Set quà Giáng sinh – Mùa sum vầy',   'qua-giang-sinh',     1350000,  2, 6),
  ('tam-nguoi-ay-thuong-men',   N'Set quà tặng người ấy – Thương mến', 'qua-tang-nguoi-ay',   990000,  1, 7),
  ('tam-nguoi-ay-tri-ky',       N'Set quà tặng người ấy – Tri kỷ',     'qua-tang-nguoi-ay',  1450000,  2, 8),
  ('tam-bo-me-an-yen',          N'Set quà tặng bố mẹ – An yên',        'qua-tang-bo-me',      890000,  1, 9),
  ('tam-bo-me-hieu-kinh',       N'Set quà tặng bố mẹ – Hiếu kính',     'qua-tang-bo-me',     1250000,  2,10);

/* Xếp hạng ảnh để mỗi sản phẩm nhận một ảnh riêng, ổn định giữa các lần chạy. */
WITH img AS (
    SELECT Url, ROW_NUMBER() OVER (ORDER BY Id) AS rn
    FROM MediaAssets
    WHERE Width >= 1000
)
INSERT INTO Products (Name, Slug, Description, Price, CompareAtPrice, Badge, IsFeatured,
                      CategoryId, CreatedAt, IsActive, SortOrder)
SELECT s.Name, s.Slug, NULL, s.Price, NULL, 0, 0, c.Id, SYSUTCDATETIME(), 1, s.Ord
FROM @seed s
JOIN Categories c ON c.Slug = s.CatSlug
WHERE NOT EXISTS (SELECT 1 FROM Products p WHERE p.Slug = s.Slug);

WITH img AS (
    SELECT Url, ROW_NUMBER() OVER (ORDER BY Id) AS rn
    FROM MediaAssets
    WHERE Width >= 1000
)
INSERT INTO ProductImages (ProductId, Url, SortOrder)
SELECT p.Id, i.Url, 0
FROM @seed s
JOIN Products p ON p.Slug = s.Slug
JOIN img i ON i.rn = s.ImgRank
WHERE NOT EXISTS (SELECT 1 FROM ProductImages x WHERE x.ProductId = p.Id);

/* Sáu "Set quà …" có sẵn trong Quà tặng theo dịp đang ẩn hết, nên trang danh mục đó cũng trống.
   Đây là sản phẩm thật, chỉ chưa bật. */
UPDATE p SET p.IsActive = 1, p.UpdatedAt = SYSUTCDATETIME()
FROM Products p JOIN Categories c ON c.Id = p.CategoryId
WHERE c.Slug = 'qua-tang-theo-dip' AND p.IsActive = 0;

COMMIT TRANSACTION;

SELECT c.Slug AS DanhMuc, COUNT(p.Id) AS SoSanPham,
       SUM(CASE WHEN p.Slug LIKE 'tam-%' THEN 1 ELSE 0 END) AS TrongDoLaTam
FROM Categories c LEFT JOIN Products p ON p.CategoryId = c.Id AND p.IsActive = 1
WHERE c.Type = 1
GROUP BY c.Slug, c.SortOrder ORDER BY c.SortOrder;
