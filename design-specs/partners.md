# Partners / Collaboration Page ("Hợp tác") — Design Spec

Figma file: `uQFY9gwfNbNSeTM6zmspzo`
Desktop node: `800:27764` (1920 × 2823) · Mobile node: `1068:39192` (430 × 2529)

Page order (both breakpoints): Breadcrumb → **Niềm tự hào** (stats + logo grid) → **Yêu cầu mua sỉ** (wholesale form) → Footer.

## Design tokens used on this page

| Token | Value |
|---|---|
| Brand red (stat numbers, heading accent) | `#AF2234` |
| Red-900 (button bg) | `#410C13` |
| Grey-50 (field bg) | `#F2F2F2` |
| Grey-300 (logo tile divider) | `#BFBFBF` |
| Grey-400 | `#9B9B9B` |
| Grey-800 (body/label text) | `#1C1C1C` |
| Placeholder image box (form) | `rgba(0,0,0,0.2)` on white, `border-radius: 8px` |
| Heading font (big stat numbers) | `LC Sac Trial, Regular` |
| Body/label/button font | `Josefin Sans` (weights: Light 300, Regular 400, Medium 500, SemiBold 600) |

Type scale referenced:
- Desktop Heading 4: Josefin Sans SemiBold 24/32
- Desktop Heading 3: Josefin Sans Medium 32/40 (section titles, e.g. "Yêu cầu mua sỉ")
- Common/Label: Josefin Sans Medium 16/20 (form field text, buttons)
- Common/Body 2: Josefin Sans Light 16/18 (mobile form field text)
- Common/Subtitle: Josefin Sans Medium 20/23 (mobile section sub-heading)
- Common/Caption: Josefin Sans Regular 13/16 (mobile stat captions)

---

## Niềm tự hào (Stats + Logos)

### Desktop

Section frame: `1920 × 932` (`y=220` within page), inner content container `1440px` wide, centered with `240px` side gutters (standard page margin).

**Stats block** — `Frame 235` (`800:27778`), padding-top 80, height 339:
- Container: flex column, full width 1440px.
- Heading: "Thương hiệu Hoài tự hào với" — Josefin Sans SemiBold 24/32, color `#000`, margin-bottom effectively via 32px gap to stats row.
- Stats row (`Frame 324`, `800:28241`): flex row, `justify-content: center`, `align-items: flex-start`, `gap: 80px`, vertical padding `40px 0` around it.
  - 3 stat items, each a flex column (`align-items: flex-start`):
    - Number: font `LC Sac Trial Regular`, size **100px**, line-height 191px (≈ optical center), color `#AF2234`.
      - "6+" → width ~178px
      - "100+" → width ~222px
      - "150.000+" → width ~349px
    - Label directly below (no explicit gap — number's tall line-height creates the spacing): Josefin Sans SemiBold 24/32, color `#000`, two lines, e.g. "Năm thành lập" / "và phát triển"; "Đối tác tập đoàn" / "hàng đầu Việt Nam" (×2, reused text for items 2 & 3 per source file).
  - CSS-ready pattern:
    ```css
    .stats-row { display:flex; justify-content:center; align-items:flex-start; gap:80px; padding:40px 0; }
    .stat-num  { font-family:"LC Sac Trial"; font-weight:400; font-size:100px; line-height:191px; color:#AF2234; }
    .stat-label{ font-family:"Josefin Sans"; font-weight:600; font-size:24px; line-height:32px; color:#000; }
    ```

**Partner logo grid** — `Frame 331` (`800:29668`), `y=483`, height `369.14`, full-bleed width 1920 with `160px` side padding → inner `1600px`:
- **3 rows × 8 columns** of logo tiles.
- Each tile: `182.01px × 96.38px`, laid out with a horizontal gap of `~20.56px` (tile pitch 202.57px − 182.01px tile width) between columns; rows stacked with the same ~20.56px vertical gap (row pitch ~136.38px − 96.38px height... actually row-to-row offset is 136.38px, i.e. `96.38 + 40px` gap — treat as `gap: 40px` row/column for simplicity, tile `182×96`).
- Logo tile component ("Logo", instance `Component 1`): 
  - `border-bottom: 1.56px solid #BFBFBF` (divider under every tile, grid-line effect)
  - `padding: 10px 45px`
  - `display:flex; align-items:center; justify-content:center;`
  - Content: greyscale/monochrome partner logo SVG (e.g. "HAKYO Fusion Bakery" sample), centered.
- Recommended CSS:
  ```css
  .logo-grid { display:grid; grid-template-columns:repeat(8,182px); grid-auto-rows:96px; column-gap:20px; row-gap:40px; justify-content:center; }
  .logo-tile { display:flex; align-items:center; justify-content:center; padding:10px 45px; border-bottom:1.56px solid #BFBFBF; }
  .logo-tile img { max-width:100%; max-height:100%; filter:grayscale(1); opacity:.7; }
  ```
- Since there are 24 tiles total (3×8) but likely fewer real partner logos, plan for a `foreach` loop over a partner-logo collection in Razor with placeholder/looping fallback if count < 24.

### Mobile

Section frame: `430 × 378.6` (`y=116` within page). Inner content padded `16px` each side → `398px` effective width.

**Stats block** (`1068:39203`), padding-top 40:
- Heading: "Thương hiệu Hoài tự hào với" — Josefin Sans Medium 20/23, color `#000`.
- Stats row (`1068:39207`): flex row, `justify-content: space-between` (NOT centered — spreads to fill width), `align-items: flex-start`, vertical padding `10px 0`.
  - Number: `LC Sac Trial Regular`, size **28px**, line-height 36px, color `#AF2234`.
  - Label: Josefin Sans Regular 13/16, color `#000`, two lines (Common/Caption token).
  - CSS:
    ```css
    .stats-row { display:flex; justify-content:space-between; align-items:flex-start; padding:10px 0; }
    .stat-num  { font-family:"LC Sac Trial"; font-size:28px; line-height:36px; color:#AF2234; }
    .stat-label{ font-family:"Josefin Sans"; font-weight:400; font-size:13px; line-height:16px; color:#000; }
    ```

**Partner logo grid** (`Frame 548`, `1142:21293`), `y=179`, height `159.6`, padded `16px` sides → `398px` inner:
- **4 rows × 6 columns** (mobile reflows to more/narrower columns instead of horizontal scroll).
- Tile size: `59.67px × 30.43px`, column gap `8px` (pitch 67.67 − 59.67), row gap `~12.6px` (row pitch 43.06 − 30.43).
- Same tile component/styling as desktop (border-bottom divider, centered logo), just smaller — the grid should probably use `object-fit:contain` scaling in a responsive CSS grid rather than a fixed pixel copy:
  ```css
  @media (max-width: 599px) {
    .logo-grid { grid-template-columns:repeat(6,1fr); grid-auto-rows:30px; column-gap:8px; row-gap:12px; }
    .logo-tile { padding:4px 8px; }
  }
  ```
- Note: Figma shows only 6 columns visible per row on mobile vs 8 on desktop — actual partner count likely reflows via CSS grid auto-wrap rather than a literal 4×6 fixed structure; treat "24 logos, wrap responsively" as the real model.

---

## Yêu cầu mua sỉ (Wholesale Form)

### Desktop

Section frame `800:29684`: full width 1920, height 822, `y=1152.14` in page. Inner container `1440px` wide, `240px`/`80px` (top) padding — i.e. standard page gutter `240px` left/right, `80px` top.

- Section title "Yêu cầu mua sỉ" — Josefin Sans Medium 32/40, color `#1C1C1C` (Heading 3 token). Margin-bottom `64px` to content.
- Content row (`Frame 339`, `800:29682`): flex row, `gap: 120px`, `align-items: flex-start`.
  - **Left**: image/illustration placeholder, `707px × 527px`, `background: rgba(0,0,0,0.2)`, `border-radius: 8px` (swap for an actual product/lifestyle image in production).
  - **Right**: form column, `flex:1` (min-width 0), `width ≈ 613px`, `display:flex; flex-direction:column; gap:27px`.

**Form fields** (`Frame 332`, `800:29669`) — CSS grid, `grid-template-columns: repeat(2, 1fr)`, `column-gap:12px`, `row-gap:11px`:
  1. Row 1 — **Tên*** — spans both columns (`grid-column: 1 / span 2`)
  2. Row 2 — **Họ*** — spans both columns
  3. Row 3 — **Email*** — spans both columns
  4. Row 4 — **Điện thoại** | **Mã bưu điện** — two separate cells side-by-side, `flex:1` each, gap `8px` between them (this row is the only true 2-up pair; rows 1–3 and 5 are full-width despite the "2-col grid" container — likely a CMS/Figma artifact of copy-pasted single-column fields inside a 2-col grid, each spanning both columns)
  5. Row 5 — **Tên doanh nghiệp*** — spans both columns

  All input "fields" (currently static placeholder text in Figma, replace with real `<input>`):
  ```css
  .field {
    background:#F2F2F2;
    border-radius:4px;
    padding:12px 16px;
    height:45.4px;              /* single-line fields */
    display:flex; align-items:center;
    font-family:"Josefin Sans"; font-weight:500; font-size:16px; line-height:20px;
    color:#1C1C1C;
  }
  .field::placeholder { color:#1C1C1C; opacity:.7; }
  .field-row-split { display:flex; gap:8px; } /* wraps Điện thoại + Mã bưu điện */
  .field-row-split .field { flex:1 0 0; }
  ```
  Razor/HTML suggestion:
  ```html
  <div class="wholesale-fields">
    <input class="field" name="Ten" placeholder="Tên*" required />
    <input class="field" name="Ho" placeholder="Họ*" required />
    <input class="field" type="email" name="Email" placeholder="Email*" required />
    <div class="field-row-split">
      <input class="field" name="DienThoai" placeholder="Điện thoại" />
      <input class="field" name="MaBuuDien" placeholder="Mã bưu điện" />
    </div>
    <input class="field" name="TenDoanhNghiep" placeholder="Tên doanh nghiệp*" required />
  </div>
  ```

**Request-type radio group** (`Frame 336`, `800:29679`), width `171px`, `gap:8px` column:
  - Label "Loại yêu cầu" — Josefin Sans Medium 16/20, `#1C1C1C`.
  - Options list, `gap:16px`, each option row: `display:flex; gap:8px; align-items:center;`
    - Custom checkbox/radio square: `20×20px`, `border:1px solid #1C1C1C`, `border-radius:2px`, unfilled (unchecked) state shown; likely a checked-state variant exists in the component (`property1` boolean prop in the generated code) — needs a filled/checkmark state for `:checked`.
    - Option label: Josefin Sans Medium 16/20, `#1C1C1C` — "Kinh doanh", "Quà doanh nghiệp".
  - Since only one should be selectable, implement as `<input type="radio" name="requestType">` visually replaced by the square (radio behaves like checkbox visually per design — square not circle).
  ```css
  .radio-group { display:flex; flex-direction:column; gap:16px; }
  .radio-option { display:flex; align-items:center; gap:8px; cursor:pointer; }
  .radio-box { width:20px; height:20px; border:1px solid #1C1C1C; border-radius:2px; flex:none; }
  .radio-option input:checked + .radio-box { background:#1C1C1C; /* add checkmark icon/pseudo-element */ }
  ```

**Message textarea** (`792:27651`): full width, `height:135px`, `background:#F2F2F2`, `border-radius:4px`, `padding:12px 16px`, label/placeholder "Tin nhắn :" top-left, same type style (Medium 16/20, `#1C1C1C`).
  ```css
  .field-textarea { background:#F2F2F2; border-radius:4px; padding:12px 16px; height:135px; width:100%; resize:vertical; font:inherit; }
  ```

**Submit button** (`792:27662`, "Gửi"): `padding:12px 24px`, `background:#410C13`, `border-radius:4px`, text Josefin Sans Medium 16/20, color `#F2F2F2`, auto width (`74×44px` rendered). Placed with `align-self:flex-start` under the textarea (27px gap from the fields block above, per parent flex column gap).
  ```css
  .btn-submit { background:#410C13; color:#F2F2F2; border:none; border-radius:4px; padding:12px 24px; font-family:"Josefin Sans"; font-weight:500; font-size:16px; line-height:20px; cursor:pointer; }
  .btn-submit:hover { background:#5a141d; } /* suggested hover, not in Figma */
  ```

### Mobile

Section frame `1068:39246`: width 430, height 1117, padded `16px` sides → `398px` content width, `40px` top padding.

- Section title "Yêu cầu mua sỉ" — same Heading-3-ish 32/40 style reused from desktop token but check actual mobile size in file (uses same text style id `792:27644`→ mobile copy at `1068:39248`, same 32/40 medium). Margin-bottom `47px` to content.
- Content stacks **vertically** (no side-by-side image + form): `Frame 339` (`1068:39249`) = flex column:
  1. **Form block** (`Frame 338`, `1068:39251`) first, `gap:27px` internally, same field grid pattern as desktop (2-col grid, most rows spanning both columns except Điện thoại/Mã bưu điện pair which uses `flex:1` split at `195px` each with visible gap).
     - Field typography differs from desktop: mobile fields use **Josefin Sans Light 16/18** (Common/Body 2) instead of Medium — desktop is Medium 16/20. Match per breakpoint.
     - Radio group: identical structure/sizing to desktop (`20×20px` box), width `171px`.
     - Textarea: same `135px` height, `#F2F2F2` bg, Light 16/18 text.
     - Submit button "Gửi": same style, but positioned `align-self:flex-end` (right-aligned) at `x=324` within the 398px column — i.e. button sits bottom-right of the form, not left-aligned like desktop.
  2. **Image placeholder** (`Rectangle 263`, `1068:39250`) comes **after** the form in source order, `398 × 352px`, same `rgba(0,0,0,0.2)` fill, `border-radius:8px`, full width, `y=638` (i.e. below the button with implicit gap).

  Recommended mobile stacking:
  ```css
  @media (max-width: 599px) {
    .wholesale-content { display:flex; flex-direction:column; gap:27px; }
    .wholesale-form { order:1; }
    .wholesale-image { order:2; width:100%; aspect-ratio: 398/352; border-radius:8px; background:rgba(0,0,0,.2); }
    .btn-submit { align-self:flex-end; }
    .field, .field-textarea { font-family:"Josefin Sans"; font-weight:300; font-size:16px; line-height:18px; }
  }
  ```

---

## Implementation notes for Razor view (`Views/.../Partners.cshtml`)

1. Use one shared partial/CSS for the `.field` / `.field-textarea` / `.radio-*` / `.btn-submit` classes so desktop and mobile only differ by font-weight/line-height and layout order via media queries — avoid duplicating markup per breakpoint.
2. Bind form fields to a `WholesaleRequestViewModel` (Ten, Ho, Email, DienThoai, MaBuuDien, TenDoanhNghiep, RequestType enum [KinhDoanh, QuaDoanhNghiep], TinNhan) with `asp-for` tag helpers; required fields marked with `*` need `[Required]` + client-side validation matching the grey `#F2F2F2` box (no visible border, so show validation state via a red outline or a `is-invalid` class over the same fill).
3. Partner logos: model as `List<PartnerLogoViewModel>{ Name, LogoUrl }`, rendered via CSS Grid (`repeat(8, ...)` desktop / `repeat(6,1fr)` or fewer mobile) with `foreach` — do not hardcode 24 static `<img>` tags.
4. Stat numbers (6+, 100+, 150.000+) can be hardcoded in the view or sourced from a small config/CMS field since they're marketing copy, not dynamic data.
5. "LC Sac Trial" is a custom/purchased font — confirm license and self-host as `@font-face` (woff2) alongside Josefin Sans (likely already loaded via Google Fonts elsewhere in the site).
