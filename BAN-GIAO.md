# Bàn giao hoaii.vn — 23/07/2026

Commit cuối: `937615d` trên `main`, đã push.

---

## 1. Việc phải làm trước khi ngồi với khách (5 phút)

### a) Đổi mật khẩu admin — bắt buộc

Tài khoản seed đang là `admin@hoaii.vn` / `Hoaii@2026`, ghi thẳng trong
`Services/Admin/AdminAuthService.cs:131` làm giá trị mặc định. Đổi bằng một trong hai cách:

- Vào `/admin/tai-khoan` → đổi mật khẩu (cách nhanh nhất, làm ngay tại chỗ); hoặc
- Đặt biến môi trường `Admin__Password` trước khi chạy — code đọc `config["Admin:Password"]`
  trước, chỉ rơi về `Hoaii@2026` khi không có.

**Đừng đọc mật khẩu này ra trong buổi bàn giao.** Đổi trước, rồi đưa mật khẩu mới cho khách.

### b) Quyết định về 67 đơn hàng test

DB đang có 67 đơn, **không đơn nào là đơn thật** — toàn bộ email thuộc miền test
(`test-auto@example.com`, `test@hoaii.test`, `auto-flow@`, `auto-skip@`, `journey@`). Đã kiểm:

```sql
SELECT COUNT(*) FROM Orders
WHERE Email NOT LIKE '%example.com' AND Email NOT LIKE '%hoaii.test';   -- = 0
```

Vấn đề khi demo: 49 đơn nằm ở *Chờ xác nhận*, 7 ở *Chờ lấy hàng*, 11 ở *Đã giao* — ba tab
*Đang giao*, *Trả hàng*, *Đã huỷ* trống trơn, khách sẽ tưởng tính năng hỏng.

Đã chuẩn bị sẵn `db/scripts/2026-07-23-don-hang-test.sql` với hai phương án, **chưa chạy cái
nào** vì xoá dữ liệu là không hoàn tác được:

| | |
|---|---|
| **A** (khuyên dùng) | Giữ đơn, rải trạng thái để mọi tab đều có nội dung. Không xoá gì, chạy lại được. |
| **B** | Xoá sạch đơn test để khách bắt đầu từ con số 0. Có chốt an toàn: nếu lỡ khớp đơn thật thì tự huỷ. |

Bỏ dấu chú thích khối tương ứng rồi chạy.

> Lưu ý: đây là DB LocalDB trên máy này. Nếu demo trên server thật của sếp thì 67 đơn này
> không tồn tại ở đó và có thể bỏ qua hẳn mục này.

---

## 2. Đã sửa gì đêm nay

| Lỗi | Ảnh hưởng |
|---|---|
| Footer dùng nhầm `:nth-of-type` thay vì `:nth-child` | Ba cột link dồn sang trái, hở **164px** ở mép phải trên **mọi trang** |
| Nút "Hủy bỏ" ở trang thanh toán là `href="#"` | Bấm giữa lúc điền form chỉ nhảy lên đầu trang, không huỷ gì |
| Form liên hệ + form hợp tác không báo lỗi khi bỏ trống | Khách bấm "Gửi", trang nạp lại y hệt, **không một dòng báo lỗi** — tưởng đã gửi được |
| Phân trang admin in đủ mọi số trang, không xuống dòng | Nhật ký 14 trang đẩy trang admin **tràn ngang 118px** trên điện thoại |

Chi tiết kỹ thuật từng lỗi nằm ở `warringFaild.md`, mục **WF-019 → WF-023**.

Lỗi form đáng nói nhất vì nó im lặng: `asp-for` chỉ sinh `data-val-required` chứ **không** sinh
thuộc tính `required` của HTML, mà `form-validate.js` lại chỉ quét `input[required]` — nên hai
form đó lọt qua sạch mọi kiểm tra ở trình duyệt, POST lên máy chủ, bị `ModelState` từ chối, rồi
render lại nguyên trạng. Nhìn từ ngoài thì y như trang bị đơ.

---

## 3. Kết quả kiểm tra (chạy lại toàn bộ sau khi sửa)

| Bộ kiểm | Kết quả |
|---|---|
| Storefront (`tests/e2e/storefront.test.js`) | **22/22** |
| Admin (`tests/e2e/admin.test.js`) | **18/18** |
| Đầu-cuối (`tests/e2e/e2e.js`) | **15/15** |
| Hành trình người dùng (`user-journey.js`) | **28/28**, cả 1440px và 430px |
| Quét 29 màn admin (`admin-sweep.js`) | tất cả OK |
| Giao diện admin 2 khung (`admin-ui.js`) | sạch |
| Tràn ngang (`overflow-sweep.js`) | **140/140** (14 trang × 10 độ rộng) |
| Rà soát toàn bộ 33 trang × 2 khung | **66/66 sạch** |
| Đối chiếu Figma (63 phép đo) | **0 chỗ lệch** |
| Luồng nghiệp vụ (40 phép thử desktop + mobile) | **40/40** |

"66/66 sạch" nghĩa là mỗi trang ở cả hai khung: không tràn ngang, không ảnh vỡ, không link chết,
không chữ bị cắt, đúng 1 thẻ `H1`, đúng 1 `footer`, không lỗi JavaScript, không request hỏng.

Đã kiểm cả bộ chọn LOẠI HỘP trên trang sản phẩm: bấm sang hộp thứ hai thì mã biến thể đổi
(104 → 105), viền chuyển sang ô mới, và giỏ hàng nhận đúng "6 Bánh — 1.099.000đ" ở cả hai khung.

---

## 4. Cách chạy để demo

```powershell
# 1. Dừng app cũ (nếu đang chạy) — nó khoá file exe
Get-Process Hoaii.Web -ErrorAction SilentlyContinue | Stop-Process -Force

# 2. Build
dotnet build hoaii.vn\src\Hoaii.Web\Hoaii.Web.csproj

# 3. BẮT BUỘC sau mỗi lần build — xem mục 6
node hoaii.vn\tools\strip-compressed-assets.js

# 4. Chạy
$env:DOTNET_ROLL_FORWARD='LatestMajor'
$env:DOTNET_ROLL_FORWARD_TO_PRERELEASE='1'
$env:ASPNETCORE_URLS='http://localhost:5167'
$env:ConnectionStrings__DefaultConnection='Server=(localdb)\MSSQLLocalDB;Database=HoaiiDb;Trusted_Connection=True;MultipleActiveResultSets=true'
.\hoaii.vn\src\Hoaii.Web\bin\Debug\net10.0\Hoaii.Web.exe
```

Web khách: <http://localhost:5167> — Admin: <http://localhost:5167/admin>

**Không sửa `appsettings.json`** (cấu hình SQL Server của sếp). Chuỗi kết nối truyền qua biến
môi trường như trên.

### Đường demo gợi ý

1. Trang chủ → danh mục *Quà Tết* → mở một sản phẩm → chọn LOẠI HỘP → thêm vào giỏ
2. Giỏ hàng → đổi số lượng → *Thanh toán* → nhập thông tin → áp mã `GIAM20` → đặt hàng
3. Sang `/admin/don-hang`, đơn vừa đặt nằm ở đầu danh sách → mở ra → đổi trạng thái
   *Chờ xác nhận → Chờ lấy hàng → Đang giao → Đã giao*
4. Quay lại `/tai-khoan/don-hang` của khách — đơn nhảy đúng tab
5. `/admin/san-pham` → sửa tên một sản phẩm → mở lại trang sản phẩm, tên đã đổi
6. `/admin/cai-dat` → đổi hotline → xem nav và footer đổi theo

---

## 5. Những chỗ chưa xong — nói trước với khách

| Mục | Trạng thái | Ghi chú |
|---|---|---|
| **VNPay** | Chỉ có giao diện, chưa nối thật | Cờ `pay_vnpay_enabled` đang tắt trong `/admin/thanh-toan`. Bật lên thì nút chính đổi nhãn thành "Thanh toán với VN PAY". Cần thông tin tích hợp thật từ VNPay mới nối được. Ô nhập thẻ trên trang thanh toán đang `disabled` và không có `name` — không gửi dữ liệu thẻ đi đâu cả (cố ý, để khỏi chạm vào PCI-DSS). |
| **Tỉnh/phường** | Mới có **5 tỉnh** mẫu trong DB | Ô địa chỉ ở trang thanh toán là ô chữ tự do nên **không chặn đặt hàng**. Muốn có dropdown đầy đủ thì import bộ hành chính vào `/admin/dia-gioi`. |
| **Email (SMTP)** | Chưa cấu hình | Mã OTP đăng nhập hiện chỉ ghi ra log, chưa gửi mail; cũng chưa có mail xác nhận đơn. Cần thông tin mail server. Cấu hình ở `/admin/email`. |
| **Phí vận chuyển** | Cả nội thành và liên tỉnh đều **Miễn phí** | Là giá trị mặc định, không phải lỗi — sửa ở `/admin/van-chuyen` là áp ngay vào trang thanh toán. |
| **Nội dung tạm** | 59/61 sản phẩm + 7/7 bài blog | Xem mục 7 để biết cách tìm lại. |

---

## 6. Cái bẫy dễ mất thời gian nhất

Máy này chỉ có .NET 11-preview, mà bản preview đó **hỏng nén gzip**: request có
`Accept-Encoding: gzip` trả về `200` với `Content-Length: 0`.

Trình duyệt thật luôn gửi gzip → **mọi file CSS và JS về rỗng** → trang hiện ra bằng phông
Times New Roman, không một chút định dạng nào. Trong khi đó `curl` và `Invoke-WebRequest`
(không gửi gzip) lại thấy trang hoàn toàn bình thường.

`tools/strip-compressed-assets.js` gỡ các endpoint nén trong manifest để tránh chuyện này.
**Phải chạy lại sau mỗi lần `dotnet build`.**

Nếu sáng mai mở web ra mà thấy trang trắng trơn không có CSS — nghĩ đến đúng chỗ này trước,
đừng đi tìm lỗi trong file CSS.

---

## 7. Nội dung tạm — cách tìm lại để thay

Khách yêu cầu giao diện phải đầy đủ, không được có chỗ trống hay "chưa có thông tin". Nên phần
nội dung chưa có bản thật đã được điền tạm, đánh dấu sẵn để tìm lại:

```sql
-- 59 sản phẩm có phần mô tả tính năng là nội dung tạm
SELECT Id, Name FROM Products WHERE FeatureBody LIKE N'%(Nội dung tạm)%';

-- 7 bài blog chưa có nội dung thật
SELECT Id, Title FROM BlogPosts WHERE Content LIKE N'%(Nội dung đang được biên tập.)%';
```

Sửa trực tiếp trong admin: `/admin/san-pham` và `/admin/blog`. Ảnh sản phẩm dùng ảnh thật từ bộ
tài nguyên đối tác; chỗ nào đối tác chưa cấp thì mượn tạm ảnh khác trong thư viện của site — đã
kiểm không có ảnh vỡ ở bất kỳ trang nào.

---

## 8. Một điểm đã cân nhắc và cố ý giữ nguyên

Bốn chỗ có tỉ lệ tương phản 3.83–4.29:1, dưới chuẩn WCAG AA (4.5:1) cho chữ thường:
`.btn-primary`, `.contact-popup__body-copy`, `.minicart__empty`, `.addon-sheet__variant`, cùng
chữ 13px ở footer.

Các màu và cỡ chữ này lấy thẳng từ Figma (đỏ thương hiệu `#AF2234`). Sửa là lệch thiết kế, nên
tôi giữ nguyên. Nếu khách muốn đạt AA thì nên đổi ở tầng bảng màu chứ đừng vá từng chỗ — ghi
tại `warringFaild.md` mục **WF-023**.
