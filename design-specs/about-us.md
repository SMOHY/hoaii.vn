# About Us — "Về chúng tôi"

Source: Figma file `uQFY9gwfNbNSeTM6zmspzo`
Nodes: Desktop `778:24532` (1920×4863), Mobile `1068:33689` (430×4580)

Page order (desktop y-offsets): Hero `0–1927` → Nền tảng thương hiệu `1927–2424` → Đội ngũ `2424–3370` → Khách hàng `3370–4014` → Footer `4014–4863` (Footer instance — reuse global footer partial, not respecced here).

Design tokens used in this spec:
- Brand gold `--brandcolor-1: #AA8656`
- Grey scale: grey-50 `#F2F2F2` (light text on dark bg), grey-100 `#D6D6D6` (secondary text on dark bg), grey-700 `#3C3C3C`, grey-900 `#0F0F0F` (heading text on light bg), body-on-light `#303030`
- Image placeholder fill (on dark sections): `rgba(0,0,0,0.2)`; image placeholder fill (on light "Đội ngũ" section): `#BFBFBF`-ish (`rgba(0,0,0,0.2)` over off-white bg reads as light grey ~`#CBC7C0`)
- Heading font: `LC Sac Trial`, Regular (large display headings, uppercase, generous line-height ~1.4×font-size)
- Body/label font: `Josefin Sans` — Light 300 (body copy), Medium 500 (labels/subtitles/nav tabs at Bold 700 for nav specifically)
- Hero + "Nền tảng thương hiệu" share one continuous **dark section** (charcoal/near-black background, sampled ~`#232323`–`#1A1A1A`; exact hex not exposed by Figma API as a solid fill token — treat as `--bg-dark` and confirm against brand guide/asset export before final lock). "Đội ngũ" and "Khách hàng" sit on a light/white section.
- This page **reuses the "Khách hàng" (customer logos) component** from the homepage. Cross-reference `design-specs/home/03-about-and-customers.md` (section "## Khách hàng") for the full marquee/grid spec, logo list, and CSS — only the heading copy and section padding are re-noted below since they differ slightly in this page's placement.

---

## Hero (Khởi đầu)

### Desktop

**Section** — `778:24533`, width `1920px`, height `1927px`, background `--bg-dark`, decorative faint line-art graphic (`Frame 82`, hidden layer — skip, not rendered) sits behind content.

Contains (top to bottom):
1. **Top bar** (`Frame 277`, height `248px`): global header instance (`Frame 494`, 168px) + sticky nav tabs row (`Frame 26`, 80px) — see "Sticky Nav Tabs" section below.
2. **Content column** (`Khởi đầu`, `1042:19914`) — `max-width:1440px; margin:0 auto; padding: 328px 240px 0;` (in-canvas offset), `display:flex; flex-direction:column;`
   - **Intro heading** (`955:18592`): `font-family:'LC Sac Trial'; font-weight:400; font-size:40px; line-height:60px; letter-spacing:0.7px; text-transform:uppercase; color:#AA8656; max-width:1183px;`
     - Copy: "Chúng tôi là một nhóm thiết kế độc lập, được thành lập vào năm 2021 và có văn phòng tại Hà Nội"
   - **Two-col media/text row** (`Frame 300`, `gap:39px; margin-top:160px;`)
     - Left: image placeholder `694×710px`, `border-radius:8px; background:#000` (or `rgba(0,0,0,0.2)` per pattern — treat as photo placeholder)
     - Right: flex column, `flex:1; padding-bottom:80px; display:flex; align-items:flex-start; justify-content:center; gap:24px;`
       - Eyebrow "KHỞI ĐẦU": `Josefin Sans Medium 500, 20px/23px, color:#F2F2F2`
       - Body (2 paragraphs, same repeated copy in source — placeholder text): `Josefin Sans Light 300, 16px/18px, color:#D6D6D6`
   - **Pull-quote** (`Frame 307`, `height:90px`, margin-top `~40px`): centered, `font-family:'LC Sac Trial'; font-weight:400; font-size:40px; line-height:70px; letter-spacing:0.7px; text-transform:uppercase; color:#AA8656; text-align:center;`
     - Copy: "“Gói ghém chân tình, viết tiếp dòng di sản”"
   - **Two-col statement/media row** (`Frame 302`, margin-top `~90px`, `display:flex; align-items:flex-start; gap:38px; justify-content:flex-end;`)
     - Left text block: `flex:1 0 0; font-family:'Josefin Sans'; font-weight:300; font-size:16px; line-height:18px; color:#D6D6D6;` — 3 paragraphs (intro line + 2 quote paragraphs), `<br>` blank line after first line
     - Right: image placeholder `830×479px; border-radius:8px; background:#000`

**Razor/CSS**
```html
<section class="hero hero--dark">
  <div class="hero__inner">
    <p class="hero__intro">Chúng tôi là một nhóm thiết kế độc lập, được thành lập vào năm 2021 và có văn phòng tại Hà Nội</p>
    <div class="hero__row hero__row--media-right">
      <div class="hero__media" style="background-image:url(...)"></div>
      <div class="hero__copy">
        <p class="eyebrow">KHỞI ĐẦU</p>
        <div class="body-text">
          <p>...</p>
          <p>...</p>
        </div>
      </div>
    </div>
    <p class="hero__quote">&ldquo;Gói ghém chân tình, viết tiếp dòng di sản&rdquo;</p>
    <div class="hero__row hero__row--media-left-text">
      <div class="body-text hero__statement">
        <p>Trên thực tế, điều này có nghĩa là:<br><br></p>
        <p>&ldquo;Văn hóa là một dòng chảy bất tận. ...&rdquo;</p>
        <p>Chúng tôi tìm thấy cảm hứng ...</p>
      </div>
      <div class="hero__media hero__media--wide"></div>
    </div>
  </div>
</section>
```
```css
:root{
  --bg-dark:#232323;
  --brand-gold:#AA8656;
  --grey-50:#F2F2F2;
  --grey-100:#D6D6D6;
}
.hero--dark{background:var(--bg-dark);color:var(--grey-50);}
.hero__inner{max-width:1440px;margin:0 auto;padding:80px 0 0;}
.hero__intro{font-family:'LC Sac Trial',serif;font-weight:400;font-size:40px;line-height:60px;letter-spacing:.7px;text-transform:uppercase;color:var(--brand-gold);max-width:1183px;margin:0;}
.hero__row{display:flex;gap:39px;margin-top:40px;}
.hero__media{border-radius:8px;background:rgba(0,0,0,.35);flex-shrink:0;}
.hero__row--media-right .hero__media{width:694px;height:710px;}
.hero__copy{flex:1 0 0;display:flex;flex-direction:column;gap:24px;justify-content:center;padding-bottom:80px;}
.eyebrow{font-family:'Josefin Sans',sans-serif;font-weight:500;font-size:20px;line-height:23px;color:var(--grey-50);margin:0;}
.body-text{font-family:'Josefin Sans',sans-serif;font-weight:300;font-size:16px;line-height:18px;color:var(--grey-100);}
.body-text p{margin:0;}
.hero__quote{text-align:center;font-family:'LC Sac Trial',serif;font-weight:400;font-size:40px;line-height:70px;letter-spacing:.7px;text-transform:uppercase;color:var(--brand-gold);margin:40px 0 0;}
.hero__row--media-left-text{justify-content:flex-end;margin-top:90px;}
.hero__row--media-left-text .hero__media--wide{width:830px;height:479px;}
.hero__statement{max-width:572px;}
```

### Mobile

**Section** — `1068:33690`, width `430px`, height `1505px`, background `--bg-dark`, `padding-inline:16px`.

Same content order, single column, gap `16px` between blocks:
- Top bar (`132px` total: header `92px` + nav `40px`)
- Intro heading: `LC Sac Trial 400, 20px/32px, letter-spacing:0.7px, uppercase, color:#AA8656`, full width
- Media (image placeholder): `398×452px`, full-width, `border-radius:8px`
- Copy block below media (stacked, not side-by-side): eyebrow "KHỞI ĐẦU" `Josefin Sans Medium 500 20px/23px #F2F2F2`, gap `27px` to body, body `Josefin Sans Light 300 16px/18px #D6D6D6`
- Pull-quote: full width, `font-size` scales down — treat as `24px/34px` (proportionally reduced from desktop 40/70; verify against screenshot) center-aligned, same color/family
- Statement text (full width, `398px`): same body-text style, stacked above media
- Media (second): `398×280px`, full width, `border-radius:8px`, placed **below** the statement text (stacked, not side-by-side, unlike desktop)

**CSS additions for mobile breakpoint**
```css
@media (max-width:767px){
  .hero__inner{padding:0 16px;max-width:430px;}
  .hero__intro{font-size:20px;line-height:32px;max-width:100%;}
  .hero__row{flex-direction:column;gap:20px;margin-top:16px;}
  .hero__row--media-right .hero__media{width:100%;height:452px;}
  .hero__copy{padding-bottom:0;gap:12px;}
  .hero__quote{font-size:24px;line-height:34px;margin-top:24px;}
  .hero__row--media-left-text{flex-direction:column;margin-top:24px;}
  .hero__row--media-left-text .hero__media--wide{width:100%;height:280px;margin-top:8px;}
  .hero__statement{max-width:100%;}
}
```

---

## Sticky Nav Tabs

In-page anchor nav for Khởi đầu / Nền tảng / Đội ngũ / Khách hàng. Sits immediately under the global header, still inside the dark hero background band. First tab ("Khởi đầu") is the active/current state by default (full opacity + bottom border); others are inactive (50% opacity, no border) until scrolled into view — implement as JS-driven `IntersectionObserver` toggling an `.is-active` class per section anchor.

### Desktop

Node `778:24899`/`778:24910`. Row: `display:flex; align-items:center; justify-content:center; gap:16px; padding-inline:240px; height:80px;` container has `border-bottom:1px solid #F2F2F2` (full width, sits under the tab row — appears to be the underline for the *entire* bar, with the active tab visually distinguished by opacity only in this capture; if a per-tab active underline is desired, add `border-bottom:2px solid var(--grey-50)` to `.is-active` instead of the wrapper).

Each tab: `padding: 4px 6px 4px 8px; border-radius:4px;`
- Label: `font-family:'Josefin Sans'; font-weight:700 (Bold); font-size:15px; line-height:24px; letter-spacing:0.4px; color:#F2F2F2;`
- Inactive tabs: `opacity:0.5`
- Active tab: `opacity:1` (default = "Khởi đầu")

```css
.about-nav{position:sticky;top:0;z-index:20;background:var(--bg-dark);border-bottom:1px solid var(--grey-50);}
.about-nav__list{display:flex;align-items:center;justify-content:center;gap:16px;max-width:1440px;margin:0 auto;padding:24px 240px;height:32px;}
.about-nav__tab{padding:4px 6px 4px 8px;border-radius:4px;font-family:'Josefin Sans',sans-serif;font-weight:700;font-size:15px;line-height:24px;letter-spacing:.4px;color:var(--grey-50);opacity:.5;background:none;border:none;cursor:pointer;white-space:nowrap;}
.about-nav__tab.is-active{opacity:1;}
```

### Mobile

Node `1068:33718`/`1068:33719`. Row: `padding-inline:20px; height:40px; display:flex; align-items:center; justify-content:center; gap:0 (tabs auto-spaced by their own width, ~small gap ~ per node offsets ~16px effective gap);` same visual treatment — Bold Josefin Sans, `color:#F2F2F2`, active=100% opacity, inactive=50%. Font size likely scales down slightly for mobile density but source shows same `15px`/`24px` line-height token reused — keep `15px` unless it overflows; reduce `gap` to `8–12px` and allow horizontal scroll (`overflow-x:auto; white-space:nowrap;`) if 4 tabs don't fit in 390px content width.

```css
@media (max-width:767px){
  .about-nav__list{padding:8px 20px;gap:8px;overflow-x:auto;-ms-overflow-style:none;scrollbar-width:none;}
  .about-nav__list::-webkit-scrollbar{display:none;}
}
```

---

## Nền tảng thương hiệu

3-column brand-foundation block: Mục đích (01) / Tầm nhìn (02) / Sứ mệnh (03). Same dark background as hero (continuous section, no visual seam).

### Desktop

**Section** — `792:24969`, width `1920px`, height `497px`. Inner container `792:24968`: `max-width:1440px; margin:0 auto; padding:80px 240px 0; display:flex; flex-direction:column; gap:40px;`

- Section eyebrow "NỀN TẢNG THƯƠNG HIỆU": `Josefin Sans Medium 500, 20px/23px, color:#F2F2F2`, full width
- **3-column row** (`792:24966`): `display:flex; gap:24px; align-items:flex-start;` — each column `flex:1 0 0; min-width:0; display:flex; flex-direction:column; gap:16px;`
  - **Number label** ("01"/"02"/"03"): `Josefin Sans Medium 500, 16px/20px, color:#F2F2F2`
  - **Divider**: `border-top:1px solid #F2F2F2` on the block below the number
  - **Heading row** (`padding-top:16px; display:flex; align-items:flex-end; justify-content:space-between;`): title "Mục đích"/"Tầm nhìn"/"Sứ mệnh" — `font-family:'LC Sac Trial'; font-weight:400; font-size:28px; line-height:32px; text-transform:uppercase; color:#F2F2F2;` (a duplicate "01" ghost label appears at `opacity:0` in source next to the heading — an unused Figma artifact, omit from markup)
  - **Body copy** (`padding:10px 0`): `Josefin Sans Light 300, 16px/18px, color:#F2F2F2`, column 1 longest (~7 lines), col 2 medium (~5 lines), col 3 shortest (~4 lines) — column heights vary (274/238/220px) but row uses `align-items:flex-start` so bottoms don't align; that's expected per design.

```css
.brand-foundation{background:var(--bg-dark);color:var(--grey-50);padding:80px 0 0;}
.brand-foundation__inner{max-width:1440px;margin:0 auto;padding:0 240px;display:flex;flex-direction:column;gap:40px;}
.brand-foundation__eyebrow{font-family:'Josefin Sans',sans-serif;font-weight:500;font-size:20px;line-height:23px;margin:0;}
.brand-foundation__cols{display:flex;gap:24px;align-items:flex-start;}
.bf-col{flex:1 0 0;min-width:0;display:flex;flex-direction:column;gap:16px;}
.bf-col__num{font-family:'Josefin Sans',sans-serif;font-weight:500;font-size:16px;line-height:20px;margin:0;}
.bf-col__body-wrap{border-top:1px solid var(--grey-50);display:flex;flex-direction:column;gap:8px;padding-top:16px;}
.bf-col__title{font-family:'LC Sac Trial',serif;font-weight:400;font-size:28px;line-height:32px;text-transform:uppercase;margin:0;}
.bf-col__text{font-family:'Josefin Sans',sans-serif;font-weight:300;font-size:16px;line-height:18px;padding:10px 0;margin:0;}
```

### Mobile

**Section** — `1068:33729`, width `430px`, height `906px`. Inner (`1068:33730`): `padding:40px 16px 0; display:flex; flex-direction:column; gap:40px;`

- Eyebrow: same style, `398px` width
- Columns stack **vertically** (`1068:33732`, `gap:40px`), each column full width (`398px`), internal `gap:8px`
  - Number label same style
  - Divider + heading row: title font size reduced to `20px/32px` (`LC Sac Trial`, still uppercase) vs desktop's 28px
  - Body copy same `16px/18px Josefin Sans Light`, no `padding:10px 0` wrapper (Figma shows `padding` removed on mobile variant — use `padding-top:0` or keep small `8px` for breathing room)
  - Third column ("Sứ mệnh") omits the ghost "01" node entirely (already invisible on desktop too)

```css
@media (max-width:767px){
  .brand-foundation__inner{padding:40px 16px 0;gap:40px;}
  .brand-foundation__cols{flex-direction:column;gap:40px;}
  .bf-col{gap:8px;}
  .bf-col__body-wrap{padding-top:8px;}
  .bf-col__title{font-size:20px;line-height:32px;}
  .bf-col__text{padding:0;}
}
```

---

## Đội ngũ

Team section — light background (switches from dark hero/brand-foundation to a light/white panel here). Heading + intro paragraph, plus a **decorative image collage** (4 placeholder blocks, asymmetric grid) with no captions/names in this Figma pass (final photography/name labels TBD).

### Desktop

**Section** — `792:24975`, width `1920px`, height `946px`. Inner (`792:24973`): `max-width:1440px; margin:0 auto; padding:80px 240px; display:flex; flex-direction:column; align-items:center; justify-content:center; gap:24px;`

- Heading block (`707px` wide, left-aligned within centered container): 
  - "ĐỘI NGŨ" eyebrow: `Josefin Sans Medium 500, 20px/23px, color:#0F0F0F`
  - Body copy (placeholder text, reused Tết filler copy — replace with real team intro): `Josefin Sans Light 300, 16px/18px, color:#303030`
- **Collage** (`792:24976`, `1189×669px`, absolute/grid-positioned overlapping rectangles, all `border-radius:8px; background:rgba(0,0,0,0.2)` placeholders):
  - Large right panel: `586×669px` at `left:603px`
  - Large top-left panel: `582×440px` at `left:0`
  - Small bottom-left panel A: `282×204px` at `left:0, top:460px`
  - Small bottom-left panel B: `282×204px` at `left:300px, top:460px`
  - (Panels A/B sit in the gap under the top-left large panel, side by side with an `18px` gutter; right panel spans the full collage height alongside them)

```css
.team-section{background:#fff;padding:80px 0;}
.team-section__inner{max-width:1440px;margin:0 auto;padding:0 240px;display:flex;flex-direction:column;align-items:center;gap:24px;}
.team-section__heading{width:707px;max-width:100%;display:flex;flex-direction:column;gap:16px;align-self:flex-start;}
.team-section__eyebrow{font-family:'Josefin Sans',sans-serif;font-weight:500;font-size:20px;line-height:23px;color:#0F0F0F;margin:0;}
.team-section__body{font-family:'Josefin Sans',sans-serif;font-weight:300;font-size:16px;line-height:18px;color:#303030;margin:0;}
.team-collage{position:relative;width:1189px;height:669px;max-width:100%;}
.team-collage__panel{position:absolute;border-radius:8px;background:rgba(0,0,0,.2);}
.team-collage__panel--right{right:0;top:0;width:586px;height:669px;}
.team-collage__panel--top-left{left:0;top:0;width:582px;height:440px;}
.team-collage__panel--bottom-a{left:0;top:460px;width:282px;height:204px;}
.team-collage__panel--bottom-b{left:300px;top:460px;width:282px;height:204px;}
```

### Mobile

**Section** — `1068:33757`, width `430px`, height `919px`. Inner (`1068:33758`): `padding:40px 16px; display:flex; flex-direction:column; align-items:center; gap:24px;`

- Heading block: full width (`398px`), same eyebrow/body styles as desktop (colors unchanged: `#0F0F0F` / `#303030`), `gap:16px`
- **Collage** stacks vertically, full width, `gap:8px`:
  1. Top panel: `398×318px`
  2. Middle panel: `398×198px`
  3. Bottom row (2 panels side by side, `gap:8px`): each `flex:1; height:196px`

```css
@media (max-width:767px){
  .team-section{padding:40px 16px;}
  .team-section__heading{width:100%;}
  .team-collage{width:100%;height:auto;display:flex;flex-direction:column;gap:8px;position:static;}
  .team-collage__panel{position:static;width:100%;}
  .team-collage__panel--top-left{height:318px;}
  .team-collage__panel--right{height:198px;}
  .team-collage__row-bottom{display:flex;gap:8px;}
  .team-collage__panel--bottom-a,.team-collage__panel--bottom-b{flex:1 0 0;height:196px;width:auto;}
}
```

---

## Khách hàng

Customer-logos section — **reuses the homepage "Khách hàng" component** (`Frame 62` / `Frame 494` instance). Only the outer section padding/heading position differ slightly from the homepage placement; the marquee/grid mechanics, logo list, and full CSS are already specced in `design-specs/home/03-about-and-customers.md` → section "## Khách hàng". Do not re-derive the logo grid/marquee here — implement as a shared partial/component (`_CustomerLogos.cshtml`) and include it on both Home and About Us pages.

### Desktop

**Section** — `792:24977`, width `1920px`, height `644px`, background `#fff` (light section, continues from "Đội ngũ").
- Heading "Khách hàng của chúng tôi" (note: sentence case in this page's copy vs. homepage's "KHÁCH HÀNG CỦA CHÚNG TÔI" all-caps string — both render visually identical because of `text-transform:uppercase` in the style, so keep markup as-is and let CSS uppercase it): positioned `80px` from section top, centered, full width. Style: `font-family:'LC Sac Trial'; font-weight:400; font-size:48px; line-height:56px; text-transform:uppercase; color:#000; text-align:center;`
- Logo strip instance starts at `216px` from section top, `1920×348px` — reuse `.customers-marquee` component from home spec verbatim.

```css
.about-customers{background:#fff;padding:80px 0 96px;text-align:center;}
.about-customers__heading{font-family:'LC Sac Trial',serif;font-weight:400;font-size:48px;line-height:56px;text-transform:uppercase;color:#000;margin:0 0 56px;}
/* .customers-marquee / .customers-track / .logo-card — reuse from design-specs/home/03-about-and-customers.md */
```

### Mobile

**Section** — `1068:36608`, width `430px`, height `~333px`.
- Heading: `40px` from top, `font-size:20px; line-height:30px;` same family/weight/color/uppercase as desktop, centered.
- Logo grid instance below at `78px` from top, height `~215px` — reuse the mobile static 3×3 grid variant from the home spec (`.customers-marquee` grid override), same greyscale logo treatment.

```css
@media (max-width:767px){
  .about-customers{padding:40px 16px;}
  .about-customers__heading{font-size:20px;line-height:30px;margin-bottom:24px;}
}
```

---

## Cross-page notes / open items

1. **Dark background exact hex**: the hero + brand-foundation dark panel's solid fill color isn't exposed as a plain hex by the Figma API in this pass (only text-color tokens like grey-50/grey-100 were resolvable). Sample the screenshot or check the frame's fill in the Figma UI directly before final CSS lock; `#232323` used as a placeholder in this spec.
2. **Ghost "01" labels**: in the "Nền tảng thương hiệu" desktop heading rows, a duplicate "01" text node exists at `opacity:0` next to each column title (Figma authoring artifact/leftover). Omit from Razor markup.
3. **Đội ngũ collage**: currently pure placeholder rectangles (`rgba(0,0,0,0.2)`), no names/roles/captions present in this Figma pass — confirm with client whether individual team-member captions are needed before hard-coding as decorative-only images.
4. **Customer logos component**: reuse as a shared partial across Home and About Us — see `design-specs/home/03-about-and-customers.md` for the full logo list, marquee CSS, and open questions (static grid vs. animated marquee) that still need client confirmation.
5. **Sticky nav active-state wiring**: needs scroll-spy JS (IntersectionObserver per section id: `#khoi-dau`, `#nen-tang`, `#doi-ngu`, `#khach-hang`) to toggle `.is-active` and smooth-scroll on click.
