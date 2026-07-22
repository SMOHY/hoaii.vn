// Bo test dau-cuoi: chay that cac luong nghiep vu, khong chi mo trang.
// Moi test tu don dep sau khi chay de co the chay lai nhieu lan.
const { chromium } = require('playwright');
const BASE = 'http://localhost:5167';

const ket = [];
function ghi(nhom, ten, dat, chiTiet = '') {
  ket.push({ nhom, ten, dat, chiTiet });
  console.log('  ' + (dat ? '.' : 'X') + ' ' + ten.padEnd(52) + (dat ? 'OK' : 'HONG') + (chiTiet ? '  ← ' + chiTiet : ''));
}

async function dangNhapAdmin(p) {
  await p.goto(BASE + '/admin/dang-nhap', { waitUntil: 'networkidle' });
  await p.fill('input[type=email], input[name*=Email]', 'admin@hoaii.vn');
  await p.fill('input[type=password]', 'Hoaii@2026');
  await p.locator('button[type=submit]').first().click();
  await p.waitForTimeout(1800);
  return !p.url().includes('dang-nhap');
}

(async () => {
  const b = await chromium.launch();

  // ══════════ NHOM 1: MUA HANG ══════════
  console.log('\n=== 1. LUONG MUA HANG ===');
  {
    const ctx = await b.newContext({ viewport: { width: 1600, height: 1000 } });
    const p = await ctx.newPage();
    const loi = [];
    p.on('pageerror', e => loi.push(e.message.slice(0, 60)));

    await p.goto(BASE + '/san-pham/tinh-hoa-bac-bo', { waitUntil: 'networkidle' });
    await p.locator('button:has-text("THÊM VÀO GIỎ HÀNG")').first().click();
    await p.waitForTimeout(1600);
    const soLuong = await p.evaluate(() => document.querySelector('.nav-cart-badge')?.textContent.trim());
    ghi('mua', 'Thêm vào giỏ cập nhật số trên icon giỏ', !!soLuong && +soLuong > 0, 'badge=' + soLuong);

    // tang so luong trong gio
    await p.goto(BASE + '/gio-hang', { waitUntil: 'networkidle' });
    const truoc = await p.evaluate(() => document.querySelector('.qty-stepper__value, [data-qty-value]')?.textContent.trim());
    await p.locator('.cart-item button').filter({ hasText: '+' }).first().click().catch(() => {});
    await p.waitForTimeout(1500);
    const sau = await p.evaluate(() => document.querySelector('.qty-stepper__value, [data-qty-value]')?.textContent.trim());
    ghi('mua', 'Tăng số lượng trong giỏ', truoc !== sau, truoc + ' → ' + sau);

    // ap voucher
    const coVoucher = await p.locator('[data-voucher-modal-open]').count();
    if (coVoucher) {
      await p.locator('[data-voucher-modal-open]').first().click();
      await p.waitForTimeout(700);
      const soMa = await p.evaluate(() => document.querySelectorAll('[data-voucher-modal] .voucher-item').length);
      ghi('mua', 'Modal voucher mở và có mã thật', soMa > 0, soMa + ' mã');
      await p.keyboard.press('Escape');
      await p.waitForTimeout(400);
    }

    // dat don
    await p.goto(BASE + '/thanh-toan', { waitUntil: 'networkidle' });
    for (const [n, v] of [['Form.Email', 'test@hoaii.test'], ['Form.FirstName', 'Long'],
                          ['Form.LastName', 'Nguyễn'], ['Form.Address', '945 Ngô Gia Tự'],
                          ['Form.ProvinceDistrictWard', 'Hà Nội'], ['Form.Phone', '0912345678']]) {
      await p.fill('[name="' + n + '"]', v).catch(() => {});
    }
    await p.locator('input[name="Form.ShippingMethod"]').first().check().catch(() => {});
    await p.locator('input[name="Form.PaymentMethod"]').first().check().catch(() => {});
    await p.locator('button[type=submit]:visible').filter({ hasText: 'đặt hàng' }).first().click();
    await p.waitForTimeout(3000);
    const maDon = (await p.evaluate(() => document.body.innerText.match(/HD\d{6}-\d{4}/)?.[0])) || '';
    ghi('mua', 'Đặt hàng thành công, sinh mã đơn', /^HD\d{6}-\d{4}$/.test(maDon), maDon);
    ghi('mua', 'Không lỗi JS suốt luồng mua', loi.length === 0, loi[0] || '');

    global.__maDon = maDon;
    await ctx.close();
  }

  // ══════════ NHOM 2: ADMIN XU LY DON ══════════
  console.log('\n=== 2. ADMIN XỬ LÝ ĐƠN ===');
  {
    const ctx = await b.newContext({ viewport: { width: 1600, height: 1000 } });
    const p = await ctx.newPage();
    ghi('admin', 'Đăng nhập admin', await dangNhapAdmin(p));

    await p.goto(BASE + '/admin/don-hang', { waitUntil: 'networkidle' });
    const thayDon = await p.evaluate(ma => document.body.innerText.includes(ma), global.__maDon);
    ghi('admin', 'Đơn vừa đặt xuất hiện trong admin', thayDon, global.__maDon);

    // mo don moi nhat va doi trang thai
    const href = await p.evaluate(() => document.querySelector('a[href^="/admin/don-hang/"]')?.getAttribute('href'));
    await p.goto(BASE + href, { waitUntil: 'networkidle' });
    const tt1 = await p.evaluate(() => document.querySelector('[class*=status], .admin-badge')?.textContent.trim());
    const nutXacNhan = await p.locator('button:has-text("Xác nhận"), button:has-text("Chuyển")').count();
    ghi('admin', 'Trang chi tiết đơn có nút đổi trạng thái', nutXacNhan > 0, 'trạng thái: ' + tt1);

    if (nutXacNhan > 0) {
      await p.locator('button:has-text("Xác nhận"), button:has-text("Chuyển")').first().click();
      await p.waitForTimeout(1800);
      const tt2 = await p.evaluate(() => document.querySelector('[class*=status], .admin-badge')?.textContent.trim());
      ghi('admin', 'Đổi trạng thái đơn có hiệu lực', tt1 !== tt2, tt1 + ' → ' + tt2);
    }
    await ctx.close();
  }

  // ══════════ NHOM 3: ADMIN SUA SAN PHAM ══════════
  console.log('\n=== 3. ADMIN SỬA SẢN PHẨM ===');
  {
    const ctx = await b.newContext({ viewport: { width: 1600, height: 1000 } });
    const p = await ctx.newPage();
    await dangNhapAdmin(p);

    await p.goto(BASE + '/admin/san-pham/20/sua', { waitUntil: 'networkidle' });
    const tenCu = await p.inputValue('[name=Name]').catch(() => null);
    if (tenCu) {
      const tenMoi = tenCu + ' ✓';
      await p.fill('[name=Name]', tenMoi);
      await p.locator('button[type=submit]:has-text("Lưu")').first().click();
      await p.waitForTimeout(2000);
      await p.goto(BASE + '/admin/san-pham/20/sua', { waitUntil: 'networkidle' });
      const doc = await p.inputValue('[name=Name]');
      ghi('admin', 'Sửa tên sản phẩm và lưu được', doc === tenMoi, doc);

      // storefront doi theo
      const slug = await p.inputValue('[name=Slug]').catch(() => '');
      if (slug) {
        await p.goto(BASE + '/san-pham/' + slug, { waitUntil: 'networkidle' });
        const tren = await p.evaluate(() => document.querySelector('h1')?.textContent.trim());
        ghi('admin', 'Storefront hiện tên mới', (tren || '').includes('✓'), tren);
      }
      // tra lai
      await p.goto(BASE + '/admin/san-pham/20/sua', { waitUntil: 'networkidle' });
      await p.fill('[name=Name]', tenCu);
      await p.locator('button[type=submit]:has-text("Lưu")').first().click();
      await p.waitForTimeout(1500);
    } else {
      ghi('admin', 'Sửa tên sản phẩm và lưu được', false, 'không tìm thấy ô tên');
    }
    await ctx.close();
  }

  // ══════════ NHOM 4: PHAN QUYEN ══════════
  console.log('\n=== 4. PHÂN QUYỀN & BẢO MẬT ===');
  {
    const ctx = await b.newContext({ viewport: { width: 1600, height: 1000 } });
    const p = await ctx.newPage();

    // chua dang nhap -> phai bi da ve trang login
    const r = await p.goto(BASE + '/admin/san-pham', { waitUntil: 'networkidle' });
    ghi('bao mat', 'Chưa đăng nhập vào /admin bị đá về login',
        p.url().includes('dang-nhap'), p.url().replace(BASE, ''));

    // sai mat khau -> khong vao duoc
    await p.goto(BASE + '/admin/dang-nhap', { waitUntil: 'networkidle' });
    await p.fill('input[type=email], input[name*=Email]', 'admin@hoaii.vn');
    await p.fill('input[type=password]', 'sai-mat-khau-hoan-toan');
    await p.locator('button[type=submit]').first().click();
    await p.waitForTimeout(1800);
    ghi('bao mat', 'Sai mật khẩu không vào được', p.url().includes('dang-nhap'));

    // POST khong co antiforgery -> phai bi tu choi
    await dangNhapAdmin(p);
    const res = await p.evaluate(async (base) => {
      const r = await fetch(base + '/admin/danh-muc/luu', {
        method: 'POST', headers: { 'Content-Type': 'application/x-www-form-urlencoded' },
        body: 'id=5&name=Hack&type=Occasion&sortOrder=0',
      });
      return r.status;
    }, BASE);
    ghi('bao mat', 'POST thiếu antiforgery token bị từ chối', res === 400 || res === 403, 'HTTP ' + res);
    await ctx.close();
  }

  // ══════════ NHOM 5: STOREFRONT FORM ══════════
  console.log('\n=== 5. FORM STOREFRONT ===');
  {
    const ctx = await b.newContext({ viewport: { width: 1600, height: 1000 } });
    const p = await ctx.newPage();

    await p.goto(BASE + '/lien-he', { waitUntil: 'networkidle' });
    const truocKhiGui = await p.evaluate(() => document.querySelectorAll('.contact-form__success').length);
    await p.fill('[name=FirstName]', 'Test').catch(() => {});
    await p.fill('[name=LastName]', 'Tự động').catch(() => {});
    await p.fill('[name=Email]', 'test@hoaii.test').catch(() => {});
    await p.fill('[name=Message]', 'Tin nhắn kiểm thử tự động.').catch(() => {});
    await p.locator('.contact-form button[type=submit]').first().click().catch(() => {});
    await p.waitForTimeout(2200);
    const daGui = await p.evaluate(() => document.querySelectorAll('.contact-form__success').length);
    ghi('form', 'Gửi form liên hệ có phản hồi', daGui > truocKhiGui);

    // newsletter
    await p.goto(BASE + '/', { waitUntil: 'networkidle' });
    await p.fill('.footer-signup input[type=email], input[name=email]', 'newsletter@hoaii.test').catch(() => {});
    await p.locator('.footer-signup button, button:has-text("Gửi")').first().click().catch(() => {});
    await p.waitForTimeout(2000);
    const nlOk = await p.evaluate(() => !!document.querySelector('.footer-signup__done, .toast'));
    ghi('form', 'Đăng ký nhận tin có phản hồi', nlOk);
    await ctx.close();
  }

  // ══════════ TONG KET ══════════
  const hong = ket.filter(k => !k.dat);
  console.log('\n' + '='.repeat(64));
  console.log('TỔNG: ' + ket.length + ' phép thử, ' + (ket.length - hong.length) + ' đạt, ' + hong.length + ' hỏng');
  if (hong.length) {
    console.log('\nCÁC CHỖ HỎNG:');
    hong.forEach(h => console.log('  [' + h.nhom + '] ' + h.ten + (h.chiTiet ? '  ← ' + h.chiTiet : '')));
  }
  await b.close();
})();
