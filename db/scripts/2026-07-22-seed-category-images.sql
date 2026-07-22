/*
  Ảnh TẠM cho banner hero và ảnh cover của các danh mục.

  Bối cảnh: Figma vẽ 8 banner hero là khối #D6D6D6 và 5 khối cover là #DCDCDC — không có ảnh nào
  được đặt vào (xem WF-020, WF-012). Bộ ảnh khách gửi là MEDIA TRUNG THU 2026, không có ảnh riêng
  cho Trà/Khăn/Tượng gốm/Rượu hay các dịp. Người dùng đã đồng ý dùng ảnh tạm cho kịp bàn giao.

  ⚠️ ẢNH TẠM — không đúng chủ đề từng danh mục.
     Thay trong admin: Danh mục → chọn danh mục → Ảnh banner / Ảnh cover.

  Chỉ dùng ảnh nằm trong repo (`/images/...`). KHÔNG dùng `/uploads/...` vì thư mục đó nằm trong
  .gitignore — bản deploy từ một checkout sạch sẽ 404 toàn bộ banner nếu trỏ vào đấy.

  Chạy được nhiều lần, chỉ ghi vào ô đang trống.
*/

SET XACT_ABORT ON;
BEGIN TRANSACTION;

/* Xoá các đường dẫn /uploads/ đã gán ở lần chạy trước — chúng không có trong repo. */
UPDATE Categories SET BannerImageUrl = NULL WHERE BannerImageUrl LIKE '/uploads/%';
UPDATE Categories SET CoverImageUrl  = NULL WHERE CoverImageUrl  LIKE '/uploads/%';

DECLARE @banner TABLE (Slug NVARCHAR(200), Url NVARCHAR(500));
INSERT INTO @banner VALUES
  /* Hai danh mục có ảnh đúng chủ đề */
  ('qua-tet',             '/images/products/tet/ma-dao-thanh-cong.jpg'),
  ('qua-trung-thu',       '/images/categories/mid-autumn/campaign/tinh-hoa-bac-bo-campaign.jpg'),
  /* Còn lại dùng tạm, mỗi danh mục một ảnh khác nhau */
  ('tra',                 '/images/products/tet/dien-bien-hoa-ca.jpg'),
  ('khan',                '/images/products/tet/dan-gian-hoa-ky.jpg'),
  ('tuong-gom',           '/images/products/tet/viet-nam-hoa-thi.jpg'),
  ('ruou',                '/images/products/tet/viet-nam-hao-ca.jpg'),
  ('ngay-le-tinh-yeu',    '/images/products/tet/phung-hoa-trinh-tuong.jpg'),
  ('ngay-quoc-te-phu-nu', '/images/products/tet/ngu-qua-nhan-gian.jpg'),
  ('qua-giang-sinh',      '/images/categories/mid-autumn/products/tinh-hoa-bac-bo.jpg'),
  ('qua-tang-nguoi-ay',   '/images/categories/mid-autumn/products/thien-dieu-lac-hong.jpg'),
  ('qua-tang-bo-me',      '/images/categories/mid-autumn/products/ngu-qua.jpg'),
  ('qua-tang-theo-dip',   '/images/category/promo-artist.jpg');

UPDATE c SET c.BannerImageUrl = b.Url
FROM Categories c JOIN @banner b ON b.Slug = c.Slug
WHERE c.BannerImageUrl IS NULL OR c.BannerImageUrl = '';

/* Cover của 5 dịp con: dùng chính ảnh sản phẩm trong danh mục đó, để cover và card cùng bộ hình.
   Các sản phẩm này đều đã có ảnh từ script seed-occasion-products. */
UPDATE c SET c.CoverImageUrl = x.Url
FROM Categories c
CROSS APPLY (
    SELECT TOP 1 pi.Url
    FROM Products p JOIN ProductImages pi ON pi.ProductId = p.Id
    WHERE p.CategoryId = c.Id AND p.IsActive = 1 AND pi.Url NOT LIKE '/uploads/%'
    ORDER BY p.SortOrder, p.Id, pi.SortOrder
) x
WHERE (c.CoverImageUrl IS NULL OR c.CoverImageUrl = '')
  AND c.Slug IN ('ngay-le-tinh-yeu','ngay-quoc-te-phu-nu','qua-giang-sinh',
                 'qua-tang-nguoi-ay','qua-tang-bo-me');

/* Sản phẩm tạm đang dùng ảnh trong /uploads (gitignore) — đổi sang ảnh trong repo. */
DECLARE @prod TABLE (Slug NVARCHAR(200), Url NVARCHAR(500));
INSERT INTO @prod VALUES
  ('tam-valentine-ngot-ngao', '/images/products/tet/phung-hoa-trinh-tuong.jpg'),
  ('tam-valentine-hen-uoc',   '/images/products/tet/viet-nam-hao-ca.jpg'),
  ('tam-83-doa-xuan',         '/images/products/tet/ngu-qua-nhan-gian.jpg'),
  ('tam-83-diu-dang',         '/images/products/tet/dan-gian-hoa-ky.jpg'),
  ('tam-noel-dem-an-lanh',    '/images/categories/mid-autumn/products/tinh-hoa-bac-bo.jpg'),
  ('tam-noel-mua-sum-vay',    '/images/products/tet/dien-bien-hoa-ca.jpg'),
  ('tam-nguoi-ay-thuong-men', '/images/categories/mid-autumn/products/thien-dieu-lac-hong.jpg'),
  ('tam-nguoi-ay-tri-ky',     '/images/products/tet/ma-dao-thanh-cong.jpg'),
  ('tam-bo-me-an-yen',        '/images/categories/mid-autumn/products/ngu-qua.jpg'),
  ('tam-bo-me-hieu-kinh',     '/images/products/tet/viet-nam-hoa-thi.jpg');

UPDATE pi SET pi.Url = t.Url
FROM ProductImages pi
JOIN Products p ON p.Id = pi.ProductId
JOIN @prod t ON t.Slug = p.Slug
WHERE pi.Url LIKE '/uploads/%';

COMMIT TRANSACTION;

SELECT Name,
       CASE WHEN BannerImageUrl IS NULL OR BannerImageUrl = '' THEN N'— trống —' ELSE BannerImageUrl END AS Banner,
       CASE WHEN CoverImageUrl  IS NULL OR CoverImageUrl  = '' THEN N'— trống —' ELSE CoverImageUrl  END AS Cover
FROM Categories ORDER BY Type, SortOrder;
