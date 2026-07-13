# Home — "Về Hoài" & "Khách hàng" Sections

Source: Figma file `uQFY9gwfNbNSeTM6zmspzo`
Nodes: Desktop "Về Hoài" `1214:38905`, Mobile "Về Hoài" `1062:12749`, Desktop "Khách hàng" `1214:38910`, Mobile "Khách hàng" `1062:12754`

Design tokens used in this spec:
- Brand gold `--brandcolor-1: #AA8656`
- Yellow-700 (subtitle text): `#795F3D`
- Grey-300 (image placeholder fill): `#BFBFBF`
- Heading font: `LC Sac Trial`, Regular
- Body/label font: `Josefin Sans` (Regular/Medium)

---

## Về Hoài

### Desktop

**Section container** (`#about`, full-bleed, 1920px design width)
- `display:flex; flex-direction:column; align-items:center; gap:48px;`
- `padding: 120px 0;`
- `background:#fff;`

**Heading block** — width `1118px`, centered, `flex-direction:column; gap:16px; align-items:center; text-align:center`
- H2 "VỀ HOÀI": font `LC Sac Trial`, Regular, `48px/56px`, color `#000`
- Subtitle paragraph: font `Josefin Sans`, Medium 500, `20px/23px`, color `#795F3D`, width `1118px`

**Feature card row** (masonry-style, 4 columns)
- Wrapper: `display:flex; gap:16px; align-items:center; justify-content:center; padding-inline:240px;` (content width 1440px on 1920 canvas), `position: sticky; top: 0;` (design has `sticky` — verify if intentional scroll-pin effect before implementing; otherwise treat as static row)
- 4 children, each `flex:1 0 0; min-width:0; display:flex; flex-direction:column; overflow:hidden; border-radius:8px;`
- Checkerboard order — odd columns (1,3): **image on top, label below**; even columns (2,4): **label on top, image below**
  - Image block: `background:#BFBFBF; height:480px; width:100%; border-radius:8px;` (placeholder for photography)
  - Label block: `height:208px; display:flex; align-items:center; justify-content:center;`
    - Background gradient: top-image variant `linear-gradient(to bottom, #fff 0%, #FBEECD 100%)`; bottom-image variant reversed `linear-gradient(to bottom, #FBEECD 0%, #fff 100%)`
    - Text: font `LC Sac Trial` Regular, `48px/56px`, color `#AA8656`, centered, 2 lines, `white-space: normal` (wraps to 2 short lines, e.g. "Tính bản sắc")
- Column captions (in order): "Tính bản sắc", "Sự tinh tế", "Tư duy khởi sinh", "Sự Tiếp nối"

**Razor/CSS notes**
```html
<section class="about-section">
  <div class="about-heading">
    <h2>VỀ HOÀI</h2>
    <p class="about-subtitle">Khởi nguồn từ tình yêu dành cho di sản Việt Nam...</p>
  </div>
  <div class="about-cards">
    <div class="about-card about-card--img-top">
      <div class="about-card__img" style="background-image:url(...)"></div>
      <div class="about-card__label about-card__label--to-gold"><span>Tính bản sắc</span></div>
    </div>
    <div class="about-card about-card--img-bottom">
      <div class="about-card__label about-card__label--from-gold"><span>Sự tinh tế</span></div>
      <div class="about-card__img"></div>
    </div>
    <!-- repeat for cards 3 & 4 -->
  </div>
</section>
```
```css
.about-section{display:flex;flex-direction:column;align-items:center;gap:48px;padding:120px 0;background:#fff;}
.about-heading{display:flex;flex-direction:column;gap:16px;align-items:center;text-align:center;max-width:1118px;margin:0 auto;}
.about-heading h2{font-family:'LC Sac Trial',serif;font-size:48px;line-height:56px;font-weight:400;color:#000;margin:0;}
.about-subtitle{font-family:'Josefin Sans',sans-serif;font-size:20px;line-height:23px;font-weight:500;color:#795F3D;margin:0;}
.about-cards{display:flex;gap:16px;align-items:center;justify-content:center;padding-inline:240px;width:100%;max-width:1920px;margin:0 auto;}
.about-card{flex:1 1 0;min-width:0;display:flex;flex-direction:column;overflow:hidden;border-radius:8px;}
.about-card__img{background:#BFBFBF;height:480px;width:100%;border-radius:8px;background-size:cover;background-position:center;}
.about-card__label{height:208px;display:flex;align-items:center;justify-content:center;text-align:center;}
.about-card__label span{font-family:'LC Sac Trial',serif;font-size:48px;line-height:56px;color:#AA8656;}
.about-card__label--to-gold{background:linear-gradient(to bottom,#fff 0%,#FBEECD 100%);}
.about-card__label--from-gold{background:linear-gradient(to bottom,#FBEECD 0%,#fff 100%);}
```

### Mobile

**Section container**
- `display:flex; flex-direction:column; align-items:center; gap:16px;`
- `padding: 40px 16px;`
- `background:#fff;`

**Heading block** — full width, `flex-direction:column; gap:16px; align-items:center; text-align:center`
- H2 "VỀ HOÀI": `LC Sac Trial` Regular, `20px/30px`, color `#000`
- Subtitle: `Josefin Sans` Regular 400, `13px/16px`, color `#795F3D`, full width

**Feature card grid** — container `width:100%; max-width:430px; height:378px; display:flex; flex-direction:column; gap:16px; padding-inline:16px;`
- 2 rows, each: `display:flex; flex:1 0 0; gap:16px; align-items:center; width:100%;` (row height splits container evenly, ~173px each)
- Row 1: col A = image (top, `flex:1; background:#BFBFBF; border-radius:8px`) + label (bottom, gradient white→gold, `padding-block:8px`, text "Tính bản sắc"); col B = label (top, gradient gold→white, "Sự tinh tế") + image (bottom, flex:1)
- Row 2: col A = image (top) + label (bottom, "Tư duy khởi sinh"); col B = label (top, "Sự Tiếp nối") + image (bottom)
- Label text: `Josefin Sans` Medium 500, `16px/20px`, color `#AA8656`, centered, single line
- Each column: `flex:1 0 0; min-width:0; display:flex; flex-direction:column; overflow:hidden; border-radius:8px; height:100%;`

**CSS additions for mobile breakpoint**
```css
@media (max-width: 767px){
  .about-section{padding:40px 16px;gap:16px;}
  .about-heading h2{font-size:20px;line-height:30px;}
  .about-subtitle{font-size:13px;line-height:16px;}
  .about-cards{flex-direction:column;padding-inline:16px;max-width:430px;height:378px;}
  .about-cards-row{display:flex;flex:1 0 0;gap:16px;width:100%;}
  .about-card__label{height:auto;padding-block:8px;}
  .about-card__label span{font-family:'Josefin Sans',sans-serif;font-size:16px;line-height:20px;font-weight:500;}
}
```

---

## Khách hàng

### Desktop

**Section container** — full-bleed, `width:1920px design / 100% actual; height:724px (design);`
- Heading "KHÁCH HÀNG CỦA CHÚNG TÔI": positioned ~120px from section top, `LC Sac Trial` Regular, `48px/56px`, color `#000`, centered, full width
- Logo strip: starts ~256px from section top, full width, height `348px` — **3 rows** of partner logo cards
  - Each logo card: `width:200px; height:100px;` fixed, logo image centered within card, letterboxed (object-fit: contain)
  - Logos rendered in flat greyscale/mono tone (desaturated), consistent with a "trusted-by" style treatment — implement via `filter: grayscale(100%); opacity:.7;` on `<img>`, restore full color on `:hover` for a nice touch (optional, not confirmed in design)
  - Screenshot shows the first/last logo in each row partially cropped and fading at the strip edges → indicates a **continuous horizontal auto-scroll ticker per row** (infinite marquee), not a static clipped grid. Recommend implementing as a CSS `@keyframes` marquee (`translateX` loop, duplicated logo list) with edge fade via `mask-image: linear-gradient(to right, transparent 0, black 5%, black 95%, transparent 100%)`.
  - ~28 client logos identified in the source file (Trường Thành, Bondex, Nano Gold, Pro Group, Jaguar, Bee Machine Vision, Core5, CyStack, Hana HP Group, MB, Nature Hotel, Avalue, Pancake, Ecomdy, King Power, iSofh, OnPoint, Everest Materials, Bình Minh HP, TopCV, Saquila Yacht, DHT Invimex, Koni, Prime, and a few more) — arrange across the 3 rows, repeat the list to fill the marquee loop seamlessly.

**Razor/CSS notes**
```html
<section class="customers-section">
  <h2 class="customers-heading">KHÁCH HÀNG CỦA CHÚNG TÔI</h2>
  <div class="customers-marquee">
    <div class="customers-row">
      <div class="customers-track">
        <div class="logo-card"><img src="..." alt="Trường Thành" /></div>
        <!-- ...repeat logos, then duplicate full set again for seamless loop... -->
      </div>
    </div>
    <!-- 2 more .customers-row for row 2 & 3, ideally reverse direction on alternating rows -->
  </div>
</section>
```
```css
.customers-section{width:100%;padding:120px 0;background:#fff;text-align:center;}
.customers-heading{font-family:'LC Sac Trial',serif;font-size:48px;line-height:56px;color:#000;margin:0 0 64px;}
.customers-marquee{display:flex;flex-direction:column;gap:24px;overflow:hidden;
  mask-image:linear-gradient(to right,transparent 0,#000 5%,#000 95%,transparent 100%);}
.customers-track{display:flex;gap:24px;width:max-content;animation:marquee 40s linear infinite;}
.customers-row:nth-child(even) .customers-track{animation-direction:reverse;}
.logo-card{width:200px;height:100px;display:flex;align-items:center;justify-content:center;flex-shrink:0;}
.logo-card img{max-width:140px;max-height:64px;object-fit:contain;filter:grayscale(100%);opacity:.7;transition:filter .2s,opacity .2s;}
.logo-card img:hover{filter:none;opacity:1;}
@keyframes marquee{from{transform:translateX(0);}to{transform:translateX(-50%);}}
```

### Mobile

**Section container** — `width:100%; max-width:430px;`
- Heading "KHÁCH HÀNG CỦA CHÚNG TÔI": `LC Sac Trial` Regular, `20px/30px`, color `#000`, centered, full width, positioned ~40px from top
- Logo grid: container `height:~215px`, horizontally centered but **wider than viewport** (design shows a 1324px-wide component offset by −447px inside a 430px frame — i.e. `overflow:hidden` clipping a larger shared strip down to a centered window)
  - Visually resolves to a static **3 columns × 3 rows grid** of logos on mobile (9 logos visible: Pro Group, Jaguar, Bee Machine Vision, Avalue, Pancake, Ecomdy, TopCV, Saquila Yacht, DHT Invimex), no visible fade/crop artifacts
  - Recommend implementing mobile simply as a static CSS grid (no animation) for simplicity/perf, reusing the same greyscale logo treatment

**CSS**
```css
@media (max-width: 767px){
  .customers-section{padding:40px 16px;}
  .customers-heading{font-size:20px;line-height:30px;margin-bottom:24px;}
  .customers-marquee{display:grid;grid-template-columns:repeat(3,1fr);gap:16px 8px;overflow:visible;mask-image:none;}
  .customers-track{display:contents;animation:none;}
  .logo-card{width:auto;height:70px;}
  .logo-card img{max-width:100px;max-height:44px;}
}
```

---

## Open questions to confirm before implementation
1. Is the `sticky top:0` on the desktop "Về Hoài" card row intentional (scroll-pin effect) or a Figma export artifact? Verify against live prototype/animation notes before coding.
2. Desktop "Khách hàng" logo strip — confirm whether it should truly auto-scroll (marquee) in production, or if the fade/crop in the screenshot is only a Figma canvas-clipping artifact of a static grid. If static is preferred, drop the `@keyframes marquee` and lay out a fixed grid instead.
3. Full ordered list + logo asset files for all ~28 partner logos (only partially enumerable from the screenshot; get final list/assets from the client).
