# Order History — Design Spec (Account / Lịch sử đơn hàng)

Source: Figma file `uQFY9gwfNbNSeTM6zmspzo`
- Desktop: node `848:14779` (frame width 1925px, scaled screenshot 1024px wide → all px values below are **actual Figma values**, not the scaled screenshot)
- Mobile (with orders): node `1142:39622` (frame width 430px)
- Mobile (empty state): node `1147:42330` (frame width 430px)

> Note on order card content: the Figma frames only contain the **product line item** (thumbnail, name, variant, price) repeated 2x inside what will become the order card — they do not show an order-number header row, date, status badge chip, or "Xem chi tiết/Mua lại" action button as separate visible layers in this crop. The card structure below documents exactly what's in Figma (thumbnail + name + variant + price, repeated per product in the order) and layers in the standard e-commerce order-card chrome (order #, date, status badge, action button) as **inferred/recommended** additions consistent with the site's existing patterns — these are marked `[inferred]` so the front-end dev knows what's spec'd vs. extrapolated.

---

## Account Sidebar/Nav (shared)

Shared across all account sub-pages (Orders, Account Info, Address Book) as `_AccountSidebar` (desktop) / `_AccountMobileHeader` (mobile, header only — mobile has no persistent sidebar, presumably a hamburger drawer).

### Desktop layout
- Page root: flex column, full width, `background:#F2F2F2` for content area.
- **Header** (`_SiteHeader`, shared with rest of site): height `120px`, `background:#FFFFFF`, bottom border `1px solid #BFBFBF`.
  - Inner container: centered, `padding: 0 362px` (at 1925px frame width — i.e. content max-width ~1201px centered), bottom border `1px solid #9B9B9B` on the row.
  - Logo: `64px × 64px`, left-aligned.
  - Avatar chip (top right): circle `40px`, `background:#BFBFBF`, `border-radius:45px`, initial letter centered, font Josefin Sans Bold 20px, color `#410C13` (brand red-900).
- **Content region** wrapper: `background:#F2F2F2`, `padding: 0 362px`, min-height `960px`, content row is `display:flex` with two columns:
  - **Sidebar nav** column: fixed intrinsic width (auto, ~220–240px based on longest label "Lịch sử đơn hàng"), `padding-top:40px`, `padding-bottom:10px`.
    - Vertical list, `gap:37px` between items.
    - Font: Josefin Sans SemiBold, `24px / 32px` line-height (token: Desktop/Heading 4).
    - Items (in order): "Lịch sử đơn hàng", "Thông tin tài khoản", "Sổ địa chỉ", "Đăng xuất".
    - **Active state** (current page, e.g. "Lịch sử đơn hàng" on this page): `color:#000000` (black).
    - **Inactive state**: `color:#7A7A7A` (grey-500).
    - **"Đăng xuất" (Logout)**: always styled distinctly — `color:#AF2234` (brand red / `--brand-color-2`), regardless of active/inactive, since it's an action not a nav target.
  - **Main content** column: `flex:1`, `padding:40px 120px 10px 120px`, `gap:40px` (vertical) between search/tabs block and order list block.

### Mobile layout
- No persistent sidebar — replaced by a **top header bar** (`_AccountMobileHeader`) with:
  - Height auto (`padding:20px 16px`), `background:#FFFFFF`, bottom border `1px solid #BFBFBF`, inner row bottom border `1px solid #9B9B9B`.
  - Left: hamburger menu icon (`28px`, "Hicon / Bold / Menu Hamburger") — opens off-canvas nav drawer containing the same 4 sidebar links.
  - Center: logo `40px × 40px`.
  - Right: avatar chip, circle `28px`, `background:#BFBFBF`, `border-radius:31.5px`, initial letter Josefin Sans Bold 14px, color `#410C13`.
  - On the empty-state variant, logo+hamburger are grouped left in a `219px` wide flex row with logo pinned right of that group (visually same position, just different grouping in the layer tree — treat as identical markup to the with-orders variant).
- Content wrapper: `background:#F2F2F2`, `padding:0 16px`, min-height `960px` (viewport-based in real build — use `min-height:100vh` minus header), inner column `padding:20px 16px 10px`, `gap:20px`.
- Sidebar nav items are **not shown inline** on mobile — they live in the hamburger drawer only (reuse the same active/inactive/logout color rules as desktop: active `#000`, inactive `#7A7A7A`, logout `#AF2234`).

### Colors used (tokens)
| Token | Hex |
|---|---|
| Foundation/Grey/grey-50 | #F2F2F2 |
| Foundation/Grey/grey-300 | #BFBFBF |
| Foundation/Grey/grey-400 | #9B9B9B |
| Foundation/Grey/grey-500 | #7A7A7A |
| Foundation/Grey/grey-600 | #5A5A5A |
| Foundation/Grey/grey-700 | #3C3C3C |
| Foundation/Grey/grey-900 | #0F0F0F |
| Foundation/Red/red-900 | #410C13 |
| Brand red (--brand-color-2) | #AF2234 |
| Body text (default) | #000000 |

### Typography tokens
| Token | Font / weight | Size / line-height |
|---|---|---|
| Desktop/Heading 4 (sidebar nav items) | Josefin Sans SemiBold | 24px / 32px |
| Mobile/Heading 4 (avatar initial, desktop too) | Josefin Sans Bold | 20px / 28px |
| Common/Label (tabs, search placeholder, card title) | Josefin Sans Medium | 16px / 20px |
| Common/Body 2 (variant line, mobile inactive tab) | Josefin Sans Light | 16px / 18px |
| Common/Subtitle (price, desktop) | Josefin Sans Medium | 20px / 23px |
| Common/Caption (mobile product title, empty-state message) | Josefin Sans Regular | 13px / 16px |

Razor partial suggestion:
```
Views/Shared/_AccountSidebar.cshtml   -- desktop-only sidebar, takes ActivePage string ("Orders"|"Account"|"Address")
Views/Shared/_AccountMobileHeader.cshtml -- mobile header w/ hamburger + drawer (shared with rest of site header if hamburger nav is global)
```
CSS class scaffold:
```css
.account-layout { display:flex; background:#F2F2F2; }
.account-sidebar { width:auto; padding:40px 0 10px; display:flex; flex-direction:column; gap:37px; }
.account-sidebar a { font-family:'Josefin Sans'; font-weight:600; font-size:24px; line-height:32px; color:#7A7A7A; text-decoration:none; }
.account-sidebar a.active { color:#000; }
.account-sidebar a.logout { color:#AF2234; }
.account-content { flex:1; padding:40px 120px 10px; display:flex; flex-direction:column; gap:40px; }

@media (max-width: 768px) {
  .account-sidebar { display:none; } /* becomes drawer content */
  .account-content { padding:20px 16px 10px; gap:20px; }
}
```

---

## Status Filter Tabs

Row of 6 tabs: **Chờ xác nhận | Chờ lấy hàng | Đang giao hàng | Đã giao | Trả hàng | Đã hủy**.

### Desktop
- Container: full width, `border-bottom:1px solid #BFBFBF`, `display:flex; justify-content:space-between` (tabs evenly spread across full content width).
- Each tab: `padding:8px 16px`.
- **Active tab**: text `color:#000000`, font Josefin Sans Medium 16px/20px, `border-bottom:1px solid #0F0F0F` (sits on top of the container's grey border, visually "cuts through" it).
- **Inactive tab**: text `color:#7A7A7A`, same font Medium 16px/20px, no bottom border.

### Mobile
- Container: `height:49px`, `overflow-x:auto; overflow-y:hidden` — **horizontally scrollable** tab strip (not evenly spaced/justified; tabs sit left-aligned and overflow).
- Each tab: `padding:8px 16px`, `white-space:nowrap`.
- **Active tab**: color `#000000`, font Josefin Sans **Medium** 16px/20px, `border-bottom:1px solid #0F0F0F`.
- **Inactive tab**: color `#7A7A7A`, font Josefin Sans **Light** 16px/18px (note: mobile inactive tabs use Light weight vs desktop's Medium for all tabs — a subtle but real difference), no border.
- Underlying row still has `border-bottom:1px solid #BFBFBF` for the full-width baseline.

### Search bar (sits above tabs, both breakpoints)
- `height:48px`, `background:#FFFFFF`, `border-radius:8px`, `padding:0 16px`, `display:flex; align-items:center; gap:8px`.
- Search icon: `18.667px` (rendered inside a 32px square hit area).
- Placeholder text: "Tìm theo mã đơn hàng hoặc tên sản phẩm", Josefin Sans Medium 16px/20px, `color:#7A7A7A`.
- Desktop: fixed within `gap:24px` column above tabs. Mobile: `gap:16px` column above tabs.

CSS scaffold:
```css
.order-search { height:48px; background:#fff; border-radius:8px; padding:0 16px; display:flex; align-items:center; gap:8px; }
.order-search input::placeholder { font-family:'Josefin Sans'; font-weight:500; font-size:16px; color:#7A7A7A; }

.status-tabs { display:flex; border-bottom:1px solid #BFBFBF; }
.status-tabs .tab { padding:8px 16px; font-family:'Josefin Sans'; font-weight:500; font-size:16px; line-height:20px; color:#7A7A7A; background:none; border:none; border-bottom:1px solid transparent; cursor:pointer; }
.status-tabs .tab.active { color:#000; border-bottom-color:#0F0F0F; }

@media (max-width: 768px) {
  .status-tabs { justify-content:flex-start; overflow-x:auto; overflow-y:hidden; height:49px; flex-wrap:nowrap; }
  .status-tabs .tab { flex:0 0 auto; font-weight:300; font-size:16px; line-height:18px; } /* Light for inactive */
  .status-tabs .tab.active { font-weight:500; } /* Medium for active */
}
```
(Desktop keeps `justify-content:space-between` — omit the mobile override there.)

---

## Order Card

### What Figma shows (both breakpoints): product line item
- Card container: `background:#FFFFFF`, `border-radius:8px`, `display:flex; align-items:center; justify-content:space-between`.
- Contains one row per product in the order, each row: `display:flex; gap:16px (desktop) / 8px (mobile); align-items:center`.
  - **Thumbnail**: placeholder `background:#D9D9D9` (real build: product image).
    - Desktop: `88px × 81px`, `border-radius:4px`.
    - Mobile: `60px × 64px`, `border-radius:2.727px` (~3px).
  - **Product name**: Josefin Sans Medium 16px/20px desktop `#000`; mobile uses Josefin Sans **Regular** 13px/16px (Common/Caption).
  - **Variant line** (e.g. "Hộp 4 bánh / màu vàng"): Josefin Sans Light — desktop 16px/18px; mobile 11px/24px.
  - **Price**: desktop Josefin Sans Medium 20px/23px (Common/Subtitle); mobile Josefin Sans Medium 16px/20px.
- Card padding: desktop `16px` all sides; mobile `8px` all sides.
- Multiple product cards in the list stack with `gap:10px` (both breakpoints).
- Desktop card observed width in one instance locked to `742px` (likely a stray fixed-width layer in Figma) — **use `width:100%`** in the real build; treat 742px as a Figma artifact, not a spec.

### `[inferred]` Order card chrome (recommended, not literally present in the crop)
Standard e-commerce order-card pattern to wrap the product line item(s), consistent with brand tokens:
- **Card header row** (above products): order number (e.g. "Mã đơn: #HD00123") in Josefin Sans Medium 16px `#000`, order date in Josefin Sans Light 14px `#7A7A7A`, right-aligned **status badge** chip.
  - Status badge: pill (`border-radius:999px`, `padding:4px 12px`), Josefin Sans Medium 13px, background/text pairs per status:
    - Chờ xác nhận → bg `#F2F2F2` / text `#7A7A7A`
    - Chờ lấy hàng → bg `#FDF3E7` / text `#AA8656` (brand gold)
    - Đang giao hàng → bg `#EAF2FB` / text `#2E6DB4` (or site's existing "info" blue if defined)
    - Đã giao → bg `#EAF7EE` / text `#2E8B57` (success green)
    - Trả hàng → bg `#FBEAEA` / text `#AF2234` (brand red)
    - Đã hủy → bg `#F2F2F2` / text `#3C3C3C`, product rows at reduced opacity (0.6) to read as inactive
  - (Exact status colors are not defined in Figma for this page — pick from the site's existing badge/tag component if one already exists elsewhere in the file; otherwise the above is a reasonable brand-consistent default.)
- **Card footer row** (below products): order total (bold, brand red `#AF2234` or black — match price styling above) + action button(s), right-aligned:
  - "Xem chi tiết" → secondary/outline button, `border:1px solid #0F0F0F`, `color:#000`, `background:transparent`, `padding:8px 20px`, `border-radius:4px` (match site's existing secondary button style if defined elsewhere).
  - "Mua lại" (shown for Đã giao / Đã hủy / Trả hàng) → primary button, `background:#AF2234`, `color:#fff`, same padding/radius.
- Divider between header/products/footer: `1px solid #F2F2F2` or simple `gap` spacing (16px), no hard rule from Figma — use spacing over borders to stay consistent with the clean white-card look already shown.

Razor partial: `Views/Shared/_OrderCard.cshtml`, model `OrderCardViewModel { OrderNumber, OrderDate, Status, List<OrderLineItem> Items, decimal Total }`, with nested `_OrderLineItem` partial for the repeated thumbnail/name/variant/price row (this inner partial is what Figma fully specifies).

CSS scaffold:
```css
.order-card { background:#fff; border-radius:8px; padding:16px; display:flex; flex-direction:column; gap:16px; }
.order-card__header { display:flex; justify-content:space-between; align-items:center; }
.order-card__items { display:flex; flex-direction:column; gap:12px; }
.order-line { display:flex; align-items:center; gap:16px; }
.order-line__thumb { width:88px; height:81px; border-radius:4px; background:#D9D9D9; object-fit:cover; }
.order-line__name { font-family:'Josefin Sans'; font-weight:500; font-size:16px; line-height:20px; color:#000; }
.order-line__variant { font-family:'Josefin Sans'; font-weight:300; font-size:16px; line-height:18px; color:#000; }
.order-line__price { font-family:'Josefin Sans'; font-weight:500; font-size:20px; line-height:23px; }
.order-card__footer { display:flex; justify-content:flex-end; align-items:center; gap:12px; }
.order-list { display:flex; flex-direction:column; gap:10px; }

@media (max-width: 768px) {
  .order-card { padding:8px; gap:8px; }
  .order-line { gap:8px; }
  .order-line__thumb { width:60px; height:64px; border-radius:3px; }
  .order-line__name { font-weight:400; font-size:13px; line-height:16px; }
  .order-line__variant { font-weight:300; font-size:11px; line-height:24px; }
  .order-line__price { font-size:16px; line-height:20px; }
}
```

---

## Empty State

Mobile-only frame captured (`1147:42330`); apply the same pattern responsively to desktop (center the illustration+message in the content area at any width).

- Wrapper: `display:flex; flex-direction:column; align-items:center; justify-content:center; gap:10px`, mobile height `267px` (i.e. vertically centered within the remaining content area, not a fixed value to hard-code — use `min-height` + `flex:1` centering instead).
- Illustration: shopping-cart-with-sad-face SVG/PNG asset (`Group 17` in Figma, downloadable), intrinsic size `~97.575px × 90.61px`. Export as a static asset (e.g. `empty-orders.svg`) into the project's asset folder.
- Message: "Chưa có đơn hàng nào", Josefin Sans Regular 13px/16px, `color:#5A5A5A` (Foundation/Grey/grey-600).
- No CTA button shown in Figma for this state — but recommend adding a "Tiếp tục mua sắm" (Continue shopping) link/button beneath the message for UX completeness, styled as brand red text link `color:#AF2234`, Josefin Sans Medium 14px. `[inferred]`
- Status tabs and search bar remain visible above the empty state (state only replaces the order-list region), on both desktop and mobile.

CSS scaffold:
```css
.orders-empty { display:flex; flex-direction:column; align-items:center; justify-content:center; gap:10px; flex:1; min-height:267px; text-align:center; }
.orders-empty img { width:97.575px; height:90.61px; }
.orders-empty p { font-family:'Josefin Sans'; font-weight:400; font-size:13px; line-height:16px; color:#5A5A5A; }
.orders-empty .cta { margin-top:8px; font-family:'Josefin Sans'; font-weight:500; font-size:14px; color:#AF2234; text-decoration:none; }
```

Razor partial: `Views/Shared/_OrdersEmptyState.cshtml`, rendered inside `Orders.cshtml` when `Model.Orders.Count == 0` for the currently selected status tab.

---

## Summary of Desktop vs Mobile differences

| Element | Desktop | Mobile |
|---|---|---|
| Header height | 120px, centered content `padding:0 362px` | Auto height, `padding:20px 16px`, hamburger + drawer nav |
| Avatar chip | 40px circle, 20px Bold initial | 28px circle, 14px Bold initial |
| Sidebar nav | Inline column, 24px/32px SemiBold, gap 37px | Hidden — lives in hamburger drawer |
| Content padding | `40px 120px 10px` | `20px 16px 10px` |
| Status tabs layout | `justify-content:space-between`, evenly spread | Horizontal scroll, left-aligned, `overflow-x:auto` |
| Status tab inactive weight | Medium 16/20 | Light 16/18 |
| Search→tabs gap | 24px | 16px |
| Order card padding | 16px | 8px |
| Order line gap | 16px | 8px |
| Thumbnail size | 88×81px, radius 4px | 60×64px, radius ~3px |
| Product name style | Medium 16/20 | Regular 13/16 (Caption) |
| Variant style | Light 16/18 | Light 11/24 |
| Price style | Medium 20/23 (Subtitle) | Medium 16/20 |
