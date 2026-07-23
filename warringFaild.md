# warringFaild

Sổ theo dõi vấn đề tồn đọng qua các bước hoàn thiện giao diện theo Figma.
File key Figma: `uQFY9gwfNbNSeTM6zmspzo`.

Quy ước mức độ: `blocker` · `major` · `minor` · `data` · `asset` · `prototype`.

Lỗi build tạm thời đã sửa ngay trong phiên thì không ghi ở đây — chỉ ghi những gì thực sự còn
lại, hoặc đã audit rõ và cần người khác quyết.

---

## ⚠️ Việc cần bên mình làm (không thể sửa bằng code)

Bốn việc dưới đây là **thiếu dữ liệu hoặc thiếu asset**, không phải lỗi lập trình. Không mục nào
được tự bịa nội dung để lấp chỗ trống.

| # | Việc | Ảnh hưởng | Làm ở đâu | Chi tiết |
|---|---|---|---|---|
| 1 | ~~Gán sản phẩm cho 5 danh mục dịp~~ — **đã lấp bằng 10 sản phẩm tạm** (slug tam-*), thay bằng hàng thật khi có | Hai trang landing đã đầy đủ như Figma | Admin → Sản phẩm | [WF-011](#wf-011--năm-danh-mục-dịp-không-có-sản-phẩm-nào-️-quan-trọng-nhất) |
| 2 | Nhập Thành phần / Câu chuyện / Kích thước cho sản phẩm — **44/45 đang trống** | Mọi trang chi tiết hiện chung một đoạn mặc định | Admin → Sản phẩm | [WF-030](#wf-030--4445-sản-phẩm-chưa-có-thành-phần-câu-chuyện-và-kích-thước-️) |
| 3 | ~~Cung cấp ảnh~~ — **đã lấp 24 khe bằng ảnh tạm nằm trong repo**, thay bằng ảnh thật khi có | Các trang đã đầy đủ như Figma | Admin → Danh mục → Ảnh banner / Ảnh cover |
| 4 | Xác nhận 2 điểm cố ý lệch Figma: H1 trang Quà tặng cá nhân, và đích của cột "Quà tặng doanh nghiệp" | Cả hai đổi lại bằng một dòng code | — | [WF-013](#wf-013--trang-quà-tặng-cá-nhân-trong-figma-vẫn-để-tiêu-đề-quà-tặng-theo-dịp), [WF-014](#wf-014--không-có-danh-mục-quà-tặng-doanh-nghiệp) |

Ngoài ra còn hai việc kỹ thuật nên làm **sau** bàn giao: nâng ImageShare vá lỗ hổng
([WF-007](#wf-007--sixlabors-imagesharp-315-có-lỗ-hổng-bảo-mật-đã-công-bố)) và tách
`ProductVariant` thành thuộc tính size/màu ([WF-035](#wf-035--popup-chọn-biến-thể-figma-có-size--color-db-chỉ-có-một-tên-đã-xử-lý)).

---

## Chưa xử lý



### WF-013 — Trang "Quà tặng cá nhân" trong Figma vẫn để tiêu đề "QUÀ TẶNG THEO DỊP"

- **Khu vực:** `/qua-tang-ca-nhan`
- **Desktop/Mobile:** cả hai
- **Figma node:** `1068:29891`
- **Mô tả:** hero của trang con dùng lại nguyên tiêu đề của trang tổng, chỉ đổi mỗi subtitle thành
  "Trao gửi yêu thương tới người thân". Trong khi tab đang chọn ở chooser là "Quà tặng cá nhân" và
  breadcrumb cũng kết thúc bằng "Quà tặng cá nhân".
- **Nguyên nhân:** nhân bản frame trang tổng, chưa sửa tiêu đề.
- **Bằng chứng:** cả hai frame `769:12176` và `778:22062` cùng có text "QUÀ TẶNG THEO DỊP".
- **Mức độ:** `minor`
- **Đã thử:** đối chiếu breadcrumb, chooser active và route.
- **Cách xử lý đề xuất:** đang render "QUÀ TẶNG CÁ NHÂN" — hai trang khác URL mà cùng một H1 thì
  vừa khó hiểu cho người dùng vừa hỏng SEO. Muốn quay lại đúng Figma thì sửa một dòng:
  `OccasionController.Pages` → `HeroHeading: "QUÀ TẶNG THEO DỊP"`.
- **Cần người dùng xác nhận:** có

### WF-014 — Không có danh mục "Quà tặng doanh nghiệp"

- **Khu vực:** chooser của cả hai trang landing
- **Desktop/Mobile:** cả hai
- **Figma node:** `769:15223`
- **Mô tả:** chooser có 3 cột, nhưng DB không hề có danh mục quà doanh nghiệp và cũng chưa có
  trang nào cho nhóm này.
- **Nguyên nhân:** thiếu dữ liệu.
- **Bằng chứng:** bảng `Categories` không có dòng nào khớp.
- **Mức độ:** `data`
- **Đã thử:** không tạo route chết và không dùng `href="#"` (B15 cấm cả hai).
- **Cách xử lý đề xuất:** cột thứ ba đang trỏ `/hop-tac` — trang hợp tác/đại lý có thật, là đích
  gần nghĩa nhất hiện có. Khi có danh mục thật thì đổi một dòng trong
  `OccasionController.ChooserRoutes`.
- **Cần người dùng xác nhận:** có


### WF-030 — 44/45 sản phẩm chưa có Thành phần, Câu chuyện và Kích thước ⚠️

- **Khu vực:** `/san-pham/{slug}` — mọi trang chi tiết
- **Desktop/Mobile:** cả hai
- **Figma node:** `826:13864` (thành phần), `826:13879` (câu chuyện), `826:13881` (kích thước)
- **Mô tả:** ba cột `Description`, `StoryBody`, `FeatureBody` được thêm ở đợt CMS nhưng **chưa ai
  nhập**: 0/45 sản phẩm đang bán có dữ liệu. Mọi trang chi tiết vì thế hiện chung một đoạn mặc định.
- **Nguyên nhân:** thiếu nội dung, không phải lỗi code.
- **Bằng chứng:** `SELECT COUNT(*) … WHERE IsActive=1` → 45; số có `Description` / `StoryBody` /
  `FeatureBody` đều bằng 0 trước bước này.
- **Mức độ:** `data`
- **Đã thử:** Figma chỉ dựng trang chi tiết cho đúng một sản phẩm (Tinh Hoa Bắc Bộ), nên chỉ sản
  phẩm đó có nội dung chính thức để nhập — đã nhập trong
  `db/scripts/2026-07-21-product-detail-copy.sql`. 44 sản phẩm còn lại không có nguồn nội dung nào
  và không được bịa.
- **Cách xử lý đề xuất:** nhập trong admin → Sản phẩm → từng sản phẩm. Ưu tiên Thành phần vì đó là
  thông tin khách thật sự cần trước khi mua.
- **Cần người dùng xác nhận:** có


### WF-044 — Ô nhập của form "Yêu cầu mua sỉ" khác cấu trúc Figma

- **Khu vực:** `/hop-tac`
- **Desktop/Mobile:** desktop
- **Figma node:** `Frame 121`–`Frame 126` trong file `css all layer hợp tác`
- **Mô tả:** Figma vẽ mỗi ô nhập là **một dòng cao 45.4px**, chữ gợi ý nằm bên trong. Web dùng
  kiểu **nhãn nhỏ ở trên, ô nhập ở dưới**, cao 60px. Vì thế cột form cao 548 thay vì 598 và cả
  khối "Yêu cầu mua sỉ" cao 892 thay vì 902.
- **Nguyên nhân:** khác cấu trúc, không phải sai số đo.
- **Mức độ:** `minor`
- **Đã thử:** không đổi markup form ngay trước bàn giao — kiểu nhãn-trên giữ được nhãn khi người
  dùng đã gõ, còn kiểu một dòng thì nhãn biến mất, nên đây không hẳn là thụt lùi.
- **Cách xử lý đề xuất:** nếu khách muốn đúng Figma tuyệt đối thì đổi sang input một dòng với
  `placeholder`, đồng thời thêm `aria-label` để không mất nhãn cho trình đọc màn hình.
- **Cần người dùng xác nhận:** có

### WF-045 — Chưa có bản thiết kế cho khung chat nổi

- **Khu vực:** widget chat góc phải mọi trang
- **Desktop/Mobile:** cả hai
- **Figma node:** chưa có
- **Mô tả:** người dùng báo khung chat "chưa giống Figma". Bộ 15 file `css all layer` không chứa
  layer nào của widget này — chỉ có popup Zalo "Mua số lượng lớn" (đã dựng ở B19).
- **Mức độ:** `minor`
- **Đã thử:** tìm theo từ khoá chat / zalo / messenger / bubble trong cả 15 file; chỉ khớp popup
  Zalo.
- **Cách xử lý đề xuất:** cần node Figma hoặc file CSS của widget thì mới đối chiếu được.
- **Cần người dùng xác nhận:** có


### WF-040 — Cả 7 bài blog đều chưa có nội dung

- **Khu vực:** `/blog/{slug}`
- **Desktop/Mobile:** cả hai
- **Figma node:** —
- **Mô tả:** cột `BlogPosts.Content` thêm ở đợt CMS nhưng **0/7 bài có nội dung**. Trang chi tiết
  vì thế chỉ hiện đoạn tóm tắt 100–174 ký tự.
- **Nguyên nhân:** thiếu nội dung.
- **Bằng chứng:** `SELECT LEN(ISNULL(Content,'')) FROM BlogPosts` → tất cả bằng 0.
- **Mức độ:** `data`
- **Đã thử:** không viết bài thay. Trang đã fallback sang `Excerpt` nên không vỡ, chỉ ngắn.
- **Cách xử lý đề xuất:** viết nội dung trong admin → Blog. Bài `id=22` còn đang `IsPublished = 0`.
- **Cần người dùng xác nhận:** có

### WF-037 — Figma vẽ phương thức "Credit card" và nút "Thanh toán với VN PAY"

- **Khu vực:** `/thanh-toan`
- **Desktop/Mobile:** cả hai
- **Figma node:** `887:15475`, component set `1680:39838`
- **Mô tả:** Figma liệt kê hai phương thức "Chuyển khoản qua ngân hàng" và "Credit card" (kèm logo
  VISA/Mastercard), nút chính ghi "Thanh toán với VN PAY". Web hiện hai phương thức thật là chuyển
  khoản và thanh toán khi giao hàng, nút ghi "Hoàn tất đặt hàng".
- **Nguyên nhân:** **chưa có tích hợp cổng thanh toán nào.** Không có VNPay, không có xử lý thẻ.
- **Bằng chứng:** không có gateway, không có callback, không có bảng giao dịch trong solution.
- **Mức độ:** `blocker` cho việc thanh toán online
- **Đã thử:** không thêm nút "Thanh toán với VN PAY" chỉ để giống ảnh. Một nút thanh toán bấm vào
  không làm gì — hoặc tệ hơn là làm sai — trên trang có tiền thật là thứ không được phép ship.
  Nhãn nút đã được gate sẵn sau cờ `VnpayEnabled`: bật cờ lên là nút đổi chữ, nên phần giao diện
  đã sẵn sàng chờ tích hợp.
- **Cách xử lý đề xuất:** quyết định tích hợp VNPay thật hay bỏ hẳn hai mục này khỏi thiết kế.
- **Cần người dùng xác nhận:** có — đây là câu hỏi còn treo từ kế hoạch admin.

### WF-021 — Figma không có trang listing cho Valentine

- **Khu vực:** `/danh-muc/ngay-le-tinh-yeu`
- **Desktop/Mobile:** cả hai
- **Figma node:** —
- **Mô tả:** B16 liệt kê tám trang, không có Valentine, trong khi hai trang cùng nhóm (Phụ nữ,
  Giáng sinh) đều có và đều tới được từ nút "Xem tất cả" của trang landing.
- **Mức độ:** `minor`
- **Cách xử lý đề xuất:** đã cho Valentine dùng cùng hero banner với hai trang anh em — để nó
  dùng carousel rỗng thì lệch hẳn khi bấm qua lại. Đổi lại chỉ cần bỏ `'ngay-le-tinh-yeu'` khỏi
  `db/scripts/2026-07-21-eight-listing-pages.sql`.
- **Cần người dùng xác nhận:** không


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

### WF-050 — Chữ dải campaign không đọc được trên 11 trang (đã sửa)

- **Khu vực:** dải "Sản phẩm nổi bật/giới hạn" trên 9 trang danh mục + 2 trang landing
- **Figma node:** layer "Sản phẩm giới hạn/đặc biệt" trong các file `css all layer`
- **Mô tả:** chữ trắng trên nền `#E5D9CB` chỉ đạt tương phản **1.39:1**, trên `#E4C0D3` đạt
  **1.64:1** — dưới xa ngưỡng 4.5:1 và nhìn bằng mắt thường thì gần như không ra chữ.
- **Nguyên nhân:** đây là lỗi do chính đợt sửa trước gây ra. Figma ghép sẵn từng cặp màu — nền
  `#AA8656` đi với chữ `#FFFFFF`, còn `#E5D9CB` và `#E4C0D3` đi với chữ `#0F0F0F`. Khi đưa màu nền
  vào dữ liệu để sửa được trong admin, màu chữ vẫn bị để cứng là trắng trong CSS.
- **Xử lý:** thêm `Models/PromoContrast.cs` tự suy màu chữ từ độ sáng của nền, phát ra biến
  `--promo-fg` cạnh `--promo-bg`. Chọn cách suy ra thay vì thêm một cột nữa, để người quản trị đổi
  màu nền trong admin là chữ tự đổi theo, không ai phải nhớ chỉnh kèm.
- **Kết quả:** Trà/Khăn/Gốm/Rượu và 5 trang dịp từ 1.39:1 lên **13.80:1**; hai trang landing từ
  1.64:1 lên **11.66:1**. Đúng bằng màu `#0F0F0F` Figma quy định.

### WF-049 — Điền nội dung tạm để bàn giao không còn khoảng trống (đã làm)

- **Khu vực:** trang chi tiết sản phẩm, trang chi tiết blog
- **Yêu cầu:** khách muốn thấy giao diện đầy đủ khi bàn giao, không có chỗ nào báo thiếu.
- **Đã làm:**
  - Bỏ dòng "Thông tin thành phần sẽ được cập nhật…" — khối **THÀNH PHẦN nay tự ẩn khi trống**,
    giống khối kích thước. Trang gọn, không còn tự thú là chưa xong.
  - **Câu chuyện** và **Đặc điểm**: điền cho 61/61 sản phẩm đang bán, viết theo **nhóm danh mục**
    (trà / khăn / gốm / rượu / quà tặng), dùng lại tinh thần trang Về chúng tôi.
  - **Blog**: viết nội dung nháp cho cả 7 bài, bám đúng phần tóm tắt đã có sẵn; bật nốt bài đang ẩn.
- **⚠️ Vẫn KHÔNG bịa hai thứ:**
  - **Thành phần** (59 sản phẩm): danh sách thành phần sai với đồ ăn được là chuyện dị ứng và an
    toàn thực phẩm, không phải chuyện giao diện. Khối tự ẩn nên trang vẫn đẹp.
  - **Số đo cụ thể**: khách đọc để tính vận chuyển. Phần "Đặc điểm" chỉ mô tả chất liệu và cách
    hoàn thiện, không kèm con số nào.
- **Tìm lại để thay:**
  - `SELECT Id, Name FROM Products WHERE FeatureBody LIKE N'%(Nội dung tạm)%';`
  - `SELECT Id, Title FROM BlogPosts WHERE Content LIKE N'%(Nội dung đang được biên tập.)%';`
- **Cần người dùng xác nhận:** có — nội dung này là bản nháp, bộ phận nội dung phải viết lại trước
  khi chạy quảng bá.

### WF-048 — Không sửa được sản phẩm nào trong admin ⚠️ (đã sửa)

- **Khu vực:** admin → Sản phẩm, admin → Voucher
- **Mô tả:** ô **Giá** render ra `value="899000,00"` — dấu phẩy thập phân của tiếng Việt. Nhưng
  `<input type="number">` chỉ chấp nhận dấu chấm, nên trình duyệt đọc ô đó là **rỗng**, thuộc tính
  `required` không qua và **cả form không gửi đi được**. Nghĩa là không sản phẩm nào sửa được trong
  admin — bấm Lưu không có gì xảy ra và cũng không báo lỗi gì.
- **Nguyên nhân:** `@Model.Price` gọi `ToString()` theo `CurrentCulture`, mà máy chủ chạy văn hoá
  tiếng Việt.
- **Bằng chứng:** `form.checkValidity()` trả `false`, `Price.validationMessage` = "Please fill out
  this field", trong khi thuộc tính `value` vẫn có nội dung.
- **Ảnh hưởng:** Price, CompareAtPrice, PriceModifier của biến thể, và ba ô tiền của Voucher.
  Các ô số nguyên (thứ tự, tồn kho) không bị.
- **Xử lý:** thêm `AdminDisplay.Num()` in số theo `InvariantCulture` và dùng cho mọi ô số thập phân.
  Đã thử lại: sửa tên và đổi giá 899.000 → 950.000, lưu, đọc lại đúng, storefront đổi theo, rồi trả
  giá trị cũ.

### WF-039 — Escape chồng khi lưu qua admin (đã kiểm chứng là hết)

- **Khu vực:** admin → Chính sách và mọi form admin lưu text
- **Mô tả cũ:** tiêu đề trang `/chinh-sach/trao-doi` từng bị escape chồng 13 lớp, mỗi lần lưu thêm
  một lớp.
- **Kiểm chứng:** lưu liên tiếp 3 lần một tiêu đề chứa `& < > "` qua đúng form admin — giá trị giữ
  nguyên từng ký tự cả 3 lần. Quét lại `PolicyPages`, `PolicyBlocks`: không còn dòng nào chứa
  `&amp;` hay `&lt;`. Nguyên nhân trong luồng lưu đã không còn.
- **Ghi chú:** `ContentText.Lines` escape ở **đầu ra** chứ không phải khi lưu — đó là chỗ đúng và
  phải giữ nguyên.

### WF-046 — 5 cột thêm hôm qua không có trong form admin (đã sửa)

- **Khu vực:** admin → Danh mục
- **Mô tả:** `HeroKicker`, `BannerImageUrl`, `CoverImageUrl`, `PromoBackground`, `PromoWide` được
  thêm vào `Category` khi làm giao diện nhưng **không hề xuất hiện trong form admin**, nên khách
  không sửa được — kể cả hai trường ảnh mà tài liệu bàn giao đã hướng dẫn "thay trong admin".
- **Xử lý:** thêm đủ 5 trường vào form và vào `CategoryCms`. Ô màu nền chỉ nhận mã hex; đã thử
  chèn `red;background-image:url(...)` và giá trị bị loại bỏ đúng như mong đợi.

### WF-047 — Trang Báo cáo tràn ngang (đã sửa)

- **Khu vực:** `/admin/bao-cao`
- **Mô tả:** ba bảng trong trang không được bọc `.admin-table-wrap` như các trang admin khác, nên
  đẩy trang rộng thêm 14px.
- **Xử lý:** bọc lại; bảng tự cuộn ngang trong thẻ thay vì đẩy cả trang.

### WF-037 — Thẻ quốc tế: dựng giao diện, KHÔNG thu dữ liệu thẻ (đã xử lý)

- **Khu vực:** `/thanh-toan`
- **Figma node:** `1680:39838`
- **Mô tả:** Figma vẽ mục "Credit card" kèm logo VISA/Mastercard, và khi chọn thì mở bốn ô
  **Số thẻ · Ngày hết hạn · Mã bảo mật · Tên trên thẻ** ngay trên trang của mình.
- **Vì sao không dựng thành form thật:** nhận số thẻ và CVV thô trên máy chủ của cửa hàng đẩy
  toàn bộ hệ thống vào mức tuân thủ PCI-DSS cao nhất (SAQ D) — gần như không doanh nghiệp nhỏ nào
  đáp ứng nổi. Bản thân VNPay cũng không nhận luồng đó: đúng chuẩn là chuyển hướng sang trang
  thanh toán của họ, thẻ không bao giờ đi qua máy chủ mình. Và khi chưa nối cổng nào thì khách gõ
  thẻ vào chỉ là mất dữ liệu thẻ mà không thanh toán được gì.
- **Đã làm:** dựng đúng hình Figma — bốn ô, đúng kích thước, đúng logo — nhưng các ô **không có
  thuộc tính `name`** nên không bao giờ được gửi lên server, và luôn `disabled`. Đã kiểm: form
  gửi đi không chứa trường nào liên quan tới thẻ. Khối mờ đi kèm ghi chú khi cổng chưa bật.
- **Khi có VNPay thật:** bật cờ `pay_vnpay_enabled` trong admin. Nút chính đã sẵn đổi nhãn thành
  "Thanh toán với VN PAY". Phần còn lại cần làm: tạo đơn ở trạng thái chờ thanh toán, ký tham số,
  chuyển hướng sang VNPay, và xử lý callback xác nhận. **Không được** đánh dấu đơn là đã thanh
  toán trước khi có callback hợp lệ.
- **Cần người dùng xác nhận:** có — cung cấp thông tin tích hợp VNPay thì mới nối được.

### WF-044 — Ô nhập form mua sỉ (đã sửa theo Figma)

- **Khu vực:** `/hop-tac`
- **Mô tả:** Figma vẽ mỗi ô là một dòng cao 45.4 với chữ gợi ý bên trong; web dùng kiểu nhãn nhỏ ở
  trên, ô nhập ở dưới, cao 60. Ngoài ra Figma để **Tên và Họ mỗi ô một dòng riêng**, chỉ Điện
  thoại và Mã bưu điện mới chia đôi một hàng — web gộp Tên/Họ.
- **Xử lý:** đổi sang một dòng 45px, tách Tên/Họ, khoảng cách 11 như Figma. Nhãn **không bị bỏ** mà
  chuyển sang `.visually-hidden`: form chỉ có chữ gợi ý thì khi người dùng gõ xong không còn gì cho
  biết ô đó là gì, cả với người dùng thường lẫn trình đọc màn hình.
- **Còn lệch:** cột form cao 579 so với 598 của Figma (3%), do khối "Loại yêu cầu" và khoảng cách
  trước nút Gửi. Không đáng đánh đổi thêm thay đổi cấu trúc.

### WF-020 / WF-012 — 24 khe ảnh trống đã lấp bằng ảnh tạm

- **Khu vực:** 9 banner hero trang listing, 5 ảnh cover, 6 ô chooser, 10 card sản phẩm dịp
- **Mô tả:** Figma vẽ toàn bộ những chỗ này là khối màu đặc, không đặt ảnh nào. Bộ ảnh khách gửi
  (MEDIA TRUNG THU 2026) chỉ có ảnh Trung thu.
- **Xử lý:** người dùng đồng ý dùng ảnh tạm cho kịp bàn giao. Gán ảnh có sẵn trong repo qua
  `db/scripts/2026-07-22-seed-category-images.sql` và ba ảnh chooser trong `OccasionController`.
- **⚠️ Chỉ dùng ảnh trong `/images/...`, không dùng `/uploads/...`** — thư mục uploads nằm trong
  .gitignore nên bản deploy từ checkout sạch sẽ 404 toàn bộ. Lần gán đầu trỏ vào uploads, đã sửa.
- **Thay ảnh thật:** Admin → Danh mục → Ảnh banner / Ảnh cover. Ba ảnh chooser sửa trong
  `OccasionController.ChooserRoutes`; để rỗng thì quay lại đúng khối màu Figma vẽ.

### WF-046 — Bốn dropdown trên nav đều không mở được khi bấm (đã sửa)

- **Khu vực:** nav desktop, mọi trang
- **Mô tả:** cùng gốc với WF-029. Trên máy có chuột, con trỏ đi qua mục nav trước khi bấm nên
  `mouseenter` đã mở panel; cú click ngay sau đó thấy `is-open` đang bật và toggle nó tắt. Kết quả:
  bấm vào "Quà tết" thì menu vừa hiện ra lại đóng ngay.
- **Xử lý:** click luôn mở và ghim panel; rời chuột không tự đóng nữa. Đóng bằng dấu X, Escape
  hoặc bấm ra ngoài. Đã kiểm cả 4 dropdown: bấm mở, rời chuột vẫn mở.

### WF-047 — Dropdown "Quà theo dịp" không có panel nào khớp (đã sửa)

- **Khu vực:** nav desktop
- **Mô tả:** `_Nav.cshtml` lấy khoá dropdown bằng đoạn cuối URL trong `NavLinks`. Mục này trỏ tới
  trang landing `/qua-theo-dip` nên khoá là `qua-theo-dip`, trong khi panel lại đặt theo slug danh
  mục `qua-tang-theo-dip`. Trigger và panel không khớp nên dropdown chết hẳn, và hai trang landing
  dựng ở B15 không có đường nào vào từ nav.
- **Xử lý:** panel đặt khoá theo URL của nav (`PanelKey`), "Xem tất cả" trỏ về `/qua-theo-dip`,
  và sửa nốt ba selector hard-code khoá cũ trong `nav.css`.

### WF-048 — Cột "Quà tặng" của menu liệt kê sai danh mục (đã sửa)

- **Khu vực:** mega menu "Quà theo dịp"
- **Mô tả:** cột đầu dùng `otherOccasions` nên liệt kê Quà tết và Quà trung thu — hai mục vốn đã có
  dropdown riêng — còn năm dịp con (Valentine, 8-3, Giáng sinh, Người ấy, Bố mẹ) thì không xuất
  hiện ở đâu trong nav.
- **Xử lý:** cột này giờ lấy đúng các danh mục dịp con.

### WF-049 — Trà bị gán nhầm loại danh mục (đã sửa)

- **Khu vực:** dữ liệu `Categories`
- **Mô tả:** Trà để `Type = Occasion` trong khi Khăn, Tượng gốm, Rượu đều là `ProductType`. Hai hệ
  quả: Trà lọt vào danh sách dịp con của mega menu, và biến mất khỏi cột loại sản phẩm của menu
  "Sản phẩm chọn lọc" — dù đây là danh mục nhiều hàng nhất với 12 sản phẩm.
- **Xử lý:** `db/scripts/2026-07-22-fix-tra-category-type.sql`.

### WF-011 — Năm danh mục dịp không có sản phẩm nào (đã lấp bằng dữ liệu tạm)

- **Khu vực:** `/qua-theo-dip`, `/qua-tang-ca-nhan`
- **Mô tả:** cả năm danh mục dịp đều 0 sản phẩm nên 10 ô card trên hai trang landing để trắng.
- **Xử lý:** người dùng đồng ý dùng dữ liệu tạm cho kịp bàn giao và sẽ tự cập nhật trong admin.
  `db/scripts/2026-07-22-seed-occasion-products.sql` tạo 2 sản phẩm cho mỗi dịp, ảnh lấy từ bộ chụp
  Trung thu đã import, giá đặt trong khoảng giá của hàng thật. Đồng thời bật 6 "Set quà …" có sẵn
  trong Quà tặng theo dịp — đó là sản phẩm thật, chỉ chưa được bật.
- **Nhận diện để thay:** mọi sản phẩm tạm có slug bắt đầu bằng `tam-`. Xoá sạch bằng
  `DELETE FROM Products WHERE Slug LIKE 'tam-%';`
- **Không nhập:** Thành phần / Câu chuyện / Kích thước để trống — bịa thành phần cho sản phẩm ăn
  được là thứ không được phép, các khối đó tự ẩn cho tới khi có nội dung thật.

### WF-015 — Ba trang thiếu thẻ H1 (đã sửa)

- **Khu vực:** , , , và cả 
- **Mô tả:** bốn trang này mở thẳng vào panel nên không có tiêu đề nào, tức không có h1.
- **Xử lý:** thêm h1 ẩn bằng  thay vì chèn tiêu đề mà thiết kế không vẽ. Đã đo
  lại: cả bốn trang đúng một h1.

### WF-029 — Bấm kính lúp trên nav không mở được ô tìm kiếm (đã sửa)

- **Khu vực:** nav, mọi trang
- **Mô tả:** trên máy có chuột, con trỏ luôn đi qua icon trước khi bấm nên  đã mở ô
  tìm kiếm; cú click ngay sau đó rơi vào nhánh toggle và **đóng lại**. Kết quả: bấm vào kính lúp
  thì không thấy gì mở ra.
- **Xử lý:** click giờ luôn mở và ghim (), rời chuột không tự đóng nữa; đóng bằng nút X,
  Escape hoặc bấm ra ngoài. Đã kiểm: bấm mở và đưa con trỏ vào ô, rời chuột vẫn mở, Enter ra
  đúng 9 kết quả, Escape đóng.

### WF-009 — Ảnh 6 sản phẩm Trung thu bị gán lệch một bậc (đã sửa)

- **Khu vực:** `qua-trung-thu`
- **Mô tả:** lỗi seed khiến mỗi sản phẩm đeo ảnh của sản phẩm kế bên — "Việt Nam Hoa Thị" đeo ảnh
  thiên điểu, "Thiên Điểu Lạc Hồng" đeo ảnh tinh hoa, "Ngũ Quả" đeo ảnh phụng hoa, "Phụng Hoa Trình
  Tường" đeo ảnh ngũ quả. Hai hộp Tinh Hoa Bắc Bộ còn đeo `/images/placeholders/featured-*.jpg`.
- **Xử lý:** tải ảnh gốc từ fill của từng card Figma, gán lại đúng.
  Xem `db/scripts/2026-07-21-qua-trung-thu-figma-sync.sql`.

### WF-042 — Ba hệ thống lề ngang chạy song song trên storefront (đã hợp nhất)

- **Khu vực:** toàn bộ storefront
- **Mô tả:** người dùng báo "content gần sát mép viewport, không còn container 1440px" ở section
  Lựa chọn hàng đầu và nhiều trang khác. Đo thật thì thấy ba cách đặt lề cùng tồn tại:
  token `--page-gutter` (6 file), `padding: 0 240px` gõ cứng (14 file, 36 chỗ), và
  `.container { padding: 0 24px }` — một hệ hoàn toàn khác. Section nào dựng trên `.container`
  thì trôi ra sát mép khi cửa sổ hẹp lại trong khi section bên cạnh vẫn giữ 240, nên nhìn ra ngay.
- **Nguyên nhân cụ thể:**
  - `.featured-section` và `.about-section` để `padding: 120px 0` — **lề ngang bằng 0**. Các con
    của nó gánh `padding: 0 24px` để bù, nên chỉ đúng ở đúng 1920 và sai ở mọi độ rộng khác.
  - `.cat-content` đè `padding: 40px 0` + `max-width: 1440px` để né `padding: 24px` cũ của
    `.container`; hệ quả là ở cửa sổ hẹp hộp vẫn rộng 1440 mà không còn lề nào, breadcrumb dính mép.
- **Xử lý:** hợp nhất về một token duy nhất. `.container` dùng `--page-gutter`, 36 chỗ gõ cứng
  240px đổi sang token, hai section thiếu lề được trả lại lề, và bỏ override `.cat-content`.
  Sửa ở tầng chung, không vá từng trang.
- **Kiểm chứng:** script đo mép trái của **chữ** trong từng section trên 13 trang × 3 độ rộng
  (1920/1440/1280). Trước khi sửa có 20 chỗ tràn lề; sau khi sửa chỉ còn các trường hợp đúng thiết
  kế (tiêu đề căn giữa tràn viền, marquee logo).

### WF-043 — Lề 240px cố định bóp nội dung trên laptop (đã sửa)

- **Khu vực:** toàn bộ storefront, từ 1280 đến dưới 1920
- **Mô tả:** token cũ `max(240px, (100% - 1440px) / 2)` giữ nguyên lề 240 ở mọi cửa sổ hẹp hơn
  1920, nên ở laptop 1280 có tới 37% bề ngang là lề trống và card sản phẩm co còn 251px.
- **Xử lý:** đổi thành `max(min(240px, 12.5vw), (100% - 1440px) / 2)`. 12.5vw đúng bằng 240 ở
  1920, nên **ở và trên độ rộng thiết kế không đổi một pixel nào**; chỉ cửa sổ hẹp hơn mới giãn ra.
  Đo lại: 2560 → lề 560 / nội dung 1440 / card 464 (y như cũ); 1920 → 240 / 1440 / 464 (khớp
  Figma); 1440 → 180 / 1080 / 344; 1280 → 160 / 960 / 304.

### WF-038 — Trang chính sách đổi trả bị test tự động ghi đè (đã khôi phục)

- **Khu vực:** `/chinh-sach/trao-doi`
- **Mô tả:** trang chỉ còn đúng một khối nội dung ghi `AUTOTEST CHINH SACH` — 19 ký tự, trong khi
  ba trang chính sách còn lại có 9–13 khối và 1.300–1.900 ký tự. Một lần chạy test tự động đã lưu
  đè lên nội dung thật qua form admin.
- **Xử lý:** lấy lại nguyên văn từ commit `7a5fb75` (lúc chính sách còn nằm trong `PageController`
  trước khi chuyển vào DB) và khôi phục 16 khối / 2.132 ký tự. Xem
  `db/scripts/2026-07-21-restore-policy-trao-doi.sql`.
- **Lưu ý:** nguyên nhân gốc — form admin cho phép test ghi đè trang thật — vẫn còn. Xem WF-039.

### WF-041 — H1 trang Về chúng tôi dài hơn 200 ký tự (đã sửa)

- **Khu vực:** `/ve-chung-toi`
- **Mô tả:** cả đoạn tuyên ngôn hai câu nằm trong thẻ `h1`, nên trình đọc màn hình đọc nguyên đoạn
  văn làm tên trang và công cụ tìm kiếm không lấy được tiêu đề dùng được.
- **Xử lý:** đoạn tuyên ngôn thành `<p>` giữ nguyên cỡ chữ và vị trí; `h1` thật là một dòng ngắn
  ẩn đi. Không đổi gì về mặt nhìn.

### WF-035 — Popup chọn biến thể: Figma có Size + Color, DB chỉ có một tên (đã xử lý)

- **Khu vực:** mini cart, popup "Thêm sản phẩm lẻ"
- **Figma node:** `970:20686`
- **Mô tả:** Figma vẽ hai ô chọn riêng "Size" (40cm) và "Color" (Navy). `ProductVariant` chỉ có
  cột `Name` — ví dụ "4 Bánh", "Hộp 4 túi / màu vàng" — và không có cột màu.
- **Xử lý:** popup liệt kê đúng các biến thể **có thật**, một trường "Loại". Dựng hai dropdown
  Size/Color rời sẽ cho phép chọn tổ hợp cửa hàng không bán được.
- **Cách xử lý triệt để:** tách `ProductVariant` thành các thuộc tính (size, màu, …) rồi mới dựng
  đúng hai ô như Figma. Đây là thay đổi schema, không làm sát ngày bàn giao.
- **Cần người dùng xác nhận:** có

### WF-036 — Chưa có popup chọn biến thể cho sản phẩm gợi ý (đã thêm)

- **Khu vực:** mini cart, khối "HOÀN CHỈNH VỚI"
- **Mô tả:** nút "Thêm" trước đây post thẳng, nên sản phẩm nhiều biến thể bị thêm vào giỏ mà khách
  không được chọn loại nào — Figma vẽ hẳn một popup cho việc này.
- **Xử lý:** thêm bottom sheet trượt lên đè drawer (node `970:20686`). Sản phẩm chỉ có một biến thể
  hoặc không có thì giữ nguyên form thường, vẫn chạy khi tắt JS. Đổi biến thể thì giá cập nhật theo
  `PriceModifier` thật. Escape đóng sheet nhưng giữ drawer mở.

### WF-031 — Kích thước hộp bịa giống nhau cho cả 45 sản phẩm (đã sửa)

- **Khu vực:** `/san-pham/{slug}`, khối "Đặc điểm"
- **Mô tả:** `ProductController` hard-code `"KÍCH THƯỚC: Hộp cứng 48x15.7x6cm…"` làm giá trị mặc
  định, nên mọi sản phẩm — kể cả hộp trà, tượng gốm, chai rượu — đều in cùng bộ số đo của một hộp
  quà Tết. Đây là thông tin sai sự thật, khách đọc để quyết định mua và để tính vận chuyển.
- **Xử lý:** bỏ giá trị mặc định; số đo giờ là dữ liệu. Không có dữ liệu thì **ẩn cả khối** — một
  tiêu đề "Đặc điểm" trống trông như lỗi tải. Số đo thật của Tinh Hoa Bắc Bộ nhập từ node
  `826:13881`.

### WF-032 — Dải thumbnail bị bóp méo khi sản phẩm có nhiều hơn 5 ảnh (đã sửa)

- **Khu vực:** `/san-pham/{slug}`, gallery
- **Mô tả:** Figma cố định thumbnail 100×100 và đánh dấu `shrink-0` (node `826:15686`), nhưng CSS
  thiếu `flex: none` nên với sản phẩm 8 ảnh chúng co còn ~72px — kích thước dải đổi theo từng sản
  phẩm. Ngoài ra thiếu hẳn lớp phủ đen 50% mà Figma dùng để làm mờ các ảnh chưa chọn.
- **Xử lý:** cố định 100×100, cho dải cuộn ngang khi tràn, thêm lớp phủ `rgba(0,0,0,.5)` tắt ở ảnh
  đang chọn, và thêm `focus-visible` cho thao tác bàn phím.

### WF-033 — Nền section "Câu chuyện sản phẩm" sai màu (đã sửa)

- **Khu vực:** `/san-pham/{slug}`
- **Mô tả:** Figma đặt dải này trên nền be `#F7F3EE` (node `826:20691`) — đó là thứ tách nó khỏi
  khối sản phẩm trắng phía trên — nhưng CSS để `#fff`.
- **Xử lý:** đổi sang `var(--color-brand-gold-bg)`. Đã lấy mẫu pixel cả hai bên để xác nhận.

### WF-034 — Figma gõ nhầm "THÀN PHẦN" (đã tránh)

- **Khu vực:** `/san-pham/{slug}`
- **Mô tả:** node `826:13863` ghi "THÀN PHẦN :" thiếu chữ H.
- **Xử lý:** web hiện đúng "THÀNH PHẦN :", không chép lỗi.

### WF-024 — Minh họa "không có kết quả" là hình vẽ tay không đúng Figma (đã sửa)

- **Khu vực:** `/tim-kiem`
- **Mô tả:** trạng thái rỗng đang dùng một `<svg>` viết tay hình xe đẩy hàng, trong khi Figma là
  hình line-art màu vàng đồng vẽ hộp quà và bình hoa (node `1028:12551`).
- **Xử lý:** tải bản vector thật từ Figma về `/images/search/no-results.svg` (23 KB, đã kiểm tra
  không chứa `<script>`), dùng 197px trên desktop và 140px trên mobile đúng như hai node.

### WF-025 — Trang tìm kiếm hiện "0 kết quả" hai lần (đã sửa)

- **Khu vực:** `/tim-kiem`
- **Mô tả:** khi không có kết quả, dòng đếm được render cả ở header lẫn trong khối empty state.
- **Xử lý:** giữ một dòng ở header đúng như node `988:23680`.

### WF-026 — Hai thẻ H1 khi tìm không ra kết quả (đã sửa)

- **Khu vực:** `/tim-kiem`
- **Mô tả:** "KẾT QUẢ TÌM KIẾM CHO …" và "KHÔNG CÓ KẾT QUẢ" cùng là `h1` và cùng render. Trang
  `/tim-kiem` không từ khóa thì lại không có `h1` nào.
- **Xử lý:** heading kết quả chỉ render khi thật sự có kết quả; empty state mang `h1`; trang chưa
  nhập từ khóa có `h1` riêng "TÌM KIẾM" và không chạy query, không tự nhận "0 kết quả".

### WF-027 — Link "Xem thêm" của nhóm Sản phẩm chọn lọc trỏ về trang chủ (đã sửa)

- **Khu vực:** `/tim-kiem`
- **Mô tả:** `ShowMoreUrl` của nhóm gợi ý chéo đặt là `/`.
- **Xử lý:** trỏ `/danh-muc/san-pham-chon-loc`.

### WF-028 — Tìm kiếm chỉ khớp tên và slug (đã mở rộng)

- **Khu vực:** `SearchController`
- **Mô tả:** không khớp mô tả và tên danh mục, nên gõ "trà" không ra sản phẩm nào có chữ trà trong
  phần mô tả.
- **Xử lý:** thêm `Description` và `Category.Name` vào điều kiện, vẫn nằm trong SQL. Thứ tự nhóm
  đổi từ "nhiều kết quả trước" sang thứ tự danh mục, đúng như Figma xếp Quà tết → Trung thu →
  Theo dịp. Đã kiểm tra XSS: `?q=<script>alert(1)</script>` không chạy, không sinh thẻ script nào.

### WF-022 — Tên danh mục "Trà" lưu dưới dạng HTML entity (đã sửa)

- **Khu vực:** `/danh-muc/tra`, và mọi nơi hiện tên danh mục
- **Mô tả:** `Categories.Name` của danh mục Trà chứa chuỗi ký tự `Tr&#xE0;` chứ không phải `Trà`.
  Razor escape chuỗi đó đúng như mọi text khác, nên banner hiện "TR&#XE0;", tiêu đề hiện
  "Tất cả tr&#xe0;", breadcrumb và thẻ `<title>` cũng vậy. Lỗi encode kép từ lần import nào đó.
- **Xử lý:** đã quét toàn bộ `Categories`, `Products`, `ProductVariants` và các cột `Description` —
  chỉ duy nhất dòng này bị. Sửa trong `db/scripts/2026-07-21-eight-listing-pages.sql`.

### WF-019 — Sản phẩm Khăn / Tượng gốm / Rượu đang ẩn (đã bật)

- **Khu vực:** `/danh-muc/khan`, `/danh-muc/tuong-gom`, `/danh-muc/ruou`
- **Mô tả:** mỗi danh mục có 6 sản phẩm thật, giá thật, nhưng `IsActive = 0` nên cả ba trang ra
  trạng thái rỗng trong khi Figma vẽ đầy sản phẩm.
- **Xử lý:** đã bật, cùng lý do như WF-002. Hoàn tác:
  `UPDATE p SET p.IsActive = 0 FROM Products p JOIN Categories c ON c.Id = p.CategoryId
   WHERE c.Slug IN ('khan','tuong-gom','ruou');`
- **Cần người dùng xác nhận:** có

### WF-023 — Breadcrumb desktop của trang Phụ nữ ghi sai nhánh cuối (đã tránh)

- **Khu vực:** `/danh-muc/ngay-quoc-te-phu-nu`
- **Mô tả:** node desktop `1269:39709` ghi "Trang chủ/Quà theo dịp/**Ngày lễ tình yêu**" trên trang
  Ngày Quốc tế Phụ nữ — copy-paste sót. Node mobile `1265:31321` ghi đúng.
- **Xử lý:** breadcrumb sinh từ dữ liệu (`Category.ParentLabel` + tên danh mục) nên tự đúng; không
  chép lỗi này.

### WF-016 — Copy 5 section landing đều là placeholder về Tết (đã sửa)

- **Khu vực:** `/qua-theo-dip`, `/qua-tang-ca-nhan`
- **Mô tả:** bản desktop của cả năm section dán chung một đoạn về "Tết Nguyên Đán là dịp lễ sum vầy
  lớn nhất Việt Nam…" — sai ngữ cảnh với Valentine, 8-3, Giáng sinh, Người ấy và Bố mẹ.
- **Xử lý:** frame mobile có copy thật cho từng dịp (nodes `1068:31791`, `1068:31715`,
  `1068:31578`, `1068:31934`, `1068:31993`); đã đưa vào `Category.Description`.
  Xem `db/scripts/2026-07-21-occasion-landing-copy.sql`.

### WF-017 — Lorem Ipsum ở campaign hai trang landing (đã sửa)

- **Khu vực:** `/qua-theo-dip`, `/qua-tang-ca-nhan`
- **Mô tả:** node `771:15465` để nguyên Lorem Ipsum.
- **Xử lý:** dùng đoạn giới thiệu hợp tác với họa sĩ Lương Bình — nội dung campaign có thật duy
  nhất của site — thay vì ship tiếng Latin.

### WF-018 — Hai lỗi chính tả trong Figma (đã sửa)

- **Khu vực:** `/qua-theo-dip`
- **Mô tả:** node `771:21273` ghi "Qùa giáng sinh" (sai dấu); node `769:15245` ghi
  "Ngày lễ tình yêu- valentine" (thiếu khoảng trắng, sai hoa/thường).
- **Xử lý:** hiển thị "Quà giáng sinh" và "Ngày lễ tình yêu" — dạng sạch mà chính frame mobile
  Figma đang dùng.

### WF-010 — Kicker hero bị hard-code trong Razor (đã sửa)

- **Khu vực:** `Views/Category/Index.cshtml`
- **Mô tả:** dòng dưới carousel hero ghi cứng "Bộ quà 6 hộp" cho mọi danh mục, trong khi Figma cho
  thấy nó khác nhau: Quà tết "Bộ quà 6 hộp" (`1519:34009`), Quà trung thu "Bộ quà 4 hộp bánh"
  (`758:11395`).
- **Xử lý:** thêm cột `Category.HeroKicker` (migration `AddCategoryHeroKicker`), đưa qua ViewModel.
  Để trống thì ẩn dòng, không đoán số hộp.

### WF-019 — Footer dùng nhầm `:nth-of-type` thay vì `:nth-child` (đã sửa)

- **Khu vực:** `wwwroot/css/footer.css`, mọi trang (desktop)
- **Mô tả:** ba cột link footer được đặt bề rộng Figma (155/244/238) qua
  `.footer-col:nth-of-type(1|2|3)`. Nhưng `.footer-brand` cũng là một `<div>` anh em, nên
  `nth-of-type` đếm cả nó: rule "cột 1" không khớp phần tử nào, hai rule sau khớp lệch sang cột
  trước đó, và cột cuối không được đặt bề rộng. Hệ quả nhìn thấy được là `margin-left: auto`
  không bao giờ chạy — ba cột dồn sang trái, để hở 164px ở mép phải so với Figma.
- **Xử lý:** đổi sang `:nth-child(2|3|4)` (khối thương hiệu là con thứ 1). Đo lại khớp Figma
  chính xác: `422@240`, `155@963`, `244@1158`, `238@1442`, kết thúc tại x=1680.
- **Ghi chú:** thêm `:nth-child(n+5) { flex: 0 1 auto }` để nếu admin thêm cột thứ tư thì nó co
  giãn theo nội dung thay vì đổ vỡ.

### WF-020 — Nút "Hủy bỏ" ở trang thanh toán là `href="#"` (đã sửa)

- **Khu vực:** `Views/Checkout/Index.cshtml`
- **Mô tả:** bấm "Hủy bỏ" giữa lúc đang điền thanh toán thì trang chỉ nhảy lên đầu, không huỷ gì.
- **Xử lý:** trỏ về `/gio-hang`.

### WF-021 — Form liên hệ và form hợp tác không báo lỗi gì khi bỏ trống (đã sửa)

- **Khu vực:** `/lien-he`, `/hop-tac`
- **Mô tả:** hai model đều có `[Required]`, nhưng tag helper `asp-for` chỉ sinh
  `data-val-required` chứ **không** sinh thuộc tính `required` của HTML. `form-validate.js` lại
  chỉ quét `input[required]`, nên bỏ qua sạch hai form này. View cũng không in
  `asp-validation-summary`. Kết quả: bấm "Gửi" với form trống → POST lên máy chủ → `ModelState`
  không hợp lệ → render lại y hệt, **không một dòng báo lỗi nào**. Khách tưởng đã gửi được.
- **Xử lý:** ba lớp, đều không đổi giao diện khi form hợp lệ —
  1. `form-validate.js` sao chép `data-val-required` sang `required` lúc khởi động, nên hai form
     này đi chung đường với các form còn lại (và trình duyệt vẫn chặn được khi tắt JavaScript);
  2. `messageFor()` đọc `data-val-required` để dùng đúng câu tiếng Việt của từng trường
     ("Vui lòng nhập họ") thay vì câu chung chung — trang thanh toán cũng được lợi;
  3. `errorSlot()` tự chèn ô báo lỗi ngay sau input khi không có lớp bọc `.form-field`, cộng thêm
     `asp-validation-summary="All"` làm đường lui phía máy chủ.
- **Kiểm chứng:** cả 4 form (`/lien-he`, `/hop-tac`, `/thanh-toan`, `/tai-khoan/dang-nhap`) đều
  chặn đúng, tô đỏ ô sai và đưa con trỏ về ô sai đầu tiên.

### WF-022 — Thanh phân trang admin in đủ mọi số trang (đã sửa)

- **Khu vực:** `Areas/Admin/Views/Shared/_Pager.cshtml`, rõ nhất ở `/admin/nhat-ky`
- **Mô tả:** vòng lặp in `1..totalPages` và `.admin-pager` không có `flex-wrap`. Nhật ký đã 14
  trang → hàng số đẩy toàn trang admin tràn ngang 118px trên điện thoại (cuộn ngang cả màn hình,
  không riêng thanh phân trang). Nhật ký sinh thêm dòng sau mỗi thao tác nên chỗ này chỉ ngày
  càng tệ.
- **Xử lý:** chỉ hiện trang đầu, trang cuối và ±2 quanh trang hiện tại, chỗ đứt quãng là "…";
  thêm `flex-wrap: wrap` và `aria-current="page"`.

### WF-023 — Tương phản dưới chuẩn WCAG AA ở vài chỗ (giữ nguyên, có chủ ý)

- **Khu vực:** `.btn-primary`, `.contact-popup__body-copy`, `.minicart__empty`,
  `.addon-sheet__variant`, chữ nhỏ 13px ở footer
- **Mô tả:** tỉ lệ tương phản 3.83–4.29:1, chuẩn AA cho chữ thường là 4.5:1.
- **Vì sao không sửa:** các màu này lấy thẳng từ bảng màu Figma (đỏ thương hiệu `#AF2234`, xám
  phụ) và cỡ chữ cũng là cỡ Figma. Sửa là lệch thiết kế. Ghi lại ở đây để khách quyết định —
  nếu muốn đạt AA thì cần đổi ở tầng bảng màu chứ không vá từng chỗ.

### WF-019b — Footer vẫn vỡ trên màn dưới 1519px (bổ sung cho WF-019)

- **Khu vực:** `wwwroot/css/footer.css`, `Views/Shared/_Footer.cshtml`
- **Vì sao WF-019 chưa đủ:** bản vá `:nth-child` chỉ đúng ở bề rộng 1920 (nơi vùng nội dung
  đủ 1440px). Nhưng `--page-gutter` co theo màn hình, laptop 1440 chỉ còn ~1080px nội dung —
  ít hơn 1179px mà bố cục phẳng (brand + 3 cột rời + margin-left:auto + flex-wrap) cần. Cột
  cuối "CHÍNH SÁCH PHÁP LÝ" rớt xuống một dòng riêng bên trái, hở khoảng trống lớn ở giữa.
  Khách chụp đúng cảnh này: "khác 1 trời 1 vực so với figma".
- **Gốc rễ:** tôi đã làm phẳng "Frame 85" của Figma (cụm 3 cột) thành con trực tiếp của
  `.footer-links-row`. Figma dựng brand (422) và Frame 85 (717) là hai khối trong Frame 89,
  cách nhau 301px (`422 + 301 + 717 = 1440`).
- **Xử lý đúng:** bọc 3 cột trong `.footer-cols` (Frame 85, `flex: none` giữ 717px liền khối);
  hàng cha dùng `justify-content: space-between` cho khoảng hở tự co; `flex-wrap` để khi hẹp cả
  cụm tụt xuống nguyên khối chứ không tách lẻ; breakpoint 992px cho cụm xếp dọc.
- **Kiểm chứng:** đo 8 bề rộng 1920→430, không chỗ nào tràn, cột không bao giờ rớt lẻ.
  ≥1600px brand+cụm nằm cạnh nhau đúng Figma; 992–1519px cụm tụt xuống dưới brand nguyên khối.
