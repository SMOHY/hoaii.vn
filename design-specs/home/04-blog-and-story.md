# Home — Section 04: Blog & "Câu chuyện trong từng sản phẩm"

Figma file key: `uQFY9gwfNbNSeTM6zmspzo`

## Blog

Section heading: "HOÀI MÁCH BẠN"

### Desktop

Node: `1214:38913`

**Section container**
- Background: `#FFFFFF`
- Layout: flex column, `align-items: center`, `gap: 80px`
- Padding: `80px 240px`
- Full width

**Heading**
- Text: "HOÀI MÁCH BẠN"
- Font: `LC Sac Trial`, Regular, 48px / line-height 56px, color `#000000`, text-align center, full width

**Cards row** (`1214:38915`)
- Layout: flex row, `gap: 40px`, `align-items: flex-start`, `justify-content: center`, full width
- Two columns, each `flex: 1 0 0` (equal width, ~50/50 split minus gap)

**Column 1 — large featured card** (`1214:38916`)
- Layout: flex column, `gap: 24px`, `align-items: flex-start`
- Image placeholder: full column width, height `340px`, `border-radius: 8px`, background `rgba(0,0,0,0.2)` (placeholder — replace with real image)
- Text block: flex column, `gap: 16px`
  - Category/label group: flex column, `gap: 8px`
    - Category label ("Đời sống"): Josefin Sans Medium 500, 16px / 20px line-height, color brand gold `#AA8656`
    - Title ("Gợi ý chọn quà tặng cho người thân yêu"): Josefin Sans (Common/Subtitle), 20px / 23px line-height, weight 500, color `#000000`
  - Excerpt (Lorem ipsum): Josefin Sans, 16px / 20px line-height, color `#525252`

**Column 2 — 3 stacked compact cards** (`1214:38923`)
- Layout: flex column, no gap between rows (dividers via border instead), `align-items: flex-start`, full width
- Each row: flex row, `gap: 24px`, `padding: 8px 0`, `align-items: flex-start`, full width
  - Rows 1 & 2 have bottom divider: `border-bottom: 1px solid #D6D6D6`
  - Row 3 (last): no divider
  - Text block (`flex: 1 0 0`, min-width 0): flex column, `gap: 16px`
    - Category/title group: flex column, `gap: 8px`
      - Category label: Josefin Sans Medium, 16px/20px, color `#AA8656`
      - Title: Josefin Sans (Subtitle), 20px/23px, weight 500, color `#000000`
    - Excerpt: Josefin Sans, 16px/20px, color `#525252`
  - Thumbnail image: fixed `240px × 140px`, `border-radius: 4.235px` (round to `4px`), background `rgba(0,0,0,0.2)` placeholder, `flex-shrink: 0`

**Typography tokens used**
- Common/Label: Josefin Sans, Medium 500, 16px, line-height 20px
- Common/Subtitle: Josefin Sans, Medium 500, 20px, line-height 23px
- Body/excerpt: Josefin Sans, Regular, 16px, line-height 20px, color `#525252`
- Grey-100 divider: `#D6D6D6`
- Brand gold/tan: `#AA8656`

**Razor/CSS notes**
- Structure as a partial `_BlogSection.cshtml` looping over a `BlogPost` view-model list (need at least 4 items: 1 featured + 3 list items).
- Card grid: use CSS Grid `grid-template-columns: 1fr 1fr` with `gap: 40px` at the row level, or flex as shown above (equal flex children is simplest to replicate exactly).
- Image placeholders should become `<img>` tags with `object-fit: cover` once real assets exist; keep `border-radius` and fixed dimensions.
- Divider pattern: apply `.blog-list-item:not(:last-child) { border-bottom: 1px solid #D6D6D6; }`.

### Mobile

Node: `1062:12757`

**Section container**
- Background `#FFFFFF`
- Layout: flex column, `align-items: center`, `gap: 8px`
- Padding: `40px 16px`
- Full width

**Heading**
- Text: "HOÀI MÁCH BẠN"
- Font: `LC Sac Trial`, Regular, 20px / line-height 30px, color `#000000`, text-align center

**Cards list** (single column, 4 stacked rows, no separate featured card on mobile — all 4 items use the same compact row layout)
- Each row: flex row, `gap: 12px`, `padding: 8px 0`, `align-items: flex-start`, full width
  - Rows 1–3 have bottom divider: `border-bottom: 1px solid #D6D6D6`
  - Row 4 (last): no divider
  - Thumbnail image: fixed `137.143px × 80px` (round to `137px × 80px`), `border-radius: 2.42px` (round to `2px`), background `rgba(0,0,0,0.2)` placeholder, `flex-shrink: 0`
  - Text block (`flex: 1 0 0`, `align-self: stretch`): flex column, `gap: 16px`
    - Category/title group (`flex: 1 0 0`): flex column, `gap: 8px`
      - Category label ("Đời sống"): Josefin Sans Regular 400, 13px / line-height 16px, color `#AA8656` (Common/Caption token)
      - Title: Josefin Sans Medium 500, 16px / line-height 20px, color `#000000` (Common/Label token)
    - No excerpt text shown on mobile cards (only category + title)

**Typography tokens used**
- Common/Caption: Josefin Sans, Regular, 13px, line-height 16px
- Common/Label: Josefin Sans, Medium 500, 16px, line-height 20px
- Grey-100 divider: `#D6D6D6`
- Brand gold/tan: `#AA8656`

**Razor/CSS notes**
- Simpler than desktop: one repeated row partial, no featured/large-card variant needed.
- `<img>` width/height ~137×80px, `border-radius: 2px`, `object-fit: cover`.
- Loop 4 `BlogPost` items; last item's row omits the divider (`:last-child` CSS selector, no extra markup needed).

---

## Câu chuyện trong từng sản phẩm

A parallax/sticky "story" banner section with a stylized wide heading overlapping a grey rectangle graphic. Content is largely a design placeholder (no real copy/image yet), so implement as a hero-style banner block.

### Desktop

Node: `1214:38902`

**Section container**
- Full-bleed section, `position: relative`
- Inner wrapper: `position: absolute; inset: 0; height: 1080px;` containing a `position: sticky; top: 0; width: 1920px; height: 1080px;` layer — this is a Figma sticky/parallax scroll effect. For a standard Razor/CSS build, simplify to a normal in-flow section with a fixed/aspect-ratio height (e.g. `height: 1080px` at 1920px design width, or use `aspect-ratio: 1920/1080` for responsiveness) — do not implement true scroll-jacking parallax unless specifically required.

**Grey graphic block**
- Position (within the 1920×1080 canvas): `left: 830px; top: 473.99px`
- Size: `260px × 145px`
- Background: `#BFBFBF` (Foundation/Grey/grey-300)
- Layout: flex, centered content (`align-items: center; justify-content: center; padding: 10px;`)
- This appears to act as a decorative "reveal" rectangle behind/behind part of the heading text (likely an image mask placeholder — treat as an `<img>`/background-image container in final build)

**Heading text** (overlapping the grey block)
- Text: "CÂU CHUYỆN TRONG TỪNG SẢN PHẨM"
- Font: `LC Sac Trial`, Regular, 56px / line-height 56px
- Color: brand red `#AF2234`
- `white-space: nowrap`, `text-align: center`
- Visually centered across the full section width, with the grey block sitting behind/under the middle portion of the text (per screenshot, the word "TRONG" sits over/near the grey rectangle)

**Colors**
- Brand red: `#AF2234`
- Grey-300: `#BFBFBF`

**Razor/CSS notes**
- Build as `.story-banner` section: `position: relative; width: 100%; aspect-ratio: 1920/1080;` (or a fixed viewport-height hero on desktop breakpoints only).
- Grey block as an absolutely-positioned `<div class="story-banner__accent">` using percentage-based left/top (830/1920 = 43.2%, 473.99/1080 = 43.9%) so it scales with the responsive container, sized 260/1920 = 13.5% width, 145/1080 = 13.4% height.
- Heading as an absolutely-centered `<h2>` (flex-centered over the whole section), `font-size: 56px`, color `#AF2234`, `white-space: nowrap` (desktop only — will need to wrap on smaller viewports, see mobile below).
- Treat the grey rectangle as a placeholder for a background image/photo reveal; real implementation likely swaps it for a product photo.

### Mobile

Node: `1062:12747`

**Section container**
- Relative wrapper, roughly `430px` design width (mobile breakpoint), inner block `430px × 340px` positioned `left: 0; top: 2px`

**Grey graphic block**
- Position: `left: 163px; top: 129px`
- Size: `104px × 82px`
- Background: `#BFBFBF`
- Layout: flex, centered content, `padding: 10px`

**Heading text** (two lines, stacked, overlapping grey block)
- Line 1: "CÂU CHUYỆN"
- Line 2: "TRONG TỪNG SẢN PHẨM"
- Font: `LC Sac Trial`, Regular, 28px / line-height 36px per line
- Color: brand red `#AF2234`
- `text-align: center`

**Razor/CSS notes**
- Mobile heading wraps to 2 lines (unlike desktop's single nowrap line) — implement as two `<span>`/`<p>` lines or allow natural wrap at a `max-width` around 300–350px.
- Grey block position as percentage of the 430×340 container: left 163/430 = 37.9%, top 129/340 = 37.9%, width 104/430 = 24.2%, height 82/340 = 24.1% — keep proportionally similar to desktop's relative placement (both roughly center-ish, slightly right of center).
- Simplify to a `.story-banner` block with `aspect-ratio: 430/340` on mobile, centered flex heading, and an absolutely positioned accent rectangle sized/positioned via percentages as above.
- Same color/font tokens as desktop (`#AF2234` text, `#BFBFBF` accent, `LC Sac Trial` font family).
