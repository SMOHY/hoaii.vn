# Section: Danh sách sản phẩm đặc biệt (Featured Products)

Figma node IDs: Desktop `1214:38732`, Mobile `1062:12577` (file `uQFY9gwfNbNSeTM6zmspzo`)

Background: `#F7F3EE` (Yellow/yellow-50) for the whole section, both breakpoints.

---

### Desktop

**Section container**
- `display: flex; flex-direction: column; align-items: center; gap: 68px;`
- `padding: 120px 362px;` (i.e. large horizontal padding — content max-width is effectively fixed; on real responsive builds clamp with a max-width container instead of fixed px, but keep proportions: outer padding ≈ 362px at 1920px viewport)
- Background: `#F7F3EE`

**Heading block ("Text")**
- `display: flex; flex-direction: column; gap: 18px; align-items: flex-start; text-align: center; width: 1084px;` (center the block; text-align center)
- Title: font `LC Sac Trial`, Regular, `48px / 56px` line-height, color `#000000`, uppercase text "LỰA CHỌN HÀNG ĐẦU"
- Subtitle/body: font `Josefin Sans`, Regular 400, `20px / 24px` line-height, color `#303030` (grey-700 region; Figma token shows `#3C3C3C`/`#000000` grey-700 — use `#3C3C3C` for body copy consistency), text is a single centered paragraph, max width matches the 1084px block

**Image/tile grid ("Hình ảnh")**
- Outer wrapper: `display: flex; flex-direction: column; gap: 16px; width: 1197.5px;` (≈ content width; on real build make this `100%` of the container, i.e. `max-width: 1197px` centered)
- Two rows, each: `display: flex; gap: 16px; align-items: flex-start; width: 100%;`
- Row = 3 equal tiles: `flex: 1 1 0; min-width: 0;`
- Tile height: `~472–473px` (472.174px plain tiles, 473.007px for the two "collection" cards — treat as `472px` uniform in CSS)
- Tile border-radius: `4px`
- Plain placeholder tiles (4 of 6): background `#BFBFBF` (grey-300) — these will hold product images later (`object-fit: cover`)
- Two "collection" cards (position 1 of row 1, position 3 of row 2 — i.e. diagonal placement) are colored CTA cards, not plain images:
  - Card A (row 1, col 1): background `#AF2234` (brand red / brand-color-2)
  - Card B (row 2, col 3): background `#AA8656` (brand gold/tan / brand-color-1)
  - Card layout: `display:flex; flex-direction:column; justify-content:space-between; padding:40px;` (39.972px), `border-radius:4px; height:473px;`
  - Card content top group: `display:flex; flex-direction:column; gap:53px;` (53.297px)
    - Eyebrow label: font `Josefin Sans` Regular, `16.7px / 26.6px`, color `#DCDCDC` (grey-200), text "Bộ sưu tập"
    - Title stack: `display:flex; flex-direction:column; gap:13.3px;` font `LC Sac Trial` Regular, color `#FFFFFF`, `letter-spacing: 0.58px`
      - Line 1 ("Xuân"): `50px` font-size, line-height `58.3px`
      - Line 2 ("2026"): `66.6px` font-size, line-height `58.3px`
  - Card footer (CTA row): `display:flex; gap:6.66px; align-items:center; justify-content:flex-end; height:33.3px;`
    - Label "Xem thêm": font `Josefin Sans` Medium 500, `16.7px / 23.3px`, color `#FFFFFF`, `white-space:nowrap`
    - Icon: 16.7px × 16.7px arrow-right SVG (white), inline after label

**Simplified CSS-ready values (round for implementation)**
```css
.featured-section        { background:#F7F3EE; display:flex; flex-direction:column; align-items:center; gap:68px; padding:120px 0; }
.featured-heading         { display:flex; flex-direction:column; gap:18px; align-items:center; text-align:center; max-width:1084px; }
.featured-title           { font-family:'LC Sac Trial', serif; font-weight:400; font-size:48px; line-height:56px; color:#000; text-transform:uppercase; }
.featured-subtitle        { font-family:'Josefin Sans', sans-serif; font-weight:400; font-size:20px; line-height:24px; color:#3C3C3C; }

.featured-grid            { display:flex; flex-direction:column; gap:16px; width:100%; max-width:1198px; }
.featured-row             { display:flex; gap:16px; width:100%; }
.featured-tile            { flex:1 1 0; min-width:0; height:472px; border-radius:4px; background:#BFBFBF; } /* plain image tile */

.featured-card            { flex:1 1 0; min-width:0; height:472px; border-radius:4px; padding:40px; display:flex; flex-direction:column; justify-content:space-between; color:#fff; }
.featured-card--red       { background:#AF2234; }
.featured-card--gold      { background:#AA8656; }
.featured-card__top       { display:flex; flex-direction:column; gap:53px; }
.featured-card__eyebrow   { font-family:'Josefin Sans'; font-weight:400; font-size:16.7px; line-height:26.6px; color:#DCDCDC; }
.featured-card__title     { font-family:'LC Sac Trial'; font-weight:400; color:#fff; letter-spacing:0.58px; line-height:58.3px; display:flex; flex-direction:column; gap:13.3px; }
.featured-card__title--sm { font-size:50px; }
.featured-card__title--lg { font-size:66.6px; }
.featured-card__cta       { display:flex; gap:6.66px; align-items:center; justify-content:flex-end; height:33.3px; }
.featured-card__cta-label { font-family:'Josefin Sans'; font-weight:500; font-size:16.7px; line-height:23.3px; color:#fff; white-space:nowrap; }
.featured-card__cta-icon  { width:16.7px; height:16.7px; }
```

Grid layout (3 columns × 2 rows, diagonal CTA cards):
```
[ RED CARD ] [ grey tile ] [ grey tile ]
[ grey tile ] [ grey tile ] [ GOLD CARD ]
```

Razor structure suggestion: a partial `_FeaturedProducts.cshtml` looping a `List<FeaturedTileVm>` (6 items) where each item has `IsCard` (bool), `AccentColor` (red/gold/none), `ImageUrl`, `CollectionLabel`, `TitleLine1`, `TitleLine2`, `LinkUrl`. Render 2 rows of 3 via chunking, or use CSS Grid (`grid-template-columns: repeat(3,1fr); grid-template-rows: repeat(2,472px); gap:16px;`) instead of nested flex rows for simpler markup.

---

### Mobile

**Section container**
- `display:flex; flex-direction:column; align-items:center; gap:16px; padding:40px 16px;`
- Background: `#F7F3EE`

**Heading block ("Text")**
- `display:flex; flex-direction:column; gap:4px; width:100%;`
- Title: font `LC Sac Trial`, Regular, `20px / 32px` line-height, color `#000000`, centered, text "Lựa chọn hàng đầu" (note: mobile design shows sentence case, not uppercase — desktop is uppercase; confirm with design system before deciding, but replicate as-is: mobile lower/sentence case)
- No subtitle/body paragraph shown on mobile (only the title line present in this frame)

**Image/tile grid ("Hình ảnh")**
- Wrapper: `display:flex; flex-direction:column; gap:4px; width:100%; height:728px;` (fixed height in design; on responsive build let height be automatic — 3 rows × tile aspect ratio)
- 3 rows, each: `display:flex; gap:4px; width:100%; flex:1 1 0; min-height:0;`
- Each row = 2 equal tiles: `flex:1 1 0; min-width:0; height:100%;` → 6 tiles total, all plain grey placeholders (no CTA card variant appears in this mobile frame)
- Tile background: `#BFBFBF` (grey-300)
- Tile border-radius: `4px`
- Effective per-tile size at 375–430px viewport: row height ≈ 728/3 ≈ 242px, tile width ≈ (100vw - 32px - 4px)/2

**Simplified CSS-ready values**
```css
.featured-section-m   { background:#F7F3EE; display:flex; flex-direction:column; align-items:center; gap:16px; padding:40px 16px; }
.featured-heading-m   { display:flex; flex-direction:column; gap:4px; width:100%; }
.featured-title-m     { font-family:'LC Sac Trial', serif; font-weight:400; font-size:20px; line-height:32px; color:#000; text-align:center; }

.featured-grid-m      { display:flex; flex-direction:column; gap:4px; width:100%; }
.featured-row-m       { display:flex; gap:4px; width:100%; }
.featured-tile-m      { flex:1 1 0; min-width:0; aspect-ratio: 1 / 1.28; border-radius:4px; background:#BFBFBF; } /* replace fixed 728px wrapper height with aspect-ratio per tile for true responsiveness */
```

Grid layout (2 columns × 3 rows, all plain image tiles):
```
[ grey tile ] [ grey tile ]
[ grey tile ] [ grey tile ]
[ grey tile ] [ grey tile ]
```

**Desktop → Mobile notes for Razor/CSS implementation**
- Same partial/markup can serve both breakpoints: render a 3×2 (desktop) / 2×3-ish (mobile) tile grid using CSS Grid with a media query switch:
  ```css
  .featured-grid { display:grid; gap:16px; grid-template-columns: repeat(3, 1fr); }
  @media (max-width: 767px) {
    .featured-grid { gap:4px; grid-template-columns: repeat(2, 1fr); }
  }
  ```
- On mobile the two CTA/collection cards are not shown in this specific frame (only 6 plain tiles) — confirm with product owner whether mobile should also surface the red/gold CTA cards elsewhere, or omit them per this design (currently: omit, mobile shows image-only tiles).
- Section padding drops from `120px 362px` (desktop) to `40px 16px` (mobile); outer gap drops from `68px` to `16px`; grid gap drops from `16px` to `4px`.
- Title font-size drops from `48px/56px` to `20px/32px`; body subtitle is removed on mobile in this frame.
