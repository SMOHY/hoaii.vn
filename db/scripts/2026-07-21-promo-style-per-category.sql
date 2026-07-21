/*
  Màu nền và bố cục của dải campaign, theo đúng bản CSS Figma xuất ra cho từng trang.
  Chạy được nhiều lần.

  Trước đây CSS dùng chung một màu #AA8656 cho mọi danh mục, trong khi Figma đổi màu theo trang:

    Quà tết        #AA8656   bố cục hẹp   (pad 80/80/80/240, chữ 760, ảnh 760)
    Quà trung thu  #AF2234   bố cục hẹp
    Tất cả còn lại #E5D9CB   bố cục rộng  (pad 80 đều, khung chữ 840 thụt 240, ảnh 840)

  Nguồn: các file "css all layer" — layer "Sản phẩm giới hạn/nổi bật" ở mỗi trang.
*/

SET XACT_ABORT ON;
BEGIN TRANSACTION;

/* Mặc định: bản rộng, nền yellow-100 — áp cho mọi danh mục. */
UPDATE Categories SET PromoBackground = '#E5D9CB', PromoWide = 1;

/* Hai trang chủ lực dùng bản hẹp với nền riêng. */
UPDATE Categories SET PromoBackground = '#AA8656', PromoWide = 0 WHERE Slug = 'qua-tet';
UPDATE Categories SET PromoBackground = '#AF2234', PromoWide = 0 WHERE Slug = 'qua-trung-thu';

COMMIT TRANSACTION;

SELECT Slug, ISNULL(PromoBackground, '(mặc định)') AS Nen,
       CASE WHEN PromoWide = 1 THEN N'rộng' ELSE N'hẹp' END AS BoCuc
FROM Categories ORDER BY Type, SortOrder, Slug;
