// Real-browser journeys: click through the site the way a customer actually would,
// on desktop and on a phone, watching for JS errors, dead controls and broken images.
const { chromium } = require('playwright');

const BASE = 'http://localhost:5167';
const out = [];
const log = (ok, msg) => { out.push({ ok, msg }); console.log(`  ${ok ? 'OK  ' : 'FAIL'}  ${msg}`); };

/// One broken step shouldn't abort the whole journey — record it and carry on.
async function step(name, fn) {
  try { return await fn(); }
  catch (e) { log(false, `${name} — ${String(e.message).split('\n')[0].slice(0, 90)}`); return null; }
}

/// The radios are visually-hidden inputs driven by a styled label, so click the label.
async function pickRadio(p, value) {
  const input = await p.$(`input[value="${value}"]`);
  if (!input) return false;
  const label = await input.evaluateHandle(el => el.closest('label'));
  const target = label.asElement() || input;
  await target.click({ force: true, timeout: 5000 });
  return await input.evaluate(el => el.checked).catch(() => false);
}

async function newPage(browser, width) {
  const ctx = await browser.newContext({
    viewport: { width, height: 900 },
    isMobile: width < 600,
    hasTouch: width < 600,
  });
  const page = await ctx.newPage();
  page.jsErrors = [];
  page.on('pageerror', e => page.jsErrors.push(e.message));
  page.on('response', r => { if (r.status() >= 500) page.jsErrors.push(`HTTP ${r.status()} ${r.url()}`); });
  return page;
}

async function journey(browser, width) {
  const label = width >= 1024 ? 'DESKTOP' : 'MOBILE';
  console.log(`\n--- ${label} (${width}px) ---`);
  const p = await newPage(browser, width);

  // 1. land on home
  await p.goto(BASE, { waitUntil: 'networkidle' });
  log((await p.title()).length > 0, 'Trang chủ mở được');

  // 2. navigate to a product via the storefront (not by typing a URL)
  await p.goto(BASE + '/danh-muc/qua-tet', { waitUntil: 'networkidle' });
  const firstCard = await p.$('.product-card__image-link');
  log(!!firstCard, 'Danh mục có sản phẩm bấm được');
  if (!firstCard) return finish(p, label);
  await firstCard.click();
  await p.waitForLoadState('networkidle');
  log(/\/san-pham\//.test(p.url()), 'Bấm thẻ -> vào trang sản phẩm: ' + p.url().replace(BASE, ''));

  // 3. pick a variant if there is one
  await step('Chọn loại hộp', async () => {
    const opts = await p.$$('[name="variantId"]');
    if (opts.length > 1) {
      const label = await opts[1].evaluateHandle(el => el.closest('label'));
      await (label.asElement() || opts[1]).click({ force: true, timeout: 5000 });
      log(true, `Chọn được loại hộp (${opts.length} lựa chọn)`);
    }
  });

  // 4. add to cart
  const addBtn = await p.$('button[type=submit].pdp-add-to-cart, .pdp-add-to-cart, form[action*="gio-hang/them"] button');
  log(!!addBtn, 'Có nút Thêm vào giỏ');
  if (addBtn) {
    await addBtn.click();
    await p.waitForTimeout(1200); // cart-live.js posts in the background
    const badge = await p.$eval('[data-cart-badge]', el => el.textContent.trim()).catch(() => null);
    log(badge && badge !== '0', `Badge giỏ hàng cập nhật: "${badge}"`);
  }

  // 5. go to cart
  await p.goto(BASE + '/gio-hang', { waitUntil: 'networkidle' });
  const hasLine = await p.$('.cart-item, [data-cart-line], .cart-row');
  log(!!hasLine, 'Giỏ hàng có sản phẩm vừa thêm');

  // 6. checkout
  await p.goto(BASE + '/thanh-toan', { waitUntil: 'networkidle' });
  log(/thanh-toan/.test(p.url()), 'Vào được trang thanh toán');

  // shipping method toggle updates the fee live
  await step('Đổi phương thức giao hàng', async () => {
    const before = await p.$eval('[data-shipping-fee]', e => e.textContent.trim()).catch(() => '');
    const picked = await pickRadio(p, 'Intercity');
    await p.waitForTimeout(400);
    const after = await p.$eval('[data-shipping-fee]', e => e.textContent.trim()).catch(() => '');
    log(picked, `Chọn giao liên tỉnh: phí "${before}" -> "${after}"`);
    await pickRadio(p, 'InnerCity');
    await p.waitForTimeout(300);
  });

  // 7. fill and submit the order. The inputs are asp-for="Form.X" so they're named "Form.X".
  const fill = async (name, val) => {
    const el = await p.$(`[name="${name}"]`);
    if (!el) { log(false, `Không tìm thấy ô nhập ${name}`); return false; }
    await el.fill(val);
    return true;
  };
  await fill('Form.Email', 'journey@example.com');
  await fill('Form.FirstName', 'Nguoi');
  await fill('Form.LastName', 'Dung');
  await fill('Form.Address', '123 Đường Thử');
  await fill('Form.ProvinceDistrictWard', 'Hà Nội');
  await fill('Form.Phone', '0912345678');
  await step('Chọn COD', async () => { await pickRadio(p, 'CashOnDelivery'); });
  // There are two submit buttons (a desktop summary CTA and a mobile one); only one is on screen.
  const candidates = await p.$$('button[form="checkout-form"], form#checkout-form button[type=submit]');
  let submit = null;
  for (const c of candidates) if (await c.isVisible()) { submit = c; break; }
  log(!!submit, `Có nút đặt hàng đang hiển thị (${candidates.length} nút trong DOM)`);
  if (submit) {
    await step('Bấm đặt hàng', async () => {
      await submit.click();
      await p.waitForLoadState('networkidle');
      const ok = /xac-nhan/.test(p.url());
      log(ok, ok ? 'Đặt hàng thành công -> ' + p.url().replace(BASE, '') : 'Đặt hàng KHÔNG tới trang xác nhận: ' + p.url().replace(BASE, ''));
      if (ok) {
        const body = await p.textContent('body');
        log(/HD\d{6}/.test(body), 'Trang xác nhận hiện mã đơn');
      }
    });
  }

  // 8. mobile menu drawer
  if (width < 600) {
    await p.goto(BASE, { waitUntil: 'networkidle' });
    const burger = await p.$('[data-nav-drawer-open]');
    log(!!burger, 'Mobile có nút menu');
    if (burger) {
      await burger.click({ force: true });
      await p.waitForTimeout(500);
      const open = await p.$eval('[data-nav-drawer]', el => el.className.includes('is-open')).catch(() => false);
      log(open, 'Menu mobile mở ra được');
    }
  }

  return finish(p, label);
}

async function finish(p, label) {
  const broken = await p.evaluate(() =>
    Array.from(document.images).filter(i => i.naturalWidth === 0 && i.currentSrc).map(i => i.currentSrc));
  log(broken.length === 0, `Không có ảnh vỡ${broken.length ? ' (' + broken.slice(0, 2) + ')' : ''}`);
  log(p.jsErrors.length === 0, `Không có lỗi JS/500${p.jsErrors.length ? ': ' + p.jsErrors.slice(0, 3).join(' | ') : ''}`);
  await p.context().close();
}

(async () => {
  console.log('===== HÀNH TRÌNH NGƯỜI DÙNG THẬT =====');
  const browser = await chromium.launch();
  await journey(browser, 1440);
  await journey(browser, 430);
  await browser.close();
  const fails = out.filter(o => !o.ok);
  console.log(`\n${'='.repeat(60)}\nNGƯỜI DÙNG: ${out.length - fails.length}/${out.length} PASS`);
  fails.forEach(f => console.log('  ✗ ' + f.msg));
  process.exit(fails.length ? 1 : 0);
})();
