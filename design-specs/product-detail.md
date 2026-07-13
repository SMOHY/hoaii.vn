# Product Detail Page — Design Spec

Source: Figma file `uQFY9gwfNbNSeTM6zmspzo`
- Desktop frame: node `826:14920` ("Sản phẩm"), 1920 × 4825
- Mobile frame: node `1142:25698` ("Sản phẩm"), 430 × 6090

Target: `Views/Product/Details.cshtml` (Razor + plain CSS, no Tailwind).

## Design Tokens (reused across all sections)

**Colors**
| Token | Hex | Usage |
|---|---|---|
| grey-50 | `#F2F2F2` | light backgrounds, secondary buttons, quantity input bg |
| grey-100 | *(not sampled, use #E8E8E8 if needed)* | — |
| grey-200 | `#DCDCDC` | thumbnail inactive, borders |
| grey-300 | `#BFBFBF` | image placeholder bg |
| grey-400 | `#9B9B9B` | active thumbnail bg, dot indicator active |
| grey-500 | `#7A7A7A` | secondary text, dividers, swatch border |
| grey-600 | `#5A5A5A` | button outline border |
| grey-700 | `#3C3C3C` / `#000000` | body text |
| grey-800 | `#1C1C1C` | headings, primary text |
| red-900 (brand) | `#410C13` | primary CTA background, "add to cart" pill on cards |
| red-50 | `#F7E9EB` | secondary CTA background, collection section bg |
| Brand red (swatch) | `#AF2234` / sample `#870000` | color swatch option |
| Brand gold/tan | `#AA8656` | color swatch option (CSS var `--brandcolor-1`) |
| Sample swatch blue | `#00488C` | color swatch option |
| Card variant swatches | `#E60000`, `#0077FF`, `#FFC300`, `#00E007` | small color-availability chips on related product cards |

**Typography** (font-family `"Josefin Sans"` unless noted)
| Token | Style/Weight | Size | Line-height |
|---|---|---|---|
| Heading (collection title, "LC Sac Trial") | Regular | 48px desktop / 20px mobile | 56px / 32px |
| Heading 3 (related products title, "LC Sac Trial") | Regular | 48px desktop / 20px mobile | 56px / 32px |
| Heading 4 (card title / product name) | SemiBold | 24px | 32px |
| Subtitle | Medium | 20px | 23px |
| Label | Medium | 16px | 20px |
| Body 2 | Light | 16px | 18px |
| Caption (mobile labels) | Regular | 13px | 16px |

Note: mobile scales most labels down (e.g. price/title block uses 16px/Medium across the board rather than 24/20 split), and CTA button text drops to 13px Regular caption size instead of 16px Medium.

---

## Main Product Info

### Desktop
Container: full-bleed section, inner content padded `240px` horizontal, `40px` top / `80px` bottom, `bg: white`.

Layout: two-column flex, `justify-content: space-between`, gap driven by fixed column widths:
- Left column (gallery): `646px` wide, `flex-direction: column`, `gap: 16px`, items centered.
  - Main image: `646×708px`, `border-radius: 8px`, `background: #BFBFBF` (placeholder for product photo).
  - Thumbnail row: `flex; gap: 16px`, 5 thumbnails, each `100×100px`, `border-radius: 4px`. First/active thumb `#9B9B9B`, rest `#DCDCDC`.
- Right column (info): `466px` wide, `flex-direction: column`, `gap: 40px`, vertically centered against gallery height (`align-self: stretch` container).

Right column internal structure (top to bottom):
1. **Title block** (`gap: 16px`)
   - Product title: 24px SemiBold, `#000`/`#1C1C1C`, `line-height: 32px` — e.g. "SET QUÀ TẾT VIỆT NAM HẠO CA"
   - Price: 20px Medium, `line-height: 23px` — e.g. "780.000 đ"
2. **Variant selectors** (`gap: 24px` block, appears right under title with `gap: 48px` from block above)
   - Color swatches: label "MÀU SẮC : đỏ" (16px Medium), row of `32×32px` circular/rounded swatch buttons, `gap: 16px`, `border-radius: 3px`, selected swatch has visible outer ring (`border: 0.8px solid #7A7A7A` container + inner `border-radius: 0.96px` fill), unselected swatches border `#303030`.
   - Box type (LOẠI HỘP): label 16px Medium, button group `gap: 16px`, each pill `160×48px`, `border-radius: 4px`, `padding: 10px`, text 16px Medium. Selected state: `bg #F2F2F2` + `border 1px solid #5A5A5A`. Unselected: `bg #F2F2F2`, no border.
3. **Quantity + Share row** (`justify-content: space-between`)
   - Stepper: `[-] [qty] [+]` — decrement/increment buttons `59.6×44.7px` (~60×45px rounded 7.5px), `border: 1.86px solid #DCDCDC`, icon centered; quantity display 24px Medium centered, width ~48px.
   - Share icon: `40×40px` outline icon, right-aligned.
4. **CTA buttons** (`gap: 16px`, full width `466px`)
   - Primary "THÊM VÀO GIỎ HÀNG" (Add to cart): full width, `height: 56px`, `bg: #410C13`, `border-radius: 4px`, text 16px Medium white centered.
   - Secondary "NHẬN ƯU ĐÃI MUA SỐ LƯỢNG LỚN" (bulk-order CTA / buy-now equivalent): full width, `height: 56px`, `bg: #F7E9EB`, text 16px Medium color `#410C13` centered.
5. **Ingredients / trust block**: top border `1px solid #7A7A7A`, `padding: 36px 0`, `gap: 13px`.
   - Label "THÀN PHẦN :" (sic — likely "THÀNH PHẦN") 16px Medium.
   - Body text 16px Light, `line-height: 18px`, e.g. ingredient list string.

Breadcrumb above gallery: full-width bar, centered content within `240px` gutters, text 16px Medium `#1C1C1C` — "Trang chủ/Mua quà tết".

**CSS-ready skeleton:**
```css
.pdp-main { display:flex; flex-direction:column; padding:40px 240px 80px; background:#fff; }
.pdp-breadcrumb { padding:0 240px; font:500 16px/20px "Josefin Sans"; color:#1C1C1C; }
.pdp-gallery { width:646px; display:flex; flex-direction:column; gap:16px; }
.pdp-gallery__main { width:646px; height:708px; border-radius:8px; background:#BFBFBF; object-fit:cover; }
.pdp-gallery__thumbs { display:flex; gap:16px; }
.pdp-gallery__thumb { width:100px; height:100px; border-radius:4px; background:#DCDCDC; cursor:pointer; }
.pdp-gallery__thumb.active { background:#9B9B9B; }
.pdp-info { width:466px; display:flex; flex-direction:column; gap:40px; }
.pdp-title { font:600 24px/32px "Josefin Sans"; color:#1C1C1C; }
.pdp-price { font:500 20px/23px "Josefin Sans"; }
.pdp-swatch { width:32px; height:32px; border-radius:3px; border:0.8px solid #303030; }
.pdp-swatch.selected { border:0.8px solid #7A7A7A; }
.pdp-box-option { width:160px; height:48px; border-radius:4px; background:#F2F2F2; font:500 16px/20px "Josefin Sans"; }
.pdp-box-option.selected { border:1px solid #5A5A5A; }
.pdp-qty-stepper button { width:60px; height:45px; border-radius:7.5px; border:1.86px solid #DCDCDC; background:transparent; }
.pdp-cta-primary { height:56px; border-radius:4px; background:#410C13; color:#fff; font:500 16px/20px "Josefin Sans"; }
.pdp-cta-secondary { height:56px; border-radius:4px; background:#F7E9EB; color:#410C13; font:500 16px/20px "Josefin Sans"; }
.pdp-ingredients { border-top:1px solid #7A7A7A; padding:36px 0; }
```

### Mobile
Container: `padding: 16px` (breadcrumb at `pt:0`, gallery block `pt:20px`), single column, `bg: white`.

- Breadcrumb: 13px Regular, `line-height:16px` — "Trang chủ/" (`#1C1C1C`) + "Quà tết" (`#7A7A7A`).
- Gallery: full width, `gap:8px`.
  - Main image: full width × `460px`, `border-radius:8px`, `#BFBFBF`.
  - Dot indicator row (replaces thumbnail strip): centered, `gap:8px`, active dot `8×8px` `#9B9B9B` circle, inactive dots `6×6px` `#DCDCDC`.
- Info block stacked below, `gap:16px`:
  1. Title+price combined block, `gap:4px`, both at 16px Medium/`line-height:20px` (title and price same size on mobile, unlike desktop).
  2. Color swatches: same pattern, swatches shrink to `24×24px`, `border:0.6px`.
  3. Box type buttons: auto-width pills `padding:8px 16px`, text 13px Regular (Caption), `gap:8px` between pills.
  4. Quantity stepper: buttons `53.3×40px` rounded `6.7px`, `border:1.67px solid #DCDCDC`; qty text 14.3px Medium. Share icon `32×32px`.
  5. CTA buttons: full width, `height:52px`, text drops to 13px Regular (not Medium 16 like desktop). Same colors: primary `#410C13`/white, secondary `#F7E9EB`/`#410C13`.
  6. Ingredients block: `border-top:1px solid #7A7A7A`, `padding:16px 0 20px`, label 16px Medium, body 16px Light/18px line-height (text size unchanged from desktop, only surrounding paddings shrink).

**CSS-ready skeleton (mobile overrides via media query or separate mobile CSS):**
```css
@media (max-width:767px){
  .pdp-main { padding:20px 16px 0; }
  .pdp-gallery__main { height:460px; width:100%; }
  .pdp-dots { display:flex; gap:8px; justify-content:center; }
  .pdp-dot { width:6px; height:6px; border-radius:20px; background:#DCDCDC; }
  .pdp-dot.active { width:8px; height:8px; border-radius:23px; background:#9B9B9B; }
  .pdp-title, .pdp-price { font:500 16px/20px "Josefin Sans"; }
  .pdp-swatch { width:24px; height:24px; }
  .pdp-box-option { width:auto; padding:8px 16px; font:400 13px/16px "Josefin Sans"; }
  .pdp-cta-primary, .pdp-cta-secondary { height:52px; font:400 13px/16px "Josefin Sans"; }
}
```

---

## Product Story

### Desktop
Section: full width, `bg:#fff`, inner container `1440px` wide centered (`240px` gutters), `padding:80px 0`, `gap:0` (two stacked blocks with fixed offsets).

**Block 1 — Story ("Câu chuyện sản phẩm")**: height `561px`, two columns:
- Image: `599×561px` rounded rect (placeholder `#D9D9D9`-equivalent, use grey-300 `#BFBFBF`).
- Text column: `702px` wide, offset `120px` gap from image (`x:719` vs image ends at `599` → gutter 120px).
  - Heading "Câu chuyện sản phẩm": 40px line-height block (use Heading 3/4 scale ~32-40px Medium), color `#000`.
  - Body paragraph: 20px-line text block, `width:702px`, `margin-top:~163px` from heading (i.e. heading + spacing ~123px gap), Lorem placeholder — replace with CMS product-story copy.
  - Icon strip below paragraph (~94px tall row of 6 icon/badge placeholders, `gap` ~17-20px each ~80-110px wide) — likely trust badges / certification icons (organic, HACCP, etc.) — `margin-top:~40px` from paragraph.

**Block 2 — Feature ("Đặc điểm")**: positioned `641px` below block 1 top (i.e. `~80px` gap after block 1's 561px height), height `380px`, two columns reversed:
- Text column: `894px` wide, inner padding-left `80px` (text starts at x:80 inside the 894px column) — heading "Đặc điểm" (same heading style) + body paragraph (16px Light or Body style), `width:814px`.
- Image: `466×380px` rounded rect, right-aligned (x:974 within 1440 container, i.e. flush to the 240px right gutter).

**CSS-ready skeleton:**
```css
.pdp-story { background:#fff; padding:80px 0; }
.pdp-story__container { max-width:1440px; margin:0 auto; padding:0 240px; }
.pdp-story__row1 { display:flex; gap:120px; align-items:flex-start; }
.pdp-story__image { width:599px; height:561px; border-radius:8px; background:#BFBFBF; }
.pdp-story__text { width:702px; }
.pdp-story__text h2 { font:500 32px/40px "Josefin Sans"; margin-bottom:123px; }
.pdp-story__text p { font:300 16px/1.5 "Josefin Sans"; }
.pdp-story__badges { display:flex; gap:20px; margin-top:40px; align-items:center; }
.pdp-story__row2 { display:flex; justify-content:space-between; margin-top:80px; }
.pdp-story__row2 .text { width:894px; padding-left:80px; }
.pdp-story__row2 .image { width:466px; height:380px; border-radius:8px; background:#BFBFBF; }
```

### Mobile
Container: `padding:16px` outer + `20px` top, inner content `398px` effective width (16px gutters), single column, stacked vertically:

**Block 1 — Story**: total height `~746px`
- Text block first: heading "Câu chuyện sản phẩm" (23px line-height, ~ Subtitle/Heading scale) + paragraph body (224px tall text block, full width `398px`).
- Icon/badge strip: `~53px` tall row of 6 small badges, `gap` tight (~10-15px), full width.
- Image below text: `398×410px` rounded rect (image moves under text on mobile, reversed order vs. desktop's side-by-side).

**Block 2 — Feature**: `766px` from top of story section (i.e. directly after block 1, no extra gap), height `701px`:
- Text: heading "Đặc điểm" (23px) + paragraph (270px tall), full width `398px`, no left padding (unlike desktop's 80px).
- Image: `398×380px` rounded rect below text.

**CSS-ready skeleton:**
```css
@media (max-width:767px){
  .pdp-story { padding:20px 16px; }
  .pdp-story__row1 { flex-direction:column; gap:0; }
  .pdp-story__text { width:100%; order:1; }
  .pdp-story__text h2 { font:500 20px/23px "Josefin Sans"; margin-bottom:8px; }
  .pdp-story__badges { margin-top:8px; }
  .pdp-story__image { width:100%; height:410px; order:2; margin-top:20px; }
  .pdp-story__row2 { flex-direction:column; margin-top:20px; }
  .pdp-story__row2 .text { width:100%; padding-left:0; }
  .pdp-story__row2 .image { width:100%; height:380px; margin-top:20px; }
}
```

---

## Collection Cross-sell

Section name in Figma: "Khám phá bộ sưu tập/nếu có nằm trong bst" (conditional — only shown if product belongs to a collection).

### Desktop
Container: full width, `bg:#F7E9EB` (red-50), `padding:80px 240px`.

Header (centered, `width:577px`, `gap:16px`):
- Eyebrow "Khám phá bộ sưu tập": 20px Medium, `line-height:23px`.
- Collection name (e.g. "XUÂN 2026 - MÃ ĐÁO THÀNH CÔNG"): font **"LC Sac Trial"** Regular, 48px, `line-height:56px`, decorative serif heading.

Grid: 3 columns, `gap:24px`, equal-width (`flex:1 0 0` each), total row width `1440px`.
Each card:
- Image area: `464×556px` (aspect roughly 0.83), `bg:#fff`, `border-radius:4px`, `overflow:hidden`.
  - Floating "Thêm vào giỏ" (Add to cart) pill button anchored bottom, offset `-102px` below image bottom edge (i.e. overlapping the caption area) — `bg:#410C13`, `height:60px`, `padding:0 16px`, icon (28px add icon) + text 20px Medium white `#F2F2F2`, `border-radius:4px`, `gap:12px`. Positioned bottom-right of card via absolute/flex justify-end.
- Caption below image: product name only, 32px Medium, `line-height:40px`, color `#1C1C1C` (Desktop/Heading 3 style — larger than related-products card title).

**CSS-ready skeleton:**
```css
.pdp-collection { background:#F7E9EB; padding:80px 240px; }
.pdp-collection__header { text-align:center; max-width:577px; margin:0 auto 40px; }
.pdp-collection__eyebrow { font:500 20px/23px "Josefin Sans"; }
.pdp-collection__title { font:400 48px/56px "LC Sac Trial"; }
.pdp-collection__grid { display:flex; gap:24px; }
.pdp-collection__card { flex:1; }
.pdp-collection__card-image { width:100%; height:556px; background:#fff; border-radius:4px; position:relative; overflow:visible; }
.pdp-collection__add-btn { position:absolute; bottom:-30px; right:24px; height:60px; padding:0 16px; background:#410C13; border-radius:4px; display:flex; align-items:center; gap:12px; color:#F2F2F2; font:500 20px/23px "Josefin Sans"; }
.pdp-collection__card-name { margin-top:20px; font:500 32px/40px "Josefin Sans"; color:#1C1C1C; }
```

### Mobile
Container: `bg:#F7E9EB`, `padding:20px 16px`.

Header: centered, `gap:4px`:
- Eyebrow 16px Medium, `line-height:20px`.
- Collection name: "LC Sac Trial" Regular, **20px**, `line-height:32px`, wraps to two lines ("XUÂN 2026" / "-MÃ ĐÁO THÀNH CÔNG").

Grid: single column, cards stacked, `gap:24px` between cards.
Each card:
- Image: full width × `470px`, `bg:#F2F2F2`, `border-radius:4px`.
  - Icon-only add-to-cart button (not full pill like desktop): `40×40px`, `bg:rgba(247,243,238,0.5)` translucent circle/rounded square, bag icon `28px`, positioned bottom-right inset `16px`, flush inside image (no overflow offset like desktop).
- Caption: product name, 20px Medium, `line-height:23px`, `#1C1C1C` (smaller than desktop's 32px card title).

**CSS-ready skeleton:**
```css
@media (max-width:767px){
  .pdp-collection { padding:20px 16px; }
  .pdp-collection__title { font-size:20px; line-height:32px; }
  .pdp-collection__grid { flex-direction:column; gap:24px; }
  .pdp-collection__card-image { height:470px; background:#F2F2F2; }
  .pdp-collection__add-btn { position:absolute; bottom:16px; right:16px; width:40px; height:40px; padding:0; background:rgba(247,243,238,0.5); border-radius:4px; }
  .pdp-collection__card-name { font-size:20px; line-height:23px; margin-top:12px; }
}
```

---

## Related Products

Section name in Figma: "Sản phẩm tương tự".

### Desktop
Container: full width, `bg:#fff`, `padding:80px 240px`.

Header: centered, font **"LC Sac Trial"** Regular 48px/56px — "SẢN PHẨM TƯƠNG TỰ".

Grid: **4 columns**, `gap:16px`, row height `488px`, each column `flex:1 0 0` (equal width, ≈ `(1440-48)/4 = 348px` — matches metadata card width `348px`).

Each card (should reuse existing `_ProductCard` component if one exists in the codebase — verify against `Views/Shared/_ProductCard.cshtml` before duplicating markup):
- Image: full width × full height of row (fills flex height), `bg:#F2F2F2`, `border-radius:4px`, `overflow:hidden`, position:relative.
  - Floating add-to-cart pill, same as collection section: anchored bottom-right, offset `-102px`, `bg:#410C13`, `height:60px`, icon+"Thêm vào giỏ" 20px Medium white.
- Caption block below image, `gap:8px`:
  - Product name: 24px SemiBold, `line-height:32px`, `#1C1C1C` (Heading 4 style).
  - Price: 24px Medium, `line-height:32px`, `#303030` — e.g. "899.000".

Note: this desktop related-products card does NOT show color swatch chips (those appear only on the mobile variant markup captured — see below); verify against live Figma if swatches are desired on desktop too.

**CSS-ready skeleton:**
```css
.pdp-related { background:#fff; padding:80px 240px; }
.pdp-related__title { text-align:center; font:400 48px/56px "LC Sac Trial"; margin-bottom:40px; }
.pdp-related__grid { display:flex; gap:16px; }
.pdp-related__card { flex:1; display:flex; flex-direction:column; gap:20px; }
.pdp-related__card-image { width:100%; height:488px; background:#F2F2F2; border-radius:4px; position:relative; }
.pdp-related__add-btn { position:absolute; bottom:-30px; right:24px; height:60px; padding:0 16px; background:#410C13; color:#F2F2F2; border-radius:4px; display:flex; align-items:center; gap:12px; font:500 20px/23px "Josefin Sans"; }
.pdp-related__name { font:600 24px/32px "Josefin Sans"; color:#1C1C1C; }
.pdp-related__price { font:500 24px/32px "Josefin Sans"; color:#303030; }
```

### Mobile
Container: `bg:#fff`, `padding:20px 16px`.

Header: "LC Sac Trial" Regular, **20px/32px** — "SẢN PHẨM TƯƠNG TỰ".

Grid: **2 columns**, uneven column widths within each row pair — first card `203px`, second `187px` (row total `398px` minus `8px` gap) — effectively a slightly asymmetric 2-col grid; safest implementation is a standard 2-column CSS grid at 50% each (`1fr 1fr`) with `gap:8px`, treating the px discrepancy as rounding in the Figma mockup.

Each card, `height:341px` image + caption below (`gap:16px` card internal):
- Image: `bg:#F2F2F2`, `border-radius:4px`.
  - Icon-only cart button `40×40px`, translucent `rgba(247,243,238,0.5)` bg, bottom-right inset `12px` (tighter than collection section's `16px`).
- Caption, `gap:4px`:
  - Name: 16px Medium, `line-height:20px`, `#1C1C1C`.
  - Price: 20px Medium, `line-height:23px`, `#303030`.
  - **Color-availability chips**: row of small color bars, `gap:9px`, each `24×16px` with `4px` vertical padding — sample colors `#E60000` (has active `border-bottom:1px solid #000` = selected state), `#0077FF`, `#FFC300`, `#00E007`. This is a variant indicator not present in the desktop capture — confirm whether desktop cards should also show it (likely yes, for consistency; add if the live desktop card includes it).

Grid repeats for as many related products as available (2 rows × 2 cols shown in mock).

**CSS-ready skeleton:**
```css
@media (max-width:767px){
  .pdp-related { padding:20px 16px; }
  .pdp-related__title { font-size:20px; line-height:32px; margin-bottom:20px; }
  .pdp-related__grid { display:grid; grid-template-columns:1fr 1fr; gap:8px 8px; row-gap:16px; }
  .pdp-related__card-image { height:341px; background:#F2F2F2; border-radius:4px; position:relative; }
  .pdp-related__add-btn { position:absolute; bottom:12px; right:12px; width:40px; height:40px; background:rgba(247,243,238,0.5); border-radius:4px; }
  .pdp-related__name { font:500 16px/20px "Josefin Sans"; }
  .pdp-related__price { font:500 20px/23px "Josefin Sans"; color:#303030; }
  .pdp-related__swatches { display:flex; gap:9px; margin-top:4px; }
  .pdp-related__swatch { width:24px; height:16px; padding:4px 0; }
  .pdp-related__swatch.selected { border-bottom:1px solid #000; }
}
```

---

## Implementation Notes for Razor

1. **Shared card partial**: The "Collection Cross-sell" cards and "Related Products" cards share the same structural pattern (image + floating/inset add-to-cart + caption). Confirm whether the codebase already has `_ProductCard.cshtml`; if so, add a `variant` param (`"collection"` vs `"related"`) to switch caption typography (32px vs 24px) and button style (full pill vs icon-only), rather than duplicating markup.
2. **Footer** and top nav ("Frame 494") are shared layout components already presumably implemented elsewhere — not respec'd here.
3. Font **"LC Sac Trial"** is only used for the two big serif-style section headings (Collection title, Related Products title). All other text uses Josefin Sans. Ensure both fonts are loaded via `@font-face` or a webfont service in `_Layout.cshtml`.
4. All image placeholders (`#BFBFBF`, `#F2F2F2`, `#9B9B9B`, `#DCDCDC`) should be replaced with real `<img>`/`<picture>` tags bound to product image URLs; the grey fills are Figma placeholder rectangles, not final visual treatment.
5. Ingredient list, product story paragraphs, and badge icons are Lorem-ipsum / generic placeholders in the design — content will come from CMS/product data fields, not hardcoded strings.
6. Desktop related-products grid = 4 columns; mobile = 2 columns; collection cross-sell = 3 columns desktop, 1 column mobile. Keep these as distinct CSS grid/flex configs, not a single reused "product grid" component, since the two sections have different card aspect ratios and CTA styles.
