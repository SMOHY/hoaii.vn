# Category Landing Page Template — Design Spec

Source: Figma file `uQFY9gwfNbNSeTM6zmspzo`
Desktop reference frame: "Trà" — node `1151:31789` (1920px wide)
Mobile reference frame: "Trà" — node `1151:33130` (430px wide)

> **This single template is reused for all 13 category/occasion landing pages**
> (Trà, Khăn, Tượng gốm, Rượu, and the 9 occasion pages: Quà tết, Quà trung thu, etc).
> Only the content changes per page: page title (H1 in hero), hero banner background image,
> intro/description text under the filter bar, and the product list/pagination data.
> Build this as one Razor view `Views/Category/Index.cshtml` driven by a `CategoryPageViewModel`
> (Title, HeroImageUrl, Description, Products, CurrentPage, TotalPages, plus the "featured/limited"
> promo block content) — do not fork the view per category.

Page structure (top to bottom), from Figma metadata (4 top-level child frames under the page frame,
plus a Nav instance and Footer instance):

1. **Hero** (`Hero`, node `1151:31790`) — contains the sticky/top Nav (`Frame 494` instance) + banner with page title
2. **Sản phẩm** ("Products", node `1151:31802`) — breadcrumb + Filter/sort bar + product grid + pagination
3. **Sản phẩm giới hạn/đặc biệt** ("Limited/Special Products" promo banner, node `1151:31826`) — split image/text CTA block, editorial promo, not a product card
4. **Footer** (instance, shared component — out of scope for this spec)

No product-detail link/modal was found wired into this template — cards only expose an
add-to-cart affordance ("Thêm vào giỏ") and a bag icon overlay; the "Sản phẩm giới hạn" promo
block has a "Mua ngay" (Buy now) button. Presumably the whole card links to `/san-pham/{slug}`
(PDP) via an anchor wrapper — confirm with dev/UX before wiring routes, and separately review UI Kit
frame `722:18319` ("Các trạng thái sản phẩm") for full badge-state coverage (new/sale/out-of-stock)
before finalizing `_ProductCard`.

---

## Design Tokens (used throughout this template)

**Colors**
| Token | Hex |
|---|---|
| Brand red (headline/CTA) | `#AF2234` |
| Brand red 600 (hover) | `#9F1F2F` |
| Brand red 900 (add-to-cart button bg) | `#410C13` |
| Grey 900 | `#0F0F0F` |
| Grey 800 (product name) | `#1C1C1C` |
| Grey 700 | `#3C3C3C` / `#000000` |
| Grey 600 (sub text) | `#5A5A5A` |
| Grey 500 | `#7A7A7A` |
| Grey 400 (input border) | `#9B9B9B` |
| Grey 100 (banner placeholder bg) | `#D6D6D6` |
| Grey 50 (card image bg / footer text on dark) | `#F2F2F2` |
| Yellow/Tan 50 (filter chip bg) | `#F7F3EE` |
| Yellow/Tan 100 (promo banner bg) | `#E5D9CB` |
| White | `#FFFFFF` |

**Typography** (font family `Josefin Sans` unless noted)
| Token | Size / Weight / Line-height |
|---|---|
| Hero H1 ("LC Sac Trial" display font) | 80px / Regular / 70px line-height, 0.7px tracking — Desktop |
| Hero H1 — Mobile | 28px / Regular / 70px line-height (same font) |
| Heading 3 (Desktop) — filter title, promo subtitle | 32px / Medium(500) / 40px |
| Heading 3 (Mobile) | 24px / Medium(500) / 32px |
| Heading 4 (Desktop) — promo eyebrow | 24px / SemiBold(600) / 32px |
| Body 1 (Common) | 20px / Regular(400) / 24px |
| Subtitle (Common) — sort chip, price, card title, pagination | 20px / Medium(500) / 23px |
| Label (Common) — mobile sort label | 16px / Medium(500) / 20px |
| Caption (Common) — filter description, mobile | 13px / Regular(400) / 16px |

---

## Hero Banner

### Desktop
- Root: `Hero` frame, full-bleed `1920px` wide, white bg, `flex-col`, items aligned to end (banner sits below nav).
- Nav bar instance ("Frame 494") is **absolutely positioned** at top (`left:0, top:0, width:1920px`), height `168px`, sits on top of/before the banner in stacking — treat as a shared `_Nav` partial with 3 stacked rows:
  - Promo strip: bg `#AF2234`, height 40px, centered white text, Josefin Sans Medium 14px/24px, tracking 0.4px — "Miễn phí đơn hàng từ 500.000đ"
  - Utility strip: bg `#F2F2F2`, height 48px, content max-width 1440px centered, `justify-between`; left = icon + announcement text (15px Regular) — driven by the site-wide `announcement_text` CMS setting, so it matches whatever the home hero uses ("Đón đầu xu hướng quà tặng với những vật phẩm thiết kế mới nhất vừa ra mắt!"), not the older "Hơn 100+ mẫu bánh..." copy still baked into this Figma frame's nav instance; right = "Về chúng tôi / Liên hệ / Đại lý / Blog" links, gap 40px
  - Main nav: height 80px, bottom border `1px solid #7A7A7A`, content max-width 1440px centered, `justify-between`: 48×48 logo | nav links (Quà tết, Quà trung thu, Quà theo dịp, Sản phẩm chọn lọc — bold 15px + dropdown chevron, gap between items ~pl-8/pr-6/py-4 padding, rounded 4px hover state) | icon cluster (search 32px, user 32px, cart/shop 32px, gap 16px) + "VN/EN" toggle
- Banner container: padding `168px 240px 0` (outer wrapper) then inner `Banner` box: width `1440px` (viewport 1920 − 2×240 margins), height `600px`, bg `#D6D6D6` (placeholder — replace with `HeroImageUrl` as `background-image: cover`), `padding: 0 0 56px 80px`, content `flex-col justify-end`.
- Title text sits inside a `1360×544` inner frame, bottom-aligned, full width — this is the page H1 ("TRÀ"), color brand red `#AF2234`, font "LC Sac Trial" 80px/70px, tracking 0.7px.

CSS-ready (desktop):
```css
.hero-banner-wrap { padding: 168px 240px 0; }
.hero-banner { width: 1440px; max-width: 100%; height: 600px; background: #D6D6D6 center/cover no-repeat;
  display: flex; flex-direction: column; justify-content: flex-end; padding: 0 0 56px 80px; }
.hero-banner__title { font-family: "LC Sac Trial", serif; font-size: 80px; line-height: 70px;
  letter-spacing: 0.7px; color: #AF2234; margin: 0; }
```

### Mobile
- Same structural pattern, no side margins. Nav collapses to a `92px`-tall condensed bar (hamburger + logo + icons — confirm against `1151:33142` "Frame 494" mobile nav instance separately; not expanded in this pull).
- Banner: full width `430px`, height `280px`, bg `#D6D6D6`, `padding: 0 28px` (inner content), `flex-col justify-end` (no bottom padding shown — text sits flush to bottom).
- Title: 28px / 70px line-height (line-height is oversized relative to font — matches desktop token reused as-is), same color/font/tracking.

```css
.hero-banner-wrap { padding-top: 92px; }
.hero-banner { width: 100%; height: 280px; background: #D6D6D6 center/cover no-repeat;
  display: flex; flex-direction: column; justify-content: flex-end; padding: 0 28px; }
.hero-banner__title { font-family: "LC Sac Trial", serif; font-size: 28px; line-height: 70px;
  letter-spacing: 0.7px; color: #AF2234; margin: 0; }
```

---

## Filter/Sort Bar

Node: `Filter` instance — desktop `1151:31807` (1424×116 slot), mobile `1151:33148` (398×107 slot).
This sits directly above the product grid, inside the same content container as breadcrumb
("Trang chủ/Mua quà tết") + grid + pagination (container: desktop `1440px` wide, centered via
`240px` side margins; mobile `398px` wide with `16px` side margins).

### Desktop
- Layout: `flex; align-items: flex-end; justify-content: space-between; padding: 16px 0 24px;`
- Left block (width 722px): title "Tất cả loại trà" — Heading 3 (32px/Medium/40px), black; description below — 24px/Regular/32px, color `#5A5A5A`, gap 4px between the two lines.
- Right block: `flex; align-items:center; gap:24px;`
  - Label "Sắp xếp theo" — 20px Regular, color `#303030`
  - Sort dropdown button: border `1px solid #9B9B9B`, height 48px, radius 4px, padding `8px 8px 8px 16px`, gap 8px, label "Nổi bật" (20px Medium `#303030`, fixed width 88px) + chevron icon (24px, rotated 90°)
  - Filter button ("Bộ lọc"): bg `#F7F3EE`, height 48px, radius 4px, same padding, 20px Medium `#303030` + chevron
  - Gap between sort dropdown and filter button: 16px

```css
.filter-bar { display: flex; align-items: flex-end; justify-content: space-between; padding: 16px 0 24px; }
.filter-bar__heading { width: 722px; }
.filter-bar__title { font: 500 32px/40px "Josefin Sans"; color: #000; margin: 0; }
.filter-bar__desc { font: 400 24px/32px "Josefin Sans"; color: #5A5A5A; margin: 4px 0 0; }
.filter-bar__actions { display: flex; align-items: center; gap: 24px; }
.filter-bar__sort-label { font: 400 20px/24px "Josefin Sans"; color: #303030; }
.filter-bar__dropdown, .filter-bar__filter-btn {
  display: flex; align-items: center; gap: 8px; height: 48px; border-radius: 4px;
  padding: 8px 8px 8px 16px; font: 500 20px/23px "Josefin Sans"; color: #303030; cursor: pointer; }
.filter-bar__dropdown { border: 1px solid #9B9B9B; background: transparent; }
.filter-bar__filter-btn { background: #F7F3EE; border: none; }
```

### Mobile
- Stacked vertically, `gap: 8px`. No side-by-side title/sort — everything flows top to bottom:
  1. Title "Tất cả" — 20px Medium/23px, black
  2. Description — 13px Regular/16px, `#5A5A5A`, gap 4px from title
  3. "Sắp xếp theo" label — 16px Medium/20px, `#303030`, gap 4px below description block
  4. Row of two chips, `gap:16px`: sort dropdown (border `#9B9B9B`, height 32px, radius 4px, padding `4px 8px 4px 16px`, gap 16px, text 13px Regular `#303030`) and filter chip (bg `#F7F3EE`, same sizing, text "Bộ lọc")
- No visible sidebar filter panel in this frame — filtering is presumably a slide-over/drawer triggered by the "Bộ lọc" chip (not expanded in the pulled frames). Note this for later drill-down into any "Filter Drawer" component if one exists in the file.

```css
.filter-bar--mobile { display: flex; flex-direction: column; gap: 8px; }
.filter-bar--mobile .filter-bar__title { font: 500 20px/23px "Josefin Sans"; }
.filter-bar--mobile .filter-bar__desc { font: 400 13px/16px "Josefin Sans"; color: #5A5A5A; }
.filter-bar--mobile .filter-bar__sort-label { font: 500 16px/20px "Josefin Sans"; color: #303030; }
.filter-bar--mobile .filter-bar__actions { display: flex; gap: 16px; }
.filter-bar--mobile .filter-bar__dropdown, .filter-bar--mobile .filter-bar__filter-btn {
  height: 32px; padding: 4px 8px 4px 16px; gap: 16px; font: 400 13px/16px "Josefin Sans"; color: #303030; }
```

No sidebar (category tree / price range / size) filter layout was present in this frame — the
page uses a top toolbar only (sort + a single "Bộ lọc" trigger, presumably opening a modal/drawer
with those facets). Flag this for a follow-up Figma pull if a filter-drawer frame exists elsewhere
in the file.

---

## Product Grid

Container: `Frame 234` (desktop `1151:31808`, mobile `1151:33149`), inside content area
(desktop 1440px wide with 240px side margins; mobile 398px wide with 16px side margins).
Grid renders in **rows of 3 desktop-cards** stacked as sibling row-frames (not a CSS grid in the
source — but a CSS grid is the correct implementation choice), each row `742px` apart center-to-center
(686px card height + 56px row gap) on desktop, and as a **single column** on mobile.

Pagination footer sits below the grid: `Frame 158`, centered, shows `‹ 1/2 ›` style pager
(prev arrow hidden/disabled state via `opacity-0` on first page, "1/2" label 20px Medium `#303030`,
next arrow icon 20×20).

### Desktop
- Grid: 3 columns × N rows, column width `464px`, column gap `24px` (976−488−464=24, consistent), row gap `56px` (742−686=56).
- Row/grid container width: `1440px` (3×464 + 2×24 = 1440 ✓).

```css
.product-grid { display: grid; grid-template-columns: repeat(3, 1fr); gap: 56px 24px; }
.pagination { display: flex; align-items: center; justify-content: center; gap: 13px; padding: 10px; }
.pagination__label { font: 500 20px/23px "Josefin Sans"; color: #303030; }
.pagination__arrow { width: 20px; height: 20px; }
.pagination__arrow--disabled { opacity: 0; pointer-events: none; }
```

### Mobile
- Single column, full width (398px cards), row gap `24px` (595−571=24 between cards within a "row" group; groups stack contiguously at 1817−1761=56 between the 3-card groups, but since it's 1-column the effective gap is uniform 24px per card — treat as one continuous list).

```css
.product-grid--mobile { display: grid; grid-template-columns: 1fr; gap: 24px; }
```

---

## Product Card (reusable `_ProductCard` partial)

Base component pulled from repeated instances "Frame 190/191/192" (naming suggests 3 *variant/state*
slots per row, not 3 different card designs — likely used to show different badge states across the
grid for QA). Also cross-referenced against UI Kit node `722:18312` "Các trạng thái sản phẩm" (Default state).
**Follow-up needed**: open UI Kit frame `722:18319` for the complete badge matrix (new / sale % /
out-of-stock) — this pull only surfaced the Default (no-badge) state plus a hidden badge-row template
(`Frame 54` with "Hàng mới" / "-20%" pills, currently `hidden=true` in the promo block, confirming
badges exist as toggleable pills but weren't visible/active on this particular page instance).

### Anatomy (Desktop) — card total 464×686
1. **Image area**: `464×556`, bg `#F2F2F2` (placeholder for product photo), `border-radius: 4px`, `overflow: hidden`, `position: relative`.
   - Badge pills (when present, top-left, currently seen only in hidden template): e.g. "Hàng mới" pill and "-20%" pill, each `height:36px`, `padding: 8px 16px`, rounded, small gap between pills.
   - Hover/persistent **Add-to-cart button**: absolutely positioned bottom-right inset (`padding: 16px 24px 24px 16px` from container, i.e. bottom-aligned flush inside image), bg `#410C13`, height 60px, radius 4px, padding `0 16px`, `display:flex; gap:12px; align-items:center`, contains a 28×28 "+" icon + label "Thêm vào giỏ" (20px Medium, color `#F2F2F2`).
2. **Info block** below image, `padding-top: 12px` implied by gap, `gap: 12px` between name/price group and color-swatch row:
   - Product name: 32px Medium/40px, color `#1C1C1C`
   - Price: 24px Regular-ish (Subtitle-ish)/32px, color `#303030`, format `"899.000"` (VND, no currency suffix shown in mock — confirm formatting convention, e.g. append "đ")
   - Color/variant swatches row: `gap: 9px`, each swatch a `24×10px` color bar with `4px` vertical padding; the *selected* swatch has a `border-bottom: 1px solid #000` beneath it (indicates active variant). Colors seen in sample: red `#E60000`, blue `#0077FF`, yellow `#FFC300`, green `#00E007` — these are placeholder demo colors per product, not fixed brand tokens.

```css
.product-card { display: flex; flex-direction: column; gap: 20px; width: 464px; }
.product-card__image { position: relative; width: 100%; height: 556px; background: #F2F2F2;
  border-radius: 4px; overflow: hidden; }
.product-card__badges { position: absolute; top: 16px; left: 16px; display: flex; gap: 8px; }
.product-card__badge { height: 36px; padding: 8px 16px; border-radius: 4px; font: 400 14px/20px "Josefin Sans"; }
.product-card__badge--new { background: #fff; color: #1C1C1C; }
.product-card__badge--sale { background: #AF2234; color: #fff; }
.product-card__add-to-cart { position: absolute; right: 16px; bottom: 24px; left: 16px;
  display: flex; align-items: center; justify-content: center; gap: 12px; height: 60px;
  background: #410C13; border: none; border-radius: 4px; color: #F2F2F2;
  font: 500 20px/23px "Josefin Sans"; }
.product-card__add-to-cart svg { width: 28px; height: 28px; }
.product-card__info { display: flex; flex-direction: column; gap: 12px; }
.product-card__name { font: 500 32px/40px "Josefin Sans"; color: #1C1C1C; margin: 0; }
.product-card__price { font: 500 24px/32px "Josefin Sans"; color: #303030; margin: 0; }
.product-card__swatches { display: flex; align-items: center; gap: 9px; }
.product-card__swatch { width: 24px; height: 10px; padding: 4px 0; background-clip: content-box; }
.product-card__swatch--active { border-bottom: 1px solid #000; }
```

### Anatomy (Mobile) — card total 398×571
- Image area: `398×470` (vs desktop's taller 556 — slightly less tall relative to width), same `#F2F2F2` bg, `border-radius: 4px`.
- Add-to-cart control differs on mobile: **no full-width button** — instead a small `40×40` circular/rounded icon-only button (bg `rgba(247,243,238,0.5)`, i.e. translucent tan `#F7F3EE` at 50% opacity), positioned bottom-right with `16px` inset, containing just the bag/shop icon (no text label). This is a lighter-weight affordance than desktop's full CTA bar — confirm tap target meets 44px a11y minimum (40px is borderline; recommend bumping to 44px in implementation).
- Info block: same structure, but type scale drops — name 20px Medium/23px (`#1C1C1C`), price 16px/20px (`#303030`), swatch row unchanged (24×10px bars, gap 9px).

```css
.product-card--mobile { width: 100%; gap: 20px; }
.product-card--mobile .product-card__image { height: 470px; }
.product-card--mobile .product-card__add-to-cart {
  position: absolute; right: 16px; bottom: 16px; left: auto; width: 40px; height: 40px;
  min-width: 44px; min-height: 44px; /* a11y: bump from Figma's 40px */
  border-radius: 4px; background: rgba(247, 243, 238, 0.5); padding: 0; }
.product-card--mobile .product-card__add-to-cart .cta-label { display: none; }
.product-card--mobile .product-card__name { font: 500 20px/23px "Josefin Sans"; }
.product-card--mobile .product-card__price { font: 500 16px/20px "Josefin Sans"; }
```

### Razor partial shape (suggested)
```
Views/Category/Index.cshtml
Views/Shared/_ProductCard.cshtml   (model: ProductCardViewModel { Slug, ImageUrl, Name, Price, Badges[], Variants[] })
```
`_ProductCard.cshtml` renders both desktop/mobile markup from one partial; visibility is handled
purely via CSS breakpoint (swap `.product-card__add-to-cart` content/position at a mobile breakpoint,
e.g. `@media (max-width: 767px)`), not via server-side UA sniffing.

---

## "Sản phẩm giới hạn/đặc biệt" — Limited/Special Editorial Promo (non-card section, FYI)

Not part of the reusable product-card component, but part of this same template (appears once,
below the product grid, before the footer) — bg `#E5D9CB`, split layout:

**Desktop** (`1151:31826`, 1920×820): `display:flex; gap:80px; padding:80px;` — left column (text: eyebrow
24px SemiBold `#0F0F0F`, body/paragraph 32px Medium, "Mua ngay" white pill button `padding:16px 20px`, 24px SemiBold `#1C1C1C`) fixed at `pl-240px`, right column: `flex:1`, image block `660px` tall, `border-radius:8px`, with the same red add-to-cart affordance overlaid bottom-left as on product cards.

**Mobile** (`1268:39670`, 430×499): stacked, image full-width `260px` tall (no radius), then text block `padding:0 16px`, eyebrow 13px Regular `#5A5A5A`, body 20px Medium `#0F0F0F`, "Mua ngay" button `padding:12px 16px` 16px Medium, right-aligned.

This block's title/subtitle/CTA/image should also be data-driven per category page (same content model — treat as `PromoBanner` field on the `CategoryPageViewModel`).
