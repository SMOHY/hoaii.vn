// Quet toan bo man admin: moi trang phai tra 200, khong loi JS, khong loi 500.
const { chromium } = require('playwright');
const BASE = 'http://localhost:5167';

const TRANG = [
  ['Tổng quan', '/admin'],
  ['Báo cáo', '/admin/bao-cao'],
  ['Đơn hàng', '/admin/don-hang'],
  ['Chi tiết đơn', '/admin/don-hang/2054'],
  ['Sản phẩm', '/admin/san-pham'],
  ['Thêm sản phẩm', '/admin/san-pham/them'],
  ['Sửa sản phẩm', '/admin/san-pham/20/sua'],
  ['Danh mục', '/admin/danh-muc'],
  ['Thêm danh mục', '/admin/danh-muc/them'],
  ['Sửa danh mục', '/admin/danh-muc/5/sua'],
  ['Voucher', '/admin/voucher'],
  ['Khách hàng', '/admin/khach-hang'],
  ['Hộp thư', '/admin/hop-thu'],
  ['Trang chủ', '/admin/trang-chu'],
  ['Bài viết', '/admin/blog'],
  ['Chính sách', '/admin/chinh-sach'],
  ['Về chúng tôi', '/admin/trang/gioi-thieu'],
  ['Liên hệ', '/admin/trang/lien-he'],
  ['Hợp tác', '/admin/trang/hop-tac'],
  ['Nội dung khác', '/admin/trang/khac'],
  ['Menu', '/admin/menu'],
  ['Thư viện ảnh', '/admin/thu-vien-anh'],
  ['Cài đặt chung', '/admin/cai-dat'],
  ['Vận chuyển', '/admin/van-chuyen'],
  ['Thanh toán', '/admin/thanh-toan'],
  ['Địa giới', '/admin/dia-gioi'],
  ['Email', '/admin/email'],
  ['Tài khoản admin', '/admin/tai-khoan'],
  ['Nhật ký', '/admin/nhat-ky'],
];

(async () => {
  const b = await chromium.launch();
  const ctx = await b.newContext({ viewport: { width: 1600, height: 1000 } });
  const p = await ctx.newPage();

  await p.goto(BASE + '/admin/dang-nhap', { waitUntil: 'networkidle' });
  await p.fill('input[type=email], input[name*=Email]', 'admin@hoaii.vn');
  await p.fill('input[type=password]', 'Hoaii@2026');
  await p.locator('button[type=submit]').first().click();
  await p.waitForTimeout(2000);
  if (p.url().includes('dang-nhap')) { console.log('!! khong dang nhap duoc'); await b.close(); return; }

  let hong = 0;
  for (const [ten, url] of TRANG) {
    const jsErr = [], httpErr = [];
    const onErr = e => jsErr.push(e.message.slice(0, 60));
    const onRes = r => { if (r.status() >= 400) httpErr.push(r.status() + ' ' + r.url().replace(BASE, '').slice(0, 50)); };
    p.on('pageerror', onErr); p.on('response', onRes);

    let status = '?';
    try {
      const res = await p.goto(BASE + url, { waitUntil: 'networkidle', timeout: 25000 });
      status = res ? res.status() : '?';
      await p.waitForTimeout(300);
    } catch (e) { status = 'LOI: ' + e.message.slice(0, 40); }

    const noiDung = await p.evaluate(() => ({
      h1: document.querySelector('h1,.admin-page__title,.admin-card h2')?.textContent.trim().slice(0, 34) || '',
      bang: document.querySelectorAll('table tbody tr').length,
      form: document.querySelectorAll('form').length,
      tran: document.documentElement.scrollWidth - document.documentElement.clientWidth,
    })).catch(() => ({}));

    p.off('pageerror', onErr); p.off('response', onRes);
    const van_de = [];
    if (status !== 200) van_de.push('HTTP ' + status);
    if (jsErr.length) van_de.push('JS: ' + jsErr[0]);
    if (httpErr.length) van_de.push(httpErr.slice(0, 2).join(', '));
    if (noiDung.tran > 1) van_de.push('tran ngang ' + noiDung.tran);
    if (van_de.length) hong++;

    console.log((van_de.length ? 'X ' : '. ') + ten.padEnd(18) + url.padEnd(26)
      + (van_de.length ? van_de.join(' | ') : 'ok  ' + (noiDung.bang ? noiDung.bang + ' dong' : '') + ' ' + (noiDung.h1 || '')));
  }
  console.log('\n' + (hong ? hong + '/' + TRANG.length + ' man co van de' : 'TAT CA ' + TRANG.length + ' MAN ADMIN OK'));
  await b.close();
})();
