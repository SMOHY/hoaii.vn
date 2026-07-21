/*
  Khôi phục trang "Chính sách đổi trả & hoàn tác" (/chinh-sach/trao-doi). Chạy được nhiều lần.

  Chuyện gì đã xảy ra: trang này chỉ còn đúng một khối nội dung ghi "AUTOTEST CHINH SACH" — một
  lần chạy test tự động đã lưu đè lên nội dung thật qua form admin. Ba trang chính sách còn lại có
  9–13 khối, 1.300–1.900 ký tự; trang này còn 19 ký tự.

  Kèm theo đó, tiêu đề bị escape chồng 13 lần: "ĐỔI TRẢ &amp;amp;amp;…amp; HOÀN TÁC". Mỗi lần lưu
  lại escape thêm một lớp, nên có thể đếm được số lần trang bị lưu đè.

  Nội dung lấy lại nguyên văn từ commit 7a5fb75 (lúc chính sách còn nằm trong PageController trước
  khi chuyển vào DB). Kind: 0 = đoạn văn, 1 = tiêu đề, 2 = gạch đầu dòng.

  ⚠️ Đây là lỗi vận hành, không phải lỗi giao diện: form admin cho phép test ghi đè trang chính
  sách thật, và tự escape lại nội dung mỗi lần lưu. Xem WF-038 và WF-039.
*/

SET XACT_ABORT ON;
BEGIN TRANSACTION;

DECLARE @id INT = (SELECT Id FROM PolicyPages WHERE Slug = 'trao-doi');
IF @id IS NULL THROW 50000, N'Không tìm thấy trang chính sách trao-doi.', 1;

UPDATE PolicyPages
SET Title = N'CHÍNH SÁCH ĐỔI TRẢ & HOÀN TÁC',
    NavLabel = N'Chính sách trao đổi',
    BreadcrumbLabel = N'Trang chủ/Chính sách trao đổi'
WHERE Id = @id;

/* Chỉ dọn khi trang vẫn đang ở trạng thái bị test ghi đè — chạy lại sau khi đã khôi phục thì
   không xoá nội dung thật. */
IF NOT EXISTS (SELECT 1 FROM PolicyBlocks WHERE PolicyPageId = @id AND SortOrder = 1)
BEGIN
    DELETE FROM PolicyBlocks WHERE PolicyPageId = @id;

    INSERT INTO PolicyBlocks (PolicyPageId, Kind, SortOrder, Text) VALUES
    (@id, 0,  0, N'Tại HOÀI, mỗi sản phẩm được trao đi là một nhân duyên lành. Chúng tôi trân quý sự tin tưởng của bạn và luôn mong muốn mang lại trải nghiệm thoải mái nhất. Trong trường hợp món đồ nhận được chưa thực sự như ý, HOÀI sẵn lòng đồng hành cùng bạn để tìm phương án vẹn toàn.'),
    (@id, 1,  1, N'I. Điều Kiện Đổi Trả Sản Phẩm'),
    (@id, 0,  2, N'Để đảm bảo quyền lợi, Quý Khách vui lòng kiểm tra kỹ tình trạng sản phẩm ngay tại thời điểm nhận hàng. HOÀI hỗ trợ đổi trả ngay nếu sản phẩm gặp các vấn đề sau:'),
    (@id, 2,  3, N'Sự cố về vận chuyển: Bao bì bị rách hỏng, sản phẩm bị bong tróc, nứt vỡ hoặc biến dạng do va đập.'),
    (@id, 2,  4, N'Sự cố về đóng gói: Sản phẩm không đúng mẫu mã, chủng loại như đơn hàng đã đặt; hoặc bị thiếu hụt số lượng, phụ kiện và quà tặng đi kèm.'),
    (@id, 0,  5, N'Lưu ý nhỏ: Quý khách vui lòng lưu lại hình ảnh và video mở hộp (unboxing) làm minh chứng để HOÀI có thể hỗ trợ xử lý thủ tục đổi trả một cách nhanh chóng và chính xác nhất.'),
    (@id, 1,  6, N'II. Quy Định Thời Gian & Phương Thức Gửi Trả'),
    (@id, 2,  7, N'Thời gian thông báo: Trong vòng 48 giờ kể từ khi ký nhận hàng (đối với các trường hợp thiếu hụt hoặc hư hỏng vật lý).'),
    (@id, 2,  8, N'Thời gian gửi hoàn sản phẩm: Trong vòng 14 ngày kể từ ngày nhận hàng thành công.'),
    (@id, 2,  9, N'Phương thức gửi trả: Bạn có thể mang sản phẩm ghé chơi và đổi trực tiếp tại cửa hàng/văn phòng của HOÀI, hoặc gửi chuyển phát qua bưu điện/các đơn vị vận chuyển thuận tiện nhất cho bạn.'),
    (@id, 1, 10, N'III. Chi Phí Vận Chuyển Hoàn Hàng'),
    (@id, 0, 11, N'Tùy thuộc vào nguyên nhân phát sinh (do sơ suất của HOÀI hay nhu cầu cá nhân từ phía khách hàng), hai bên sẽ cùng trao đổi để thống nhất phương án hỗ trợ chi phí vận chuyển hợp lý và vẹn cả đôi đường.'),
    (@id, 1, 12, N'IV. Quy Trình Hoàn Tiền'),
    (@id, 0, 13, N'Ngay sau khi nhận lại sản phẩm và hoàn tất việc kiểm tra tình trạng hàng hóa, HOÀI sẽ tiến hành hoàn trả tiền hàng cho bạn. Thời gian hoàn tiền được xử lý nhanh chóng trong vòng 48 giờ kể từ khi HOÀI xác nhận nhận lại hàng thành công.'),
    (@id, 0, 14, N'Trước khi gửi hoàn sản phẩm, bạn hãy liên hệ trước với HOÀI qua hotline hoặc hộp thư tin nhắn để HOÀI chuẩn bị và đón nhận kiện hàng một cách chu đáo nhất.'),
    (@id, 0, 15, N'Mọi ý kiến đóng góp hoặc phản hồi về chất lượng, HOÀI luôn lắng nghe tại đường dây chăm sóc khách hàng. Sự hài lòng của bạn chính là động lực để HOÀI hoàn thiện hơn mỗi ngày. Cảm ơn bạn đã thương mến!');
END

COMMIT TRANSACTION;

SELECT p.Slug, p.Title,
       (SELECT COUNT(*) FROM PolicyBlocks b WHERE b.PolicyPageId = p.Id) AS SoKhoi,
       (SELECT SUM(LEN(b.Text)) FROM PolicyBlocks b WHERE b.PolicyPageId = p.Id) AS SoKyTu
FROM PolicyPages p ORDER BY p.SortOrder;
