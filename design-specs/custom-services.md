# Dịch vụ theo yêu cầu — Design Spec

Figma file: `uQFY9gwfNbNSeTM6zmspzo`
Component: "Dịch vụ theo yêu cầu" (tabbed section, homepage instance)
Desktop frame: 1920×938 (`px-240 py-120`, bg `#F7F3EE`)
Mobile frame: 430×741 (`px-16 py-40`, bg `#F7F3EE`)

Node IDs referenced:
- Desktop: In khắc `1234:40239`, Lựa chọn gói quà `1234:40241`, Thiết kế `1234:40290`
- Mobile: In khắc `1257:37267`, Lựa chọn gói quà `1259:37508`, Thiết kế `1259:37555`

---

## Tokens used

| Token | Hex |
|---|---|
| Brand red (`--brand-color-2`) | `#AF2234` |
| Red-50 (light label on red) | `#F7E9EB` |
| Red-400 (icon bubble on active tab) | `#BF4E5D` |
| Red-900 (CTA button text) | `#410C13` |
| Grey-50 (inactive icon bubble bg) | `#F2F2F2` |
| Grey-400 (inactive tab label) | `#9B9B9B` |
| Grey-500 (eyebrow label) | `#7A7A7A` |
| Yellow-50 / section bg | `#F7F3EE` |
| Panel placeholder image bg | `#C4C4C4` |
| Panel gradient overlay | `rgba(159,31,47,0)` → `#9F1F2F` (linear, bottom-up) |
| Panel caption text | `#F2F2F2` (or `#F7E9EB` on some tabs) |

Fonts:
- Eyebrow label ("Dịch vụ theo yêu cầu") & tab labels & CTA & panel caption: **Josefin Sans**, SemiBold 600 desktop / Medium 500 mobile.
- Big headline ("CÁ NHÂN HÓA SẢN PHẨM ĐỂ MANG DẤU ẤN CỦA RIÊNG BẠN"): **LC Sac Trial**, Regular, uppercase, black.

---

## Tab Switcher

### Structure
A 2-column layout (desktop) / stacked layout (mobile):
1. Left/top: eyebrow label + headline (shared, static across tabs) + a vertical stack of 3 tab buttons.
2. Right/bottom: one content panel (image + gradient + caption text + CTA) that swaps per active tab.

Only one component instance renders at a time; `property1` variant = active tab name (`"In khắc"`, `"Lựa chọn gói quà"`, `"Thiết kế"`). No client-side animation is defined in Figma — treat tab switch as an instant state swap (content panel image/caption/CTA text change, active tab pill restyle).

### Tab button — Desktop
- Container: `width: 480px`, `padding: 16px`, `border-radius: 40px`, `display:flex; align-items:center; gap:24px`.
- **Active**: `background:#AF2234`; icon bubble `background:#BF4E5D`, `size:68px`, `border-radius:39.95px` (i.e. ~50%), icon glyph white/red-50 line icon centered; label `color:#F7E9EB`, Josefin Sans SemiBold 24px/32px.
- **Inactive**: `background:#FFFFFF`; icon bubble `background:#F2F2F2`, same 68px circle; label `color:#9B9B9B`, same typography; `cursor:pointer`.
- Icons (24px design, ~51px/30px glyph inside bubble): In khắc = wine-glass/engraving line icon; Lựa chọn gói quà = gift line icon; Thiết kế = notepad-edit line icon (mirrored/flipped in source — just use a normal "edit/notepad" icon, no need to replicate the flip transform).

### Tab button — Mobile
Same states/colors, scaled down:
- Container: `width:100%`, `padding:8px`, `border-radius:20px`, `gap:8px`.
- Icon bubble: `size:40px`, `border-radius:23.5px` (~50%).
- Label: Josefin Sans Medium 16px/20px.
- Note: in the raw Figma export the inactive "Thiết kế ấn phẩm" button on the "In khắc" mobile variant carries a stray `opacity:0.8` not present on the "Lựa chọn gói quà" inactive button — treat this as a Figma inconsistency; implement all inactive tab buttons identically (`opacity:1`, white bg, grey-400 label).

### Suggested markup (razor partial, `_CustomServices.cshtml`)
```html
<section class="custom-services" data-component="custom-services">
  <div class="custom-services__inner">
    <div class="custom-services__col">
      <header class="custom-services__heading">
        <p class="custom-services__eyebrow">Dịch vụ theo yêu cầu</p>
        <h2 class="custom-services__title">
          CÁ NHÂN HÓA SẢN PHẨM<br />
          ĐỂ MANG DẤU ẤN CỦA RIÊNG BẠN
        </h2>
      </header>

      <div class="custom-services__tabs" role="tablist">
        <button type="button" class="custom-services__tab is-active"
                role="tab" aria-selected="true" data-tab="in-khac">
          <span class="custom-services__tab-icon"><!-- icon: engraving --></span>
          <span class="custom-services__tab-label">In khắc logo cá nhân</span>
        </button>
        <button type="button" class="custom-services__tab"
                role="tab" aria-selected="false" data-tab="goi-qua">
          <span class="custom-services__tab-icon"><!-- icon: gift --></span>
          <span class="custom-services__tab-label">Lựa chọn gói quà</span>
        </button>
        <button type="button" class="custom-services__tab"
                role="tab" aria-selected="false" data-tab="thiet-ke">
          <span class="custom-services__tab-icon"><!-- icon: notepad-edit --></span>
          <span class="custom-services__tab-label">Thiết kế ấn phẩm</span>
        </button>
      </div>
    </div>

    <div class="custom-services__panel">
      <!-- one panel per tab, toggled via .is-active; or a single panel whose img/text/href swap via JS -->
      <div class="custom-services__panel-item is-active" data-panel="in-khac">
        <img class="custom-services__panel-img" src="/images/services/in-khac.jpg" alt="In khắc logo cá nhân" />
        <div class="custom-services__panel-overlay">
          <p class="custom-services__panel-caption">Cá nhân hóa sản phẩm bằng logo, tên riêng của bạn.</p>
          <a href="#" class="custom-services__cta">
            <span>Bắt đầu</span>
            <svg class="custom-services__cta-icon"><!-- arrow-right --></svg>
          </a>
        </div>
      </div>
      <div class="custom-services__panel-item" data-panel="goi-qua">…</div>
      <div class="custom-services__panel-item" data-panel="thiet-ke">…</div>
    </div>
  </div>
</section>
```

### CSS-ready values (desktop)
```css
.custom-services {
  background: #F7F3EE;
  padding: 120px 240px;
}
.custom-services__inner {
  display: flex;
  gap: 80px;
  align-items: center;
  height: 676px;
}
.custom-services__col {
  flex: 1 0 0;
  display: flex;
  flex-direction: column;
  justify-content: space-between;
  height: 100%;
}
.custom-services__eyebrow {
  font-family: "Josefin Sans", sans-serif;
  font-weight: 600;
  font-size: 24px;
  line-height: 32px;
  color: #7A7A7A;
}
.custom-services__title {
  font-family: "LC Sac Trial", serif;
  font-weight: 400;
  font-size: 48px;
  line-height: 60px;
  color: #000;
  text-transform: uppercase;
}
.custom-services__tabs {
  display: flex;
  flex-direction: column;
  gap: 24px;
  width: 480px;
}
.custom-services__tab {
  display: flex;
  align-items: center;
  gap: 24px;
  padding: 16px;
  width: 480px;
  border-radius: 40px;
  background: #fff;
  border: none;
  cursor: pointer;
  text-align: left;
}
.custom-services__tab.is-active { background: #AF2234; }
.custom-services__tab-icon {
  display: flex;
  align-items: center;
  justify-content: center;
  width: 68px;
  height: 68px;
  border-radius: 50%;
  background: #F2F2F2;
}
.custom-services__tab.is-active .custom-services__tab-icon { background: #BF4E5D; }
.custom-services__tab-label {
  font-family: "Josefin Sans", sans-serif;
  font-weight: 600;
  font-size: 24px;
  line-height: 32px;
  color: #9B9B9B;
}
.custom-services__tab.is-active .custom-services__tab-label { color: #F7E9EB; }

.custom-services__panel {
  flex: 1 0 0;
  height: 100%;
  border-radius: 16px;
  overflow: hidden;
  position: relative;
  background: #C4C4C4;
}
.custom-services__panel-img {
  position: absolute; inset: 0;
  width: 100%; height: 100%;
  object-fit: cover;
}
.custom-services__panel-overlay {
  position: absolute;
  left: 0; bottom: 0; width: 680px;
  display: flex; align-items: center; justify-content: center; gap: 40px;
  padding: 64px 40px;
  background: linear-gradient(to bottom, rgba(159,31,47,0) 0%, #9F1F2F 100%);
}
.custom-services__panel-caption {
  font-family: "Josefin Sans", sans-serif;
  font-weight: 600;
  font-size: 24px;
  line-height: 32px;
  color: #F2F2F2; /* or #F7E9EB per tab, see below */
  width: 374px;
}
.custom-services__cta {
  display: flex;
  align-items: center;
  gap: 8px;
  height: 80px;
  padding: 16px 40px;
  border-radius: 40px;
  background: #fff;
  text-decoration: none;
}
.custom-services__cta span {
  font-family: "Josefin Sans", sans-serif;
  font-weight: 600;
  font-size: 24px;
  line-height: 32px;
  color: #410C13;
}
.custom-services__cta-icon { width: 40px; height: 40px; }
```

### CSS-ready values (mobile, ≤430px — adjust to real breakpoint e.g. `@media (max-width:767px)`)
```css
.custom-services { padding: 40px 16px; }
.custom-services__inner { flex-direction: column; gap: 20px; height: auto; align-items: stretch; }
.custom-services__col { height: auto; gap: 24px; }
.custom-services__heading { text-align: center; gap: 8px; }
.custom-services__eyebrow { font-size: 16px; line-height: 20px; font-weight: 500; }
.custom-services__title { font-size: 20px; line-height: 30px; }
.custom-services__tabs { width: 100%; gap: 8px; }
.custom-services__tab { width: 100%; padding: 8px; border-radius: 20px; gap: 8px; }
.custom-services__tab-icon { width: 40px; height: 40px; border-radius: 50%; }
.custom-services__tab-label { font-size: 16px; line-height: 20px; font-weight: 500; }
.custom-services__panel { height: 345px; width: 100%; }
.custom-services__panel-overlay {
  flex-direction: column; gap: 20px; width: 100%; padding: 16px 40px;
}
.custom-services__panel-caption { font-size: 16px; line-height: 20px; font-weight: 500; text-align: center; width: auto; max-width: 374px; }
.custom-services__cta { height: auto; padding: 8px 16px; gap: 4px; }
.custom-services__cta span { font-size: 16px; line-height: 20px; font-weight: 500; }
.custom-services__cta-icon { width: 24px; height: 24px; }
```

### JS tab-switching behavior
Minimal vanilla JS, no framework dependency — attach to the partial's root or a shared site script:

```js
(function () {
  document.querySelectorAll('[data-component="custom-services"]').forEach(function (root) {
    var tabs = root.querySelectorAll('.custom-services__tab');
    var panels = root.querySelectorAll('.custom-services__panel-item');

    tabs.forEach(function (tab) {
      tab.addEventListener('click', function () {
        var key = tab.getAttribute('data-tab');

        tabs.forEach(function (t) {
          t.classList.toggle('is-active', t === tab);
          t.setAttribute('aria-selected', t === tab ? 'true' : 'false');
        });
        panels.forEach(function (p) {
          p.classList.toggle('is-active', p.getAttribute('data-panel') === key);
        });
      });
    });
  });
})();
```
CSS pairs with this by showing only `.custom-services__panel-item.is-active` (`display:none` otherwise, or stack with `position:absolute` + opacity fade if a transition is wanted — Figma defines no transition, so a simple instant swap or a 150–200ms opacity crossfade both match intent).

---

## In khắc (Engraving) — active by default

### Desktop
- Tab pill: active state (red `#AF2234` bg, red-400 icon bubble, engraving/wine-glass icon).
- Panel image: generic product/engraving placeholder photo (grey `#C4C4C4` placeholder in Figma — swap for real photography), gradient overlay `rgba(159,31,47,0)→#9F1F2F`.
- Caption: "Cá nhân hóa sản phẩm bằng logo, tên riêng của bạn." — color `#F2F2F2`, Josefin Sans SemiBold 24/32, width 374px.
- CTA: white pill button, "Bắt đầu" (`#410C13`, SemiBold 24/32) + arrow-right icon (40×40).

### Mobile
- Same tab active styling scaled (40px icon bubble, 16/20 label).
- Panel: 345px tall, full width, image + bottom gradient overlay, caption centered "Cá nhân hóa sản phẩm bằng logo, tên riêng của bạn." (16/20 Medium, `#F2F2F2`), CTA pill 24px arrow icon.

---

## Lựa chọn gói quà (Gift wrap selection)

### Desktop
- Tab pill becomes active (gift icon, red bg `#AF2234`, red-400 `#BF4E5D` icon bubble); other two tabs inactive white/grey-400.
- Panel caption changes to: "Tự do phối hợp gói quà theo sở thích và ngân sách" — note caption text color is `#F7E9EB` for this tab (slightly different from In khắc's `#F2F2F2`; both are visually near-white, use `#F7E9EB` for consistency with this tab's Figma value).
- Panel image/gradient/CTA structurally identical to In khắc (swap product photo to gift-wrap imagery).
- Minor Figma-only artifact: overlay `bottom` offset is `0.01px` instead of `0` — negligible, implement as `bottom:0`.

### Mobile
- Active tab: gift icon, red bg, 40px bubble, label `#F7E9EB`.
- Panel caption: "Tự do phối hợp gói quà theo sở thích và ngân sách", `#F7E9EB`, 16/20 Medium, centered.
- Same 345px panel height, gradient, CTA pill.

---

## Thiết kế (Custom design)

### Desktop
- Tab pill active: notepad-edit icon, red bg `#AF2234`, red-400 icon bubble; label "Thiết kế ấn phẩm".
- Panel caption: "Ấn phẩm đi kèm được thiết kế riêng, độc bản" — color `#F7E9EB`, SemiBold 24/32, width 374px.
- Panel/CTA structurally identical to the other two tabs (swap product photo to custom-design/print imagery).

### Mobile
- Active tab: notepad-edit icon, 40px red bubble, label `#F7E9EB`, full-width red pill (note: in this mobile variant the whole tab row item — icon bubble bg `#BF4E5D` + row bg `#AF2234` — matches the same active pattern as the other two tabs; treat consistently).
- Panel caption: "Ấn phẩm đi kèm được thiết kế riêng, độc bản", 16/20 Medium, `#F7E9EB`, centered.
- Same 345px panel height, gradient, CTA pill (note this variant's CTA row uses `gap:8px` vs `gap:4px` on other tabs in the raw export — treat as inconsistency, standardize to `gap:8px` for all mobile CTAs since 8px reads correctly against the 24px icon).

---

## Implementation notes for Razor partial

- Static copy (eyebrow, headline, tab labels) never changes across tabs — keep in the shared partial, not duplicated per tab.
- Per-tab dynamic data (panel image src/alt, caption text, caption color, CTA href) should come from a small view-model list, e.g.:
  ```csharp
  public class CustomServiceTabVM
  {
      public string Key { get; set; }          // "in-khac" | "goi-qua" | "thiet-ke"
      public string Label { get; set; }
      public string IconSvgPath { get; set; }
      public string PanelImageUrl { get; set; }
      public string Caption { get; set; }
      public string CaptionColorHex { get; set; } // #F2F2F2 or #F7E9EB
      public string CtaText { get; set; } = "Bắt đầu";
      public string CtaUrl { get; set; }
  }
  ```
- Render tabs + panels via `@foreach` over 3 items; first item gets `is-active` by default server-side (matches Figma default = "In khắc").
- Icons: source as inline SVG (engraving/wine-glass, gift, notepad-edit, arrow-right) sized 51×51 desktop / 30×30 mobile inside the icon bubble — export directly from Figma assets referenced in the raw extraction (imgGroup21/imgLinearEssentionalUiGift/imgGroup20/imgVector) or replace with an icon set already used elsewhere on the site (e.g. Feather/Lucide equivalents) for consistency.
- Panel background image is currently a grey placeholder (`#C4C4C4`) in Figma for all 3 tabs — real photography assets need to be supplied separately; use `object-fit:cover` to fill the 680px-wide desktop / full-width mobile panel without distortion.
