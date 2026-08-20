# CLAUDE.md — hoaii.vn

> Claude tự đọc file này ở mọi phiên trong dự án — không cần nhắc lại.

---

## 1. DỰ ÁN NÀY LÀ GÌ

| Mục | Giá trị |
|---|---|
| Loại | E-commerce hoa tươi + CMS nội dung (storefront + admin) |
| Stack | ASP.NET Core MVC + EF Core + SQL Server LocalDB, Razor, admin CMS tự viết |
| Site tham chiếu | Figma (design-specs/) — đối chiếu 63 phép đo, hiện 0 chỗ lệch |
| **Admin là** | CÔNG CỤ nội bộ — miễn dễ dùng, không cần đẹp như storefront |
| Mobile | Quan trọng — mọi trang phải kiểm cả 1440px và 430px |
| Song ngữ | Không (chỉ tiếng Việt) |
| Ngành có tiêu chuẩn/giấy phép | Không |
| KHÔNG làm gì | Không đụng 67 đơn test trong DB khi chưa được duyệt · không đọc/đổi mật khẩu admin seed trong buổi bàn giao · không tự mở rộng scope ngoài 3 việc chờ khách quyết (xem BAN-GIAO.md) |

### Trạng thái hiện tại (20/08/2026)

- Đợt 1 & 2 (admin + full content CMS) đã xong và đã push. Đợt 3 (vận hành) còn lại.
- Bàn giao 23/07/2026 — 3 việc **chờ khách quyết**: đổi mật khẩu admin · xử lý 67 đơn test · tương phản màu theo Figma. Chi tiết: `BAN-GIAO.md`.
- Nhật ký lỗi/nợ kỹ thuật: `warringFaild.md` (đang tới WF-023).

### Bẫy môi trường riêng của dự án này

- Build/run: dùng `tools\chay.ps1`, **không dùng `--no-build`** kiểu tuỳ tiện; tách `build` và `run`.
- Sau mỗi build: chạy `tools/strip-compressed-assets.js` (gzip + MapStaticAssets làm CSS trả rỗng).
- DB: LocalDB. `dotnet ef` cần `DOTNET_ROLL_FORWARD=LatestMajor`. **Đừng xoá file .mdf mồ côi** — dùng `CREATE DATABASE ... FOR ATTACH`.
- Migration: `run-migration.ps1`. **Phải hỏi Long trước khi chạy migration.**

---

## 2. DÙNG CẢ 3 LỚP CÔNG CỤ — KHÔNG BỎ LỚP NÀO

Long đã cài sẵn 3 lớp. **Dùng đủ cả 3, chọn cái phù hợp với việc đang làm.**

**LỚP 1 — 49 skill** (superpowers · mattpocock-skills · frontend-design · skill-creator ·
hookify · commit-commands · feature-dev · security-guidance · csharp-lsp).
Khai báo ở `~/.claude/settings.json`, có mặt ở **mọi dự án**.
→ Bảng chọn cái nào ở **mục 3** bên dưới.

**LỚP 2 — `~/.claude/CLAUDE.md`** — chỉ dẫn chung + bảng định tuyến cấp user.
Tự nạp mọi phiên. Nếu mục 3 dưới đây và lớp 2 nói khác nhau → **file này thắng** (gần dự án hơn).

**LỚP 3 — skill `du-an-web`** (`~/.claude/skills/du-an-web/`) — luật rút từ 7 dự án thật của Long.
**Khi trùng với bất kỳ skill nào ở lớp 1, `du-an-web` THẮNG** — nó là kinh nghiệm thật, không phải
hướng dẫn chung.
Nạp thêm khi đúng tình huống: `references/intake.md` (dự án mới) · `moi-truong.md` (setup, lỗi môi
trường) · `clone.md` (clone site) · `phap-ly.md` (tiêu chuẩn/chứng nhận) · `ban-giao.md` (QA, bàn
giao) · `kho-do.md` (component tái dùng)

> Thứ tự khi mâu thuẫn: **du-an-web → file này → lớp 2 → skill lớp 1.**

---

## 3. DÙNG SKILL NÀO Ở GIAI ĐOẠN NÀO

**Tự gọi khi đúng tình huống — đừng chờ Long yêu cầu.**

| Giai đoạn | Skill tự gọi | Gợi ý Long gõ (Claude không tự chạy được) |
|---|---|---|
| **Ý tưởng còn mơ hồ** | `mattpocock-skills:grilling` | `/grill-me` — bảo mở **hội thoại MỚI** |
| **Chốt phạm vi, chưa rõ cách làm** | `superpowers:brainstorming` | — |
| **Biến ý tưởng thành spec** | `superpowers:writing-plans` | `/to-spec` → `/to-tickets` |
| **Đặt tên khái niệm nghiệp vụ, mô hình dữ liệu** | `mattpocock-skills:domain-modeling` | — |
| **Thiết kế interface/module** | `mattpocock-skills:codebase-design` | — |
| **Setup máy, lỗi môi trường** | `du-an-web` → `references/moi-truong.md` | — |
| **Làm/sửa giao diện, chọn màu, typography** | **`frontend-design`** + `du-an-web` | — |
| **Viết code tính năng/sửa bug** | `superpowers:test-driven-development` | `/implement` (nếu đã có spec) |
| **Có plan rồi, thực thi** | `superpowers:executing-plans` | — |
| **Nhiều việc độc lập** | `superpowers:dispatching-parallel-agents` | — |
| **Bug khó, không rõ nguyên nhân** | `superpowers:systematic-debugging` | — |
| **Merge conflict** | `mattpocock-skills:resolving-merge-conflicts` | — |
| **Tra cứu công nghệ mới** | `mattpocock-skills:research` | — |
| **Trước khi báo xong** | **`superpowers:verification-before-completion`** | — |
| **Review trước khi push** | — | **`/code-review`** (built-in, có `ultra`) |
| **Commit** | — | `/commit` |
| **Bàn giao** | `du-an-web` → `references/ban-giao.md` | `/handoff` (bàn giao ngữ cảnh sang phiên sau) |
| **Không biết bắt đầu từ đâu** | — | `/wayfinder` |
| **Việc lộn xộn cần xếp thứ tự** | — | `/triage` |

**Chạy ngầm, không cần làm gì:** `security-guidance` (soi bảo mật sau mỗi lần sửa file, trước
commit/push) · `csharp-lsp` (báo lỗi kiểu C# lúc viết).
→ `security-guidance` báo lỗi thì **xử lý hoặc nói rõ lý do bỏ qua**, đừng lờ đi.

**Trùng chức năng — chọn cái này:** TDD → `superpowers` · Debug → `superpowers:systematic-debugging`
· Code review → `/code-review` built-in.

---

## 4. NGỮ CẢNH NGƯỜI GIAO VIỆC

Long — dev/agency. Giao lô lớn, câu ngắn không dấu, **duyệt bằng MẮT ~30 giây**, không đọc báo cáo
test. Khách hàng cuối KHÔNG nói trực tiếp — mọi feedback đều là bản thuật lại, đã mất chi tiết.

| Long nói | Nghĩa | Làm gì |
|---|---|---|
| "làm đi / chiến đi / làm tiếp đi" | Đã duyệt | Làm luôn, ngừng hỏi |
| "ok / ổn" | Đã duyệt | Đi tiếp, đừng xin xác nhận thêm |
| "tính sau" | Đừng chặn tôi | Ghi `warringFaild.md` WF-xxx rồi đi tiếp |
| **"chưa ổn / ko ổn / đập đi làm lại"** | **ĐÃ QUYẾT làm lại** | **Làm lại NGAY**, chẩn đoán SAU khi có bản mới |
| "ê ..." | Scope mới sắp tới | Chuẩn bị nhận yêu cầu mới |
| "sếp bảo..." | Requirement bị override | Freeze scope, xác nhận lại trước khi code |
| "cho đẹp hơn / premium hơn" | Thiếu chiều sâu (layer, shadow, spacing) | Đưa **ảnh** để chọn, đừng hỏi bằng lời |
| "content chưa ổn" | Thường là **sai sự thật/pháp lý** | Soi claim trước, đừng sửa câu chữ |
| "sửa X không có tác dụng gì" | **Hardcode ở view đè lên field CMS** | GREP view tìm giá trị cứng TRƯỚC |
| "gộp chung vào 1 cái đi" | Model thiếu 1 khái niệm trung gian | Yêu cầu KIẾN TRÚC, không phải UI |
| "cái này nhìn thừa / rác" | Hết tác dụng, không phải xấu | **Ẩn đi, đừng xoá** |

---

## 5. LUẬT KHI LÀM

**Trước khi sửa**
1. Yêu cầu dạng GIẢI PHÁP ("bỏ X", "gộp Y", "tự động hoá Z") → nêu lại VẤN ĐỀ, xác nhận rồi mới
   code. Nếu nó **xoá một năng lực đang có**: LUÔN hỏi lại. **Ẩn trước, xoá sau.**
2. Đọc mã nguồn THẬT. Không suy hành vi từ tên file/route/biến. **Đọc markup TRƯỚC khi viết test.**
3. Thêm field ảnh/nội dung: **GREP toàn bộ view** tìm hardcode có thể đè lên nó.
4. Thay đổi số liệu tài chính: trình bày bảng "trạng thái × có tính không", chờ xác nhận.

**Khi sửa**
- Không regex sửa hàng loạt CSS/HTML/code. Không chép logic sang file thứ hai.
- Không viết chữ trên UI mô tả tính năng chưa xác minh là có thật.
- **Không tự chọn bảng màu** — đưa 3 phương án kèm ảnh.
- Không đoán giới tính/danh xưng · slug · tên file · số liệu kỹ thuật (để `—`).
- Mọi trường liên kết dùng **picker**, không free-text URL. Mọi field optional có nhánh rỗng.
- **Dọn dẹp/refactor = RỦI RO CAO** → sau đó mở lại trang chính ở mọi breakpoint.

**Tự động ghi vào dữ liệu người dùng** (dịch máy, tự điền, đồng bộ)
- Phân biệt "hệ thống điền" vs "người đã sửa". **CHỈ điền ô trống.**
- Dịch vụ ngoài lỗi → **KHÔNG ghi gì.** Không bao giờ ghi rỗng đè lên dữ liệu.
- Việc chậm trong luồng save → đẩy background, đừng chặn request.

**Trước khi báo xong**
```
□ Giao diện → CHỤP ẢNH app thật đang chạy và NHÌN (desktop + mobile)
□ Form     → post dữ liệu SAI, xác minh chữ đã gõ CÒN NGUYÊN
□ Danh sách→ chạy 500+ bản ghi, và xem trạng thái RỖNG
□ Admin    → sửa thử 1 field, xác minh ngoài web ĐỔI THEO
□ Upload   → đọc lại qua HTTP trên BUILD THẬT
□ Báo 3 dòng: file nào sửa · test pass/fail (SỐ THẬT) · CÁI GÌ CHƯA KIỂM
```

**Phải hỏi — kể cả khi chắc chắn đúng:** `git push` · xoá file/thư mục · chạy migration ·
gọi dịch vụ ngoài · dùng email/dữ liệu thật của khách · mở rộng phạm vi · đổi stack.

**Trung thực:** test fail thì nói fail kèm output. Bỏ qua bước nào thì nói rõ. Đoán thì nói là đoán.

---

## 6. BẪY CỦA MÁY NÀY — đã dính thật, đừng dính lại

- **PowerShell 5.1**: `git commit -F <file>` (không dùng `-m` với chuỗi nhiều dòng).
  Tham số nhận mảng khai `[string[]]`, không `[string]`.
- **.NET preview + MapStaticAssets**: CSS/JS trả **RỖNG** khi gzip → chạy
  `strip-compressed-assets` sau **mỗi** lần build. *Triệu chứng: trang mất sạch style nhưng
  `curl` thường vẫn ổn.*
- **File upload** cần `UseStaticFiles` riêng cho `/uploads` — `MapStaticAssets` chỉ đọc manifest
  lúc build → ảnh admin trả **404 im lặng**.
- **LocalDB .mdf mồ côi** → `CREATE DATABASE ... FOR ATTACH`. **ĐỪNG xoá file .mdf.**
- **Server chạy dài**: tách `build` và `run --no-build`, chạy **ngoài sandbox**.
- **Ổ C** phải còn ≥20GB trước mỗi đợt lớn.
- **Không** đặt tên tham số action trùng `action`/`controller`/`area` → MVC bind sai **im lặng**.
- **Sau khi nạp thêm JS library**: audit MỌI form (library mới kích hoạt validate đang ngủ).
- **Tìm kiếm tiếng Việt**: collation `Latin1_General_CI_AI`, đặt cả ở **từng cột**.
- **Cache busting** (`asp-append-version`) — thiếu thì Long thấy bản cũ và báo "nút không hoạt động".

---

## 7. LỆNH & TÀI LIỆU

```
tools\moi-truong.ps1     # dò môi trường — chạy TRƯỚC khi bắt đầu
tools\chay.ps1           # build + strip assets + run, 1 lệnh
dotnet test
```

| File | Viết cho ai |
|---|---|
| `BUILD-SPEC.md` | Chính mình — phạm vi + **mục "KHÔNG làm gì"** + tiêu chí nghiệm thu |
| `warringFaild.md` | Sổ WF-xxx: *cần Long xử lý / cố ý làm khác / chưa làm* |
| `BAN-GIAO.md` | **Người dùng cuối**, không phải lập trình viên |
