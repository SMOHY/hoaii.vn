/*
    Rút gọn mô tả danh mục về một dòng subtitle như Figma.

    Bối cảnh: khe mô tả dưới tiêu đề "Tất cả …" trong Figma (node 778:22921) là một dòng phụ đề
    ngắn ("Mỗi sản phẩm quà tặng đều mang một câu chuyện riêng"). Năm danh mục theo-dịp lại được
    seed bằng cả đoạn văn 140–384 ký tự, làm phần đầu trang cao nhòng và lệch hẳn tỉ lệ Figma
    (khách phản ánh "quá to"). Thay bằng một câu ngắn, giữ đúng chủ đề từng dịp.

    Các danh mục còn lại (qua-tet, tra, khan…) đang để Description = NULL nên đã tự dùng phụ đề
    mặc định ngắn ở CategoryController — không cần đụng.

    An toàn chạy lại nhiều lần (chỉ UPDATE theo slug).
*/

UPDATE Categories SET Description = N'Ấm áp mùa Giáng sinh, gửi trao yêu thương qua từng món quà tinh tế.'
WHERE Slug = 'qua-giang-sinh';

UPDATE Categories SET Description = N'Tôn vinh phái đẹp với những món quà tinh tế, chan chứa yêu thương.'
WHERE Slug = 'ngay-quoc-te-phu-nu';

UPDATE Categories SET Description = N'Gửi lời yêu thương chân thành qua món quà chọn bằng cả trái tim.'
WHERE Slug = 'ngay-le-tinh-yeu';

UPDATE Categories SET Description = N'Tri ân đấng sinh thành bằng những món quà chỉn chu, ý nghĩa.'
WHERE Slug = 'qua-tang-bo-me';

UPDATE Categories SET Description = N'Trao gửi tình cảm đến người thương qua món quà đầy dụng ý.'
WHERE Slug = 'qua-tang-nguoi-ay';

SELECT Slug, LEN(Description) AS Len, Description FROM Categories
WHERE Slug IN ('qua-giang-sinh','ngay-quoc-te-phu-nu','ngay-le-tinh-yeu','qua-tang-bo-me','qua-tang-nguoi-ay')
ORDER BY Slug;
