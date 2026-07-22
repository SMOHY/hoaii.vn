/*
  Trà đang bị gán CategoryType = Occasion (1) trong khi Khăn, Tượng gốm, Rượu đều là
  ProductType (0). Trà là một loại sản phẩm, không phải một dịp tặng quà.

  Hai chỗ hỏng vì lỗi này:
    1. Mega menu "Quà theo dịp" liệt kê danh mục dịp con — Trà lọt vào giữa Valentine,
       Giáng sinh, Quà tặng bố mẹ.
    2. Mega menu "Sản phẩm chọn lọc" lấy cột đầu bằng `Type == ProductType`, nên Trà —
       danh mục nhiều hàng nhất, 12 sản phẩm — không hề xuất hiện.

  Chạy được nhiều lần.
*/

SET XACT_ABORT ON;
BEGIN TRANSACTION;

UPDATE Categories SET Type = 0 WHERE Slug = 'tra' AND Type <> 0;

COMMIT TRANSACTION;

SELECT Name, Slug, CASE Type WHEN 0 THEN N'Loại sản phẩm' ELSE N'Dịp' END AS Loai
FROM Categories ORDER BY Type, SortOrder;
