/*
  Tám trang danh mục dùng hero banner thay vì carousel (B16).
  Chạy được nhiều lần.

  Figma nodes: 1269:39694 (Phụ nữ), 1269:40145 (Giáng sinh), 1269:40590 (Người ấy),
  1269:41035 (Bố mẹ), 1151:31789 (Trà), 1151:32236 (Khăn), 1151:32683 (Tượng gốm),
  1151:34918 (Rượu).

  Ba việc:

  1. HeroStyle = 1 (Banner). Quà tết và Quà trung thu giữ nguyên carousel — mặc định 0 nên không
     cần đụng tới, và nếu script này không được chạy thì chúng vẫn hiển thị đúng như cũ.

  2. Breadcrumb ba cấp cho bốn trang theo dịp. Figma desktop node 1269:39709 ghi
     "Trang chủ/Quà theo dịp/Ngày lễ tình yêu" — sai nhánh cuối do copy-paste; bản mobile
     node 1265:31321 ghi đúng "Trang chủ/Quà tặng theo dịp/Ngày quốc tế phụ nữ". Lấy theo mobile.
     Người ấy và Bố mẹ nằm dưới trang "Quà tặng cá nhân".

  3. Bật sản phẩm của Khăn, Tượng gốm, Rượu. Mỗi danh mục có 6 sản phẩm thật với giá thật nhưng
     đang IsActive = 0, nên ba trang này ra trạng thái rỗng trong khi Figma vẽ đầy sản phẩm.
     Trà đã active sẵn 12 sản phẩm. Xem WF-019.

  BannerImageUrl để NULL: Figma không đặt ảnh vào bất kỳ banner nào trong tám trang — đã kiểm tra
  bằng download_assets trên node 1151:31798, trả về rawImages rỗng. Xem WF-020.
*/

SET XACT_ABORT ON;
BEGIN TRANSACTION;

/* ---- 1. Kiểu hero ---------------------------------------------------------------------- */
UPDATE Categories SET HeroStyle = 1
WHERE Slug IN ('ngay-quoc-te-phu-nu','qua-giang-sinh','qua-tang-nguoi-ay','qua-tang-bo-me',
               'tra','khan','tuong-gom','ruou',
               /* Figma không vẽ trang listing cho Valentine, nhưng nó đi cùng bộ với Phụ nữ và
                  Giáng sinh và cũng tới được từ nút "Xem tất cả" của trang landing. Để nó dùng
                  carousel rỗng trong khi hai trang anh em dùng banner thì lệch hẳn. Xem WF-021. */
               'ngay-le-tinh-yeu')
  AND HeroStyle <> 1;

/* ---- 2. Breadcrumb ba cấp -------------------------------------------------------------- */
UPDATE Categories SET ParentLabel = N'Quà tặng theo dịp', ParentUrl = '/qua-theo-dip'
WHERE Slug IN ('ngay-le-tinh-yeu','ngay-quoc-te-phu-nu','qua-giang-sinh');

UPDATE Categories SET ParentLabel = N'Quà tặng cá nhân', ParentUrl = '/qua-tang-ca-nhan'
WHERE Slug IN ('qua-tang-nguoi-ay','qua-tang-bo-me');

/* ---- 3. Bật sản phẩm thật của ba danh mục sản phẩm ------------------------------------- */
UPDATE p SET p.IsActive = 1, p.UpdatedAt = SYSUTCDATETIME()
FROM Products p
JOIN Categories c ON c.Id = p.CategoryId
WHERE c.Slug IN ('khan','tuong-gom','ruou') AND p.IsActive = 0;

/* ---- 4. Tên danh mục Trà đang lưu dưới dạng HTML entity -------------------------------- */
/* Giá trị trong DB là chuỗi ký tự `Tr&#xE0;` chứ không phải `Trà`, nên trang hiện nguyên
   "TR&#XE0;" ở banner và "Tất cả tr&#xe0;" ở tiêu đề — Razor escape chuỗi này đúng như mọi text
   khác. Lỗi encode kép từ lần import nào đó. Đã quét toàn bộ Categories, Products,
   ProductVariants và các cột Description: chỉ duy nhất dòng này bị. */
UPDATE Categories SET Name = N'Trà' WHERE Slug = 'tra' AND Name <> N'Trà';

COMMIT TRANSACTION;

SELECT c.Slug, c.HeroStyle, ISNULL(c.ParentLabel,'-') AS Parent,
       (SELECT COUNT(*) FROM Products p WHERE p.CategoryId = c.Id AND p.IsActive = 1) AS SanPhamHien
FROM Categories c
WHERE c.Slug IN ('ngay-le-tinh-yeu','ngay-quoc-te-phu-nu','qua-giang-sinh','qua-tang-nguoi-ay',
                 'qua-tang-bo-me','tra','khan','tuong-gom','ruou','qua-tet','qua-trung-thu')
ORDER BY c.HeroStyle, c.Slug;
