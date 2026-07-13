# Home Page — Section 01: Hero & Đãi ngộ (Benefits)

Figma file: `uQFY9gwfNbNSeTM6zmspzo`

> **Section order:** Figma places Đãi ngộ (Benefits) *after* "Danh sách sản phẩm đặc biệt",
> not directly after the Hero. See `Views/Home/Index.cshtml` for the authoritative order.
>
> **Copy updated in Figma since this spec was first written** — the values below now match Figma:
> announcement bar, utility-bar tagline, hero caption, and all three benefit columns.

Design tokens referenced throughout:
- Brand red `#AF2234` (red-500), red-600 `#9F1F2F`
- Brand gold/tan `#AA8656` (yellow-500), yellow-400 `#BB9E78`, yellow-50 `#F7F3EE`
- Grey scale: grey-50 `#F2F2F2`, grey-100 `#D6D6D6`, grey-300 `#BFBFBF`, grey-500 `#7A7A7A`, grey-600 `#5A5A5A`
- Fonts: **Josefin Sans** (body/nav/labels, weights Light/Regular/Medium/SemiBold/Bold), **LC Sac Trial** Regular (large display headings only)

---

## Hero

### Desktop

Node: `1214:38715` — 1920px wide section, `flex-col`, `justify-between`.

Background is the product plate photo (`wwwroot/images/home/hero.jpg`), exported from the node's
*raw image fill* — **not** an export of the frame itself, which bakes the nav and caption into the
bitmap and double-renders them under the real markup.

The caption sits on a red scrim (node `1214:38719`), `top: 768px`, padding `40px 240px 120px`:
`linear-gradient(to bottom, rgba(175,34,52,0) 0%, rgba(175,34,52,0.8) 46.765%, #AF2234 100%)`.

**Structure (top → bottom, all children `position: absolute` within a 1920×~950px stage):**

1. **Nav bar** (`top: 0`, full width 1920px, `flex-col`, gap 8px) — sits on top of hero image, transparent/overlaid:
   - Announcement bar: height 40px, bg `#AF2234`, centered text, padding 10px. Text: "Hotline: 0941.686.682" — Josefin Sans Medium, 14px, color white, letter-spacing 0.4px, line-height 24px.
   - Secondary bar: height 48px, bg `#F2F2F2`, inner row max-width 1440px (centered), `justify-content: space-between`, height 24px.
     - Left: icon (24×24) + text "Đón đầu xu hướng quà tặng với những vật phẩm thiết kế mới nhất vừa ra mắt!" — Josefin Sans Regular, 15px, black, letter-spacing 0.4px.
     - Right: nav links row, gap 40px — "Về chúng tôi", "Liên hệ", "Đại lý", "Blog" — Josefin Sans Regular, 15px, black, letter-spacing 0.4px.
   - Main nav row: height 80px, border-bottom 1px solid white, inner row max-width 1440px centered, `justify-content: space-between`.
     - Logo: 48×48px, left-aligned.
     - Center menu (gap 16px between items, each item padding 8px 6px 4px, border-radius 4px): "Quà tết", "Quà trung thu", "Quà theo dịp", "Sản phẩm chọn lọc" — each Josefin Sans **Bold**, 15px, white, letter-spacing 0.4px, each followed by a 16×16 down-chevron icon (gap 4px).
     - Right icons (gap 16px): Search (32×32, vector icon ~18.7px), User (32×32 icon), Cart/Shop (32×32, vector icon), and language switch "VN/EN" — Josefin Sans Bold, 15px, "VN/" white + "EN" white @ 50% opacity.

2. **Slide arrows row**: `top: 551px`, full width 1920px, `justify-content: space-between`, horizontal padding 20px. Two circular arrow buttons, 48×48px, bg `rgba(255,255,255,0.2)`, border-radius 52px (fully round), centered 20×10px chevron icon (rotated 90°), right one rotated 180° (points right).

3. **Hero text block**: `top: 826px`, width 1920px, padding `16px 240px 80px`, `flex-col`, gap 32px, align items start.
   - Slide-dots indicator: 75×12px image/sprite (4 dots, pagination indicator placeholder), `Frame163`.
   - Text group (gap 8px):
     - H1: "TINH HOA VIỆT NAM" — **LC Sac Trial Regular**, 70px, line-height 80px, letter-spacing 0.7px, color `#F7F3EE`, width 100%.
     - Subtitle: "Bộ sưu tập Quà tặng Trung Thu 2026" — Josefin Sans SemiBold, 24px, line-height 32px, white.

4. **Chat/contact widget** (bottom-right, `left: 1802px`, `top: 826px`, box 69×65px): stack of 3 circular buttons (60×60px, bg `#F2F2F2`, border-radius 150px/circle, padding 15px, positioned with slight left offset 4px, top 2.5px, likely stacked/expandable on hover) — icons: phone call (36×36), Zalo logo, Messenger-style icon. Plus one "toggle" circular button 46.667px core icon, bg `#D6D6D6`, border 1.667px solid `#BFBFBF`, border-radius 46px.

5. **Scroll-to-top / back-to-top button**: `left: 1804px`, `top: 904px`, 64×64px circle, bg `#F2F2F2`, border 1.067px solid `#DCDCDC`, border-radius 76.8px, centered double-chevron-up icon (38.4×38.4px, rotated -90°).

**Razor/CSS notes:** Build as a `position: relative` hero container (min-height ~950px, background-image cover) with nav as a top overlay bar (can be a shared `_Nav.cshtml` partial, likely reused across pages), slide arrows and pagination dots as carousel controls, hero caption block absolutely positioned bottom-left, and a floating contact-widget partial fixed to viewport (typically `position: fixed` in production even though Figma shows it inside the hero frame).

---

### Mobile

Node: `1062:12563` — 430px wide, background `#000000` (black placeholder for hero image/video), `flex-col`, `justify-between`.

**Structure (absolute-positioned within ~700px stage):**

1. **Nav** (`top: 0`, width 430px, `flex-col`, gap 16px):
   - Announcement bar: height 32px, bg `#9F1F2F` (red-600), padding 10px, centered. Text "Miễn phí đơn hàng từ 500.000đ" — Josefin Sans Regular, 13px, white (Caption token, line-height 16px).
   - Main bar: height 60px, horizontal padding 20px, border-bottom 1.5px solid `#F2F2F2`, row `justify-content: space-between`, vertical padding 4px.
     - Left: hamburger menu icon, 28×28px.
     - Center: Logo, 36×36px.
     - Right (gap 8px): Search icon (28×28, vector ~16.3px), Cart/Shop icon (28×28, vector ~16.3×18.7px).

2. **Slide arrows row**: `top: 303px`, width 430px, padding 0 20px, `justify-content: space-between`. Two circular buttons 40×40px, bg `rgba(255,255,255,0.2)`, border-radius 52px, 16.7×8.3px chevron icon rotated 90° (right button additionally rotated 180°).

3. **Hero text block**: `top: 594px`, width 390px, padding `16px 16px 28px 28px`, `flex-col`, gap 16px, align items start.
   - Pagination dots sprite: 50×8px.
   - Text group (gap 8px, color white):
     - H1: "VIỆT NAM HOA THỊ" — **LC Sac Trial Regular**, 28px, line-height 26px, letter-spacing 0.7px, `white-space: nowrap`.
     - Subtitle: "Concept tết mới nhất 2026" — Josefin Sans **Light**, 16px, line-height 18px (Body 2 token).

**Razor/CSS notes:** Mobile hero collapses the desktop's 3-tier nav (announcement + utility + main links) down to announcement + single compact bar with hamburger (utility links and category menu move into an off-canvas drawer). No chat-widget / back-to-top nodes present in this mobile hero node — treat those as viewport-fixed globally, not hero-scoped.

---

## Đãi ngộ (Benefits)

3-column trust-badge strip, icon + title (brand red) + description (grey), repeated for: **Giao hàng toàn quốc**, **Cam kết chất lượng**, **Chiết khấu lên tới 35%**.

### Desktop

Node: `1214:38761` — full-width white (`#FFFFFF`) section, `display:flex; justify-content:space-between; align-items:flex-start`, padding `40px 240px 48px` (top/L-R/bottom).

Three equal columns, each `width: 356px`, `flex-col`, `align-items:center`, `gap: 28px`:

- **Icon**: 120×120px box, image bottom-aligned within (`justify-content:end`), icon graphic itself ~90-111px wide (delivery truck, checked box, discount tag/percent — decorative illustration assets, export as PNG/SVG per icon).
- **Text block**: `flex-col`, `align-items:center`, `text-align:center`, gap 8px (7px for 3rd column):
  - Title — Josefin Sans **SemiBold**, 24px, line-height 32px, color `#AF2234` (Heading 4 token).
  - Description — Josefin Sans Medium, 16px, line-height 20px, color `#5A5A5A` (Label token).

Column content:
| # | Title | Description |
|---|-------|--------------|
| 1 | Giao hàng toàn quốc | Thay bạn kết nối những tri âm, đưa quà đến từng ô cửa |
| 2 | Cam kết chất lượng | Tận tâm trong từng sản phẩm, an tâm tuyệt đối |
| 3 | Chiết khấu lên tới 35% | Giải pháp ngân sách tối ưu cho đơn hàng doanh nghiệp |

**CSS approach:** `.benefits { display:flex; justify-content:space-between; padding:40px 240px 48px; background:#fff; } .benefit-col { width:356px; display:flex; flex-direction:column; align-items:center; gap:28px; } .benefit-icon { width:120px; height:120px; display:flex; align-items:flex-end; justify-content:center; } .benefit-title { font:600 24px/32px 'Josefin Sans'; color:#AF2234; text-align:center; } .benefit-desc { font:500 16px/20px 'Josefin Sans'; color:#5A5A5A; text-align:center; }`

---

### Mobile

Node: `1062:12606` — full-width white section, `flex; justify-content:space-between; align-items:flex-start`, padding `40px 16px`.

Three columns, each `flex: 1 0 0` (equal share, no fixed width), `flex-col`, `align-items:center`, `gap: 16px` (7px gap in text block for 3rd column):

- **Icon**: 40×40px box (scaled down from desktop's 120px), bottom-aligned.
- **Text block**: `flex-col`, `align-items:center`, gap 8px, text centered, **two-line stacked labels** (no single-line description — mobile only shows the short title split across 2 lines):
  - Josefin Sans Regular, 13px, line-height 16px, color `#AF2234` (Caption token). Each label is two `<p>` lines, e.g. "Miễn phí" / "giao hàng".

Column content (mobile shows title only, split in two lines — no separate grey description paragraph):
| # | Line 1 | Line 2 |
|---|--------|--------|
| 1 | Giao hàng | toàn quốc |
| 2 | Cam kết | chất lượng |
| 3 | Chiết khấu | lên tới 35% |

**CSS approach:** `.benefits-mobile { display:flex; justify-content:space-between; padding:40px 16px; background:#fff; } .benefit-col { flex:1 1 0; min-width:0; display:flex; flex-direction:column; align-items:center; gap:16px; } .benefit-icon { width:40px; height:40px; display:flex; align-items:flex-end; justify-content:center; } .benefit-label { font:400 13px/16px 'Josefin Sans'; color:#AF2234; text-align:center; }`

**Razor/CSS notes:** Same 3 icon assets reused between breakpoints (export at 2x for the 120px desktop size, downscale via CSS `width/height:40px` on mobile — no need for separate mobile-only icon exports). Mobile drops the grey description copy entirely; only show it ≥768px (media query or a `d-none d-md-block` style utility class).
