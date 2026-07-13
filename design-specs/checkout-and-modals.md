# Checkout Page & Modals — Design Spec (hoaii.vn)

Source: Figma file `uQFY9gwfNbNSeTM6zmspzo`
Nodes: Desktop checkout 887:15475, Desktop filled 848:14548, Mobile checkout 1146:41232, Voucher modal 1239:40930, Bulk-order Zalo popup 1246:41064

## Design Tokens

**Colors**
| Token | Hex | Usage |
|---|---|---|
| Brand red / red-900 | `#410C13` | Primary CTA button bg (checkout button — note: darker than the `#AF2234` brand red used elsewhere on site; use `#410C13` specifically for the "Thanh toán" button) |
| Red-50 | `#F7E9EB` | Secondary button bg ("Sao chép số zalo") |
| Yellow/gold-500 | `#AA8656` | Accent (tan/gold, brand token — not directly seen on this page but reserve for accents) |
| Yellow-50 | `#F7F3EE` | Order-summary panel background (desktop right column) |
| Yellow-700/800 | `#795F3D` / `#5E4A2F` | Reserved accents |
| Grey-900 | `#0F0F0F` | Selected radio label / active state text |
| Grey-800 | `#1C1C1C` | Body text, icons, borders (radio circle, close icon) |
| Grey-700 | `#3C3C3C` / `#000000` | Headings/black text |
| Grey-600 | `#5A5A5A` | Secondary text, placeholder-filled field value in unselected state |
| Grey-500 | `#7A7A7A` | Muted text (Zalo popup body, timings) |
| Grey-400 | `#9B9B9B` | Borders (payment method box), placeholder text |
| Grey-300 | `#BFBFBF` | Input borders, dividers |
| Grey-200 | `#DCDCDC` | Qty stepper border, shipping-method box border |
| Grey-100 | `#D6D6D6` | Voucher icon tile bg |
| Grey-50 | `#F2F2F2` | Selected option bg, note-field bg, Zalo popup info-card bg |

**Typography** (font family: Josefin Sans unless noted)
| Token | Weight/Style | Size/Line-height | Usage |
|---|---|---|---|
| Subtitle | Medium 500 | 20/23 | Section headings ("Liên hệ", "Thông tin giao hàng", "Phương thức thanh toán", totals) |
| Label | Medium 500 | 16/20 | Field filled value, radio option label, button text (mobile) |
| Body 2 | Light 300 | 16/18 | Empty-state placeholder text inside inputs, notes field |
| Caption | Regular 400 | 13/16 | Floating field label (once filled), footer policy links, voucher item title |
| Mobile Heading 3 | Medium 500 | 24/32 | Zalo popup title "Liên hệ qua Zalo" |
| CTA button text | Medium 500 | 20/23 (desktop) or 16/20 (mobile) | "Thanh toán với VN PAY" |

Large display headings elsewhere on site use **"LC Sac Trial"** — not present on checkout page itself (checkout uses Josefin Sans throughout).

---

## Checkout Page — Desktop (1925px canvas, content ~1728px effective two-column)

### Structure
- Root: column flex, full width, bg white.
- **Header** (`h:120px`, bg white, border-bottom `#BFBFBF`): centered row `w:964px` with bottom border `#9B9B9B`; contains 64×64 Logo (left), spacer, 40×40 cart/shop icon (right). Row is vertically centered within the 120px band, `justify-content:space-between`.
- **Body row** (`h:960px`, flex row, justify-content:center):
  - **Left column "Thông tin"** — bg white, flex:1, content right-aligned to a fixed **440px** form column, padding `40px 40px 10px 0` (pt 40, pr 40, pb 10, pl 0 — pushes form toward center divider). Border right `#BFBFBF`.
  - **Right column "Thanh toán"** — bg `#F7F3EE`, flex:1, border-left `#BFBFBF`, padding `40px 0 10px 40px`, contains a fixed **426px** summary column.

### Left column — Form (440px wide, vertical stack, gap 16px between sections)

1. **Liên hệ** (contact)
   - Row: "Liên hệ" (Subtitle, 20px Medium, black) ﹕ "Đăng nhập" link (16px, `#1C1C1C`, right-aligned).
   - Field: **Email*** — floating-label input, `56px` tall, `border 1px #BFBFBF`, `radius 8px`, `padding 0 16px`, bg white. Empty state: placeholder centered vertically (Body2 16px light black). Filled state (see 848:14548): label shrinks to Caption (13px, `#5A5A5A`) top-aligned, value shown below in Label style (16px Medium, `#0F0F0F`).

2. **Thông tin giao hàng** (shipping info) — heading Subtitle 20px Medium.
   - Row of 2 fields, `gap:16px`, each `flex:1`: **Tên*** (first name) / **Họ*** (last name). Same 56px floating-label input style.
   - **Tên doanh nghiệp (Tùy chọn)** — business name, optional, full width, same input style.
   - **Địa chỉ*** — address, full width.
   - **Tỉnh/TP, Quận/Huyện, Phường/Xã** — combined province/district/ward field (single input in this design; implement as a single free-text or a searchable combo — no separate dropdown widgets shown in Figma, but real build should likely use 3 cascading `<select>`s styled to match the same 56px bordered box, or one autocomplete input matching this placeholder).
   - **Số điện thoại*** — phone, full width.
   - All fields share: `h:56px`, `border 1px solid #BFBFBF`, `border-radius:8px`, `padding:0 16px`, `bg:#FFFFFF`, vertical-center content. Gap between fields in this section: 16px (8px for the field's internal label/value stack).

3. **Phương thức vận chuyển** (shipping method) — heading Subtitle 20px.
   - Empty/no-address state: single box, `border 1px solid #DCDCDC`, `radius:8px`, `padding:12px 16px`, text centered, `color:#5A5A5A`, 16px Light: "Nhập địa chỉ đề xem phương thức giao hàng".
   - Filled state (848:14548 / mobile 1146): becomes a **radio group card** matching the payment-method pattern below — options "Nội thành Hà Nội" (selected) / "Vận chuyển liên tỉnh".

4. **Phương thức thanh toán** (payment method) — heading Subtitle 20px.
   - Container: `border 1px solid #9B9B9B`, `radius:8px`, column, overflow clip so children corners are clipped to container radius.
   - **Option row** (each `h:48px`, `padding:12px 16px`, `justify-content:space-between`):
     - Selected: `bg:#F2F2F2`, `border:1.5px solid #0F0F0F` (only on selected row, creates emphasis), rounded top corners if first item. Radio: 20×20 circle, 6px black ring border, inner white/grey 12px dot (custom radio, not native). Label: Label style 16px Medium `#0F0F0F`. Trailing: bank icon graphic (32×~21px) right-aligned.
     - Unselected: no bg/border override (transparent), label `#5A5A5A`, radio is plain 1px `#1C1C1C` outline 20×20 circle (no dot), trailing icon for that method (e.g., COD icon 32×26px).
   - Two options confirmed: "Chuyển khoản qua ngân hàng" (bank transfer, has bank-card icon) and "Thanh toán khi giao hàng" (COD, has cash icon).

5. **Ghi chú đơn hàng** (order notes) — single full-width 56px bordered input/textarea, placeholder "Thêm ghi chú đơn hàng", same styling as address fields (Body2 16px Light black placeholder).

### Footer policy strip (desktop, absolutely positioned under form column, `top:910–1044px` depending on state)
- Bg white, `border-top:1px solid #9B9B9B`, `padding:16px 10px`, width matches form column (440/441px).
- Row of policy links, `gap:16px`, Caption style (13px Regular, `#1C1C1C`), `line-height:16px`, no-wrap: "Chính sách hoàn tiền" · "Vận chuyển" · "Chính sách bảo mật" · "Hủy bỏ" (empty-state variant swaps first two items to "Chính sách vận chuyển" once form filled — reconcile to consistent set: **Vận chuyển | Chính sách bảo mật | Hủy bỏ**, plus refund policy on the very first empty state only).

### Right column — Order Summary "Thanh toán" (426px wide)

- **Product list** — scrollable/clipped block `h:434px`, `gap:36px` between line items.
  - Each line item: row, `gap:16px`.
    - Thumbnail: `68×68px`, `border-radius:3.28px`, `object-fit:cover`.
    - Info block (`flex:1`, column, `gap:16px`):
      - Row: product name (Label 16px Medium black, truncate ~203px) + trash/delete icon (20×20, right-aligned).
      - Variant chip: bg white, `radius:8px`, `padding:4px 8px`, inline row with variant text (16px Light black) + small right-chevron (14×14) — acts as a variant-change affordance.
      - Bottom row (`align-items:flex-end`, `justify-content:space-between`): unit price "899.000" (Subtitle 20px Medium black) + **quantity stepper**: `[ − ][ qty ][ + ]`, each control `40×30px`, `border 1.25px solid #DCDCDC`, `radius:5px`, qty number centered (Label 16px Medium).
  - Sample data: repeating "Set quà Việt Nam Hoa Thị" / variant "Hộp 4 bánh / màu vàng" / price 899.000 (VND, no decimal, dot thousands-separator).

- **Promo/voucher entry row**: bg white, `border 1px solid #BFBFBF`, `radius:8px`, `padding:8px` (`pr:8px py:8px`), row `justify-content:space-between`.
  - Left: icon tile with right border divider (`border-right 1px #BFBFBF`, `padding:8px 16px`) containing 24×24 ticket/sale icon, then label "Ưu đãi & khuyến mãi" (Label 16px Medium `#1C1C1C`).
  - Right: 40×40 "+" add button (opens Voucher Modal).

- **Totals block** (column, `gap:16px`, Subtitle-weight Medium text, black):
  - Row: "Tổng : N mặt hàng" (16px) — value right-aligned (20px).
  - Row: "Vận chuyển" (16px) — "Miễn phí" (20px).
  - Divider-less, then **Tổng cộng** row (20px both sides, bold/medium) with grand total, plus a savings caption below: "TỔNG SỐ TIẾT KIỆM 230.000đ" (16px, full width).

- **CTA button**: full width (`426px` desktop / 100% mobile), `h:60px`, `bg:#410C13`, `radius:8px`, centered, text "Thanh toán với VN PAY" (white, 20px Medium desktop / 16px Medium mobile).

---

## Checkout Page — Mobile (430×1086, node 1146:41232)

Single-column, `bg:white`.

1. **Header** — `border-bottom #BFBFBF`, inner row `padding:16px`, `justify-content:space-between`: 40×40 logo (left), 32×32 cart icon (right). No divider line under logo row on mobile (unlike desktop's inner `w:964px` bordered row).

2. **"Tóm tắt đơn hàng" summary bar** — collapsible header bar, `h:60px`, `bg:#F2F2F2`, `border-bottom #BFBFBF`, `padding:0 16px`, row `justify-content:space-between`:
   - Left: "Tóm tắt đơn hàng" (Caption 13px Regular black) + 20×20 chevron icon rotated 90° (expand/collapse indicator).
   - Right: running total "899.000đ" (Label 16px Medium black).
   - This bar toggles a collapsed product-list drawer above the form on mobile (order summary is above the form, not side-by-side as on desktop).

3. **Form section "Thông tin"** — bg white, `padding:20px 16px 10px 16px`, column `gap:20px`:
   - Same field set/order as desktop: Liên hệ (Email*) → Thông tin giao hàng (Tên*/Họ* row, Tên doanh nghiệp, Địa chỉ*, Tỉnh/TP.../ Phường/Xã, Số điện thoại*) → Phương thức vận chuyển (radio card) → Phương thức thanh toán (radio card) → Ghi chú đơn hàng.
   - Field height on mobile: `48px` (vs 56px desktop) — more compact; other field styling identical (`border 1px #BFBFBF`, `radius:8px`, `padding:0 16px`).
   - Section heading gap reduced to `12px` (vs 16px desktop) between heading and first field.
   - CTA button directly follows the form (no separate right-column state on mobile): full width, `h:60px`, `bg:#410C13`, `radius:8px`, text 16px Medium white.

4. **Footer policy strip** — full width, `border-top #BFBFBF`, `padding:16px`, links row `gap:16px`, 11px Regular `#1C1C1C`, `line-height:20px`: "Chính sách vận chuyển · Chính sách bảo mật · Hủy bỏ".

### Razor/CSS notes (Checkout/Index.cshtml)
- Use CSS Grid for the desktop two-column layout: `grid-template-columns: 1fr 1fr;` with a shared `max-width` per side (`440px` / `426px`) and `justify-content:end` / `start` respectively, collapsing to single column `<992px` (or whatever mobile breakpoint) matching the 430px mobile spec — reorder so the order-summary bar appears above the form on mobile (use CSS `order` or separate partial include with `d-none d-lg-block` / `d-lg-none` toggles, or a `<details>`/JS-accordion for the "Tóm tắt đơn hàng" bar).
- Floating-label pattern: wrap each `<input>` in `.form-field` with `<label>` absolutely positioned; on `:focus`/`:not(:placeholder-shown)` shrink label to Caption style and reveal value — mirrors the two Figma states (empty vs filled).
- Custom radio buttons for shipping-method/payment-method: hide native `<input type="radio">`, style a 20×20 `span` matching selected (6px black ring + white/grey 12px dot) vs unselected (1px outline) — apply `.option-row.is-selected { background:#F2F2F2; border:1.5px solid #0F0F0F; }`.
- Quantity stepper: 3 flex children, buttons `40×30px` border `1.25px solid #DCDCDC` radius `5px`.

---

## Voucher Modal (`_VoucherModal` partial, node 1239:40930, 548×1080)

- Modal panel: `bg:white`, `padding:40px 24px 40px 24px` (top 20/ bottom 40 per spec: `pb-40 pt-20 px-24`), `border-radius` per site modal convention, column layout with `justify-content:space-between` (header+list pinned top, totals+CTA pinned bottom — use `display:flex; flex-direction:column; height:100%`).
- **Header row**: title "Phương thức thanh toán" (Subtitle 20px Medium black) + close button (32×32 "X" icon, `#1C1C1C`).
- **Code entry row**: bottom-border only (`border-bottom:1px solid #1C1C1C`), row `justify-content:space-between`, `padding-bottom` implicit: placeholder "Nhập mã ưu đãi hoặc voucher" (Caption 13px, `#9B9B9B`) + "ÁP DỤNG" action label (13px, `#5A5A5A`, uppercase, clickable).
- **Voucher list** — column `gap:16px`:
  - Each voucher card: `h:80px`, `border:1px solid #9B9B9B` (selected) or `#F2F2F2` (unselected), `radius:8px`, `overflow:hidden`, row `justify-content:space-between`, `padding-right:24px`.
    - Icon tile: `80×80px` (full card height), `bg:#D6D6D6`, centered 32×32 ticket-sale icon, flush-left (no radius on this side since parent clips overflow).
    - Text block (`gap:8px`): voucher title e.g. "Miễn phí vận chuyển" / "Giảm giá 20%" (Caption 13px Regular black) + tag pill (`bg:#F2F2F2`, `radius:2px`, `padding:4px 8px`, 11px Light black) reading "Ưu đãi" or "Voucher".
    - Right: checkbox/radio square 20×20 — selected state filled (`border:1px solid #1C1C1C` + inner 12px `#1C1C1C` square, radius 1.5px), unselected state just outline (`border:1px solid #9B9B9B`).
- **Bottom summary block** (`gap:16px`):
  - Text: "Đã áp dụng 1 ưu đãi" / "Tổng tiền : 1280.000đ" (Caption 13px Regular black, stacked, `gap:8px`).
  - CTA button: full width, `h:60px`, `bg:#410C13`, `radius:8px`, text "Đồng ý" (white 20px Medium) — reuses the same checkout-button component/instance as the main page.

### Razor notes
- Implement as `_VoucherModal.cshtml` partial rendered inside a Bootstrap-style modal (or custom dialog) triggered by the "+" button on the order summary's "Ưu đãi & khuyến mãi" row.
- Voucher list items should be a repeated `<label class="voucher-item">` wrapping a hidden checkbox/radio input (allow multi-select if business rule permits stacking, else radio group) + the visual card markup above.

---

## Bulk-order Contact Popup (`_ContactPopup` partial, node 1246:41064, 634×550)

- Panel: `bg:white`, `border-radius:16px`, `padding:40px`, column `gap:28px`.
- **Header block** (`gap:16px`):
  - Row `justify-content:space-between`: 
    - Left group (`gap:16px`): Zalo logo avatar — `40×40px` circle, `bg:#F2F2F2`, `border:1.111px solid #7A7A7A`, contains Zalo icon image (clipped circular, `aspect-ratio:84/83`); title "Liên hệ qua Zalo" (Mobile/Heading 3 — 24px Medium, `line-height:32px`, black).
    - Right: close "X" icon, 32×32, `#1C1C1C`.
  - Body copy (16px Medium `#7A7A7A`, `line-height:20px`, full width): "Chat với chúng tôi để được tư vấn nhanh về thiết kế, gói quà và ưu đãi khi mua số lượng lớn".
  - Info card: `bg:#F2F2F2`, `radius:8px`, `padding:16px`, column `gap:8px`:
    - Name "Hoài" (Subtitle 20px Medium black).
    - Phone + response time "033 500 6783 -Phản hồi trong 5-10 phút" (16px Medium `#7A7A7A`).
    - Hours row (`gap:8px`, align-items:end): 24×24 alarm-clock icon + "09:00-18:00 (T2 - T7)" (16px Medium `#7A7A7A`).
- **QR + actions row** (`gap:24px`, align-items:center):
  - QR image: `148×143.5px`, `border:2px solid #BFBFBF`, `radius:15.16px`, `object-fit:cover`.
  - Action column (`flex:1`, `gap:16px`):
    - Primary button: `bg:#410C13`, `radius:8px`, `padding:16px 24px`, row centered `gap:16px`, 24×24 chat-bubble icon (white) + "Mở zalo để chat" (16px Medium, `#F2F2F2`).
    - Secondary button: `bg:#F7E9EB`, same padding/radius, 24×24 copy icon (brand red) + "Sao chép số zalo" (16px Medium, `#410C13`).
- **Footer caption** (`padding:0 16px`, column `gap:8px`, 16px Light `#303030`, `line-height:18px`): "Quét để mở zalo" / "Nếu không mở được, hãy sao chép số và tìm trong ứng dụng zalo".

### Razor notes
- `_ContactPopup.cshtml`: static content except phone number / QR src, which should come from a config/view-model (e.g., `SiteSettings.ZaloPhone`, `SiteSettings.ZaloQrImageUrl`).
- "Sao chép số zalo" button should trigger a small JS clipboard-copy handler bound to `data-copy="033 500 6783"`.
- Popup is a centered modal/dialog (`max-width:634px`), not full-screen; use same modal chrome/close-icon pattern as Voucher Modal for consistency.

---

## Cross-cutting Notes
- Currency format throughout: Vietnamese-style, dot as thousands separator, no decimals, suffixed "đ" only in compact contexts (summary bar "899.000đ"); full totals area omits the "đ" suffix in most totals lines except line items where it's implied.
- All monetary/product/voucher content in this spec is Figma placeholder/sample data — bind to real cart/pricing/voucher view-models at implementation time.
- Reusable button component: "Button thanh toán" — `bg:#410C13`, `radius:8px`, `h:60px`, white text, used identically on Checkout page and Voucher Modal; build as a shared Razor `<partial>` or CSS class `.btn-checkout-primary`.
