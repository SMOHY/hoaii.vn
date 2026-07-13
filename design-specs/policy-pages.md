# Policy Pages ("Chính sách") — Design Spec

Figma file: `uQFY9gwfNbNSeTM6zmspzo`. Section "Chính sách" (`1379:34054`) contains 4 policy documents, each with a Desktop and Mobile frame. All 4 share **one identical template** — only the page title / breadcrumb text and the (not-yet-authored) body copy differ. No unique body layout exists in Figma for any of them; the actual legal text is expected to be CMS/markdown-driven long-form content added later.

## Pages covered (all use the shared template)

| Page (VN) | Desktop node | Mobile node | Breadcrumb text |
|---|---|---|---|
| Chính sách trao đổi (Exchange/Return Policy) | `1246:42533` | `1367:67771` | Trang chủ/Chính sách trao đổi |
| Chính sách giao hàng (Shipping Policy) | `1246:43442` | `1367:66813` | Trang chủ/Chính sách giao hàng |
| Điều khoản sử dụng (Terms of Use) | `1246:43583` | `1367:67553` | Trang chủ/Điều khoản sử dụng |
| Chính sách bảo vệ dữ liệu cá nhân (Privacy Policy) | `1246:43724` | `1367:67662` | Trang chủ/Chính sách bảo vệ dữ liệu cá nhân |

## Shared template structure

### Desktop (1920 wide)
- `Nav desktop` instance, 168px tall, at top (0,0)
- Breadcrumb text row: y=200, height=20, x=240 (matches 1440px content width, 240px gutters), Josefin Sans ~14px grey
- Title heading block ("Niềm tự hào" frame name is a mislabeled/reused component name — not literal content): y=220, height=376
  - Inner heading text is horizontally centered within the 1440px content column, uppercase, large display size (~40–56px based on title length, e.g. "CHÍNH SÁCH TRAO ĐỔI" spans 440px at what reads as ~48–56px — consistent with the `LC Sac Trial`/Heading-2 token used elsewhere: 48px/56px line-height)
  - No body copy authored in Figma below the heading — page is otherwise just Nav + Title block + Footer
- `Footer` instance directly below the title block (y=596), 849px tall
- Total frame height 1445px = 168 (nav overlap) + 596 (title block bottom) + 849 (footer)

### Mobile (430 wide)
- `Nav desktop` instance (mislabeled — actually the mobile nav), 92px tall
- Breadcrumb: y=100, x=16, width=398, Josefin Sans ~13px
- Title block: y=116, height=106, heading centered, ~20–26px (e.g. "CHÍNH SÁCH GIAO HÀNG" spans 195×26)
- `Footer` instance at y=222, 917px tall
- Total frame height 1139px = 222 (header+title) + 917 (footer)

## Design tokens (shared across all policy pages)
- Grey/body text: `#5A5A5A`–`#3C3C3C` range (Foundation/Grey scale)
- Heading font: `LC Sac Trial`, uppercase, ~48/56 desktop, ~20/26 mobile (scaled down proportionally to fit container width — treat as a fluid heading, not a fixed size, since titles vary in length)
- Breadcrumb font: Josefin Sans, Caption/Body-2 scale (13–16px)

## Razor implementation recommendation

Since all 4 pages are identical in layout, do **not** create 4 separate Views. Use a single shared view:

```
Views/Page/Policy.cshtml
```

with a route like `/chinh-sach/{slug}` (`trao-doi`, `giao-hang`, `dieu-khoan-su-dung`, `bao-mat`), backed by a `PolicyPage` model (Slug, Title, BreadcrumbLabel, BodyHtml/Markdown) — likely stored in DB or as static Markdown files under `Content/Policies/` — so legal text can be edited without redeploying code. The template itself is just:

```
_Layout (Nav/Footer)
  > breadcrumb partial
  > <h1 class="policy-title">@Model.Title</h1>
  > <div class="policy-body">@Html.Raw(Model.BodyHtml)</div>   // not present in Figma, needs its own typographic scale (prose styles: headings, lists, paragraphs) chosen at implementation time
```

## Open question
Figma has no design for the actual policy **body copy** (paragraphs, lists, sub-headings) — only the page header. When implementing, apply a sensible "prose" typographic scale using the site's existing type tokens (Josefin Sans body/label, grey-700 text, red-600 for links) rather than inventing a new style system.
