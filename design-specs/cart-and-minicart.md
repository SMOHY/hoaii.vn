# Cart & Mini-cart Design Spec — hoaii.vn

Source: Figma file `uQFY9gwfNbNSeTM6zmspzo`
Nodes: Desktop cart `1154:35991` · Mobile cart `1142:38498` · Mini-cart drawer `1271:53763` · Add-on drawer `970:20686`

## Design Tokens

**Colors**
| Token | Hex | Usage |
|---|---|---|
| Brand red / red-500 | `#AF2234` | Announcement bar bg, section eyebrow labels ("VỀ HOÀI", "SOCIAL") |
| Brand red / red-600 | `#9F1F2F` | Hover state (derived) |
| Deep red / red-900 | `#410C13` | Primary CTA buttons ("Thêm vào giỏ", "Thêm", "Thanh toán", "Kiểm tra") |
| Grey-50 | `#F2F2F2` | Product thumbnail placeholder bg, promo strip bg |
| Grey-200 | `#DCDCDC` | Qty stepper border |
| Grey-300 | `#BFBFBF` | Card/row borders, dividers |
| Grey-400 | `#9B9B9B` | Totals-block top divider, newsletter input border |
| Grey-500 | `#7A7A7A` | Secondary/breadcrumb muted text |
| Grey-600 | `#5A5A5A` | Mobile nav bottom border |
| Grey-700/900 | `#3C3C3C` / `#0F0F0F` | Nav text, body copy |
| Grey-800 | `#1C1C1C` | Breadcrumb text |
| Green (success) | `#00985B` | Discount amount value |
| Tan/gold yellow-50 | `#F7F3EE` | Footer bg block |
| Tan/gold yellow-100 | `#E5D9CB` | Mini-cart / add-on drawer header bg |
| Tan/gold yellow-800 | `#5E4A2F` | Footer body text/links |
| White | `#FFFFFF` | Card bg, close-pill bg |
| Overlay | `rgba(0,0,0,0.4)` | Drawer/modal backdrop |

**Typography** (font-family `Josefin Sans` unless noted)
| Style | Size/Line-height/Weight | Used for |
|---|---|---|
| Heading (LC Sac Trial, Regular) | 48/56 desktop, 20/30 mobile | Footer "ĐĂNG KÝ VÀO DANH SÁCH" |
| Heading 4 (Semibold) | 24/32 | Add-on drawer header "Giỏ hàng" |
| Subtitle (Medium) | 20/23 | Mini-cart header "Giỏ hàng", totals value, CTA label |
| Label (Medium) | 16/20 | Item name, qty number, section headers ("HOÀN CHỈNH VỚI"), "Giảm giá"/"Tổng cộng" label desktop |
| Body 1 (Regular) | 20/24 (used at 15px in promo bar variants) | Promo strip text |
| Caption (Regular) | 13/16 | Mobile item name/variant, add-on name/price, "Đóng" pill text, footer legal/body copy |
| Small nav (Regular) | 14–15/18–24 | Nav links, mobile input placeholder |

**Radii**: 4px (thumbnails, add-on rows, buttons), 5px (desktop qty stepper), 6–8px (cards, inputs, drawer top corners), 34–45px (pill/close buttons).

**Shadows**: Mini-cart & add-on drawer: `0px 4px 10px rgba(0,0,0,0.15)` (mini-cart), `0px 4px 20px rgba(0,0,0,0.15)` (add-on drawer).

---

## Cart Page

### Desktop (1920px canvas, content column 750px, centered)

**Page structure** (top→bottom, `flex-direction: column`):
1. Hero: sticky/static header (announcement bar 40px `#AF2234` + utility nav 48px `#F2F2F2` + main nav 80px w/ logo, category links, search/user/cart icons, VN/EN) + breadcrumb row (`padding: 0 240px`, height auto, text "Trang chủ/Giỏ hàng" — 16px Medium `#1C1C1C`).
2. Cart body: centered `flex` container, gap 80px between cart card and footer.
3. Footer (shared site footer, tan `#F7F3EE` block + newsletter + link columns + social).

**Cart card** (`.cart-panel`):
```css
width: 750px;
border: 1px solid #BFBFBF;
border-radius: 8px;
overflow: hidden; /* clip */
display: flex; flex-direction: column;
background: #fff;
```

**Item list container**: `padding: 16px 32px 0; display:flex; flex-direction:column; align-items:center; gap:40px;` (gap separates item-list block from the add-on/upsell block).

**Cart item row** (`.cart-item`):
```css
display: flex; gap: 16px; align-items: flex-start;
padding: 16px 0;
border-bottom: 1px solid #BFBFBF; /* omit on last row */
```
- **Thumbnail**: `88 × 96px`, `background:#F2F2F2; border-radius:4px; overflow:hidden` (product image or placeholder; contains an absolutely-positioned "Thêm vào giỏ" hover CTA overlay for related/empty states — likely a hover affordance, not needed for filled cart rows in Razor).
- **Info column**: `flex:1; display:flex; flex-direction:column; gap:44px; align-items:flex-end;`
  - Row 1: name (left, `font:16px Medium #000`) + remove "×" icon button (20×20, `Xnix/Line/Cross`) on right, `justify-content:space-between; width:100%`.
  - Row 2: qty stepper (left) + price (right, `20px Medium #000`, format `899.000`), `justify-content:space-between; width:100%`.

**Quantity stepper** (`.qty-stepper`, desktop):
```css
display:flex; align-items:center;
```
- Decrement button: `40×40px` box, `border:1.25px solid #DCDCDC; border-radius:5px;` centered "−" glyph (9.56px line).
- Count: `32×24px` box, `16px Medium #000`, centered.
- Increment button: `40×40px`, `border:1.25px solid #DCDCDC; border-radius:5px;` centered "+" glyph (22.95px icon `Xnix/Line/Add`).
- No visible gap between the three (buttons abut count).

**Remove/close icon**: `Xnix/Line/Cross`, 20×20px, simple vector "×", no button chrome — click target is the icon itself.

**Upsell / add-on block** (`HOÀN CHỈNH VỚI`), inside same card, `padding-bottom:16px`:
- Heading: centered, `16px Medium #000`, `"HOÀN CHỈNH VỚI"`.
- List container: `border:1px solid #BFBFBF; border-radius:8px;` — rows stacked, each row separated by `border-bottom:1px solid #BFBFBF` (last row no border).
- **Add-on row** (`.addon-row`): `display:flex; align-items:center; justify-content:space-between; padding:8px;`
  - Left: thumbnail `44×48px` (`#F2F2F2`, `radius:4px`) + text stack (name `13px Regular #000` line-height 18, price `13px Regular` line-height 16), `gap:12px`.
  - Right: "Thêm" button — `background:#410C13; color:#F2F2F2; padding:4px 12px; border-radius:4px; font:16px Medium` (desktop) / `13px Regular` (mobile — slightly smaller/lighter weight on mobile, verify against brand button component before final).

**Discount row** (`.cart-discount`):
```css
display:flex; justify-content:space-between; align-items:center;
padding:16px; /* desktop: 16px all sides; mobile: 8px 16px */
border-top:1px solid #BFBFBF;
font:16px Medium; /* mobile: 13px Regular label / 16px Medium value */
```
Label "Giảm giá" in `#000`; value e.g. "230.000đ" in `#00985B`.
(No visible discount-code text input in this specific frame instance — value is pre-applied. If a code entry field is required, model it as a bordered pill matching the newsletter-input style: `border:1px solid #BFBFBF; border-radius:8px; padding:8px 16px;` with a "Áp dụng" button in `#410C13`.)

**Totals + checkout footer** (`.cart-summary`):
```css
display:flex; align-items:stretch;
border-top:1px solid #BFBFBF;
height:80px; /* mobile 60px */
```
- Left cell: `flex:1; display:flex; flex-direction:column; justify-content:center; padding:16px; gap:4px;` — label "Tổng cộng" (16px Medium, mobile 13px Regular) + amount (20px Medium desktop / 16px Medium mobile), both `#000`.
- Right cell (CTA): `flex:1; background:#410C13; display:flex; align-items:center; justify-content:center;` — text "Thanh toán", `20px Medium #fff` (desktop) / `16px Medium #fff` (mobile).

### Mobile (430px canvas)

Same component tree, tighter spacing:
- Hero collapses to: promo bar (32px, 13px Regular white) + 60px nav row (hamburger 28px left, logo 36px center, search+cart icons 28px right), bottom border `1.5px solid #5A5A5A`.
- Breadcrumb: `padding:0 16px`, 13px Regular, "Trang chủ/" in `#1C1C1C` + "Giỏ hàng" in `#7A7A7A` (two-tone, unlike desktop's single color).
- Cart card: `width:100%` inside `padding:0 16px` wrapper, same 1px `#BFBFBF` border/8px radius.
- Item row thumbnail shrinks to `60×65.45px`, radius `2.7px`; name/price text drop to `13px Regular` (name) — note desktop uses 16px Medium for name, mobile uses 13px Regular, both `#000`.
- Qty stepper mobile: buttons `32×24px`, `border:1px solid #DCDCDC`, `radius:4px` (smaller than desktop's 40×30/5px).
- Add-on "Thêm" button mobile: `13px Regular` (vs. desktop `16px Medium`) — mobile de-emphasizes this CTA slightly.
- Discount row mobile: `padding:8px 16px` (vs desktop 16px all).
- Totals/checkout footer mobile: height `60px` (vs desktop 80px), label 13px Regular, amount 16px Medium, CTA 16px Medium.
- Footer (site footer) stacks to single column, heading font drops to 20px (LC Sac Trial), logo 60px, link columns full-width stacked with 20–40px gaps.

### Razor/CSS notes (Cart/Index.cshtml)

- Build `.cart-item`, `.qty-stepper`, `.addon-row`, `.cart-summary` as reusable CSS classes (BEM-ish), shared between desktop/mobile via responsive breakpoints (`@media (max-width: 767px)`) rather than duplicating markup — the two Figma frames are the same component tree with token overrides only (font-size, padding, icon size).
- Model each cart line as a partial `_CartItemRow.cshtml` taking a `CartItemViewModel` (Id, ThumbnailUrl, Name, Variant, UnitPrice, Qty, LineTotal).
- Qty stepper buttons should be `<button type="submit" formaction="/Cart/UpdateQty">` or JS-driven fetch; decrement/increment icons are simple inline SVG "−"/"+" matching `Xnix/Line/Add` vector (single horizontal/plus line, not a boxed icon font).
- Remove button ("×") maps to `Xnix/Line/Cross` — 20×20 inline SVG, position top-right of the name row.
- Price formatting: Vietnamese thousands-dot, no currency symbol on line items ("899.000"), but totals include "đ" suffix ("1.628.000đ").

---

## Mini-cart Drawer

Node `1271:53763`, instance wraps "Trạng thái giỏ hàng" (cart-state) component. Canvas 430×831, drawer content 430 wide, drop-shadow `0px 4px 10px rgba(0,0,0,0.15)`.

**Container**:
```css
width: 430px;
display:flex; flex-direction:column;
border-radius: 8px 8px 0 0; /* rounded top corners only — slides up from bottom or docks as a panel */
box-shadow: 0 4px 10px rgba(0,0,0,0.15);
overflow: hidden;
background: #fff;
```

**Header** (`.minicart-header`):
```css
height:60px; background:#E5D9CB;
display:flex; align-items:center; justify-content:space-between;
padding:0 24px;
```
- Title "Giỏ hàng": `20px Medium #000`.
- Close control: white pill, `padding:4px 4px 4px 16px; border-radius:34px;` containing text "Đóng" (`13px Regular #000`) + `Xnix/Line/Cross` 20×20 icon, `gap` implicit via flex.

**Body** (scrollable item list):
```css
padding: 16px 16px 80px; /* bottom padding reserves space so last item isn't hidden under sticky footer */
display:flex; flex-direction:column; align-items:center;
overflow-y:auto;
```
Item rows identical anatomy to cart page rows (thumbnail 88×96 `#F2F2F2`/4px radius, name 13px Regular, cross icon 20px, qty stepper same 40×30/5px desktop-style buttons, price 16px Medium), separated by `border-bottom:1px solid #BFBFBF` except last.

**Upsell block**: identical "HOÀN CHỈNH VỚI" pattern as cart page (heading 16px Medium centered, bordered list `#BFBFBF`/8px radius, add-on rows with 44×48 thumb, 13px Regular name/price, "Thêm" button `#410C13`/`#F2F2F2` text, `13px Regular` here — matches mobile weight not desktop).

**Footer (sticky, non-scrolling)**:
- Discount row: `border-top:1px solid #9B9B9B` (note: darker grey-400 divider here vs `#BFBFBF` elsewhere), `padding:16px`, "Giảm giá" 16px Medium `#000` / value 16px Medium `#00985B`.
- Totals+CTA row: `border-top:1px solid #BFBFBF; height:80px;` split 50/50 — left "Tổng cộng" (16px Medium label / 20px Medium amount), right CTA `background:#410C13`, label **"Kiểm tra"** (not "Thanh toán" — mini-cart uses different checkout copy), `20px Medium #fff`.

### Razor notes (_MiniCart.cshtml partial)

- This is the drawer/offcanvas partial triggered from the header cart icon. Implement as a fixed-position panel (`position:fixed; right:0; bottom:0;` or a right-side slide-in — confirm animation direction with product; Figma only shows static state) with the overlay backdrop `rgba(0,0,0,0.4)` behind it (seen explicitly in the add-on drawer node, and implied here).
- Reuse the same `_CartItemRow` and add-on row partials as the full cart page — only container chrome (header bar, tan bg, "Đóng" pill, "Kiểm tra" CTA copy) differs.
- Divider color inconsistency to note for QA: discount-row top border is `#9B9B9B` here vs `#BFBFBF` on the full cart page — replicate exactly per node or normalize to one token (recommend normalizing to `#BFBFBF` for consistency unless design intends emphasis).

---

## Add-on Drawer ("Thêm sản phẩm lẻ")

Node `970:20686`, 568×816 canvas — a wider variant of the mini-cart used mid-flow (e.g., opened from a product page or the "Thêm" upsell action), plus a **nested bottom-sheet modal** for picking add-on variants (size/color) before adding.

**Outer container**: same recipe as mini-cart but wider (`width:568px`), `border-radius:8px`, `box-shadow:0 4px 20px rgba(0,0,0,0.15)`.

**Header**: taller than mini-cart — `height:88px`, `background:#E5D9CB`, `padding:0 24px`. Title "Giỏ hàng" uses **Heading 4** (`Josefin Sans SemiBold 24/32`) — larger/bolder than the mini-cart's 20px Medium. Close pill: white, `"Đóng"` **16px Medium** (vs mini-cart's 13px Regular) + cross icon.

**Body**: `padding:16px 32px 80px` (wider horizontal padding than mini-cart's 16px, matching desktop cart's 32px). Item rows and add-on/upsell block are structurally identical to the cart page desktop version (44×48 add-on thumb, "Thêm" button 16px Medium).

**Footer**: identical structure to mini-cart footer — discount row (border-top `#9B9B9B`) + totals/CTA row (border-top `#BFBFBF`, height 80px, CTA label **"Kiểm tra"**, 20px Medium).

**Nested variant-picker bottom sheet** (overlay state, `970:20887`–`970:20931`):
```css
position:absolute; inset:0;
background: rgba(0,0,0,0.4);
display:flex; flex-direction:column; align-items:center; justify-content:flex-end;
```
- Circular white close button (`44px` incl. 10px padding, `border-radius:45px`) centered above the sheet, containing 24×24 cross icon.
- Sheet panel: `background:#fff; border-radius:8px 8px 0 0; padding:24px 24px 40px; display:flex; flex-direction:column; gap:32px; width:100%;`
  - Row 1: product thumb (44×48, `#F2F2F2`) + name (16px Medium) + variant subtitle (13px Regular, e.g. "40cm,navy") on left, price (13px Regular) on right.
  - Option fields ("Size", "Color"): label `16px Medium #000` + value box `border:1px solid #BFBFBF; border-radius:4px; height:40px; padding:8px 16px; font:13px Regular;` — read as a select/dropdown trigger (style like a disabled-looking input; likely opens its own picker).
  - Bottom row: qty stepper (scaled up ~1.4×: buttons ~59.6×44.7px, `border:1.86px solid #DCDCDC`, `radius:7.5px`; count text 23.86px Medium) + primary CTA "Thêm vào giỏ" (`flex:1; background:#410C13; height:56px; border-radius:8px; color:#fff; font:20px Medium;`), `gap:33px` between stepper and button.

### Razor notes (_AddOnDrawer.cshtml / _VariantPickerSheet.cshtml)

- Treat as two stacked partials: the drawer itself (`_MiniCart`-like but with the larger 24px/88px header and 32px body padding) plus a modal/bottom-sheet partial for variant selection, toggled via JS when a "Thêm" button on an add-on row is clicked.
- The variant sheet's "Size"/"Color" boxes render as static text in the Figma mock (single option shown) — in Razor these should be `<select>` or custom dropdown components; visually style as bordered box per spec above regardless of implementation.
- Qty stepper in the sheet is visually the same component as elsewhere, just scaled ~1.4×; keep a single `.qty-stepper` CSS component with a `--scale` or size-variant modifier class (`.qty-stepper--lg`) rather than duplicating styles.
