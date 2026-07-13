# Search Results Page — Design Spec

Source: Figma file `uQFY9gwfNbNSeTM6zmspzo`
- Desktop, with results: node `988:21321` (1920px wide) — metadata-only pull (too large for full code); child nodes read individually (`988:21328` "Frame 517" tree).
- Desktop, no results: node `988:23672` (1920px wide) — full code + screenshot.
- Mobile, with results: node `1142:21872` (430px wide) — full code + screenshot.
- Mobile, no results: node `1142:24026` (430px wide) — full code + screenshot.

> Reuses the shared `_Nav` and `_Footer` partials documented in `shared-layout.md` /
> `category-page-template.md`. This spec only covers the search-specific content area:
> result-count line, heading, category-grouped product sections, "Xem thêm" links, and the
> empty-state block.

---

## Product cards: SAME as `_ProductCard` (category-page-template.md) — reuse the partial

Confirmed from Figma instance names/sizes in the search grid: repeated component instances
"Frame 190/191/192" at **464×686** on desktop (`988:22479` etc., in 3-column rows with 24px column
gap / 56px row gap — identical math to the category template: `3×464 + 2×24 = 1440`, `742−686=56`
row pitch). On mobile the same card component ("CacTrngThaiSnPhm") shows a 470px-tall image area
(`1068:21555`, height 470) which matches the established mobile card image height from
category-page-template.md (398×571 card, 398×470 image) — the raw metadata reports the
component's unscaled default width (464px) rather than the as-placed instance width, but the
image-area height (470) and overall anatomy (name/price/swatch row, small rounded icon-only
add-to-cart button, no full CTA bar) are pixel-identical to the mobile `_ProductCard` spec.

**Conclusion: do not build a new card partial for search — render `_ProductCard.cshtml` inside the
search results grid, same as the category pages.**

---

## Search Results (grouped by category)

### Desktop

Frame `988:21328` ("Frame 517"), 1920×7608 total (content only; footer appended after).

Layout, top to bottom:
1. Content wrapper starts at `padding-top: 240px` (below the fixed `_Nav`, which is 168px tall —
   there's an extra ~72px of breathing room above the result count vs. the category-page hero).
2. **Result count** ("Frame 359", full-width, 20px tall): centered text `"20 kết quả"` — Common/Label
   token: 16px Medium/20px, color grey-800 `#1C1C1C`.
3. **Heading** ("Text", 136px-tall block, 40px top offset within it): centered
   `"KẾT QUẢ TÌM KIẾM CHO "HỘP QUÀ""` — inferred **48px "LC Sac Trial" Regular / 56px line-height**
   (text bounding box height = 56, matching the no-results desktop heading's exact font metrics);
   color not directly confirmed in this metadata-only pull (no-results uses grey `#7A7A7A` — verify
   actual color for the *with-results* heading via a screenshot before finalizing; likely black or
   brand red given it's an active-state heading, not an empty state).
4. **Category-grouped sections**, each in a `1440px`-wide column (240px side margins, same content
   width as category-page-template.md):
   - Section header row (`Frame 532`, 40px tall, `justify-content: space-between; align-items: flex-end`):
     - Category name (e.g. "Quà tết", "Quà trung thu", "Quà theo dịp", "Sản phẩm chọn lọc") — 32px
       Medium/40px, black (Heading 3 token, matches category-template filter-title size).
     - Product count, right-aligned — `"16 sản phẩm"` — 16px Medium/20px (Label token), black.
   - Product grid: 3 columns × 2 rows shown (6 cards), `464×686` cards, `gap: 56px 24px` — identical
     `.product-grid` CSS from category-page-template.md.
   - **"Xem thêm" (show more) link**: centered below the grid, `~40px` gap from grid bottom, text
     `20px Medium/23px`, color grey-500 `#7A7A7A`, underlined.
   - Each section's total block height is `1731px` (80px top gap to header + 40 header + 80 gap to
     grid + 1428 grid + ~gap + 23 link + bottom padding).
5. Sections stack directly with **no extra gap** between the three category-match groups (Quà tết →
   Quà trung thu → Quà theo dịp: y-offsets 0, 1731, 3462 — exactly contiguous). The final
   **"Sản phẩm chọn lọc"** fallback section sits `64px` below that cluster (y=5557 vs. cluster end
   5493) — treat this as a visually distinct "you might also like" block appended after the
   matched-category groups.
6. Footer (`Footer` instance, shared partial) follows at the bottom.

```css
.search-header { padding-top: 240px; }
.search-header__count { text-align: center; font: 500 16px/20px "Josefin Sans"; color: #1C1C1C; }
.search-header__title { text-align: center; font: 400 48px/56px "LC Sac Trial"; margin: 40px 0 0; }

.search-group { width: 1440px; max-width: 100%; margin: 0 auto; }
.search-group + .search-group { margin-top: 0; } /* contiguous within a matched-category cluster */
.search-group--fallback { margin-top: 64px; } /* "Sản phẩm chọn lọc" gap from cluster above */

.search-group__header { display: flex; align-items: flex-end; justify-content: space-between;
  height: 40px; margin: 80px 0 80px; }
.search-group__title { font: 500 32px/40px "Josefin Sans"; color: #000; margin: 0; }
.search-group__count { font: 500 16px/20px "Josefin Sans"; color: #000; }

.search-group__grid { display: grid; grid-template-columns: repeat(3, 1fr); gap: 56px 24px; }

.search-group__more { display: flex; justify-content: center; margin-top: 40px; }
.search-group__more a { font: 500 20px/23px "Josefin Sans"; color: #7A7A7A; text-decoration: underline; }
```

### Mobile

Frame `1142:21872`, 430px wide, `padding-top: 120px` (below condensed mobile nav: 32px promo strip
+ 60px main row).

Layout, top to bottom:
1. **Result count** — centered, `16px Medium/20px`, grey-800 `#1C1C1C` — `"20 kết quả"`.
2. **Heading** — centered, `"LC Sac Trial" Regular`, **20px / 56px line-height** (oversized
   line-height reused verbatim from the desktop token), color **black** — `"KẾT QUẢ TÌM KIẾM CHO
   "HỘP QUÀ""`. Note: on mobile this heading is dramatically smaller than desktop's 48px — confirmed
   directly from code (not inferred).
3. Content padded `16px` each side (`px-16`).
4. **Category-grouped sections**, each wrapped with a **bottom border** `1px solid #BFBFBF` and
   `padding-bottom: 20px` (this is how mobile visually separates groups — no card-based margin
   trick like desktop):
   - Header row: category name `20px Medium/23px` black + count `"16 sản phẩm"` `16px/20px` black,
     `justify-content: space-between`.
   - Product grid: **1 column**, cards stacked with `gap: 24px` between groups-of-3 and `24px` gap
     within — effectively one continuous vertical list (matches category-template mobile grid: `1fr`
     columns, `24px` gap).
   - "Xem thêm" link centered below, `20px Medium/23px`, grey-500 `#7A7A7A`, underlined.
5. Last section ("Sản phẩm chọn lọc") drops the bottom-border treatment and instead uses
   `padding: 20px 0 80px` before its own "Xem thêm" link, then the footer follows directly (no
   distinct extra-gap treatment was visible in this pull, unlike desktop's 64px fallback gap — verify
   against a full mobile screenshot scroll if pixel parity matters).

```css
.search-header--mobile { padding-top: 120px; }
.search-header--mobile .search-header__title { font-size: 20px; line-height: 56px; color: #000; }

.search-group--mobile { padding: 0 16px; }
.search-group--mobile__inner { border-bottom: 1px solid #BFBFBF; padding-bottom: 20px; }
.search-group--mobile__header { display: flex; align-items: flex-end; justify-content: space-between; }
.search-group--mobile__title { font: 500 20px/23px "Josefin Sans"; color: #000; }
.search-group--mobile__count { font: 500 16px/20px "Josefin Sans"; color: #000; }
.search-group--mobile__grid { display: grid; grid-template-columns: 1fr; gap: 24px; margin-top: 24px; }
.search-group--mobile__more { display: flex; justify-content: center; margin-top: 24px; }
.search-group--mobile__more a { font: 500 20px/23px "Josefin Sans"; color: #7A7A7A; text-decoration: underline; }
```

---

## Empty State

No search-bar/input control appears in either empty-state scope — the search box itself lives in
the shared `_Nav` partial (search icon in the nav triggers an overlay/input elsewhere, not rendered
in this frame).

### Desktop

Frame `988:23672`, 1920px wide, `padding-top: 240px`.

1. `"0 kết quả"` — centered, 16px Medium/20px, grey-800 `#1C1C1C`, inside a `px: 240px` container
   (1440px content width).
2. Message block (white bg section, `padding: 80px 0`), centered column, `gap: 40px` (py-40 wrapper
   around the text group):
   - Title `"KHÔNG CÓ KẾT QUẢ"` — **48px "LC Sac Trial" Regular / 56px line-height, uppercase**,
     color grey-500 `#7A7A7A`.
   - Subtext (gap 8px below title) — `16px Medium/20px Josefin Sans`, grey-800 `#1C1C1C`:
     `"Vui lòng thử lại từ khóa khác hoặc quay lại "` + underlined link `"trang chủ"`.
   - Illustration: `Group` image, **197 × 190.86px**, centered directly below the text block (no
     extra top margin beyond the block's own padding).
3. Footer / newsletter signup band follows immediately (shared partial, out of scope — see
   `shared-layout.md`).

```css
.empty-state { padding: 80px 0; display: flex; flex-direction: column; align-items: center; }
.empty-state__count { text-align: center; font: 500 16px/20px "Josefin Sans"; color: #1C1C1C;
  padding: 0 240px; }
.empty-state__title { font: 400 48px/56px "LC Sac Trial"; text-transform: uppercase;
  color: #7A7A7A; margin: 0 0 8px; text-align: center; }
.empty-state__subtext { font: 500 16px/20px "Josefin Sans"; color: #1C1C1C; text-align: center; }
.empty-state__subtext a { text-decoration: underline; }
.empty-state__illustration { width: 197px; height: 190.86px; margin-top: 40px; }
```

### Mobile

Frame `1142:24026`, 430px wide, `padding-top: 120px`.

1. `"0 kết quả"` — centered, 16px Medium/20px, grey-800 `#1C1C1C`. Note: the raw Figma export
   carries a leftover `px-[240px]` class on this node (copy-pasted from the desktop artboard) —
   this is a **design-file authoring bug**, not an intentional mobile layout; implement with sane
   mobile padding (e.g. `16–24px`) instead of literal 240px.
2. Title `"KHÔNG CÓ KẾT QUẢ"` — **20px "LC Sac Trial" Regular / 56px line-height** (same
   oversized-line-height quirk as the "with results" mobile heading), color **black** — differs
   from desktop's grey-500; captured as-is from the file, flag for design QA if unintentional.
3. Subtext block (`padding: 20px 0 40px`) — `13px Regular/16px Josefin Sans` (Caption token), grey-800
   `#1C1C1C`: `"Vui lòng thử lại từ khóa khác hoặc quay lại "` + underlined `"trang chủ"`.
4. Illustration: `Group` image, **140 × 135.64px** (smaller than desktop's 197×191), centered.
5. Footer / newsletter band follows (shared partial).

```css
.empty-state--mobile { padding-top: 120px; }
.empty-state--mobile .empty-state__count { padding: 0 16px; } /* override the 240px authoring bug */
.empty-state--mobile .empty-state__title { font-size: 20px; line-height: 56px; color: #000; }
.empty-state--mobile .empty-state__subtext { font: 400 13px/16px "Josefin Sans"; padding: 20px 0 40px; }
.empty-state--mobile .empty-state__illustration { width: 140px; height: 135.64px; }
```

---

## Design Tokens Used (cross-check against category-page-template.md — all consistent)

| Token | Hex / Value |
|---|---|
| Grey 800 (body text) | `#1C1C1C` |
| Grey 500 (empty-state title, desktop / "Xem thêm" links) | `#7A7A7A` |
| Grey 300 (mobile group divider) | `#BFBFBF` |
| Black | `#000000` |
| Font — body/label/subtitle | Josefin Sans (Regular/Medium/Bold, per token table in category-page-template.md) |
| Font — large display headings | "LC Sac Trial" Regular |

## Open Items / Follow-ups
- Desktop "with results" heading color unconfirmed (metadata-only pull, no screenshot) — verify
  against a rendered screenshot of node `988:21321` before implementation sign-off.
- Mobile empty-state heading color (black) vs. desktop (grey-500) inconsistency — confirm with
  design whether intentional.
- Mobile `px-[240px]` on the "0 kết quả" line in both mobile frames is almost certainly a stray
  desktop-token leftover; do not implement literally.
- Search input control itself was not visible in either scope pulled — it lives in the shared Nav
  (search icon triggers something not expanded here); confirm search overlay/input design
  separately if not already covered elsewhere.
