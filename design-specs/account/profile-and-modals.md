# Account Info / Profile — Design Spec

Figma file `uQFY9gwfNbNSeTM6zmspzo`. Source nodes:
- Desktop: `848:20065` (1925w)
- Mobile: `1146:41850` (430w)
- Modal – Họ và tên (text): `1265:39448` (461x220)
- Modal – Giới tính (radio): `1265:39450` (461x244)
- Modal – Ngày sinh (date): `1265:39485` (461x220)
- Modal – Email: `1265:39520` (461x220)

## Design Tokens

| Token | Value |
|---|---|
| Brand red (deep) | `#410C13` (used for CTA buttons, active nav "Đăng xuất", avatar initial text) |
| Brand red 2 (link/nav) | `#AF2234` |
| Brand gold/tan | `#AA8656` (not directly present in these nodes; reserve for other pages) |
| Grey 50 | `#F2F2F2` (page background, secondary "Hủy" button bg) |
| Grey 300 | `#BFBFBF` (borders — header divider, card row dividers, input borders, avatar bg) |
| Grey 400 | `#9B9B9B` (sub-divider under header, unfilled radio border) |
| Grey 500 | `#7A7A7A` (inactive nav items) |
| Grey 600 | `#5A5A5A` (floating input label / caption text) |
| Grey 700 | `#3C3C3C` |
| Grey 800 | `#1C1C1C` (selected radio ring + dot) |
| Grey 900 | `#0F0F0F` (primary text, active nav item, field values) |
| Black | `#000000` (row label / page heading text) |
| White | `#FFFFFF` (card/modal background, button text on red) |

Typography (font-family "Josefin Sans" unless noted):
- Heading 4 (mobile logo badge etc.): Bold 20/28
- Desktop Heading 4 (side-nav items): SemiBold 24/32
- Subtitle ("Thông tin cá nhân" section title, modal Hủy/Lưu button label): Medium 20/23
- Label (row value text, field values, input filled value, radio option text): Medium 16/20
- Body 2 (row label text "Họ và tên", "Giới tính", etc., date placeholder): Light 16/18
- Caption (input floating/top label inside text fields): Regular 13/16

Large display headings ("LC Sac Trial") are not used within this Profile screen/modals scope — reserve token for hero/marketing sections elsewhere.

## Page Chrome (both breakpoints)

- Header: white bg, bottom border `#BFBFBF` 1px; contains logo + inner row with bottom border `#9B9B9B` 1px, and circular avatar badge (bg `#BFBFBF`, 40px desktop / 28px mobile, initial letter in `#410C13`, Bold).
- Page body background: `#F2F2F2`.
- Desktop: content centered, left Nav column (Josefin Sans SemiBold 24/32, gap 37px between items): "Lịch sử đơn hàng" (grey-500, inactive), "Thông tin tài khoản" (grey-900, active/current page), "Sổ địa chỉ" (grey-500), "Đăng xuất" (brand red `#AF2234`).
- Mobile: nav collapses to hamburger icon in header; no visible sidebar list on this frame (page shows only card content + "Thông tin cá nhân" H2 title, 20px Medium).

## Profile Field Rows

Card container: white background, `border-radius: 8px`, left padding `24px` (desktop) — no top/right/bottom padding (rows manage their own edges), full width, sits inside a `gap:16px` column below a "Thông tin cá nhân" subtitle (Medium 20/23, black).

Each row is `height: 56px`, `display:flex; align-items:center; justify-content:space-between`, bottom border `1px solid #BFBFBF` (omitted on the last row / Email row). Row content:
- Left: label text, Josefin Sans Light 16/18, color `#000000`, fixed width `240px` (desktop) — acts as a label column.
- Right: value group, `padding: 0 24px`, `gap: 8px`, `align-items:center`:
  - Value text: Josefin Sans Medium 16/20, color `#0F0F0F`, `white-space: nowrap`.
  - Edit chevron: 16x16 icon, `Xnix/Line/Down_Arrow_5` rotated `-90deg` (renders as a right-pointing chevron `>`), only present on editable rows (Họ và tên, Giới tính, Ngày sinh). Email row has **no** chevron/edit affordance (immutable in this design, or edited via different flow) — value only.

Row data (desktop = mobile, same values/pattern, mobile card full width with page horizontal padding `16px` instead of desktop `362px`):

| Row | Label | Value shown (filled) | Value shown (empty) | Editable |
|---|---|---|---|---|
| 1 | Họ và tên | "Phan Dương" | — | Yes → opens text modal |
| 2 | Giới tính | (gender value) | "Thêm thông tin" | Yes → opens radio modal |
| 3 | Ngày sinh | (date value) | "Thêm thông tin" | Yes → opens date modal |
| 4 | Email | "duong06.design@gmail.com" | — | No chevron shown on profile row (edit still available via separate "Email" modal spec, likely triggered elsewhere e.g. security settings) |

Desktop-specific container: `Thông tin cá nhân` main column has `padding: 40px 160px 10px 80px` inside the `960px`-tall content area, content max width driven by `flex:1`.
Mobile-specific: main column `padding: 20px 16px 10px 16px` roughly (page px 16px, inner block `pt-20 pb-10`).

### Row CSS reference (desktop + mobile identical, only outer padding differs)

```css
.profile-card {
  background: #fff;
  border-radius: 8px;
  padding-left: 24px;
  width: 100%;
}
.profile-row {
  height: 56px;
  display: flex;
  align-items: center;
  justify-content: space-between;
  border-bottom: 1px solid #BFBFBF;
}
.profile-row:last-child { border-bottom: none; }
.profile-row__label {
  font-family: 'Josefin Sans', sans-serif;
  font-weight: 300; /* Light */
  font-size: 16px;
  line-height: 18px;
  color: #000;
  width: 240px;
}
.profile-row__value-group {
  display: flex;
  align-items: center;
  gap: 8px;
  padding: 0 24px;
}
.profile-row__value {
  font-family: 'Josefin Sans', sans-serif;
  font-weight: 500; /* Medium */
  font-size: 16px;
  line-height: 20px;
  color: #0F0F0F;
  white-space: nowrap;
}
.profile-row__chevron {
  width: 16px;
  height: 16px;
  transform: rotate(-90deg); /* down-arrow icon rotated to point right */
}
```

## Edit Modals

Shared modal shell across all 4 field-edit modals (implement as one Razor partial `_EditFieldModal.cshtml`, parameterized by field type: `text`, `radio`, `date`, `email`):

- Container: white, `border-radius: 16px`, `padding: 0 20px 20px 20px` (no top padding — header row supplies it), width `461px` fixed on desktop (center-modal dialog), full-width minus margin on mobile.
- Header row: `height: 60px`, `display:flex; align-items:center; justify-content:space-between`, bottom border `1px solid #BFBFBF`.
  - Left: close (X) icon button, 32x32, `Xnix/Line/Cross`.
  - Center: title, Josefin Sans Medium 16/20, color `#0F0F0F` (e.g. "Họ và tên", "Giới tính", "Ngày sinh", "Email").
  - Right: an invisible/opacity-0 32x32 spacer (mirrors the close icon so title stays centered) — implement as an empty `<span class="w-8 h-8">` or just `justify-content:center` with absolute-positioned close icon; simplest CSS: `display:grid; grid-template-columns:32px 1fr 32px;` with close in col 1, title in col 2 centered, col 3 empty.
- Body (between header and footer): `gap: 16px` column, `padding-top: 16px` implicit via parent gap.
- Footer button row: `height: 48px`, `display:flex; gap:15px`, two buttons each `flex:1`, `padding:10px`, `border-radius:8px`, `align-items:center; justify-content:center`.
  - Cancel ("Hủy"): background `#F2F2F2`, text Josefin Sans Medium 20/23, color `#0F0F0F`. No border.
  - Save ("Lưu"): background `#410C13`, text Josefin Sans Medium 20/23, color `#FFFFFF`.
  - (Hover/disabled states not shown in Figma frame — recommend standard darken-10% on hover for Save, and disabled: `background:#BFBFBF; color:#7A7A7A; cursor:not-allowed`.)

### Shared modal shell CSS

```css
.edit-modal {
  background: #fff;
  border-radius: 16px;
  width: 461px;
  max-width: calc(100vw - 32px);
  padding: 0 20px 20px 20px;
  display: flex;
  flex-direction: column;
  gap: 20px;
}
.edit-modal__header {
  height: 60px;
  display: flex;
  align-items: center;
  justify-content: space-between;
  border-bottom: 1px solid #BFBFBF;
}
.edit-modal__close { width: 32px; height: 32px; cursor: pointer; }
.edit-modal__title {
  font-family: 'Josefin Sans', sans-serif;
  font-weight: 500;
  font-size: 16px;
  line-height: 20px;
  color: #0F0F0F;
}
.edit-modal__spacer { width: 32px; height: 32px; opacity: 0; }
.edit-modal__body { display: flex; flex-direction: column; gap: 16px; width: 100%; }
.edit-modal__actions { display: flex; gap: 15px; height: 48px; width: 100%; }
.edit-modal__btn {
  flex: 1;
  height: 100%;
  padding: 10px;
  border-radius: 8px;
  border: none;
  display: flex; align-items: center; justify-content: center;
  font-family: 'Josefin Sans', sans-serif;
  font-weight: 500;
  font-size: 20px;
  line-height: 23px;
  cursor: pointer;
}
.edit-modal__btn--cancel { background: #F2F2F2; color: #0F0F0F; }
.edit-modal__btn--save   { background: #410C13; color: #fff; }
.edit-modal__btn--save:disabled { background: #BFBFBF; color: #7A7A7A; cursor: not-allowed; }
```

### 1. Text field modal — "Họ và tên" (node 1265:39448, 461x220)

Floating-label text input, filled state:
- Input container: `56px` height, `1px solid #BFBFBF` border, `border-radius:8px`, `padding: 0 16px`, background white, column layout with vertical centering.
- Top line: small label "Họ và tên" — Josefin Sans Regular 13/16, color `#5A5A5A` (acts as floating label, always visible above value once filled).
- Below: value text "Phan Dương" — Josefin Sans Medium 16/20, color `#0F0F0F`.
- Equivalent to a standard Material-style floating label input; when empty, likely label centers as placeholder (not captured in this frame — assume standard floating-label behavior: label floats up on focus/filled, sits as placeholder at 16px Medium `#5A5A5A`-ish when empty).

```css
.field-input {
  width: 100%;
  border: 1px solid #BFBFBF;
  border-radius: 8px;
  padding: 8px 16px;
  background: #fff;
  display: flex; flex-direction: column; justify-content: center;
  min-height: 56px;
}
.field-input__label {
  font-family: 'Josefin Sans', sans-serif;
  font-weight: 400;
  font-size: 13px;
  line-height: 16px;
  color: #5A5A5A;
}
.field-input__value {
  font-family: 'Josefin Sans', sans-serif;
  font-weight: 500;
  font-size: 16px;
  line-height: 20px;
  color: #0F0F0F;
  border: none;
  outline: none;
  width: 100%;
  background: transparent;
}
.field-input:focus-within { border-color: #1C1C1C; } /* recommended focus state, not in source frame */
```

Razor: `<input type="text" name="FullName" class="field-input__value" value="Phan Dương" />` wrapped in `.field-input` with a `<label>` reading "Họ và tên" rendered above per floating-label pattern (label element or CSS `:placeholder-shown` trick).

### 2. Radio modal — "Giới tính" (node 1265:39450, 461x244)

Two stacked options, no fieldset border, `width: 65px` intrinsic (icons+label), each option row `height: 40px`, `gap: 8px`, vertically stacked (no gap value defined between the two rows beyond row height — treat as `gap: 0` column, rows self-contained at 40px each).

- Radio control: 20x20 circular button.
  - **Selected** ("Nam" in mock): outer ring `border: 1px solid #1C1C1C`, `border-radius: 23px` (effectively circle), inner filled dot 12x12 `background:#1C1C1C`, `border-radius:32px`, centered via `padding:10px` on the 20px button (dot rendered via inner div).
  - **Unselected** ("Nữ"): outer ring `border: 1px solid #9B9B9B`, no inner dot, `border-radius: 23px`.
- Option label: Josefin Sans Medium 16/20, color `#0F0F0F`, immediately right of the radio control.

```css
.radio-group { display: flex; flex-direction: column; }
.radio-option { display: flex; align-items: center; gap: 8px; height: 40px; }
.radio-control {
  width: 20px; height: 20px;
  border-radius: 50%;
  border: 1px solid #9B9B9B; /* unselected */
  display: flex; align-items: center; justify-content: center;
  cursor: pointer;
}
.radio-control.is-selected { border-color: #1C1C1C; }
.radio-control.is-selected::after {
  content: '';
  width: 12px; height: 12px;
  border-radius: 50%;
  background: #1C1C1C;
}
.radio-option__label {
  font-family: 'Josefin Sans', sans-serif;
  font-weight: 500;
  font-size: 16px;
  line-height: 20px;
  color: #0F0F0F;
}
```

Razor: two `<label class="radio-option">` wrapping `<input type="radio" name="Gender" value="Nam"/ >` (visually hidden, styled via `.radio-control` sibling span) + "Nam" / "Nữ" text.

### 3. Date modal — "Ngày sinh" (node 1265:39485, 461x220)

Single input row, `height: 56px`, `border: 1px solid #BFBFBF`, `border-radius: 8px`, `padding: 0 16px`, `gap: 10px`, flex row (not column like text input — no floating label captured, this is placeholder-only state):
- Text/placeholder: "DD/MM/YYYY", Josefin Sans Light 16/18, color `#000000` (placeholder styling; once a date is entered it likely displays similarly or switches to Medium 16/20 `#0F0F0F` consistent with other filled fields).
- Right-aligned calendar icon: 24x24 (`Outline / Time / Calendar`), clickable to open native/date-picker.
- Error text: spec mentions error text under the field (not present in this filled/default frame capture) — reserve a `.field-error` element below the input: Josefin Sans Regular ~13px, color brand red `#AF2234` or `#410C13`, e.g. "Ngày sinh không hợp lệ".

```css
.date-input {
  width: 100%;
  height: 56px;
  border: 1px solid #BFBFBF;
  border-radius: 8px;
  padding: 0 16px;
  display: flex;
  align-items: center;
  gap: 10px;
}
.date-input__text {
  flex: 1;
  border: none; outline: none; background: transparent;
  font-family: 'Josefin Sans', sans-serif;
  font-weight: 300; /* Light placeholder */
  font-size: 16px;
  line-height: 18px;
  color: #000;
}
.date-input__icon { width: 24px; height: 24px; flex-shrink: 0; cursor: pointer; }
.date-input.has-error { border-color: #AF2234; }
.field-error {
  font-family: 'Josefin Sans', sans-serif;
  font-size: 13px;
  line-height: 16px;
  color: #AF2234;
}
```

Razor: `<input type="text" inputmode="numeric" placeholder="DD/MM/YYYY" name="DateOfBirth" class="date-input__text" />` + calendar icon button that triggers a JS/flatpickr-style date picker; `<span class="field-error" asp-validation-for="DateOfBirth"></span>` below `.date-input`.

### 4. Email modal — "Email" (node 1265:39520, 461x220)

Identical floating-label pattern to the text (Họ và tên) modal:
- `.field-input` container, label "Email" (13/16, `#5A5A5A`), value "Duong06.design@gmail.com" (Medium 16/20, `#0F0F0F`).
- Use `<input type="email">` for native validation; same CSS classes as text modal (`.field-input`, `.field-input__label`, `.field-input__value`).

## Implementation Notes for Razor

- Single partial `_EditFieldModal.cshtml` accepting a view model: `{ FieldKey, Title, FieldType ("text"|"radio"|"date"|"email"), CurrentValue, Options (for radio) }`. Render body via `<partial name="_EditField_{FieldType}" model="..." />` sub-partials, or an `if/switch` on `FieldType` inside one file — either works; shared shell/header/footer markup stays in the parent partial.
- All 4 modals share `.edit-modal`, header, and `.edit-modal__actions` footer markup/CSS — only the body content (`.edit-modal__body`) differs per field type as documented above.
- Profile page itself: `Account/Profile.cshtml` renders `.profile-card` with 4 `.profile-row`s; each editable row's chevron/value area is a button/link (`data-modal-target="#edit-fullname-modal"` etc.) that opens the corresponding modal (Bootstrap modal, or custom JS dialog — match whatever modal mechanism the rest of the site already uses).
- Email row on the profile page has no chevron per the Figma capture — confirm with design whether email is meant to be editable from this page at all before wiring an edit trigger; the Email modal (node 1265:39520) exists in Figma regardless, so keep the partial available even if not directly linked from the row.
- Responsive breakpoint: switch page horizontal padding from `362px` (desktop container) down to `16px` (mobile ≤430px design), and hide the desktop sidebar Nav list in favor of a hamburger-triggered menu at ≤768px (exact breakpoint not specified in Figma; recommend standard `768px` tablet/mobile cutover consistent with rest of site).
