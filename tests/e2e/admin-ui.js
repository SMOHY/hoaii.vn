// Walk every admin screen in a real browser at desktop + phone width, watching for JS errors,
// server errors, horizontal overflow and broken images.
const { chromium } = require('playwright');

const BASE = 'http://localhost:5167';
const PAGES = [
  ['Tổng quan', '/admin'], ['Báo cáo', '/admin/bao-cao'], ['Đơn hàng', '/admin/don-hang'],
  ['Sản phẩm', '/admin/san-pham'], ['Sửa sản phẩm', '/admin/san-pham/20/sua'],
  ['Danh mục', '/admin/danh-muc'], ['Voucher', '/admin/voucher'], ['Thêm voucher', '/admin/voucher/them'],
  ['Khách hàng', '/admin/khach-hang'], ['Hộp thư', '/admin/hop-thu'], ['Bán buôn', '/admin/hop-thu/ban-buon'],
  ['Newsletter', '/admin/hop-thu/newsletter'], ['Trang chủ', '/admin/trang-chu'], ['Bài viết', '/admin/blog'],
  ['Chính sách', '/admin/chinh-sach'], ['Về chúng tôi', '/admin/trang/gioi-thieu'],
  ['Liên hệ', '/admin/trang/lien-he'], ['Hợp tác', '/admin/trang/hop-tac'], ['Nội dung khác', '/admin/trang/khac'],
  ['Menu', '/admin/menu'], ['Thư viện ảnh', '/admin/thu-vien-anh'], ['Cài đặt', '/admin/cai-dat'],
  ['Vận chuyển', '/admin/van-chuyen'], ['Thanh toán', '/admin/thanh-toan'], ['Địa giới', '/admin/dia-gioi'],
  ['Email', '/admin/email'], ['Tài khoản admin', '/admin/tai-khoan'], ['Nhật ký', '/admin/nhat-ky'],
];

const problems = [];

async function run(browser, width) {
  const label = width >= 1024 ? 'DESKTOP' : 'MOBILE';
  console.log(`\n--- ADMIN ${label} (${width}px) ---`);
  const ctx = await browser.newContext({ viewport: { width, height: 900 }, isMobile: width < 600 });
  const p = await ctx.newPage();
  const errs = [];
  p.on('pageerror', e => errs.push('JS: ' + e.message));
  p.on('response', r => { if (r.status() >= 500) errs.push(`HTTP ${r.status()} ${r.url().replace(BASE, '')}`); });

  await p.goto(BASE + '/admin/dang-nhap', { waitUntil: 'networkidle' });
  await p.fill('input[name="Email"]', 'admin@hoaii.vn');
  await p.fill('input[name="Password"]', 'Hoaii@2026');
  await p.click('button[type=submit]');
  await p.waitForLoadState('networkidle');

  for (const [name, path] of PAGES) {
    errs.length = 0;
    const res = await p.goto(BASE + path, { waitUntil: 'networkidle' });
    await p.waitForFunction(() => Array.from(document.images).every(i => i.complete), null, { timeout: 8000 }).catch(() => {});

    const overflow = await p.evaluate(() => document.documentElement.scrollWidth - document.documentElement.clientWidth);
    const broken = await p.evaluate(() =>
      Array.from(document.images).filter(i => i.naturalWidth === 0 && i.currentSrc).length);

    const issues = [];
    if (res.status() !== 200) issues.push(`HTTP ${res.status()}`);
    if (overflow > 2) issues.push(`tràn ngang ${overflow}px`);
    if (broken > 0) issues.push(`${broken} ảnh vỡ`);
    if (errs.length) issues.push(errs.slice(0, 2).join(' | '));

    if (issues.length) {
      console.log(`  FAIL  ${name} (${path}): ${issues.join('; ')}`);
      problems.push(`${label} — ${name}: ${issues.join('; ')}`);
    } else {
      console.log(`  OK    ${name}`);
    }
  }
  await ctx.close();
}

(async () => {
  console.log('===== ADMIN UI (trình duyệt thật) =====');
  const browser = await chromium.launch();
  await run(browser, 1440);
  await run(browser, 430);
  await browser.close();
  console.log(`\n${'='.repeat(60)}`);
  console.log(problems.length === 0 ? 'ADMIN UI: TẤT CẢ SẠCH' : `ADMIN UI: ${problems.length} vấn đề`);
  problems.forEach(p => console.log('  ✗ ' + p));
  process.exit(problems.length ? 1 : 0);
})();
