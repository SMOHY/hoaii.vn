/*
  Nội dung cho các section của hai trang landing "Quà theo dịp" và "Quà tặng cá nhân".
  Chạy được nhiều lần.

  Vì sao lấy copy từ frame mobile: bản desktop của cả 5 section đều đang dán cùng một đoạn
  placeholder về Tết Nguyên Đán (nodes 769:15246, 769:15394, 771:21274) — sai ngữ cảnh hoàn toàn.
  Frame mobile mới là nơi có copy thật cho từng dịp:
     Valentine   1068:31791
     Phụ nữ      1068:31715
     Giáng sinh  1068:31578
     Người ấy    1068:31934
     Bố mẹ       1068:31993

  CoverImageUrl để NULL: Figma chưa đặt ảnh nào vào các khối cover này, chúng chỉ là khối màu
  grey-200. Trang sẽ render đúng khối màu đó cho tới khi có ảnh thật. Xem WF-011.
*/

SET XACT_ABORT ON;
BEGIN TRANSACTION;

UPDATE Categories SET Description = N'Ngày lễ tình yêu (Valentine) là dịp để bạn gửi đi một lời nhắn chân thành và trao tặng một món quà nhỏ thay cho sự quan tâm. Không cần cầu kỳ, chỉ cần đúng người – đúng cảm xúc là đủ để ngày này trở nên đặc biệt. Chúc bạn có một Valentine ấm áp, ngọt ngào và đầy kỷ niệm.'
WHERE Slug = 'ngay-le-tinh-yeu';

UPDATE Categories SET Description = N'Ngày Quốc tế Phụ nữ 8/3 là dịp để tôn vinh và gửi lời cảm ơn đến những người phụ nữ vì những đóng góp, yêu thương và sự cố gắng mỗi ngày. Đây cũng là cơ hội để mình quan tâm nhiều hơn một chút – bằng một lời chúc chân thành, một cái ôm ấm áp, hay một món quà nhỏ được chọn kỹ.'
WHERE Slug = 'ngay-quoc-te-phu-nu';

UPDATE Categories SET Description = N'Giáng sinh (Noel) là dịp lễ ấm áp nhất trong năm, khi mọi người cùng hướng về gia đình, bạn bè và những điều tốt lành. Không chỉ gắn với hình ảnh cây thông, ánh đèn lung linh hay những bản nhạc quen thuộc, Giáng sinh còn là mùa của sẻ chia và yêu thương – nơi một lời chúc chân thành, một cuộc gặp gỡ nhỏ hay một món quà được chuẩn bị bằng sự quan tâm cũng đủ làm ai đó thấy hạnh phúc'
WHERE Slug = 'qua-giang-sinh';

UPDATE Categories SET Description = N'Một món quà nhỏ để nói với người ấy rằng: “Mình luôn nghĩ về bạn”. Chọn điều vừa đủ tinh tế, vừa đủ ấm áp để người ấy mỉm cười ngay khi mở ra.'
WHERE Slug = 'qua-tang-nguoi-ay';

UPDATE Categories SET Description = N'Một món quà nhỏ, được chọn kỹ và gói ghém chỉn chu sẽ thay bạn gửi lời quan tâm mỗi ngày—chúc bố mẹ luôn mạnh khoẻ, bình an và luôn vui trong những điều giản dị. Đây là cách nhẹ nhàng để mình yêu thương bố mẹ nhiều hơn, đúng lúc và đúng cách.'
WHERE Slug = 'qua-tang-bo-me';

/* Nav trước đây trỏ "Quà theo dịp" vào lưới danh mục; giờ có trang landing riêng. */
UPDATE NavLinks SET Url = '/qua-theo-dip' WHERE Url = '/danh-muc/qua-tang-theo-dip';

COMMIT TRANSACTION;

SELECT Slug, LEFT(ISNULL(Description, '(trống)'), 60) AS Copy, ISNULL(CoverImageUrl, '(chưa có ảnh)') AS Cover
FROM Categories
WHERE Slug IN ('ngay-le-tinh-yeu','ngay-quoc-te-phu-nu','qua-giang-sinh','qua-tang-nguoi-ay','qua-tang-bo-me');
