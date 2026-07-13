# Login & OTP Verification — Design Spec

Source: Figma file `uQFY9gwfNbNSeTM6zmspzo`
Nodes: Desktop login `853:19405`, Desktop OTP `857:19584`, Mobile login `1142:39090`, Mobile OTP `1142:39126`

Target: ASP.NET Core MVC + Razor (`Account/Login.cshtml`, `Account/VerifyOtp.cshtml`), plain CSS (no Tailwind).

## Design tokens (shared)

Colors:
- Brand red: `#AF2234`
- Brand gold/tan: `#AA8656`
- Grey-50 (page bg): `#F2F2F2`
- Grey-300 (borders, dividers): `#BFBFBF`
- Grey-400 (input border default): `#9B9B9B`
- Grey-500 (disabled/secondary button bg, placeholder text): `#7A7A7A`
- Grey-900 (primary text/links): `#0F0F0F`
- White: `#FFFFFF`
- Black (Apple button bg, text): `#000000`

Typography (font-family: "Josefin Sans", sans-serif fallback):
- Heading 3 (OTP title, desktop): Medium 500, 24px / 32px line-height
- Subtitle (OTP title, mobile; SSO button label desktop): Medium 500, 20px / 23px
- Label (buttons, links, mobile SSO text, resend bold part): Medium 500, 16px / 20px
- Body 2 (input placeholder/value, OTP helper text): Light 300, 16px / 18px
- Caption ("Hoặc" divider text): Regular 400, 13px / 16px

Large display heading font "LC Sac Trial" is not used on these two screens (no hero headline present) — reserve it for other marketing pages.

Border radius: `8px` (inputs, buttons), `16px` (card container).

## Login (Email step)

### Desktop (1925px canvas, node 853:19405)

Layout:
- Header bar: full-width, white bg, `border-bottom: 1px solid #BFBFBF`, height `120px`, logo (64x64) horizontally centered, with an extra `1px solid #9B9B9B` bottom border directly under the logo mark itself.
- Page body: bg `#F2F2F2`, height `960px` (viewport section), content centered horizontally and vertically, `padding-bottom: 120px`.
- Card: white, `border-radius: 16px`, `padding: 40px`, fixed `width: 483px`, `gap: 17px` between the SSO block / divider / form block (flex column).

Card contents (top to bottom), inner gap `16px` between button groups:
1. **Google SSO button**: full width, bg `#F2F2F2`, `border-radius: 8px`, `padding: 8px 16px`, height auto (~56px with 40px icon), flex row centered, `gap: 3px`. Icon: 40x40 circular Google "G" logo. Label: "Tiếp tục với Google", Josefin Sans Medium 20px/23px, color `#000`.
2. **Apple SSO button**: full width, bg `#000000`, `border-radius: 8px`, `padding: 8px 16px`, flex row centered. Icon: 40x40 Apple logo (white). Label: "Tiếp tục với Apple", Medium 20px/23px, color `#FFFFFF`.
3. **Divider**: flex row, `gap: 8px`, centered; two horizontal `1px` lines (`#BFBFBF`-ish, `flex:1`) with "Hoặc" text between them (Caption: Regular 13px/16px, color `#000`).
4. **Email input**: full width, white bg, `border: 1px solid #9B9B9B`, `border-radius: 8px`, height `56px`, `padding: 0 16px`, vertically centered content. Placeholder "Nhập địa chỉ email", Body 2 Light 16px/18px, color `#000` (placeholder likely lighter grey in practice — treat as `#7A7A7A` placeholder / `#000` typed value).
5. **Primary CTA "Tiếp tục"**: full width, height `56px`, bg `#7A7A7A` (this is the disabled/default state — treat as button's neutral state until email is valid, then swap to brand red `#AF2234` per site's likely active-state convention), `border-radius: 8px`, centered, label Medium 20px/23px white.
6. **Secondary link "Quay lại và tiếp tục mua hàng"**: full width, height `56px`, centered, no background, text Medium 16px/20px, color `#0F0F0F`, no underline (plain link/button).

No visible focus-state styling in the design capture — for the coded implementation, apply a conventional focus ring: `border-color: #AF2234` + subtle `box-shadow: 0 0 0 3px rgba(175,34,52,0.15)` on `:focus`, consistent with brand red.

CSS-ready values:
```css
.auth-header { background:#fff; border-bottom:1px solid #BFBFBF; height:120px; display:flex; align-items:center; justify-content:center; }
.auth-header__logo-wrap { border-bottom:1px solid #9B9B9B; display:flex; align-items:center; }
.auth-page { background:#F2F2F2; min-height:960px; display:flex; align-items:center; justify-content:center; padding-bottom:120px; }
.auth-card { background:#fff; border-radius:16px; padding:40px; width:483px; display:flex; flex-direction:column; gap:17px; }
.btn-sso-google { width:100%; background:#F2F2F2; border-radius:8px; padding:8px 16px; display:flex; align-items:center; justify-content:center; gap:3px; font:500 20px/23px "Josefin Sans"; color:#000; border:none; }
.btn-sso-apple { width:100%; background:#000; border-radius:8px; padding:8px 16px; display:flex; align-items:center; justify-content:center; gap:3px; font:500 20px/23px "Josefin Sans"; color:#fff; border:none; }
.divider { display:flex; align-items:center; gap:8px; width:100%; }
.divider hr { flex:1; height:1px; background:#BFBFBF; border:none; }
.divider span { font:400 13px/16px "Josefin Sans"; color:#000; white-space:nowrap; }
.form-input { width:100%; height:56px; background:#fff; border:1px solid #9B9B9B; border-radius:8px; padding:0 16px; font:300 16px/18px "Josefin Sans"; color:#000; }
.form-input::placeholder { color:#7A7A7A; }
.form-input:focus { outline:none; border-color:#AF2234; box-shadow:0 0 0 3px rgba(175,34,52,0.15); }
.btn-primary { width:100%; height:56px; background:#7A7A7A; border-radius:8px; border:none; font:500 20px/23px "Josefin Sans"; color:#fff; }
.btn-primary:enabled:hover { background:#AF2234; }
.btn-link-secondary { width:100%; height:56px; display:flex; align-items:center; justify-content:center; background:none; border:none; font:500 16px/20px "Josefin Sans"; color:#0F0F0F; }
```

### Mobile (430px canvas, node 1142:39090)

Layout differences from desktop:
- Header: white bg, `border-bottom: 1px solid #BFBFBF`, `padding: 16px 0` (no fixed height), logo shrinks to `40x40`.
- Page body bg `#F2F2F2`, section height `960px` (design frame — in real responsive use this is `min-height: 100vh`), content area padded `20px` horizontally, card positioned near top (`top: 124px` in the mock, i.e. not perfectly vertically centered — treat as top-anchored with ~124px offset from header).
- Card: white, `border-radius: 16px`, `padding: 40px 20px`, width fills available space (max `~389px` inner within 429px outer wrapper that has `20px` side padding), inner `gap: 12px` (tighter than desktop's 17px).
- SSO buttons: height `48px` (vs 56px desktop), icon size `32x32` (vs 40x40), label font drops to Label 16px/20px (vs Subtitle 20px/23px on desktop).
- Divider: same as desktop (13px/16px "Hoặc").
- Email input: same `56px` height, same border/radius/typography as desktop.
- Primary CTA "Tiếp tục": height `48px` (vs 56 desktop), bg `#7A7A7A`, label Label 16px/20px white (down from 20px/23px).
- Secondary link: same as desktop, height `56px`, Label 16px/20px, `#0F0F0F`.

CSS-ready mobile overrides (max-width: 480px):
```css
@media (max-width: 480px) {
  .auth-header { height:auto; padding:16px 0; }
  .auth-header__logo-wrap img { width:40px; height:40px; }
  .auth-card { width:100%; max-width:389px; padding:40px 20px; gap:12px; }
  .btn-sso-google, .btn-sso-apple { height:48px; font-size:16px; line-height:20px; }
  .btn-sso-google img, .btn-sso-apple img { width:32px; height:32px; }
  .btn-primary { height:48px; font-size:16px; line-height:20px; }
}
```

## OTP Verification

### Desktop (1925px canvas, node 857:19584)

Layout:
- Header: identical to login desktop header (white, `border-bottom:1px solid #BFBFBF`, height `120px`, centered 64x64 logo) but without the extra inner border under the logo on this screen's capture (minor inconsistency — safe to keep the same header partial as login for consistency).
- Page body: `#F2F2F2`, height `960px`, centered, `padding-bottom:120px`.
- Card: white, `border-radius:16px`, `padding:40px`, width `483px`, no explicit gap value between blocks (rely on children margins) — use `gap:24px` between the (title+subtitle+input) block and the (resend/back) block for even spacing consistent with desktop login's rhythm.

Card contents:
1. **Title** "Nhập mã xác thực": Heading 3, Medium 500, 24px/32px, color `#000`, centered.
2. **Subtitle block** (`gap:0`, stacked, centered text):
   - Line 1: "Vui lòng nhập mã xác thực gửi qua email" — Body 2 Light 16px/18px, `#000`, centered.
   - Line 2: masked/actual email e.g. "duong06.design@gmail.com" — Label Medium 16px/20px, `#000`, centered (bold-ish weight to stand out).
3. **OTP code input**: single full-width text field (NOT multi-box/segmented) — `border:1px solid #BFBFBF`, `border-radius:8px`, height `56px`, `padding:8px 16px`, placeholder "Nhập mã xác thực" Body 2 Light 16px/18px, color `#7A7A7A`. This is a single input; recommend `inputmode="numeric"` `maxlength` per OTP length, letter-spaced for readability if desired, but visually it's one plain text box, not 6 separate boxes.
4. **Resend row**: flex row centered, `gap:3px`, height `56px`: "Chưa nhận được mã xác thực?" (Body 2 Light 16px/18px, `#0F0F0F`) + "Gửi lại sau 00:24" (Label Medium 16px/20px, `#0F0F0F`, bold countdown). Countdown text replaces with an active "Gửi lại mã" link (styled as link/underline) once timer hits 0 — timer format `mm:ss`, starting value in mock `00:24`.
5. **"Quay lại" link**: height `56px`, centered, Body 2 Light 16px/18px, color `#0F0F0F`, `text-decoration: underline`.

CSS-ready:
```css
.otp-card { background:#fff; border-radius:16px; padding:40px; width:483px; display:flex; flex-direction:column; gap:24px; }
.otp-title { font:500 24px/32px "Josefin Sans"; color:#000; text-align:center; }
.otp-subtitle { font:300 16px/18px "Josefin Sans"; color:#000; text-align:center; }
.otp-email { font:500 16px/20px "Josefin Sans"; color:#000; text-align:center; }
.otp-input { width:100%; height:56px; border:1px solid #BFBFBF; border-radius:8px; padding:8px 16px; font:300 16px/18px "Josefin Sans"; color:#000; }
.otp-input::placeholder { color:#7A7A7A; }
.otp-input:focus { outline:none; border-color:#AF2234; box-shadow:0 0 0 3px rgba(175,34,52,0.15); }
.otp-resend-row { display:flex; align-items:center; justify-content:center; gap:3px; height:56px; font-size:16px; color:#0F0F0F; }
.otp-resend-row .prompt { font-weight:300; line-height:18px; }
.otp-resend-row .timer { font-weight:500; line-height:20px; }
.otp-resend-row .resend-link { font-weight:500; line-height:20px; text-decoration:underline; cursor:pointer; color:#AF2234; }
.otp-back-link { display:flex; align-items:center; justify-content:center; height:56px; font:300 16px/18px "Josefin Sans"; color:#0F0F0F; text-decoration:underline; }
```

### Mobile (430px canvas, node 1142:39126)

Layout differences:
- Header: same collapsed mobile header as login mobile (`padding:16px 0`, 40x40 logo).
- Page body: `#F2F2F2`, `padding:0 16px` horizontal, content column with `padding-bottom:80px`.
- Card: white, `border-radius:16px`, `padding:20px` (uniform, not 40/20 split like login), positioned `top:165px` from container top in the mock (top-anchored, not perfectly centered), width `398px` (fills the 430px viewport minus the 16px side gutters, ~398px).
- Title "Nhập mã xác thực": drops to Subtitle size — Medium 20px/23px (vs 24px/32px desktop).
- Subtitle/email lines: same sizes as desktop (16px/18px and 16px/20px).
- OTP input: identical `56px` height, `1px solid #BFBFBF`, `border-radius:8px` — no size reduction on mobile.
- Resend row & back link: identical typography/height to desktop (16px/18px + 16px/20px, `56px` row height).

CSS-ready mobile overrides:
```css
@media (max-width: 480px) {
  .otp-card { width:100%; max-width:398px; padding:20px; }
  .otp-title { font-size:20px; line-height:23px; }
}
```

## Implementation notes for Razor views

- `Account/Login.cshtml`: single form with email `<input type="email">`, hidden logic to toggle `.btn-primary` bg from `#7A7A7A` (disabled-looking) to brand red `#AF2234` once the field passes client-side validation (`disabled` attribute driven by JS or just style via `:disabled` pseudo-class if the button itself is disabled until input has content).
- `Account/VerifyOtp.cshtml`: server should render the masked/actual destination email (`@Model.Email`) in the `.otp-email` element; the OTP field is a single text input (`type="text"`, `inputmode="numeric"`, `pattern="[0-9]*"`, `autocomplete="one-time-code"`) — not six separate boxes, matching the Figma source exactly.
- Resend timer: implement client-side countdown in JS starting at whatever seconds value the server passes (e.g. `data-resend-after="24"`), swapping the `<span class="timer">` text for a clickable `<a class="resend-link">Gửi lại mã</a>` at zero, then posting to a `ResendOtp` action.
- Both screens share one `_AuthLayout.cshtml` partial for the top header (logo + bottom border), differing only in logo size per breakpoint (40px mobile / 64px desktop) via CSS media query, not separate markup.
- Card border-radius `16px`, input/button radius `8px` are the only two radii used across both flows — encode as CSS custom properties `--radius-card: 16px; --radius-control: 8px;`.
