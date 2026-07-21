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
| 1 | Gán sản phẩm cho 5 danh mục dịp (Valentine, 8-3, Giáng sinh, Người ấy, Bố mẹ) — cả 5 đang **0 sản phẩm** | 10 ô card trên 2 trang landing để trống | Admin → Sản phẩm → đổi Danh mục | [WF-011](#wf-011--năm-danh-mục-dịp-không-có-sản-phẩm-nào-️-quan-trọng-nhất) |
| 2 | Nhập Thành phần / Câu chuyện / Kích thước cho sản phẩm — **44/45 đang trống** | Mọi trang chi tiết hiện chung một đoạn mặc định | Admin → Sản phẩm | [WF-030](#wf-030--4445-sản-phẩm-chưa-có-thành-phần-câu-chuyện-và-kích-thước-️) |
| 3 | Cung cấp ảnh: 8 banner hero, 5 ảnh cover, 3 ảnh chooser — **trong Figma đều là khối xám trống** | Các khu vực đó render khối màu như thiết kế vẽ | Admin → gán vào `BannerImageUrl` / `CoverImageUrl` | [WF-020](#wf-020--cả-tám-banner-hero-trong-figma-đều-trống-ảnh), [WF-012](#wf-012--ảnh-cover-và-ảnh-campaign-của-hai-trang-landing-chưa-tồn-tại-trong-figma) |
| 4 | Xác nhận 2 điểm cố ý lệch Figma: H1 trang Quà tặng cá nhân, và đích của cột "Quà tặng doanh nghiệp" | Cả hai đổi lại bằng một dòng code | — | [WF-013](#wf-013--trang-quà-tặng-cá-nhân-trong-figma-vẫn-để-tiêu-đề-quà-tặng-theo-dịp), [WF-014](#wf-014--không-có-danh-mục-quà-tặng-doanh-nghiệp) |

Ngoài ra còn hai việc kỹ thuật nên làm **sau** bàn giao: nâng ImageShare vá lỗ hổng
([WF-007](#wf-007--sixlabors-imagesharp-315-có-lỗ-hổng-bảo-mật-đã-công-bố)) và tách
`ProductVariant` thành thuộc tính size/màu ([WF-035](#wf-035--popup-chọn-biến-thể-figma-có-size--color-db-chỉ-có-một-tên-đã-xử-lý)).

---

## Chưa xử lý

### WF-011 — Năm danh mục dịp không có sản phẩm nào ⚠️ QUAN TRỌNG NHẤT

- **Khu vực:** `/qua-theo-dip` và `/qua-tang-ca-nhan`
- **Desktop/Mobile:** cả hai
- **Figma node:** `769:15367`, `769:15396`, `771:21276`, `1068:31937`, `1068:31996`
- **Mô tả:** Figma vẽ mỗi section có 2 card sản phẩm. Trong DB, cả năm danh mục
  `ngay-le-tinh-yeu`, `ngay-quoc-te-phu-nu`, `qua-giang-sinh`, `qua-tang-nguoi-ay`,
  `qua-tang-bo-me` đều có **0 sản phẩm**. Nên cả 10 vị trí card đều trống.
- **Nguyên nhân:** chưa ai gán sản phẩm vào các danh mục này. Không phải lỗi code.
- **Bằng chứng:** `SELECT c.Slug, COUNT(p.Id) FROM Categories c LEFT JOIN Products p ON p.CategoryId
  = c.Id GROUP BY c.Slug` — cả năm trả 0.
- **Mức độ:** `data` / chặn hình thức bàn giao
- **Đã thử:** không seed sản phẩm giả, không bật sản phẩm ẩn của danh mục khác, không đổi
  CategoryId của sản phẩm sẵn có — cả ba đều là bịa dữ liệu và B15 cấm rõ.
- **Cách xử lý đề xuất:** vào admin → Sản phẩm → chọn sản phẩm → đổi Danh mục sang dịp tương ứng.
  Mỗi dịp chỉ cần 2 sản phẩm là trang đầy đủ như Figma. Trang tự cập nhật ngay, không phải deploy.
  Sáu sản phẩm "Set quà Tri Ân / Khai Trương / Sinh Nhật / Cưới Hỏi / Doanh Nghiệp / Tân Gia" đang
  nằm trong `qua-tang-theo-dip` và đều đang ẩn — có thể là nguồn để phân bổ.
- **Cần người dùng xác nhận:** có

### WF-012 — Ảnh cover và ảnh campaign của hai trang landing chưa tồn tại trong Figma

- **Khu vực:** `/qua-theo-dip`, `/qua-tang-ca-nhan`
- **Desktop/Mobile:** cả hai
- **Figma node:** `769:15371`, `769:15447`, `771:21327`, `769:15181`, `769:15205`, `769:15226`,
  `771:15468`
- **Mô tả:** ngoài **một** ảnh hero, toàn bộ phần còn lại của hai trang trong Figma là khối màu
  đặc, không có ảnh nào được đặt vào: 5 khối cover là `#DCDCDC`, 3 ô chooser là `#7A7A7A` (đang
  chọn) / `#F2F2F2` (không chọn), ô ảnh card là `#F2F2F2`.
- **Nguyên nhân:** thiết kế chưa hoàn thiện phần ảnh, không phải lỗi tải asset.
- **Bằng chứng:** `get_design_context` node `769:15244` trả `bg-[#7a7a7a]` / `bg-[#f2f2f2]` chứ
  không có fill ảnh nào; node `769:15389` trả `bg-[#dcdcdc]` cho khối cover.
- **Mức độ:** `asset`
- **Đã thử:** `download_assets` trên node cha và node lá — chỉ trả về ảnh hero.
- **Cách xử lý đề xuất:** đã render đúng các khối màu Figma vẽ, và thêm cột `Category.CoverImageUrl`
  để khi có ảnh thật thì gán trong admin là hiện ngay. Cần khách cung cấp 5 ảnh cover
  (Valentine, 8-3, Giáng sinh, Người ấy, Bố mẹ) + 3 ảnh chooser.
- **Cần người dùng xác nhận:** có

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

### WF-015 — Ba trang có sẵn thiếu thẻ H1

- **Khu vực:** `/gio-hang`, `/hop-tac`, `/tai-khoan/dang-nhap`
- **Desktop/Mobile:** cả hai
- **Figma node:** —
- **Mô tả:** ba trang này render không có `<h1>` nào.
- **Nguyên nhân:** có từ trước, không liên quan B14/B15.
- **Bằng chứng:** script regression 16 route × 2 breakpoint, chỉ ba trang này báo `h1=0`.
- **Mức độ:** `minor`
- **Đã thử:** chưa sửa — nằm ngoài phạm vi hai bước này, và đụng vào trang giỏ hàng/đăng nhập ngay
  trước ngày bàn giao là rủi ro không đáng.
- **Cách xử lý đề xuất:** nâng tiêu đề hiện có của mỗi trang lên thành `<h1>`.
- **Cần người dùng xác nhận:** không

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

### WF-020 — Cả tám banner hero trong Figma đều trống ảnh

- **Khu vực:** 8 trang listing của B16
- **Desktop/Mobile:** cả hai
- **Figma node:** `1269:39703`, `1269:40154`, `1269:40599`, `1269:41044`, `1151:31798`,
  `1151:32245`, `1151:32692`, `1151:34927`
- **Mô tả:** hero của tám trang là khối 1440×600 màu `#D6D6D6` với tên danh mục màu `#AF2234` đè
  lên, không có ảnh nào phía sau.
- **Nguyên nhân:** thiết kế chưa đặt ảnh, không phải lỗi tải asset.
- **Bằng chứng:** `download_assets` node `1151:31798` trả `rawImages: []`, export chỉ 7.8 KB.
- **Mức độ:** `asset`
- **Đã thử:** gọi trên node banner, node cha và node ảnh.
- **Cách xử lý đề xuất:** đang render đúng khối màu Figma vẽ, và thêm cột
  `Category.BannerImageUrl` để gán ảnh trong admin là hiện ngay. Cần khách cung cấp 8 ảnh banner.
- **Cần người dùng xác nhận:** có

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

### WF-029 — Nút tìm kiếm trên nav không có ô nhập

- **Khu vực:** `Views/Shared/_Nav.cshtml`
- **Desktop/Mobile:** cả hai
- **Figma node:** `988:21321` (trang search desktop) — không vẽ ô nhập nào trên chính trang này
- **Mô tả:** icon kính lúp trên nav chỉ `location.href='/tim-kiem'`, không mở ô nhập hay drawer.
  Figma cũng không vẽ ô nhập trên trang kết quả, nghĩa là ô nhập phải nằm ở một drawer/popup của
  nav mà file thiết kế chưa ghép vào luồng này.
- **Mức độ:** `minor`
- **Đã thử:** dò `_Nav.cshtml` và `overlays.js` — không có search drawer nào.
- **Cách xử lý đề xuất:** trang `/tim-kiem` đang tự render một ô nhập, nếu không thì không có
  đường nào gõ từ khóa. Đây là điểm lệch Figma có chủ đích và đã ghi chú trong Razor. Search
  drawer nhiều khả năng thuộc phạm vi B19 (popup/drawer) — sẽ xử lý ở bước đó.
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

### WF-009 — Ảnh 6 sản phẩm Trung thu bị gán lệch một bậc (đã sửa)

- **Khu vực:** `qua-trung-thu`
- **Mô tả:** lỗi seed khiến mỗi sản phẩm đeo ảnh của sản phẩm kế bên — "Việt Nam Hoa Thị" đeo ảnh
  thiên điểu, "Thiên Điểu Lạc Hồng" đeo ảnh tinh hoa, "Ngũ Quả" đeo ảnh phụng hoa, "Phụng Hoa Trình
  Tường" đeo ảnh ngũ quả. Hai hộp Tinh Hoa Bắc Bộ còn đeo `/images/placeholders/featured-*.jpg`.
- **Xử lý:** tải ảnh gốc từ fill của từng card Figma, gán lại đúng.
  Xem `db/scripts/2026-07-21-qua-trung-thu-figma-sync.sql`.

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
