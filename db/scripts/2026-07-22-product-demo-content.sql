/*
  Nội dung TẠM cho trang chi tiết sản phẩm, để bàn giao không còn khối nào trống.

  ⚠️ ĐÂY LÀ NỘI DUNG TẠM. Bộ phận nội dung cần thay bằng thông tin thật.

  Nguyên tắc khi viết, cố ý giữ chặt:

    1. KHÔNG bịa thành phần (cột Description). Với thực phẩm, một danh sách thành phần sai là
       chuyện dị ứng và an toàn, không phải chuyện giao diện. Chỉ hai sản phẩm Tinh Hoa Bắc Bộ có
       danh sách thật lấy từ Figma là được điền. Các sản phẩm còn lại để trống — khối "THÀNH PHẦN"
       nay tự ẩn nên trang vẫn gọn, không còn dòng "sẽ được cập nhật".

    2. KHÔNG bịa số đo (cột FeatureBody) cho từng sản phẩm. Khách đọc số này để hình dung món quà
       và để tính vận chuyển. Thay vào đó điền phần "Đặc điểm" mô tả chất liệu và cách hoàn thiện
       theo nhóm sản phẩm — đúng với thứ đang bán, không kèm con số nào.

    3. Câu chuyện (StoryBody) viết theo NHÓM danh mục, dùng lại tinh thần đã có trong trang
       Về chúng tôi, không gán sự tích riêng cho từng món.

  Tìm lại để thay:
      SELECT Id, Name FROM Products WHERE FeatureBody LIKE N'%(Nội dung tạm)%';

  Chạy được nhiều lần — chỉ ghi vào ô đang trống.
*/

SET XACT_ABORT ON;
BEGIN TRANSACTION;

DECLARE @theoNhom TABLE (CatSlug NVARCHAR(200), Story NVARCHAR(MAX), Feature NVARCHAR(MAX));

INSERT INTO @theoNhom VALUES
('tra',
 N'Trà trong bộ sưu tập của HOÀI được chọn từ những vùng trà có tiếng của Việt Nam, đóng gói theo mẻ nhỏ để giữ hương. Phần hộp làm riêng cho từng dòng trà, đủ kín để bảo quản và đủ đẹp để đặt lên bàn tiếp khách.',
 N'Hộp giấy mỹ thuật, in nhiều lớp, hoàn thiện thủ công.
Lớp lót bên trong giữ ẩm và giữ hương cho trà.
Có thể kèm thiệp viết tay theo yêu cầu.
(Nội dung tạm)'),

('khan',
 N'Khăn được dệt và hoàn thiện thủ công, hoa văn lấy cảm hứng từ hoạ tiết dân gian Việt. Mỗi mẫu ra đời từ bản vẽ tay trước khi lên khung, nên nét hoa văn giữ được độ mềm mà in công nghiệp khó có.',
 N'Chất liệu mềm, giữ nếp tốt, không xù sau nhiều lần dùng.
Hoa văn vẽ tay rồi mới đưa lên khung dệt.
Gấp sẵn trong hộp, mở ra là dùng được ngay.
(Nội dung tạm)'),

('tuong-gom',
 N'Gốm trong bộ sưu tập được làm tại làng nghề truyền thống, nung theo mẻ nhỏ. Cùng một dáng nhưng mỗi lần ra lò lại cho sắc men hơi khác nhau — đó là điều đồ sản xuất hàng loạt không có được.',
 N'Gốm nung ở nhiệt độ cao, men bám đều, bền màu.
Mỗi sản phẩm có sắc men riêng do vị trí trong lò.
Đóng gói chống va đập cho vận chuyển xa.
(Nội dung tạm)'),

('ruou',
 N'Phần vỏ hộp và nhãn chai được HOÀI thiết kế riêng, lấy hoạ tiết từ vốn văn hoá Việt. Món quà vì thế giữ được vẻ trang trọng khi mang đi biếu mà vẫn có nét riêng, không lẫn với hộp quà đại trà.',
 N'Hộp cứng có khay giữ chai, hạn chế xê dịch khi mang đi.
Nhãn và hộp in nhiều lớp, hoàn thiện thủ công.
Phù hợp biếu tặng đối tác và dịp lễ Tết.
(Nội dung tạm)');

/* Câu chuyện + đặc điểm mặc định cho các danh mục quà tặng còn lại. */
DECLARE @storyQua NVARCHAR(MAX) = N'Khởi nguồn từ tình yêu dành cho di sản Việt Nam, HOÀI chọn hành trình gìn giữ và lan toả giá trị truyền thống thông qua những tặng phẩm văn hoá. Mỗi bộ quà được dựng từ bản vẽ tay, in nhiều lớp và hoàn thiện thủ công, để phần vỏ và phần ruột cùng kể một câu chuyện.';
DECLARE @featQua NVARCHAR(MAX) = N'Hộp cứng bọc giấy mỹ thuật, in nhiều lớp, hoàn thiện thủ công.
Khay bên trong giữ từng phần quà cố định khi vận chuyển.
Có quai xách, mang đi biếu tặng thuận tiện.
Nhận khắc tên hoặc kèm thiệp viết tay theo yêu cầu.
(Nội dung tạm)';

UPDATE p SET
    p.StoryTitle = COALESCE(NULLIF(p.StoryTitle, ''), N'Câu chuyện sản phẩm'),
    p.StoryBody  = COALESCE(NULLIF(p.StoryBody, ''),  COALESCE(t.Story, @storyQua)),
    p.FeatureTitle = COALESCE(NULLIF(p.FeatureTitle, ''), N'Đặc điểm'),
    p.FeatureBody  = COALESCE(NULLIF(p.FeatureBody, ''),  COALESCE(t.Feature, @featQua)),
    p.UpdatedAt = SYSUTCDATETIME()
FROM Products p
JOIN Categories c ON c.Id = p.CategoryId
LEFT JOIN @theoNhom t ON t.CatSlug = c.Slug
WHERE p.IsActive = 1;

COMMIT TRANSACTION;

SELECT COUNT(*) AS DangBan,
       SUM(CASE WHEN ISNULL(StoryBody,'')   = '' THEN 1 ELSE 0 END) AS ThieuCauChuyen,
       SUM(CASE WHEN ISNULL(FeatureBody,'') = '' THEN 1 ELSE 0 END) AS ThieuDacDiem,
       SUM(CASE WHEN ISNULL(Description,'') = '' THEN 1 ELSE 0 END) AS ThieuThanhPhan
FROM Products WHERE IsActive = 1;
