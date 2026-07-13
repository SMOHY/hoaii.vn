# Saved Addresses — Design Spec

Figma file `uQFY9gwfNbNSeTM6zmspzo`, page "Địa chỉ đã lưu".
Source nodes: Desktop `853:10989` (1925w frame, content column ~702px effective), Mobile `1272:54230` (430w).

**Scope note:** Only the "Thêm địa chỉ mới" (add new address) form was found in both scoped nodes. No saved-address **list** view (cards with edit/delete actions) exists in this part of the file. The list view will need to be designed by extension — reuse the same field/card visual language (white 8px-radius rows on `#F2F2F2` background, `Josefin Sans`, grey-500 borders/buttons) for address cards, each with an edit (pencil) and delete (trash) icon button plus a "default address" indicator, consistent with the account section's existing style. Flag this to the designer before hand-coding a list screen.

## Design Tokens Used On This Screen

**Colors**
| Token | Hex | Usage |
|---|---|---|
| grey-50 | `#F2F2F2` | Page/content background |
| grey-300 | `#BFBFBF` | Header border, avatar chip background |
| grey-400 | `#9B9B9B` | Inactive nav item text ("Lịch sử đơn hàng"), mobile header divider |
| grey-500 | `#7A7A7A` | Secondary nav text, input/button border |
| grey-600 | `#5A5A5A` | Floating (filled) label text |
| grey-700 | `#3C3C3C` | (token present in mobile frame; general body text) |
| grey-900 | `#0F0F0F` | Active nav item, filled field value text, button label |
| red-900 | `#410C13` | Avatar initial letter "D" |
| brand red | `#AF2234` | "Đăng xuất" (logout) nav link — CSS var `--brand-color-2` |
| white | `#FFFFFF` | Input/select field fill |
| black | `#000000` | Section heading, empty-state field label |

Brand gold `#AA8656` does **not** appear anywhere in this screen's extracted styles — do not force it in; reserve it for other pages (buttons/promo accents) unless a future frame shows it here.

**Typography** (all `Josefin Sans`)
| Style name | Weight | Size / line-height | Used for |
|---|---|---|---|
| Desktop Heading 4 | SemiBold 600 | 24px / 32px | Left sidebar nav items (desktop) |
| Common/Subtitle | Medium 500 | 20px / 23px | "Thêm địa chỉ mới" section heading |
| Common/Label | Medium 500 | 16px / 20px | Filled field value text; button label; avatar-chip letter (mobile scale) |
| Common/Body 2 | Light 300 | 16px / 18px | Empty-state field label (e.g. "Tỉnh thành phố *") |
| Common/Caption | Regular 400 | 13px / 16px | Floating label once field has a value, color `#5A5A5A` |
| Mobile Heading 4 | Bold 700 | 20px / 28px | Avatar initial glyph |

## Field Component: Floating-Label Input/Select

Both text inputs and dropdown selects share one visual container, two states:

**Container (both states)**
```css
background: #FFFFFF;
border-radius: 8px;
height: 56px;
padding: 0 16px;
display: flex;
flex-direction: column;
justify-content: center;
width: 100%;
```
No visible border in the mockup (relies on white-on-`#F2F2F2` contrast) and no box-shadow. For focus/error states (not shown in Figma) recommend adding a 1px border using brand red on `:invalid`/error and grey-400 on `:focus` since none is specified.

**State: Default / empty**
- Single line, label vertically centered: `font: 300 16px/18px "Josefin Sans"; color:#000`.
- A 16×16 chevron-down icon sits at the far right (flex `justify-content: space-between`) for **select** fields; icon is present but `opacity:0` (invisible) on the plain **text** fields (Họ và tên, Số điện thoại) — i.e. text inputs reserve the icon's space but hide it.

**State: Filled / has value** (floating label pattern)
- Small label on top: `font: 400 13px/16px "Josefin Sans"; color:#5A5A5A;`
- Value text below it: `font: 500 16px/20px "Josefin Sans"; color:#0F0F0F;`
- Example shown: "Họ và tên *" → value "Tittle" (placeholder name); "Số điện thoại *" → "0868118318".

**Chevron icon** (select fields): 16×16px, vector inset roughly center, standard down-caret glyph — implement as a small inline SVG or background-image positioned `right:16px; top:50%; transform:translateY(-50%)`, pointer-events none.

Recommendation for Razor/CSS: build one `.form-field` block (label + input) that toggles a `.is-filled` class via JS (or CSS `:placeholder-shown` trick) to move the label from centered-16px to top-13px-grey. Selects get the chevron always visible; plain text inputs omit the chevron element entirely (cleaner than hiding it with opacity).

## Address Form — Desktop (from node `853:10989`)

**Page chrome**
- Header: white, `border-bottom: 1px solid #BFBFBF`, height 120px, centered content max-width row with logo (64×64) left and round avatar chip (40×40, `#BFBFBF` bg, `#410C13` bold 20px letter) right.
- Body: background `#F2F2F2`, min-height 960px, two-column layout centered in the page:
  - **Left nav column**: width auto, `padding: 40px 0 10px`, vertical list `gap:37px`, 24px/32px SemiBold items: `Lịch sử đơn hàng` (#9B9B9B) → `Thông tin tài khoản` (#7A7A7A) → `Địa chỉ đã lưu` (#0F0F0F, current/active — no extra bold/underline styling beyond darker color) → `Đăng xuất` (#AF2234, brand red).
  - **Form column**: `flex:1; padding: 40px 160px 10px 80px; display:flex; flex-direction:column; gap:24px;`

**Form field grid (desktop)**
1. Row 1 — full width: "Họ và tên *" (text input, filled-state example "Tittle")
2. Row 2 — full width: "Số điện thoại *" (text input, tel, filled-state example "0868118318")
3. Row 3 — full width: "Tỉnh thành phố *" (select, empty state, chevron visible)
4. Row 4 — **2-column grid**, `display:flex; gap:8px;` each child `flex:1`: "Quận huyện *" (select) | "Phường xã *" (select)
5. Row 5 — full width: "Địa chỉ cụ thể *" (rendered in Figma with the select/chevron styling, but semantically a free-text street/house-number field — **implement as `<input type="text">`**, keep the same 56px/8px-radius white box, drop the chevron)
6. Row 6 — full width: "Mã bưu chính" (no asterisk = optional; also rendered with chevron in Figma — **implement as optional text input** for postal code, not a real dropdown)

Vertical gap between all 6 rows: `8px`. Gap between the "Thêm địa chỉ mới" heading block and the field stack: `16px`. Gap between the whole form-fields group and the Save button: `24px`.

**Save button (desktop)**
```css
display: inline-flex;
align-items: center;
justify-content: center;
height: 40px;
padding: 8px 16px;
border: 1px solid #7A7A7A;
border-radius: 4px;
background: transparent; /* sits on white page bg here, but page bg is #F2F2F2 so effectively white/none */
```
Label: "Lưu địa chỉ", `Josefin Sans Medium 16px/20px`, color `#0F0F0F`. Button is intrinsic width (hugs content), left-aligned under the form column (not full width) on desktop.

## Address Form — Mobile (from node `1272:54230`, 430px width)

**Page chrome**
- Header: white, `border-bottom` region uses `#9B9B9B` divider under a `padding:20px 16px` row: hamburger icon (28×28) + logo (40×40) on the left cluster, round avatar chip (28×28, `#BFBFBF` bg, `#410C13` bold 14px letter — scaled-down version of desktop chip) on the right. No visible sidebar nav — assumed the hamburger opens an off-canvas drawer containing the same 4 nav links seen on desktop.
- Body: background `#F2F2F2`, `padding: 20px 16px 10px`, single flex column, `gap:20px` between heading block and (implicitly) any following content; the field group itself uses `gap:16px` between the heading row and the field stack, `gap:8px` between individual fields — identical spacing tokens to desktop, just no side nav and no 160/80px column padding.

**Form field grid (mobile)** — identical field set/order to desktop:
1. Họ và tên * — full width text input
2. Số điện thoại * — full width text input
3. Tỉnh thành phố * — full width select
4. Quận huyện * | Phường xã * — **still side-by-side** even at 430px width (`display:flex; gap:8px;` each `flex:1`), i.e. this row does NOT stack to single column on mobile in the source file. (If real device widths run narrower than 430px or content overflows, consider stacking to 1-column with `flex-wrap` as a responsive improvement, but the Figma spec itself keeps them inline.)
5. Địa chỉ cụ thể * — full width (same "should be a text input" note as desktop)
6. Mã bưu chính — full width, optional (same "should be a text input" note as desktop)

**Save button (mobile)**
Same visual (`border:1px solid #7A7A7A; border-radius:4px; height:40px; padding:8px 16px;` label 16px/20px Medium `#0F0F0F` "Lưu địa chỉ") but **full width** here: `flex:1; width:100%` (the button stretches to the container width, unlike the desktop hug-content version).

## Razor Implementation Notes

Suggested structure:
- `Views/Account/Addresses.cshtml` — page shell: header partial, sidebar nav partial (desktop) / hamburger drawer (mobile), and `@await Html.PartialAsync("_AddressForm", Model.NewAddress)`.
- `Views/Account/_AddressForm.cshtml` — the reusable form partial (used both for "add new" and, by extension, "edit existing" — pass an `AddressViewModel` that may be pre-populated).
- CSS: one `.address-form` component stylesheet with:
  - `.form-field` (the 56px/8px-radius white box, floating label variant via `.form-field--select` modifier for the chevron)
  - `.form-row--split` (`display:flex; gap:8px;` wrapping two `.form-field` at `flex:1 1 0` each) for the Quận huyện/Phường xã pair
  - `.btn-outline` for "Lưu địa chỉ" (`border:1px solid #7A7A7A; border-radius:4px; height:40px; padding:8px 16px; font:500 16px/20px "Josefin Sans"; color:#0F0F0F; background:transparent;`), with a `.btn-outline--block` (or simple `width:100%`) modifier for the mobile full-width variant.
  - Media query breakpoint: collapse the 80/160px desktop column padding and hide the sidebar nav (replace with hamburger) below ~768–1024px; keep the Quận huyện/Phường xã row inline down to at least 430px per the source file.

## Vietnam Administrative Cascading Dropdowns — Backend Implications

The three geo selects (Tỉnh/thành phố → Quận/huyện → Phường/xã) are a classic 3-level cascade and need real data + endpoints, not just static `<option>` lists:

1. **Dataset**: no address data exists elsewhere in the Figma file — a full VN administrative-divisions dataset must be sourced/seeded (e.g. GSO official list, or a maintained open dataset such as `vietnam-provinces`/`dvhcvn`). Load once into 3 lookup tables:
   - `Provinces (Id, Name, Code)`
   - `Districts (Id, ProvinceId FK, Name, Code)`
   - `Wards (Id, DistrictId FK, Name, Code)`
2. **Important 2025 reform caveat**: Vietnam's mid-2025 administrative restructuring merged many provinces and eliminated the district (quận/huyện) tier in favor of a 2-tier province → xã/phường model. Since "today" in this project's context is 2026-07-09, confirm with the client/stakeholder **which structure** the form should actually target — the Figma mockup still shows the old 3-tier UI (Tỉnh → Quận huyện → Phường xã), which may be legacy and need updating to a 2-select cascade (Tỉnh/thành phố → Phường/xã) reflecting the current post-reform divisions. Do not silently assume the 3-tier model is still correct.
3. **API shape** (assuming 3-tier is kept, or adapt to 2-tier by dropping the middle call):
   - `GET /api/geo/provinces` → `[{id, name}]`
   - `GET /api/geo/districts?provinceId=` → `[{id, name}]`
   - `GET /api/geo/wards?districtId=` → `[{id, name}]`
   - Client-side JS: on province `change`, fetch districts, repopulate + enable the district `<select>` (disabled until a province is chosen); same cascade district→ward. Preserve selected values on validation postback (repopulate all three selects server-side in the partial view model).
4. **Address model** for persistence: `FullName, Phone, ProvinceId, DistrictId (nullable if 2-tier), WardId, AddressDetail (street/house no., free text), PostalCode (optional free text)`, plus a computed/denormalized `DisplayAddress` string for showing on the (not-yet-designed) saved-address list cards.
5. Since "Địa chỉ cụ thể" and "Mã bưu chính" are visually built from the same select component in Figma but are semantically free text, do not wire them to any lookup API — bind them as plain string inputs in the view model.
