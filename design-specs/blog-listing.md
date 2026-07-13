# Blog Listing Page — Design Spec

Figma file: `uQFY9gwfNbNSeTM6zmspzo`
- Desktop node: `1154:36558` (1920px canvas)
- Mobile node: `1154:37190` (430px canvas)

Target implementation: ASP.NET Core MVC + Razor. `Blog/Index.cshtml` with reusable partials `Views/Blog/_BlogCard.cshtml` (regular grid card) and `Views/Blog/_FeaturedBlogCard.cshtml` (desktop-only hero card).

---

## Design Tokens

**Colors**
| Token | Hex |
|---|---|
| Brand red (buttons/accent) | `#AF2234` |
| Brand red dark (CTA button bg) | `#410C13` |
| Brand gold/tan (category label) | `#AA8656` |
| Yellow-800 (footer text) | `#5E4A2F` |
| Yellow-50 (footer bg) | `#F7F3EE` |
| Grey-900 | `#0F0F0F` |
| Grey-800 (headings/body strong) | `#1C1C1C` / `#000000` |
| Grey-700 | `#3C3C3C` |
| Grey-500 (date, meta) | `#7A7A7A` |
| Grey-400 (borders, input border) | `#9B9B9B` |
| Grey-300 (mobile input border) | `#BFBFBF` |
| Grey-100 (card divider line) | `#D6D6D6` |
| Grey-50 (topbar bg) | `#F2F2F2` |
| Body/excerpt text | `#525252` |
| Image placeholder (no image loaded) | `rgba(0,0,0,0.2)` on light grey box |

**Typography**
| Role | Font | Size / Line-height | Weight |
|---|---|---|---|
| Page heading ("HOÀI MÁCH BẠN", featured title) | LC Sac Trial, Regular | 48px / 56px (desktop); 20px/30px (mobile "ĐĂNG KÝ..." style) | 400 |
| Card title (grid cards) | Josefin Sans | 20px / 23px (Common/Subtitle) | 500 (Medium) |
| Category label (gold) | Josefin Sans | 16px / 20px (Common/Label) | 500 (Medium) |
| Excerpt / body | Josefin Sans, Light | 16px / 18px (desktop grid) or 16px/20px (mobile) | 300 |
| Date / meta | Josefin Sans | 16px / 20px (Common/Label) | 500 (Medium), color `#7A7A7A` |
| Breadcrumb | Josefin Sans | 16px/20px desktop (Medium); 13px/16px mobile (Regular) | — |
| CTA button label ("Xem bài viết") | Josefin Sans | 20px / 23px (Common/Subtitle) | 500, color white |

Both breakpoints reuse the same site header/footer (not detailed here — out of scope for blog-specific spec).

---

## Hero

### Desktop (1920px canvas, content padded `240px` each side → 1440px content width)
- Hero wrapper: full-width, `background:#fff`, vertical stack, gap `32px` below nav.
- Breadcrumb row: `padding: 0 240px`, text "Trang chủ/Blog", Josefin Sans Medium 16/20, color `#1C1C1C`.
- Page heading "HOÀI MÁCH BẠN": centered, full width, LC Sac Trial Regular 48/56, color `#000`, no padding constraint (already centered text).
- Spacing: `24px` gap between breadcrumb block and heading is implicit via parent `gap:32px` on Hero container; heading sits directly above the featured-card section (which starts with `gap:40px` from heading in the outer page flex).

### Mobile (430px canvas)
- Header is `position:absolute` overlay at top (hamburger + centered logo + search/cart icons), total height `92px`, red announcement bar `32px` height ("Miễn phí đơn hàng từ 500.000đ", Josefin Sans Regular 13/16, white on `#AF2234`).
- Page content wrapper starts with `padding-top:100px` to clear the fixed/absolute header.
- Breadcrumb: `padding: 0 16px`, "Trang chủ/" in `#1C1C1C` + "Blog" in `#7A7A7A`, Josefin Sans Regular 13/16.
- Note: the mobile crop provided does **not** show the "HOÀI MÁCH BẠN" H1 or a featured card — the mobile layout goes straight from breadcrumb into the card list (`gap:40px`, `padding:0 16px`). If the H1 is required on mobile for parity, insert it centered between breadcrumb and card list using LC Sac Trial 20/30 (matches the footer heading style at mobile scale) — treat as an assumption, verify against full mobile scroll before shipping.

---

## Featured Post Card (Desktop only)

Location: directly below the "HOÀI MÁCH BẠN" heading, node `1154:36749`. Not present in the mobile crop — mobile goes straight into the regular-card list.

**Container**
- `padding: 0 240px` (page-level), row layout, `gap:24px`, `padding-top/bottom:40px`, `border-bottom:1px solid #D6D6D6` (this row acts as a divider before the grid).
- Two flex children, each `flex:1 0 0` (roughly 50/50 of the 1440px content width ≈ **708px** each, matching the "708x486 image" note).

**Left: image**
- `width: 100%` of its flex column, `height:486px`, `border-radius:4.235px` (~4px), background placeholder `rgba(0,0,0,0.2)` (swap for real `<img>` with `object-fit:cover`).

**Right: content column**
- `flex:1`, `display:flex; flex-direction:column; justify-content:space-between; padding:8px 0; align-self:stretch` (so CTA button pins to bottom, aligned with image bottom).
- Top block (`gap:16px`):
  - Category tag: "Quà tết" — Josefin Sans Medium 16/20, color `#AA8656`.
  - Title: LC Sac Trial Regular 48/56, color `#000` (e.g. "Gợi ý chọn quà tặng cho người thân yêu").
  - Excerpt: Josefin Sans Light 16/18, color `#525252` (long lorem ipsum, ~3x repeated paragraph in source — treat as placeholder body copy, clamp to ~4-5 lines in real content via `-webkit-line-clamp`).
  - Date: Josefin Sans Medium 16/20, color `#7A7A7A` (e.g. "02/07/2026").
- CTA button (bottom-aligned): `background:#410C13`, `border-radius:8px`, `height:48px`, `padding:8px 24px`, label "Xem bài viết" Josefin Sans Medium 20/23, white, centered.

---

## Post Card Grid

### Desktop (1154:36571)
- Grid container: `padding:0 240px`, `display:flex; flex-direction:column; gap:40px`.
- Each row: `display:flex; gap:40px; align-items:center` — **3 columns per row**, 2 rows = 6 cards total.
- Card width: `flex:1 0 0` → content width 1440px minus 2×40px gaps = 1360px / 3 ≈ **453.3px** per card (matches "~453 wide" spec note).
- Card wrapper: `flex-direction:column; gap:24px; padding:8px 0; border-bottom:1px solid #D6D6D6` (bottom divider per card).

**Regular card anatomy**
1. Image: `width:100%; height:299px; border-radius:4.235px`; placeholder `rgba(0,0,0,0.2)`; replace with `<img>` `object-fit:cover`.
2. Text block (`gap:16px` from image; inner `gap:16px`):
   - Category tag: Josefin Sans Medium 16/20, color `#AA8656` (e.g. "Đời sống").
   - Title: Josefin Sans Medium 20/23, color `#000` (e.g. "Gợi ý chọn quà tặng cho người thân yêu").
   - Excerpt: Josefin Sans Light 16/18, color `#525252`, single paragraph (shorter than featured card's), clamp to ~3 lines.
3. Date: Josefin Sans Medium 16/20, color `#7A7A7A`, sits below excerpt block with same `gap:16px`.

### Mobile (1154:36793, 430px canvas)
- Single column stack: `padding:0 16px; display:flex; flex-direction:column; gap:40px`.
- Card width: full content width = `430 - 32 = 398px` (matches spec note).
- Card wrapper: `flex-direction:column; gap:8px; padding:8px 0; border-bottom:1px solid #D6D6D6`.
- Image: `width:100%; height:299px; border-radius:4.235px` — same height as desktop grid cards, just full-bleed width.
- Text block: `gap:16px` (category `gap:8px` + title, Josefin Sans Medium — note mobile category/title use same 16px/20px "Đời sống" style but title renders at 20px/23px black, consistent with desktop).
  - Category: `#AA8656`, 16/20 Medium.
  - Title: `#000`, 20/23 (renders as Medium weight per source, though grouped under a `font-medium` wrapper — treat title weight as 500).
  - Excerpt: `#525252`, 16/20 (note: mobile excerpt line-height is 20px vs desktop grid's 18px — Body 1-ish, slightly larger than desktop grid card).
  - Date: `#7A7A7A`, 16/20.
- Only 3 cards shown in the mobile crop (pagination/infinite-scroll assumed for the rest — same 6+ posts as desktop, just single column).

---

## Razor / CSS Implementation Notes

**Suggested partial contract**

`_BlogCard.cshtml` (model: `BlogCardViewModel { ImageUrl, Category, Title, Excerpt, DateText, Url }`)
```html
<a class="blog-card" href="@Model.Url">
  <img class="blog-card__img" src="@Model.ImageUrl" alt="@Model.Title" />
  <div class="blog-card__body">
    <div class="blog-card__meta">
      <span class="blog-card__category">@Model.Category</span>
      <h3 class="blog-card__title">@Model.Title</h3>
      <p class="blog-card__excerpt">@Model.Excerpt</p>
    </div>
    <span class="blog-card__date">@Model.DateText</span>
  </div>
</a>
```

`_FeaturedBlogCard.cshtml` — same model, different markup: image + text column with CTA `<a class="btn btn--dark">Xem bài viết</a>`, wrapped in a two-column flex row, image fixed to `flex:1` / 486px height, hidden on mobile (`d-none d-lg-flex` equivalent).

**Core CSS (desktop-first, mobile override via media query)**

```css
.blog-hero { display:flex; flex-direction:column; gap:32px; }
.blog-breadcrumb { padding:0 240px; font:500 16px/20px "Josefin Sans"; color:#1C1C1C; }
.blog-heading { text-align:center; font:400 48px/56px "LC Sac Trial", serif; color:#000; }

.blog-featured { display:flex; gap:24px; padding:40px 240px; border-bottom:1px solid #D6D6D6; }
.blog-featured__img { flex:1; height:486px; border-radius:4px; object-fit:cover; }
.blog-featured__body { flex:1; display:flex; flex-direction:column; justify-content:space-between; padding:8px 0; }
.blog-featured__category { font:500 16px/20px "Josefin Sans"; color:#AA8656; }
.blog-featured__title { font:400 48px/56px "LC Sac Trial", serif; color:#000; }
.blog-featured__excerpt { font:300 16px/18px "Josefin Sans"; color:#525252; }
.blog-featured__date { font:500 16px/20px "Josefin Sans"; color:#7A7A7A; }
.btn--dark { background:#410C13; color:#fff; border-radius:8px; height:48px; padding:8px 24px;
  display:inline-flex; align-items:center; justify-content:center; font:500 20px/23px "Josefin Sans"; }

.blog-grid { display:flex; flex-direction:column; gap:40px; padding:0 240px; }
.blog-grid__row { display:flex; gap:40px; align-items:flex-start; }
.blog-card { flex:1 0 0; display:flex; flex-direction:column; gap:24px; padding:8px 0; border-bottom:1px solid #D6D6D6; }
.blog-card__img { width:100%; height:299px; border-radius:4px; object-fit:cover; }
.blog-card__category { font:500 16px/20px "Josefin Sans"; color:#AA8656; }
.blog-card__title { font:500 20px/23px "Josefin Sans"; color:#000; }
.blog-card__excerpt { font:300 16px/18px "Josefin Sans"; color:#525252; }
.blog-card__date { font:500 16px/20px "Josefin Sans"; color:#7A7A7A; }

@media (max-width: 767px) {
  .blog-breadcrumb, .blog-grid { padding:0 16px; }
  .blog-heading { font-size:20px; line-height:30px; }
  .blog-featured { display:none; } /* no featured card on mobile per design */
  .blog-grid__row { flex-direction:column; gap:40px; }
  .blog-card { gap:8px; }
  .blog-card__excerpt { font-size:16px; line-height:20px; }
}
```

**Layout math reference**
- Desktop content width: `1920 - 2×240 = 1440px`.
- Featured card halves: `(1440 - 24) / 2 ≈ 708px` each → matches 708×486 image spec.
- Grid card width: `(1440 - 2×40) / 3 ≈ 453.3px` → matches ~453px spec note.
- Mobile content width: `430 - 2×16 = 398px` → matches 398px card spec note.
- Card image border-radius consistently `4.235px` (round to `4px` in CSS).
- Card-to-card / row gaps: `40px` both directions on desktop grid; `40px` between mobile cards.
- Internal card gaps: `24px` (image→text, desktop grid) / `8px` (image→text, mobile) / `16px` (text block internals, both breakpoints).
