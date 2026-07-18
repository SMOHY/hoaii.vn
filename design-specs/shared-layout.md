# hoaii.vn — Shared Layout Design Spec (Nav / Footer / Chatbox)

Source: Figma file `uQFY9gwfNbNSeTM6zmspzo`. Extracted for hand-coded ASP.NET Core MVC Razor partials (`_Nav.cshtml`, `_Footer.cshtml`, `_ChatWidget.cshtml`) under `_Layout.cshtml`.

## Design tokens (shared)

| Token | Hex |
|---|---|
| Red / brand primary (red-500) | `#AF2234` |
| Red-600 (hover/darker) | `#9F1F2F` |
| Red-900 (deep maroon, buttons) | `#410C13` |
| Grey-50 (page/section bg) | `#F2F2F2` |
| Grey-100 | `#D6D6D6` |
| Grey-300 | `#BFBFBF` |
| Grey-400 | `#9B9B9B` |
| Grey-500 | `#7A7A7A` |
| Grey-700 | `#3C3C3C` / `#000000` |
| Yellow/tan-50 (footer section bg) | `#F7F3EE` |
| Yellow/tan-400 (gold accent) | `#BB9E78` |
| Yellow/tan-800 (footer link/body text) | `#5E4A2F` |
| Text default | `#303030` |
| White | `#FFFFFF` |

Fonts:
- **Josefin Sans** — body/label/nav text. Weights used: Regular (400), Medium (500), Bold (700), Light (300).
- **LC Sac Trial** — large decorative headings (e.g. footer newsletter heading), Regular style only.

Common type styles:
- Caption: Josefin Sans Regular 13/16
- Label: Josefin Sans Medium 16/20
- Body 1: Josefin Sans Regular 20/24
- Subtitle: Josefin Sans Medium 20/23
- Nav item text (desktop): Josefin Sans Regular 15px/24px, letter-spacing 0.4px
- Nav pill/CTA text (desktop): Josefin Sans Bold 15px/24px, letter-spacing 0.4px

---

## Nav

### Desktop (node 923:16871, "Nav / Property 1=Desktop/white")

Full-bleed component, design frame width 1920px; inner content constrained to **1440px**, centered. Three stacked rows:

**Row 1 — Announcement bar**
- Height: 40px
- Background: `#AF2234`
- Content: centered, `padding: 10px`
- Text: "Miễn phí đơn hàng từ 500.000đ" — Josefin Sans Medium 14px/24px, color white, letter-spacing 0.4px

**Row 2 — Sub-nav strip**
- Height: 48px
- Background: `#F2F2F2`
- Inner row: width 1440px, height 24px, `display:flex; justify-content:space-between; align-items:center; padding: 0 10px` (wrapped in outer `padding:10px` container)
- Left cluster: icon (24×24, "Variant3" badge icon) + gap 12px + text "Đón đầu xu hướng quà tặng với những vật phẩm thiết kế mới nhất vừa ra mắt!" (Josefin Sans Regular 15px/24px, black, letter-spacing 0.4px) — this is a single site-wide CMS setting (`announcement_text`), so it renders identically on every page even though some older Figma frames (e.g. category-page nav instances) still show the prior copy "Hơn 100+ mẫu bánh và quà tặng độc đáo".
- Right cluster: flex row, gap 40px, items: "Về chúng tôi", "Liên hệ", "Đại lý", "Blog" — same type style as above, black

**Row 3 — Main nav bar**
- Height: 80px, `#FFFFFF` background (implicit), content row width 1440px, centered vertically, bottom border 1px solid white (visual separator under logo area)
- Layout: `display:flex; justify-content:space-between; align-items:center` across full 1440px width
  - **Logo** (far left): 48×48px image
  - **Center menu** (flex row, gap 16px): 4 pill items, each `padding: 4px 6px 4px 8px; border-radius:4px; display:flex; align-items:center; gap:4px`. Label text Josefin Sans Bold 15px/24px white (pill background implied dark/transparent depending on state), plus a 16×16 chevron-down icon. Items: "Quà tết", "Quà trung thu", "Quà theo dịp", "Sản phẩm chọn lọc"
  - **Right icon cluster** (flex row, gap 16px):
    - Search icon button, 32×32 box, icon glyph ~18.67×18.67px centered
    - User icon, 32×32px
    - Cart/Shop icon, 32×32 box, glyph ~18.67×21.33px
    - Language switcher pill: `padding:4px 8px; border-radius:4px`, text "VN/" (white, bold) + "EN" (white 50% opacity), Josefin Sans Bold 15px, line-height 24px

CSS-ready reference:
```css
.nav-announcement { height:40px; background:#AF2234; display:flex; align-items:center; justify-content:center; padding:10px; }
.nav-announcement span { font-family:'Josefin Sans'; font-weight:500; font-size:14px; line-height:24px; color:#fff; letter-spacing:0.4px; }

.nav-subbar { height:48px; background:#F2F2F2; display:flex; align-items:center; justify-content:center; padding:0 10px; }
.nav-subbar-inner { width:1440px; max-width:100%; height:24px; display:flex; align-items:center; justify-content:space-between; }
.nav-subbar-left { display:flex; align-items:center; gap:12px; }
.nav-subbar-right { display:flex; align-items:center; gap:40px; }
.nav-subbar-inner, .nav-subbar-left span, .nav-subbar-right a { font-family:'Josefin Sans'; font-weight:400; font-size:15px; line-height:24px; color:#000; letter-spacing:0.4px; }

.nav-main { height:80px; display:flex; align-items:center; justify-content:center; }
.nav-main-inner { width:1440px; max-width:100%; display:flex; align-items:center; justify-content:space-between; border-bottom:1px solid #fff; height:100%; }
.nav-logo { width:48px; height:48px; }
.nav-menu { display:flex; align-items:center; gap:16px; }
.nav-menu .item { display:flex; align-items:center; gap:4px; padding:4px 6px 4px 8px; border-radius:4px; }
.nav-menu .item span { font-family:'Josefin Sans'; font-weight:700; font-size:15px; line-height:24px; color:#fff; letter-spacing:0.4px; }
.nav-menu .chevron { width:16px; height:16px; }
.nav-icons { display:flex; align-items:center; gap:16px; }
.nav-icons .icon-btn { width:32px; height:32px; display:flex; align-items:center; justify-content:center; }
.nav-lang { padding:4px 8px; border-radius:4px; font-family:'Josefin Sans'; font-weight:700; font-size:15px; line-height:24px; }
.nav-lang .en { color:rgba(255,255,255,0.5); }
```

Razor partial notes (`_Nav.cshtml`):
- Build as 3 sections in one partial, or split into `_NavTopBar`, `_NavSubBar`, `_NavMain`.
- Menu pill items ("Quà tết", "Quà trung thu", "Quà theo dịp", "Sản phẩm chọn lọc") should be a `foreach` over a small view-model list (label + url + hasDropdown) so dropdown megamenus can be added later.
- Icons (search/user/cart) as inline SVG partials or `<img>` referencing `wwwroot/images/icons/*.svg` — replace Figma asset URLs with local assets (Figma URLs expire in 7 days).

### Mobile (node 1068:20349, "Nav / Property 1=Mobile/white")

Frame width 430px (mobile reference). Vertical stack, gap 16px between announcement block and rest (only one section shown at this depth):

**Row 1 — Announcement bar**
- Height: 32px, background `#AF2234`, padding 10px, centered
- Text: Josefin Sans Regular 13px/16px, white (no letter-spacing token specified)

**Row 2 — Main bar**
- Height: 60px, horizontal padding 20px
- Inner row: bottom border 1.5px solid `#F2F2F2`, vertical padding 4px, `display:flex; justify-content:space-between; align-items:center`
  - Left: hamburger menu icon button, 28×28px icon in a 64px-wide tap area
  - Center: logo, 36×36px
  - Right: icon cluster gap 8px — search icon (28×28 box, glyph ~16.33×16.33) and cart icon (28×28 box, glyph ~16.33×18.67)

No visible text menu — mobile nav relies on the hamburger to open an off-canvas/drawer menu (not present in this node; should reuse desktop menu items list in a slide-out drawer built separately).

CSS-ready reference:
```css
.nav-m-announcement { height:32px; background:#AF2234; display:flex; align-items:center; justify-content:center; padding:10px; }
.nav-m-announcement span { font-family:'Josefin Sans'; font-weight:400; font-size:13px; line-height:16px; color:#fff; }

.nav-m-main { height:60px; padding:0 20px; display:flex; align-items:center; justify-content:center; }
.nav-m-main-inner { width:100%; border-bottom:1.5px solid #F2F2F2; padding:4px 0; display:flex; align-items:center; justify-content:space-between; }
.nav-m-hamburger { width:28px; height:28px; }
.nav-m-logo { width:36px; height:36px; }
.nav-m-icons { display:flex; align-items:center; gap:8px; }
.nav-m-icons .icon-btn { width:28px; height:28px; }
```

Razor notes: implement as a Bootstrap-style off-canvas (`<div class="offcanvas">`) or a custom `<details>/<dialog>` triggered by the hamburger button; reuse the same menu-items view-model as desktop.

---

## Footer

### Desktop (node 683:4440, "Footer / Property 1=Desktop")

Frame width 1920px, white background, `display:flex; flex-direction:column; align-items:center; gap:80px; padding-top:80px`.

**Section A — Newsletter signup** (centered, gap 32px)
- Heading: "ĐĂNG KÝ VÀO DANH SÁCH" — font **LC Sac Trial** Regular, 48px/56px, black, centered
- Subheading: "Bên trong có gì? Bộ sưu tập mới, ưu đãi độc quyền,nguồn cảm hứng và nhiều hơn nữa" — Josefin Sans Regular 20px/24px, `#303030`, centered
- Email input group: border 1px solid `#9B9B9B`, border-radius 8px, padding 8px, flex row (space-between via large gap ~366px at 1440-design width — in practice use `justify-content:space-between` with `width` constrained, e.g. max-width ~480–560px)
  - Placeholder text "Nhập email của bạn": Josefin Sans Medium 20px/23px, `#7A7A7A`
  - Submit button "Gửi": background `#410C13`, `padding:8px 24px`, border-radius 8px, text white Josefin Sans Medium 20px/23px

**Section B — Link columns** (background `#F7F3EE`, full width, `padding: 80px 240px 120px`)
- Row: `display:flex; justify-content:space-between; align-items:flex-start` (design shows large fixed gaps; use `justify-content:space-between` in production for responsiveness)
  - **Brand block**: logo 114×114px + company info block (width 284px), Josefin Sans Medium 16px/20px, color `#5E4A2F`:
    - "Công ty TNHH MTV Hoài"
    - "Địa chỉ : 945 Ngô Gia Tự, P. Việt Hưng, TP. Hà Nội"
    - "MST : 0101287214" (MST value is a link to masothue.com)
  - **Column "VỀ HOÀI"** (width 155px): heading Josefin Sans Medium 20px/23px `#AF2234`; links gap 16px, Josefin Sans Medium 16px/20px `#5E4A2F`: Quà tết, Quà trung thu, Quà theo dịp, Sản phẩm chọn lọc, Câu chuyện, Đối tác
  - **Column "HỖ TRỢ KHÁCH HÀNG"** (width 244px): same heading style; links: Liên hệ, Chính sách trao đổi, Chính sách giao hàng
  - **Column "CHÍNH SÁCH PHÁP LÝ" + SOCIAL** (width 238px, stacked, gap 40px):
    - "CHÍNH SÁCH PHÁP LÝ": Điều khoản sử dụng, Chính sách bảo vệ dữ liệu cá nhân
    - "SOCIAL" heading (20px/23px `#AF2234`), then social icon row: 3 icons (Zalo/Facebook/Instagram/TikTok-style — actual assets: `Frame65`, 2×`SocialIcons`) each in a 48×48px tap target with 3px padding around a 24×24 glyph, row has bottom border 1px `#9B9B9B` under first group, gap 16px
    - Below: "Đã thông báo Bộ Công Thương" badge image, 106×40px, in a 122px-wide/40px-tall container, padding 8px left

CSS-ready reference:
```css
.footer { display:flex; flex-direction:column; align-items:center; gap:80px; padding-top:80px; background:#fff; }

.footer-newsletter { display:flex; flex-direction:column; align-items:center; gap:32px; text-align:center; }
.footer-newsletter h2 { font-family:'LC Sac Trial'; font-weight:400; font-size:48px; line-height:56px; color:#000; }
.footer-newsletter p { font-family:'Josefin Sans'; font-weight:400; font-size:20px; line-height:24px; color:#303030; }
.footer-signup { border:1px solid #9B9B9B; border-radius:8px; padding:8px; display:flex; align-items:center; justify-content:space-between; gap:16px; max-width:560px; width:100%; }
.footer-signup input { border:none; outline:none; padding:0 16px; font-family:'Josefin Sans'; font-weight:500; font-size:20px; line-height:23px; color:#7A7A7A; flex:1; }
.footer-signup button { background:#410C13; color:#fff; border:none; border-radius:8px; padding:8px 24px; font-family:'Josefin Sans'; font-weight:500; font-size:20px; line-height:23px; }

.footer-links-section { background:#F7F3EE; width:100%; padding:80px 240px 120px; display:flex; justify-content:center; }
.footer-links-row { display:flex; justify-content:space-between; align-items:flex-start; width:100%; max-width:1440px; gap:40px; flex-wrap:wrap; }

.footer-brand { display:flex; gap:24px; align-items:flex-start; }
.footer-brand img.logo { width:114px; height:114px; }
.footer-brand-info { display:flex; flex-direction:column; gap:16px; max-width:284px; font-family:'Josefin Sans'; font-weight:500; font-size:16px; line-height:20px; color:#5E4A2F; }

.footer-col { display:flex; flex-direction:column; gap:24px; }
.footer-col h3 { font-family:'Josefin Sans'; font-weight:500; font-size:20px; line-height:23px; color:#AF2234; }
.footer-col ul { display:flex; flex-direction:column; gap:16px; list-style:none; padding:0; margin:0; }
.footer-col a { font-family:'Josefin Sans'; font-weight:500; font-size:16px; line-height:20px; color:#5E4A2F; text-decoration:none; }

.footer-social { display:flex; flex-direction:column; gap:16px; }
.footer-social .icons-row { display:flex; align-items:center; border-bottom:1px solid #9B9B9B; padding-bottom:8px; }
.footer-social .icon-btn { width:48px; height:48px; padding:3px; display:flex; align-items:center; justify-content:center; }
.footer-social .icon-btn img { width:24px; height:24px; }
.footer-badge { width:122px; height:40px; padding-left:8px; }
.footer-badge img { width:106px; height:40px; object-fit:cover; }
```

Razor notes: the three link columns and social icons are natural candidates for a strongly-typed `FooterViewModel` (list of `FooterColumn { Title, Links[] }` + `SocialLink[]`) rendered via `@foreach` in `_Footer.cshtml`, avoiding hardcoded markup per column.

### Mobile (node 1068:16577, "Footer / Property 1=Mobile")

Frame width 430px. `display:flex; flex-direction:column; align-items:center; gap:40px; padding-top:40px`.

**Section A — Newsletter** (`padding:0 16px`, gap 16px, centered)
- Heading: LC Sac Trial Regular 20px/30px, black
- Subheading: Josefin Sans Regular 13px/16px, `#303030`, two lines
- Signup group: border 1px `#BFBFBF`, radius 8px, padding 4px, `display:flex; justify-content:space-between; align-items:center`, full width
  - Placeholder: Josefin Sans Light 14px/18px, `#7A7A7A`
  - Button "Gửi": background `#410C13`, radius 6px, padding 8px 24px, text white Josefin Sans Medium 16px/20px

**Section B — Link columns** (background `#F7F3EE`, `padding:40px 16px`, single-column stacked, gap 16px)
- Brand block: logo 60×60px + info text (Josefin Sans Regular 13px/16px, `#5E4A2F`), gap 24px between logo and text, gap 4px between text lines
- Column groups stacked vertically, gap 20px between groups:
  - "VỀ HOÀI" (heading Josefin Sans Medium 16px/20px `#AF2234`; links gap 8px, 13px/16px `#5E4A2F`)
  - "HỖ TRỢ KHÁCH HÀNG" (same style)
  - gap 40px, then "CHÍNH SÁCH PHÁP LÝ" (same style) and "SOCIAL" (heading 20px/23px `#AF2234`, icon row gap 8px, same 48×48 tap targets/24px icons, badge same as desktop)

CSS-ready reference (delta from desktop, mobile-specific):
```css
.footer-m { display:flex; flex-direction:column; align-items:center; gap:40px; padding-top:40px; }
.footer-m-newsletter { padding:0 16px; display:flex; flex-direction:column; align-items:center; gap:16px; text-align:center; width:100%; }
.footer-m-newsletter h2 { font-family:'LC Sac Trial'; font-size:20px; line-height:30px; color:#000; }
.footer-m-newsletter p { font-family:'Josefin Sans'; font-weight:400; font-size:13px; line-height:16px; color:#303030; }
.footer-m-signup { border:1px solid #BFBFBF; border-radius:8px; padding:4px; display:flex; align-items:center; justify-content:space-between; width:100%; }
.footer-m-signup input { font-family:'Josefin Sans'; font-weight:300; font-size:14px; line-height:18px; color:#7A7A7A; border:none; padding:0 16px; flex:1; }
.footer-m-signup button { background:#410C13; color:#fff; border-radius:6px; padding:8px 24px; font-family:'Josefin Sans'; font-weight:500; font-size:16px; line-height:20px; }

.footer-m-links { background:#F7F3EE; padding:40px 16px; display:flex; flex-direction:column; gap:16px; width:100%; }
.footer-m-brand { display:flex; gap:24px; align-items:flex-start; }
.footer-m-brand img.logo { width:60px; height:60px; }
.footer-m-brand-info { display:flex; flex-direction:column; gap:4px; font-family:'Josefin Sans'; font-weight:400; font-size:13px; line-height:16px; color:#5E4A2F; }
.footer-m-col { display:flex; flex-direction:column; gap:8px; }
.footer-m-col h3 { font-family:'Josefin Sans'; font-weight:500; font-size:16px; line-height:20px; color:#AF2234; }
.footer-m-col a { font-family:'Josefin Sans'; font-weight:400; font-size:13px; line-height:16px; color:#5E4A2F; }
```

Razor notes: use CSS media queries (`@media (max-width: 767.98px)`) to switch `.footer` from row to column rather than maintaining two separate partials — reuse the same `FooterViewModel`/markup, only restyle via CSS to avoid duplicated HTML/Razor.

---

## Chatbox widget

Source nodes: `1154:28893` (collapsed "Default" state, 69×65px) and `1154:28894`/Variant2 (expanded state, 139×179px). Fixed-position floating widget, typically bottom-right of viewport.

### Collapsed state ("Default", property1=Default)
- Container: 69×65px
- Main toggle button: 60×60px circle, background `#D6D6D6`, border 1.667px solid `#BFBFBF`, positioned top:2.5px, left:4px, contains chat-bubble icon (46.667px, flipped/rotated 180°) centered via `padding:6.667px`
- Behind/around it (only revealed on hover or as decorative — same position `left:4px, top:2.5px` stacked): phone-call icon (36px) and Zalo icon (36px) circles, both 60×60px, background `#F2F2F2`, fully overlapping the toggle position in the "Default" variant (they become visible/spread out in the expanded variant)

### Expanded state (Variant2, 139×179px) — appears on click/hover
Four circular buttons (60×60px each, background `#F2F2F2` except the close button), vertically fanned out to the left of a close (X) button:
- Phone/Call icon button: `left:72px, top:2.5px`
- Zalo icon button: `left:3px, top:40.5px`
- Messenger/chat icon button (`images (1)`): `left:12px, top:114.5px`
- Close (X) toggle button: 60px circle, background `#D6D6D6`, `left:72px, top:74.5px`, contains 46.667px cross icon (no border in expanded state)

All icon glyphs are 36×36px (or ~36–37.8px accounting for aspect-corrected assets), centered in their 60px circles via 15px padding.

CSS-ready reference:
```css
.chat-widget { position: fixed; right: 24px; bottom: 24px; width: 69px; height: 179px; z-index: 1000; }

.chat-widget .bubble {
  position: absolute; width: 60px; height: 60px; border-radius: 150px;
  background: #F2F2F2; display:flex; align-items:center; justify-content:center;
  padding: 15px; box-shadow: 0 2px 8px rgba(0,0,0,0.15);
}
.chat-widget .bubble img { width: 36px; height: 36px; }

.chat-widget .bubble--phone   { left: 72px; top: 2.5px; }
.chat-widget .bubble--zalo    { left: 3px;  top: 40.5px; }
.chat-widget .bubble--messenger { left: 12px; top: 114.5px; }

.chat-widget .toggle {
  position: absolute; width: 60px; height: 60px; border-radius: 150px;
  background: #D6D6D6; border: none; cursor: pointer;
  display:flex; align-items:center; justify-content:center; padding: 6.667px;
  left: 72px; top: 74.5px; /* collapsed state: left:4px; top:2.5px; add border 1.667px solid #BFBFBF */
}
.chat-widget .toggle img { width: 46.667px; height: 46.667px; }

/* Collapsed (closed) state overrides */
.chat-widget.is-collapsed { width: 69px; height: 65px; }
.chat-widget.is-collapsed .toggle { left: 4px; top: 2.5px; border: 1.667px solid #BFBFBF; }
.chat-widget.is-collapsed .bubble--phone,
.chat-widget.is-collapsed .bubble--zalo,
.chat-widget.is-collapsed .bubble--messenger { left: 4px; top: 2.5px; opacity: 0; pointer-events: none; }
```

Behavior: `.is-collapsed` shows only the toggle bubble (chat icon). On click/tap, remove `.is-collapsed` to fan out the phone/Zalo/Messenger bubbles and morph the toggle into an X (close) button — animate via `transition: left 0.25s ease, top 0.25s ease` on each `.bubble`/`.toggle`.

Razor notes: implement as a small partial `_ChatWidget.cshtml` rendered once in `_Layout.cshtml` (outside main `<main>`), with vanilla JS (or a small `chat-widget.js`) toggling an `is-collapsed` class on click. Icons (phone, Zalo, Messenger, close X) should be saved as local SVG/PNG assets under `wwwroot/images/chat/` since Figma asset URLs expire after 7 days.

---

## Notes / caveats
- Figma reference code was React + Tailwind (arbitrary-value classes); all pixel values above were read directly from those class names and node metadata, then translated to plain CSS for Razor/CSS use.
- Figma-hosted asset URLs (logo, icons, badges) expire ~7 days after generation — export and store all icons/images locally in `wwwroot/images/` before implementation.
- Desktop nav/footer frames were authored at 1920px canvas width with 1440px max-content; use `max-width:1440px; margin:0 auto` wrapper pattern for the real site to remain responsive between 1440–1920px+.
- Mobile frames authored at 430px (iPhone-ish) reference width — treat as `max-width: 480px` mobile breakpoint styling, fluid otherwise.
