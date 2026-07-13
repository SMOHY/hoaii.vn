# Contact Page ("Liên hệ") — Design Spec

Figma file: `uQFY9gwfNbNSeTM6zmspzo`
Desktop frame: node `809:29893` (1920×~3866, base width 1920, content gutter `px-240px`)
Mobile frame: node `1142:25243` (430 wide, content gutter `px-16px`)

Target: `Views/Page/Contact.cshtml` (ASP.NET Core MVC + Razor), plain CSS (no Tailwind/utility framework — convert utility classes below to BEM-ish classes, e.g. `.contact-hero`, `.contact-address`, etc.)

## Design Tokens (reused across the page)

**Colors**
| Token | Hex |
|---|---|
| Brand red / red-500 | `#AF2234` |
| Red-600 | `#9F1F2F` |
| Red-900 (dark maroon, buttons/CTAs) | `#410C13` |
| Brand gold/tan (yellow-800, footer text) | `#5E4A2F` |
| Yellow-50 (footer bg) | `#F7F3EE` |
| Grey-50 (input bg) | `#F2F2F2` |
| Grey-200 (hairline borders) | `#DCDCDC` |
| Grey-300 (section divider) | `#BFBFBF` |
| Grey-400 | `#9B9B9B` |
| Grey-500 | `#7A7A7A` |
| Grey-700 | `#3C3C3C` |
| Grey-800 (body text) | `#1C1C1C` |
| White | `#FFFFFF` |

**Typography**
- Display heading ("LIÊN HỆ"): `LC Sac Trial, Regular` — Desktop 80px/70px line-height, letter-spacing 0.7px, color `#F2F2F2`; Mobile 28px/30px, same tracking.
- Section headings ("Địa chỉ", "Liên hệ", "Hãy gửi email cho chúng tôi") — Desktop Heading 3: Josefin Sans Medium 32px/40px, color `#1C1C1C`; Mobile: Josefin Sans Medium 20px/23px.
- Card subtitle/title (card labels, e.g. "TRÒ CHUYỆN TRỰC TIẾP"): Josefin Sans Medium 20px/23px `#1C1C1C` (desktop); Mobile Josefin Sans Medium 16px/20px.
- Body copy (card description, form intro paragraph): Desktop Josefin Sans Regular 20px/24px `#1C1C1C`; Mobile Josefin Sans Regular 13px/16px.
- Card link/CTA text (e.g. "Liên hệ facebook", phone, email): Josefin Sans Bold 20px/28px (desktop) `#1C1C1C`; Mobile Josefin Sans Medium 16px/20px.
- Form field label/placeholder & submit button label: Josefin Sans Medium 16px/20px `#1C1C1C` (labels) / `#F2F2F2` (submit button text, white on maroon).
- Breadcrumb ("Trang chủ/Mua quà tết"): Josefin Sans Medium 16px/20px `#F2F2F2` (desktop); Regular 13px/16px (mobile).

---

## Hero

### Desktop
- Container: full-width, `background: #AF2234`, height `1080px`, `display:flex; flex-direction:column; align-items:flex-end; gap:32px`.
- Nav bar is absolutely positioned on top (announcement bar `#AF2234` 40px h, sub-header `#F2F2F2` 48px h, main nav 80px h with logo, menu, search/user/cart icons) — reuse the site's shared `_Header` partial; not unique to Contact.
- Content row: `padding: 200px 240px 0`, `display:flex; flex:1; justify-content:flex-end; align-items:flex-start`, split into two columns:
  - Left column (flex:1, height 100%, `display:flex; flex-direction:column; justify-content:space-between; padding-bottom:80px`):
    - Breadcrumb text, 16px/20px, `#F2F2F2`.
    - `<h1>LIÊN HỆ</h1>` — LC Sac Trial 80px/70px, `#F2F2F2`, letter-spacing 0.7px.
  - Right column (width `617px`, height 100%, `display:flex; flex-direction:column; align-items:center; gap:40px; padding-bottom:56px`):
    - Decorative background watermark image (Group/pattern, 5% opacity, absolutely positioned, `width:1920px; height:567px`) — low-priority decorative asset, can be a repeating background-image behind the hero.
    - Placeholder image/panel: `background:#F2F2F2; border-radius:8px; flex:1; width:100%` (this is where a hero photo goes — currently blank in Figma, treat as `<img class="hero-photo">` placeholder).
    - Caption below image: Josefin Sans SemiBold 24px/32px, `#F2F2F2`, centered — "Hãy liên hệ với chúng tôi".

### Mobile
- Container: full width, `background:#AF2234`, height `720px`, column, `align-items:flex-end; gap:32px`.
- Content: `padding: 100px 16px 40px` in a single centered column (`align-items:center; gap:20px`):
  - Breadcrumb, 13px/16px, `#F2F2F2`, full width left-aligned.
  - Placeholder panel: `background:#F2F2F2; border-radius:4px`, flexible height filling remaining space.
  - Heading block (`gap:12px`): `<h1>LIÊN HỆ</h1>` LC Sac Trial 28px/30px `#F2F2F2` tracking 0.7px, then caption "Hãy liên hệ với chúng tôi" Josefin Sans Medium 16px/20px `#F2F2F2`.
- Nav: hamburger + logo + search/cart icons only (simplified mobile header), absolutely positioned over hero top.

---

## Địa chỉ (Address/Map)

### Desktop
- Section wrapper: `padding: 0 240px`, full width, white background.
- Inner block: `padding: 80px 0; border-bottom: 2px solid #BFBFBF; display:flex; flex-direction:column; gap:32px`.
- Heading: "Địa chỉ" — Heading 3 style (32px/40px Medium, `#1C1C1C`).
- Row: `display:flex; gap:40px; align-items:flex-start`:
  - Map/photo image: `width:751px; height:464px; border-radius:8px; object-fit:cover(bottom)` — placeholder photo (`image 15`), acts as a static map/street image.
  - Address text block: centered vertically (`display:flex; align-items:center`), Josefin Sans SemiBold 24px/32px `#303030`, two lines: "945 Ngô Gia Tự, P. Việt Hưng," / "TP. Hà Nội".
  - Small overlay image ("image 16", a map-pin/mini-map screenshot, `297×162px`) absolutely positioned near top-left of the section, offset `left:26px; top:171px` relative to the block — overlaps the bottom-left corner of the main photo like a floating map widget/badge.

### Mobile
- Section wrapper: `padding: 0 16px`.
- Inner block: `padding: 20px 0 40px; border-bottom: 2px solid #BFBFBF; display:flex; flex-direction:column; gap:16px`.
- Heading: "Địa chỉ" Heading (20px/23px Medium `#1C1C1C`).
- Column layout (`gap:20px`):
  - Map/photo image: full width, aspect-ratio `751/464`, `border-radius:8px`.
  - Address text (centered): Josefin Sans Medium 16px/20px `#303030`, single line "945 Ngô Gia Tự, P. Việt Hưng,TP. Hà Nội".
  - Overlay mini-map image: `206×112px`, absolutely positioned `left:17px; top:86px` (overlapping bottom-left of main photo, scaled version of desktop treatment).

---

## Contact Method Cards

### Desktop
- Section wrapper: `padding: 0 240px; border-bottom: 1px solid #BFBFBF`.
- Inner block: `padding: 80px 0; display:flex; flex-direction:column; gap:40px`.
- Heading: "Liên hệ" (Heading 3, 32px/40px Medium `#1C1C1C`).
- Grid: 2 rows × 2 columns, `display:flex; flex-direction:column; gap:32px`, each row `display:flex; gap:24px`.
- Card (each `width:708px`, ~flexible/equal): `background:#FFFFFF; border-radius:8px; box-shadow:0 4px 10px rgba(0,0,0,0.15); padding:16px 24px; display:flex; flex-direction:column; gap:10px`.
  - Title: Josefin Sans Medium 20px/23px `#1C1C1C`, uppercase (e.g. TRÒ CHUYỆN TRỰC TIẾP).
  - Description: Josefin Sans Regular 20px/24px `#1C1C1C` (2–3 lines).
  - Divider: `border-top:1px solid #DCDCDC; padding-top:8px`.
  - Link/value: Josefin Sans Bold 20px/28px `#1C1C1C` (e.g. "Liên hệ facebook", "Hoai@gmail.com", "033 500 6783", "Trò chuyện trực tiếp").
- 4 cards, content:
  1. TRÒ CHUYỆN TRỰC TIẾP — "Hãy truy cập facebook hoặc zalo của chúng tôi để nói chuyện trực tiếp với đội ngũ chăm sóc khách hàng về bất kỳ thắc mắc nào." → link: "Liên hệ facebook"
  2. LIÊN QUAN ĐẾN ĐƠN HÀNG — same description text → link: "Trò chuyện trực tiếp"
  3. YÊU CẦU BÁN BUÔN — "Bạn muốn xem mẫu? Vui lòng gửi email cho chúng tôi" → value: "Hoai@gmail.com"
  4. ĐIỆN THOẠI — "Hãy gửi cho chúng tôi một lời nhắn" → value: "033 500 6783"
- Note: In desktop the icons are not present in this frame (no icon assets in the returned nodes) — card is text-only (title/description/link). If brand icon set exists elsewhere, add a small icon before the title.

### Mobile
- Section wrapper: `padding: 0 16px`.
- Inner block: `padding: 20px 0 40px; border-bottom: 1px solid #BFBFBF; display:flex; flex-direction:column; gap:12px`.
- Heading "Liên hệ" 20px/23px Medium `#1C1C1C`.
- Stacked single column, `gap:8px` between cards (two groups of 2, but visually one continuous stack `gap:8px`).
- Card: `background:#FFFFFF; border:1px solid #DCDCDC (or #BFBFBF for last card); border-radius:8px; padding:12px 16px; display:flex; flex-direction:column; gap:10px`.
  - Title: Josefin Sans Medium 16px/20px `#1C1C1C`.
  - Description: Josefin Sans Regular 13px/16px `#1C1C1C`.
  - Divider: `border-top:1px solid #DCDCDC; padding-top:8px`.
  - Link/value: Josefin Sans Medium 16px/20px `#1C1C1C`.
- Same 4 cards/content as desktop, stacked full-width in document order (chat, order, wholesale, phone).

---

## Contact Form

### Desktop
- Section wrapper: `padding: 0 240px`.
- Inner block: `padding: 80px 0; display:flex; flex-direction:column; gap:16px`.
- Heading: "Hãy gửi email cho chúng tôi" (Heading 3, 32px/40px Medium `#1C1C1C`).
- Two-column row (`display:flex; gap:40px; align-items:flex-start`):
  - Left: intro paragraph, `width:766px`, Josefin Sans Regular 20px/24px `#1C1C1C` — "Chúng tôi rất vui vì điều gì đó ở đây đã thu hút sự chú ý của bạn. Hãy liên hệ để chào hỏi, trao đổi về nhu cầu của bạn và cùng nhau tìm hiểu cách hợp tác nhé."
  - Right: form column (`flex:1; display:flex; flex-direction:column; gap:24px`):
    - Field stack (`gap:16px`), each field:
      - `background:#F2F2F2; border-radius:4px; padding:12px 16px` container, label/placeholder text Josefin Sans Medium 16px/20px `#1C1C1C`.
      - Fields in order: **Tên\*** (required), **Họ\*** (required), **Email\*** (required, type=email), **Điện thoại** (optional, type=tel), **Tin nhắn:** (textarea, `height:135px`, `align-items:flex-start` so label/placeholder sits at top).
    - Submit button: `background:#410C13; color:#F2F2F2; border-radius:4px; padding:12px 24px; display:inline-flex; align-self:flex-start`, text "Gửi", Josefin Sans Medium 16px/20px.

### Mobile
- Section wrapper: `padding: 0 16px`.
- Inner block: `padding: 20px 0 40px; display:flex; flex-direction:column; gap:8px; align-items:flex-end` (heading/paragraph/form each full width but block aligned right per Figma "items-end" — in practice keep text left-aligned, just note container alignment quirk from Figma export).
- Heading "Hãy gửi email cho chúng tôi" 20px/23px Medium `#1C1C1C`, full width.
- Intro paragraph: Josefin Sans Regular 13px/16px `#1C1C1C`, full width, gap 20px below heading.
- Form column: `display:flex; flex-direction:column; gap:24px`:
  - Field stack `gap:12px`, same fields (Tên*, Họ*, Email*, Điện thoại, Tin nhắn:) each `background:#F2F2F2; border-radius:4px; padding:12px`, text Josefin Sans Regular 13px/16px `#1C1C1C`; textarea height `135px`.
  - Submit button: same style as desktop (`#410C13` bg, `#F2F2F2` text, `border-radius:4px; padding:12px 24px`), Josefin Sans Medium 16px/20px.

---

## Implementation Notes for Razor/CSS
- Use a shared `_Header.cshtml` / `_Footer.cshtml` partial — nav and footer markup is identical to other pages (confirmed footer node reused via `Footer` component in both frames: newsletter signup band, 4-column link/social area on `#F7F3EE` background, `#AF2234` for column titles, `#5E4A2F` for link text).
- Footer newsletter row: bordered pill input (`border:1px solid #9B9B9B` desktop / `#BFBFBF` mobile, `border-radius:8px`) with "Nhập email của bạn" placeholder + maroon "Gửi" button (`#410C13`).
- Build one `.contact-card` component/partial reused 4× for the contact-method cards; pass title/description/link as model or ViewData.
- Build one `.form-field` styled `<label>`/`<input>` wrapper reused for all 5 form fields (last one a `<textarea>`).
- Suggested CSS breakpoint: mobile styles apply ≤ 430–768px; desktop styles apply ≥ 1024–1920px (design uses fixed 1920 canvas, so scale container `max-width:1920px; margin:0 auto` with fluid gutter, or fix `padding:0 240px` down to a percentage/clamp for real-world widths between 1024–1920).
