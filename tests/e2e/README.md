# Kiểm thử chức năng (end-to-end)

Bộ test chạy **thẳng vào ứng dụng đang chạy** — không mock gì cả, nên nó bắt được đúng những
lỗi mà người dùng thật gặp phải.

## Chạy thế nào

1. Bật app (cần `wwwroot` giải đúng, nên chạy từ thư mục project):

```bash
# từ thư mục gốc repo
ASPNETCORE_URLS=http://localhost:5167 \
ConnectionStrings__DefaultConnection='Server=(localdb)\MSSQLLocalDB;Database=HoaiiDb;Trusted_Connection=True;MultipleActiveResultSets=true' \
dotnet run --project src/Hoaii.Web
```

2. Chạy test (cần Node 18+). Hai file cuối điều khiển trình duyệt thật nên cần Playwright —
   nếu chưa có thì cài trong chính thư mục này: `cd tests/e2e && npm init -y && npm i playwright && npx playwright install chromium`

```bash
node tests/e2e/storefront.test.js   # chức năng phía khách  (HTTP, nhanh)
node tests/e2e/admin.test.js        # chức năng phía admin  (HTTP, nhanh)
node tests/e2e/user-journey.js      # mua hàng thật trên trình duyệt (desktop + mobile)
node tests/e2e/admin-ui.js          # quét mọi màn admin: lỗi JS, ảnh vỡ, tràn ngang
```

Mỗi file thoát với mã `1` nếu có test hỏng, nên cắm vào CI được ngay.

## Bộ test gồm gì

| File | Nội dung |
|---|---|
| `storefront.test.js` | trang công khai, 404, phân trang, sắp xếp, tìm kiếm, giỏ hàng, voucher (đúng/sai/gỡ), thanh toán (validation + đặt hàng), form liên hệ/bán buôn/newsletter, CSRF, chặn truy cập admin, OTP |
| `admin.test.js` | đăng nhập, mọi route admin, CRUD sản phẩm, chặn slug trùng, **luồng trạng thái đơn** (kể cả chặn nhảy cóc), voucher, blog nháp/đăng, **chặn upload file giả mạo ảnh**, cài đặt/vận chuyển/thanh toán áp vào web, CMS, **phân quyền Nhân viên vs Chủ shop**, hộp thư |
| `user-journey.js` | bấm chuột thật: danh mục → sản phẩm → chọn loại hộp → thêm giỏ → thanh toán → đặt hàng, ở 1440px và 430px |
| `admin-ui.js` | mở 28 màn admin ở 2 khổ màn hình, bắt lỗi JS, lỗi 500, ảnh vỡ, tràn ngang |

## Lưu ý khi viết thêm

- **Tên trường ở form thanh toán là `Form.Email`…** (do `asp-for="Form.X"`), không phải `Email`.
- **Đổi trạng thái đơn dùng tham số `to`**, không phải `status`.
- Radio/checkbox bị ẩn bằng CSS — phải bấm vào `<label>`, không bấm vào `<input>`.
- Đừng đếm `product-card` để đếm số sản phẩm: nó khớp cả class con (`product-card__name`…).
  Đếm `product-card__image-link` (mỗi thẻ đúng một cái).
- Ô tìm kiếm ở admin echo lại từ khóa vào `value=""`, nên đừng dùng `body.includes(từ khóa)`
  để kết luận "có kết quả" — hãy đếm dòng trong bảng.
- Test có tạo dữ liệu thật (đơn hàng, liên hệ…). Dữ liệu dùng email `*@example.com` để dễ dọn.

## Các bộ test

| File | Nội dung | Số phép thử |
|---|---|---|
| `storefront.test.js` | HTTP thuần: route, phân trang, lọc, giỏ, checkout, OTP | 22 |
| `admin.test.js` | HTTP thuần: đăng nhập, CRUD, phân quyền, CMS, cài đặt | 18 |
| `e2e.js` | Trình duyệt thật: mua hàng → admin xử lý đơn → sửa sản phẩm → bảo mật → form | 15 |
| `admin-sweep.js` | 29 màn admin: HTTP, lỗi JS, lỗi 500, tràn ngang | 29 |
| `storefront-sweep.js` | 22 route storefront x 2 breakpoint | 44 |
| `overflow-sweep.js` | 14 trang x 10 độ rộng, 2560 → 375 | 140 |

Chạy tất cả:

```bash
for f in storefront.test.js admin.test.js e2e.js admin-sweep.js storefront-sweep.js overflow-sweep.js; do
  echo "=== $f ==="; node "$f" | tail -3
done
```

## Tài khoản dùng để test

`admin@hoaii.vn` / `Hoaii@2026` — **đổi mật khẩu này trước khi lên production.**
Giá trị mặc định nằm trong `AdminAuthService.EnsureSeedAdminAsync`, ai đọc repo đều biết.

## Lưu ý

- `e2e.js` tự trả dữ liệu về trạng thái ban đầu, chạy lại được nhiều lần. Riêng đơn hàng thì mỗi
  lần chạy tạo thêm một đơn thật — xoá trong admin nếu không muốn giữ.
- Đổi CSS/JS phải `dotnet build` lại rồi mới chạy test, vì manifest static asset chỉ sinh khi build.
