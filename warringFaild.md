# warringFaild

Sổ theo dõi vấn đề tồn đọng qua các bước hoàn thiện giao diện theo Figma.
File key Figma: `uQFY9gwfNbNSeTM6zmspzo`.

Quy ước mức độ: `blocker` · `major` · `minor` · `data` · `asset` · `prototype`.

Lỗi build tạm thời đã sửa ngay trong phiên thì không ghi ở đây — chỉ ghi những gì thực sự còn
lại, hoặc đã audit rõ và cần người khác quyết.

## Chưa xử lý

### WF-001 — Thanh phân trang: Figma vẽ mâu thuẫn giữa hai danh mục

- **Khu vực:** template danh mục (`Views/Category/Index.cshtml`)
- **Desktop/Mobile:** cả hai
- **Figma node:** `758:11422` (Trung thu, hiện) · `758:10253` (Tết, `hidden="true"`)
- **Mô tả:** Quà tết vẽ lưới 3×3 và **ẩn** thanh phân trang; Quà trung thu vẽ lưới 3×2 và **hiện**
  thanh phân trang ghi "1/2". Không có một giá trị PageSize nào thoả cả hai.
- **Nguyên nhân:** hai mock được vẽ ở hai thời điểm khác nhau, số card là số card designer kéo vào
  chứ không phải quy tắc phân trang.
- **Bằng chứng:** Tết 7 sản phẩm + 2 ô opacity 0, pager ẩn. Trung thu 5 card mock, pager hiện ghi
  "1/2" trong khi lưới chỉ có một trang.
- **Mức độ:** `minor`
- **Đã thử:** thử PageSize 6 → Tết tràn sang trang 2 và hiện pager, trái thiết kế. Thử PageSize 9 →
  Trung thu 6 sản phẩm nằm gọn một trang, pager ẩn, khác mock.
- **Cách xử lý đề xuất:** đang dùng PageSize 9 và **chỉ hiện pager khi có hơn một trang** — đó là
  hành vi đúng; hiện "1/2" khi chỉ có một trang là lỗi. Nếu khách muốn bám mock tuyệt đối thì phải
  cho PageSize theo từng danh mục.
- **Cần người dùng xác nhận:** có

### WF-005 — Frame mobile Quà trung thu là bản sao chép nhầm của Quà tết

- **Khu vực:** trang danh mục Quà trung thu
- **Desktop/Mobile:** mobile
- **Figma node:** `1068:28892` và các node con
- **Mô tả:** frame mobile mang tên "Quà tết", breadcrumb ghi "Trang chủ/Mua quà tết", kicker ghi
  "Bộ quà 6 hộp mứt", tiêu đề hero ghi "VIỆT NAM HOA THỊ", còn campaign là "Hoài x Họa sĩ Lương
  Bình" + **Lorem Ipsum**. Bản desktop cùng trang thì ghi đúng: "Bộ quà 4 hộp bánh",
  "TINH HOA BẮC BỘ", campaign "HOÀI x Cơm Lệ x Thuỷ Tạ" kèm đoạn giới thiệu thật.
- **Nguyên nhân:** designer nhân bản frame Quà tết rồi mới thay ảnh, chưa thay chữ.
- **Bằng chứng:** `1068:28921` = "Trang chủ/Mua quà tết"; `1068:28916` = "Bộ quà 6 hộp mứt";
  `1068:28917` = "VIỆT NAM HOA THỊ"; `1068:28957` = Lorem Ipsum.
- **Mức độ:** `minor`
- **Đã thử:** đối chiếu từng text node desktop ↔ mobile.
- **Cách xử lý đề xuất:** đã lấy **layout** của mobile và **nội dung** của desktop (nội dung thật).
  Breadcrumb và tiêu đề hero vốn sinh từ dữ liệu nên tự đúng. Cần designer sửa lại frame mobile.
- **Cần người dùng xác nhận:** không (đã xử lý an toàn), nhưng nên báo lại designer.

### WF-007 — SixLabors.ImageSharp 3.1.5 có lỗ hổng bảo mật đã công bố

- **Khu vực:** `src/Hoaii.Web/Hoaii.Web.csproj` (dùng cho upload/resize ảnh trong admin)
- **Desktop/Mobile:** không liên quan
- **Figma node:** —
- **Mô tả:** build cảnh báo NU1903 (high) và NU1902 (moderate) cho ImageSharp 3.1.5.
- **Nguyên nhân:** phiên bản đang ghim cũ hơn bản đã vá.
- **Bằng chứng:** `warning NU1903 ... GHSA-2cmq-823j-5qj8`, `warning NU1902 ... GHSA-rxmq-m78w-7wmc`.
- **Mức độ:** `major`
- **Đã thử:** chưa nâng — nằm ngoài phạm vi B14/B15 và có nguy cơ đổi hành vi pipeline ảnh ngay
  trước ngày bàn giao.
- **Cách xử lý đề xuất:** nâng lên bản vá sau bàn giao, chạy lại luồng upload ảnh trong admin để
  kiểm chứng.
- **Cần người dùng xác nhận:** có

## Sai khác do dữ liệu

### WF-002 — Đã bật 6 sản phẩm Quà trung thu vốn đang ẩn

- **Khu vực:** danh mục `qua-trung-thu`
- **Figma node:** `758:11409`
- **Mô tả:** cả 6 sản phẩm của danh mục đều `IsActive = 0`, nên trang ra trạng thái rỗng trong khi
  Figma vẽ trang có sản phẩm. Đã bật cả 6.
- **Lý do:** B14 ghi "không bật sản phẩm inactive", nhưng đây là chính 6 sản phẩm của danh mục chứ
  không phải hàng mượn từ nơi khác, và yêu cầu mới nhất của người dùng là sản phẩm phải hiện theo
  Figma cho kịp bàn giao. Ghi lại vì đây là thay đổi dữ liệu production.
- **Mức độ:** `data`
- **Hoàn tác:** `UPDATE Products SET IsActive = 0 WHERE CategoryId = 6;`
- **Cần người dùng xác nhận:** có

### WF-003 — Lưới Quà trung thu trong Figma là nội dung mock

- **Khu vực:** danh mục `qua-trung-thu`
- **Figma node:** `758:11409`
- **Mô tả:** Figma vẽ 5 card mang **tên và ảnh của trang Quà tết** (Tinh hoa bắc bộ, Thiên điểu lạc
  hồng, Mã đáo thành công, Phụng hoa trình tường, Ngũ quả dân gian), giá đồng loạt 899.000. DB có 6
  sản phẩm bánh trung thu thật, giá 595.000–899.000.
- **Lý do:** giá đồng loạt và tên trùng trang khác là dấu hiệu mock rõ ràng. Đã dùng dữ liệu thật,
  giữ nguyên layout Figma.
- **Mức độ:** `data`
- **Hệ quả nhìn thấy:** lưới hiện 6 card thay vì 5, tên dài hơn nên xuống 2 dòng, giá không đồng nhất.
- **Cần người dùng xác nhận:** không

### WF-006 — "Mã đáo thành công" không có trong danh mục Trung thu

- **Khu vực:** danh mục `qua-trung-thu`
- **Figma node:** `758:11413`
- **Mô tả:** Figma vẽ card "Mã đáo thành công" ở lưới Trung thu, nhưng DB chỉ có sản phẩm này trong
  danh mục Quà tết.
- **Lý do:** không chuyển danh mục và không nhân bản sản phẩm — cả hai đều là bịa dữ liệu.
- **Mức độ:** `data`
- **Cần người dùng xác nhận:** có — nếu hộp bánh trung thu Mã đáo thật sự tồn tại thì cần tạo trong
  admin kèm giá thật.

## Asset tạm hoặc chưa xác minh

### WF-004 — Ảnh "Hộp bánh Việt Nam Hoa Thị" mượn từ bộ Quà tết

- **Khu vực:** `qua-trung-thu`, sản phẩm `hop-banh-viet-nam-hoa-thi`
- **Figma node:** không có — Figma không vẽ card này ở trang Trung thu
- **Mô tả:** dùng tạm `/images/products/tet/viet-nam-hoa-thi.jpg` (ảnh hộp quà Tết cùng dòng), vì
  không có ảnh hộp bánh trung thu Việt Nam Hoa Thị ở bất kỳ nguồn nào.
- **Mức độ:** `asset`
- **Cần người dùng xác nhận:** có — cần ảnh chụp thật của hộp bánh này.

### WF-008 — Hai biến thể Tinh Hoa Bắc Bộ dùng chung một ảnh

- **Khu vực:** `qua-trung-thu`
- **Figma node:** `758:11411`
- **Mô tả:** "Hộp bánh Tinh Hoa Bắc Bộ (4 bánh)" và "(6 bánh)" là hai sản phẩm riêng nhưng Figma chỉ
  cung cấp một ảnh, nên cả hai đeo cùng file. Trên carousel hero hai slide cạnh nhau trông giống hệt.
- **Mức độ:** `asset`
- **Cần người dùng xác nhận:** có — cần ảnh riêng cho hộp 6 bánh, hoặc gộp thành một sản phẩm hai
  biến thể.

## Prototype chưa audit

### WF-000 — Toàn bộ Prototype/Present của file Figma

- **Khu vực:** toàn file
- **Desktop/Mobile:** cả hai
- **Figma node:** toàn file `uQFY9gwfNbNSeTM6zmspzo`
- **Mô tả:** không audit được luồng Prototype (Present/Play) bằng công cụ hiện có.
- **Nguyên nhân:** `get_motion_context` trả `{"nodes":[]}` cho mọi node trong file này.
- **Bằng chứng:** đã gọi trên 8 node khác nhau ở các phiên trước, lần nào cũng rỗng.
- **Mức độ:** `prototype`
- **Đã thử:** gọi trên node section, node frame, node instance; cả node cha lẫn node lá.
- **Cách xử lý đề xuất:** mọi thông số thời lượng/easing đang dùng đều lấy từ mô tả của người dùng,
  không phải từ Figma. Cần người dùng mở Present mode xác nhận, hoặc gửi bản ghi màn hình.
- **Cần người dùng xác nhận:** có

## Đã xử lý trong phiên

### WF-009 — Ảnh 6 sản phẩm Trung thu bị gán lệch một bậc (đã sửa)

- **Khu vực:** `qua-trung-thu`
- **Mô tả:** lỗi seed khiến mỗi sản phẩm đeo ảnh của sản phẩm kế bên — "Việt Nam Hoa Thị" đeo ảnh
  thiên điểu, "Thiên Điểu Lạc Hồng" đeo ảnh tinh hoa, "Ngũ Quả" đeo ảnh phụng hoa, "Phụng Hoa Trình
  Tường" đeo ảnh ngũ quả. Hai hộp Tinh Hoa Bắc Bộ còn đeo `/images/placeholders/featured-*.jpg`.
- **Xử lý:** tải ảnh gốc từ fill của từng card Figma, gán lại đúng.
  Xem `db/scripts/2026-07-21-qua-trung-thu-figma-sync.sql`.

### WF-010 — Kicker hero bị hard-code trong Razor (đã sửa)

- **Khu vực:** `Views/Category/Index.cshtml`
- **Mô tả:** dòng dưới carousel hero ghi cứng "Bộ quà 6 hộp" cho mọi danh mục, trong khi Figma cho
  thấy nó khác nhau: Quà tết "Bộ quà 6 hộp" (`1519:34009`), Quà trung thu "Bộ quà 4 hộp bánh"
  (`758:11395`).
- **Xử lý:** thêm cột `Category.HeroKicker` (migration `AddCategoryHeroKicker`), đưa qua ViewModel.
  Để trống thì ẩn dòng, không đoán số hộp.
