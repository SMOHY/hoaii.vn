/*
  Nội dung NHÁP cho 7 bài viết, để trang chi tiết blog không còn chỉ vỏn vẹn hai dòng.

  ⚠️ ĐÂY LÀ BẢN NHÁP, KHÔNG PHẢI BÀI VIẾT CHÍNH THỨC CỦA HOÀI.
     Viết bám đúng phần tóm tắt đã có sẵn trong DB, cố ý tránh mọi con số, mốc thời gian, tên
     riêng và lời khẳng định có thể sai — vì đây sẽ là giọng của thương hiệu trước bạn đọc thật.
     Bộ phận nội dung cần viết lại trước khi chạy quảng bá.

  Tìm lại để thay: mọi bài có Content kết thúc bằng dòng "(Nội dung đang được biên tập.)"
      SELECT Id, Title FROM BlogPosts WHERE Content LIKE '%(Nội dung đang được biên tập.)%';

  Chạy được nhiều lần — chỉ ghi vào bài đang trống.
*/

SET XACT_ABORT ON;
BEGIN TRANSACTION;

DECLARE @noi TABLE (Id INT, Content NVARCHAR(MAX));

INSERT INTO @noi VALUES
(1, N'Chọn quà cho người thân khó ở chỗ: món quà phải nói đúng điều mình muốn nói, mà không cần nói ra thành lời.

Trước khi nghĩ đến sản phẩm, hãy nghĩ đến người nhận. Một người quen dùng đồ gọn gàng sẽ trân trọng thứ nhỏ mà tinh; một người thích bày biện lại vui hơn với bộ quà có thể đặt lên bàn tiếp khách. Dịp tặng cũng đổi cách chọn — quà Tết thiên về sum vầy, quà sinh nhật thiên về cá nhân, quà cảm ơn thì cần vừa đủ để người nhận không thấy nặng nề.

Kế đến là phần nhìn. Hộp quà được gói kỹ tạo ra khoảng lặng dễ chịu trước khi mở, và chính khoảnh khắc ấy mới là thứ người ta nhớ lâu. Ở HOÀI, phần bao bì luôn được làm cùng lúc với phần ruột, để hai thứ kể chung một câu chuyện.

Cuối cùng, đừng bỏ qua tấm thiệp. Một dòng viết tay khiến món quà thành của riêng người nhận.

(Nội dung đang được biên tập.)'),

(2, N'Trà sen là một trong những cách người Hà Nội gửi sự chậm rãi vào một thức uống.

Cái khó của trà sen nằm ở nhịp. Sen phải hái lúc còn sớm, khi hương chưa kịp bay theo nắng. Trà phải khô đúng độ để giữ được hương mà không át đi vị. Người làm nghề canh từng mẻ bằng tay và bằng mũi, không thể vội.

Vì thế trà sen hiếm khi được làm số lượng lớn. Mỗi mẻ là một lần đánh cược với thời tiết, với độ nở của hoa, với cả tay nghề của người ướp.

Khi pha, nên dùng nước vừa sôi rồi để nguội bớt, tráng ấm trước, và rót thành nhiều lần nhỏ thay vì một lần đầy. Hương sen sẽ mở dần thay vì bung ra rồi tắt.

(Nội dung đang được biên tập.)'),

(3, N'Mỗi mùa Tết, thứ được chọn nhiều nhất thường không phải món cầu kỳ nhất, mà là món dễ trao đi nhất.

Hộp quà biếu đối tác cần chừng mực: màu trầm, chữ ít, không phô. Hộp tặng gia đình thì ngược lại — được phép rực rỡ, được phép có chi tiết vui mắt để trẻ con cũng thích.

Kích thước cũng quan trọng hơn nhiều người nghĩ. Hộp quá lớn khiến người nhận lúng túng khi mang về; hộp quá nhỏ lại dễ bị hiểu là làm cho có. Cỡ vừa tay, có quai xách, thường là lựa chọn an toàn.

Nếu tặng số lượng lớn, nên thống nhất một mẫu và chỉ đổi thiệp. Sự đồng bộ tự nó đã là một thông điệp.

(Nội dung đang được biên tập.)'),

(4, N'Furoshiki là cách gói quà bằng một tấm vải vuông, không dùng băng dính, không cần cắt bỏ thứ gì.

Điều đáng chú ý là tấm vải không bị xem như rác sau khi mở. Người nhận giữ lại, dùng tiếp, và món quà vì thế kéo dài thêm một đời sống nữa.

Cách gói cơ bản chỉ gồm vài nếp: đặt vật vào giữa, gấp hai góc đối diện phủ lên, rồi buộc hai góc còn lại thành nút. Vải mềm vừa phải sẽ giữ nếp tốt hơn vải quá trơn hoặc quá dày.

Tinh thần của cách gói này gần với điều HOÀI theo đuổi: phần vỏ không chỉ để bảo vệ, mà là một phần của món quà.

(Nội dung đang được biên tập.)'),

(20, N'Quà cuối năm là dịp hiếm hoi doanh nghiệp được nói chuyện với đối tác mà không bàn công việc.

Vì thế món quà nên nhẹ phần quảng bá. Logo in quá lớn biến món quà thành ấn phẩm truyền thông, và người nhận cảm nhận được điều đó ngay. Một dấu nhỏ, đặt đúng chỗ, thường đủ.

Nên thống nhất trước ba thứ: ngân sách cho mỗi phần, số lượng, và thời điểm giao. Quà đến sớm vài ngày luôn tốt hơn đến đúng hạn nhưng vội.

Nếu danh sách nhận có nhiều cấp khác nhau, hãy giữ chung một ngôn ngữ thiết kế và chỉ thay đổi quy mô. Cách đó tránh được sự so đo, thứ dễ làm hỏng thiện chí ban đầu.

(Nội dung đang được biên tập.)'),

(21, N'Gốm Bát Tràng gắn với một vùng đất, một dòng sông và nhiều thế hệ người làm nghề.

Đất được xử lý kỹ trước khi lên bàn xoay: nhào, lọc, để nghỉ. Bước nào vội thì sản phẩm sẽ tự khai ra khi nung — nứt, cong, hoặc men không bám đều.

Men là phần khiến mỗi lò một khác. Cùng một công thức, cùng một dáng, nhưng nhiệt độ và vị trí trong lò đủ tạo ra sắc độ riêng. Người trong nghề xem đó là chuyện thường, còn người mua lại thấy đấy là cái duyên.

Một món gốm dùng lâu sẽ đổi màu rất chậm theo tay người dùng. Đó là điều đồ sản xuất hàng loạt khó có được.

(Nội dung đang được biên tập.)'),

(22, N'Người nhận chạm vào bao bì trước khi chạm vào món quà. Ấn tượng đầu tiên hình thành ở đó.

Chất giấy quyết định phần lớn cảm giác. Giấy có độ nhám nhẹ cầm chắc tay hơn giấy bóng, và giữ màu in trầm hơn. Nếp gấp cũng vậy — gấp sắc thì hộp đứng dáng, gấp ẩu thì dù in đẹp vẫn thấy lệch.

Màu mực nên chọn theo ánh sáng nơi món quà được mở, chứ không chỉ theo màn hình thiết kế. Một sắc đỏ đẹp dưới đèn studio có thể trở nên gắt dưới đèn vàng trong nhà.

Và cuối cùng là chi tiết đóng mở. Hộp mở ra nhẹ nhàng khiến người nhận nấn ná lâu hơn — khoảng nấn ná ấy chính là lúc món quà làm xong việc của nó.

(Nội dung đang được biên tập.)');

UPDATE b SET b.Content = n.Content
FROM BlogPosts b JOIN @noi n ON n.Id = b.Id
WHERE ISNULL(b.Content, '') = '';

/* Bài cuối đang ẩn — bật lên để trang blog đủ 7 bài như thiết kế. */
UPDATE BlogPosts SET IsPublished = 1 WHERE IsPublished = 0;

COMMIT TRANSACTION;

SELECT Id, Title, LEN(ISNULL(Content, '')) AS SoKyTu, IsPublished
FROM BlogPosts ORDER BY Id;
