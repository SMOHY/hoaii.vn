// Kiểm 12 mục khách HOÀI nêu trong "Web HOÀI.xlsx" (bản 21/08/2026), chạy trên trình duyệt thật.
//
// Chạy: khởi động app ở cổng 5167 rồi `node tests/e2e/kiem-yeu-cau-khach.js`.
// Mỗi phép thử tự đưa dữ liệu về trạng thái đầu trước khi chạy, nên chạy lại nhiều lần vẫn cho
// cùng kết quả — bài học từ hai lần đầu, khi kết quả lần chạy trước làm lần sau báo hỏng oan.
const { chromium } = require('playwright');
const B = 'http://localhost:5167';

const results = [];
function note(stt, ten, ok, chiTiet) {
  results.push({ stt, ten, ok, chiTiet });
  console.log((ok ? 'DAT  | ' : 'HONG | ') + stt + '. ' + ten + ' — ' + chiTiet);
}

(async () => {
  const br = await chromium.launch();
  const jsErrors = [];

  const admin = await br.newPage({ viewport: { width: 1440, height: 900 } });
  admin.on('pageerror', e => jsErrors.push('admin: ' + e.message));
  await admin.goto(B + '/admin/dang-nhap');
  await admin.fill('input[name="Email"]', 'admin@hoaii.vn');
  await admin.fill('input[name="Password"]', 'Hoaii@2026');
  await admin.click('form button[type=submit]');
  await admin.waitForLoadState('networkidle');

  const web = await br.newPage({ viewport: { width: 1440, height: 900 } });
  web.on('pageerror', e => jsErrors.push('web: ' + e.message));
  const mob = await br.newPage({ viewport: { width: 430, height: 932 }, deviceScaleFactor: 2, isMobile: true, hasTouch: true });

  // ---- 1. Ảnh riêng cho mobile ----
  try {
    await admin.goto(B + '/admin/trang/gioi-thieu');
    // Làm việc trong đúng một ô ảnh, không dùng selector chung: các ô khác có thể đang ở
    // trạng thái khác nhau và selector chung sẽ bắt nhầm ô.
    const slot = admin.locator('[data-imgslot]').first();
    await slot.locator('[data-imgtab="mobile"]').click();
    await admin.waitForTimeout(300);
    // Trả ô về "dùng chung ảnh desktop" trước, để lần chạy nào cũng bắt đầu như nhau.
    if (await slot.locator('[data-imgslot-share]').isVisible()) {
      await slot.locator('[data-imgslot-share]').click();
      await admin.waitForTimeout(300);
    }
    const sharedText = (await slot.locator('[data-imgslot-shared]').innerText()).split('\n')[0].trim();
    await slot.locator('[data-imgslot-override]').click();
    await admin.waitForTimeout(1200);
    await admin.locator('[data-picker-grid] .admin-picker-cell').nth(2).click();
    await admin.waitForTimeout(400);
    await admin.click('form.admin-form button[type=submit]');
    await admin.waitForLoadState('networkidle');
    await web.goto(B + '/ve-chung-toi', { waitUntil: 'domcontentloaded' });
    const picture = await web.locator('picture source[srcset]').count();
    note('1', 'Ảnh riêng cho mobile', picture > 0,
      'tab Mobile mặc định "' + sharedText + '"; sau khi đặt ảnh riêng web sinh ' + picture + ' thẻ <source> cho màn hẹp');
  } catch (e) { note('1', 'Ảnh riêng cho mobile', false, 'lỗi: ' + e.message.split('\n')[0]); }

  // ---- 1b. Điểm neo ----
  try {
    await admin.goto(B + '/admin/trang/gioi-thieu');
    await admin.locator('[data-focal-target]').first().click({ position: { x: 30, y: 20 } });
    const focalVal = await admin.locator('[data-focal-input]').first().inputValue();
    note('1b', 'Điểm neo ảnh', /^\d+% \d+%$/.test(focalVal), 'bấm vào ảnh xem trước → lưu điểm neo "' + focalVal + '"');
  } catch (e) { note('1b', 'Điểm neo ảnh', false, 'lỗi: ' + e.message.split('\n')[0]); }

  // ---- 2. Dropdown menu ----
  try {
    await admin.goto(B + '/admin/menu');
    const nhanNut = await admin.locator('[data-item-toggle]').first().innerText();
    await admin.locator('[data-item-toggle]').first().click();
    await admin.waitForTimeout(600);
    const t = await admin.locator('body').innerText();
    const tuKhoa = ['Chọn tay sản phẩm', 'Theo bộ sưu tập', 'Liên kết sang danh mục', 'Bán chạy nhất', 'Phiên bản giới hạn'];
    const thay = tuKhoa.filter(k => t.includes(k));
    note('2', 'Sửa dropdown menu', thay.length > 0,
      thay.length ? 'nút "' + nhanNut.replace(/\s+/g, ' ').trim() + '" mở ra: ' + thay.join(', ') : 'không thấy chỗ nào sửa được cột dropdown');
  } catch (e) { note('2', 'Sửa dropdown menu', false, 'lỗi: ' + e.message.split('\n')[0]); }

  // ---- 3. Thư viện ảnh ----
  try {
    await admin.goto(B + '/admin/thu-vien-anh');
    await admin.setInputFiles('input[type=file]', require('path').join(__dirname, '../../src/Hoaii.Web/wwwroot/images/pdp/story.jpg'));
    await admin.click('form[action*="tai-len"] button[type=submit]');
    await admin.waitForLoadState('networkidle');
    await admin.waitForTimeout(2000);
    const broken = await admin.$$eval('img', els => els.filter(i => i.complete && i.naturalWidth === 0).length);
    const total = await admin.locator('.admin-card img').count();
    note('3', 'Thư viện ảnh hiển thị', broken === 0, 'tải 1 ảnh lên; ' + total + ' ảnh trên trang, ' + broken + ' ảnh hỏng');
  } catch (e) { note('3', 'Thư viện ảnh hiển thị', false, 'lỗi: ' + e.message.split('\n')[0]); }

  // ---- 4. Gắn link cho thẻ "Khám phá" ----
  try {
    await admin.goto(B + '/admin/trang-chu');
    const links = await admin.$$eval('a[href$="/sua"]', els => els.map(e => e.getAttribute('href')));
    const tile = links.find(h => h.includes('/o-noi-bat/') && h.endsWith('/sua'));
    let ok = false, ct = 'không thấy trang sửa ô ảnh trong /admin/trang-chu';
    if (tile) {
      await admin.goto(B + tile);
      ok = (await admin.locator('select[name="dest"]').count()) > 0 || (await admin.locator('[data-dest-picker]').count()) > 0;
      ct = ok ? tile + ' có ô chọn đích cho liên kết' : tile + ' không có ô chọn đích';
    }
    note('4', 'Gắn link vào thẻ "Khám phá"', ok, ct);
  } catch (e) { note('4', 'Gắn link vào thẻ "Khám phá"', false, 'lỗi: ' + e.message.split('\n')[0]); }

  // ---- 5. Live chat ----
  try {
    await web.goto(B + '/', { waitUntil: 'domcontentloaded' });
    const zalo = await web.locator('.chat-widget a.bubble--zalo').getAttribute('href');
    const tel = await web.locator('.chat-widget .bubble--phone').getAttribute('onclick');
    note('5', 'Live chat Zalo / hotline', !!zalo && !!tel, 'Zalo → ' + zalo + ' · gọi điện → ' + String(tel).replace(/location\.href=|'/g, ''));
  } catch (e) { note('5', 'Live chat Zalo / hotline', false, 'lỗi: ' + e.message.split('\n')[0]); }

  // ---- 6. Cảnh báo SMTP ----
  try {
    // Cảnh báo phải khớp trạng thái thật, không phải luôn luôn hiện: đã cấu hình SMTP thì
    // KHÔNG được báo đỏ nữa. Kiểm cả hai chiều để phép thử không đỏ oan sau khi shop điền SMTP.
    await admin.goto(B + '/admin/email');
    const trangThaiSmtp = await admin.locator('.admin-badge').first().innerText();
    const daCauHinh = trangThaiSmtp.includes('Đã cấu hình');
    await admin.goto(B + '/admin');
    const warn = await admin.locator('text=Chưa cấu hình email gửi đi').count();
    note('6', 'Cảnh báo email khớp trạng thái SMTP', daCauHinh ? warn === 0 : warn > 0,
      'SMTP: ' + trangThaiSmtp + ' -> Tổng quan ' + (warn ? 'có' : 'không có') + ' cảnh báo');
  } catch (e) { note('6', 'Cảnh báo email chưa cấu hình', false, 'lỗi: ' + e.message.split('\n')[0]); }

  // ---- 7. Chọn nhiều dòng ----
  try {
    await admin.goto(B + '/admin/san-pham');
    // Đưa hai dòng đầu về "Đang bán" trước, để phép thử luôn bắt đầu từ cùng một chỗ.
    const boxes0 = await admin.$$('[data-bulk-item]');
    await boxes0[0].check();
    await boxes0[1].check();
    await admin.click('[data-bulk-do="true"]');
    await admin.waitForLoadState('networkidle');
    const boxes = await admin.$$('[data-bulk-item]');
    await boxes[0].check();
    await boxes[1].check();
    const label = (await admin.locator('[data-bulkbar]').innerText()).replace(/\n/g, ' ');
    const before = await admin.locator('tbody tr').nth(0).locator('.admin-badge').first().innerText();
    await admin.click('[data-bulk-do="false"]');
    await admin.waitForLoadState('networkidle');
    const after = await admin.locator('tbody tr').nth(0).locator('.admin-badge').first().innerText();
    note('7', 'Chọn nhiều dòng để ẩn/hiện', before !== after,
      'thanh hành động: "' + label + '" · dòng đầu: ' + before + ' → ' + after);
  } catch (e) { note('7', 'Chọn nhiều dòng để ẩn/hiện', false, 'lỗi: ' + e.message.split('\n')[0]); }

  // ---- 8. Xuống dòng ----
  try {
    await admin.goto(B + '/admin/danh-muc');
    const catHref = await admin.locator('a[href$="/sua"]').first().getAttribute('href');
    await admin.goto(B + catHref);
    const ta = admin.locator('textarea').first();
    await ta.fill('DONG MOT\nDONG HAI');
    await admin.click('form.admin-form button[type=submit]');
    await admin.waitForLoadState('networkidle');
    await admin.goto(B + catHref);
    const saved = await admin.locator('textarea[name="cms.Description"]').inputValue();
    const slug = await admin.locator('input[name="slug"]').inputValue();
    await web.goto(B + '/danh-muc/' + slug, { waitUntil: 'domcontentloaded' });
    let ws = 'khong thay';
    if (await web.locator('.filter-bar__desc').count()) {
      ws = await web.locator('.filter-bar__desc').evaluate(e => getComputedStyle(e).whiteSpace);
    }
    note('8', 'Giữ xuống dòng khách gõ', saved.includes('\n') && ws === 'pre-line',
      'lưu giữ ' + (saved.match(/\n/g) || []).length + ' dấu xuống dòng; web /danh-muc/' + slug + ' dùng white-space: ' + ws);
  } catch (e) { note('8', 'Giữ xuống dòng khách gõ', false, 'lỗi: ' + e.message.split('\n')[0]); }

  // ---- 9. Pick ảnh câu chuyện ----
  try {
    await admin.goto(B + '/admin/san-pham/20/sua');
    await admin.click('[data-open-picker][data-target="story-img"]');
    await admin.waitForTimeout(1200);
    const cells = await admin.locator('[data-picker-grid] .admin-picker-cell').count();
    await admin.locator('[data-picker-grid] .admin-picker-cell').first().click();
    await admin.waitForTimeout(400);
    const storyVal = await admin.inputValue('#story-img');
    note('9', 'Chọn ảnh câu chuyện từ thư viện', cells > 0 && storyVal.startsWith('/'),
      'bộ chọn liệt kê ' + cells + ' ảnh; chọn xong điền "' + storyVal + '"');
  } catch (e) { note('9', 'Chọn ảnh câu chuyện từ thư viện', false, 'lỗi: ' + e.message.split('\n')[0]); }

  // ---- 10. Ảnh dịch vụ luân phiên ----
  try {
    await web.goto(B + '/', { waitUntil: 'domcontentloaded' });
    const idx = () => web.evaluate(() => {
      const b = document.querySelector('[data-service-slides]');
      return [...b.querySelectorAll('.custom-services__panel-img')].findIndex(e => e.classList.contains('is-active'));
    });
    const i0 = await idx();
    await web.waitForTimeout(3000);
    const i1 = await idx();
    note('10', 'Ảnh dịch vụ chạy luân phiên', i0 !== i1, 'sau 3 giây: ảnh ' + i0 + ' → ảnh ' + i1);
  } catch (e) { note('10', 'Ảnh dịch vụ chạy luân phiên', false, 'lỗi: ' + e.message.split('\n')[0]); }

  // ---- 11. Video sản phẩm ----
  try {
    await web.goto(B + '/san-pham/thien-dieu-lac-hong', { waitUntil: 'domcontentloaded' });
    await web.waitForTimeout(800);
    await web.click('.pdp-gallery__thumb--video');
    await web.waitForTimeout(1500);
    const vDesk = await web.evaluate(() => {
      const v = document.querySelector('[data-video-panel] video, [data-video-panel] iframe');
      const r = v && v.getBoundingClientRect();
      return r ? { rong: Math.round(r.width), cao: Math.round(r.height), the: v.tagName.toLowerCase() } : null;
    });
    await mob.goto(B + '/san-pham/thien-dieu-lac-hong', { waitUntil: 'domcontentloaded' });
    await mob.waitForTimeout(800);
    await mob.click('.pdp-gallery__video-fab');
    await mob.waitForTimeout(1500);
    const vMob = await mob.evaluate(() => {
      const v = document.querySelector('[data-video-panel] video, [data-video-panel] iframe');
      const r = v && v.getBoundingClientRect();
      return r ? { rong: Math.round(r.width), cao: Math.round(r.height) } : null;
    });
    note('11', 'Video sản phẩm', !!vDesk && vDesk.rong > 100 && !!vMob && vMob.rong > 100,
      'desktop <' + (vDesk && vDesk.the) + '> ' + (vDesk && vDesk.rong) + '×' + (vDesk && vDesk.cao) +
      'px · mobile ' + (vMob && vMob.rong) + '×' + (vMob && vMob.cao) + 'px');
  } catch (e) { note('11', 'Video sản phẩm', false, 'lỗi: ' + e.message.split('\n')[0]); }

  // ---- 12. Tài liệu PDF ----
  try {
    await web.goto(B + '/hop-tac', { waitUntil: 'domcontentloaded' });
    const docs = await web.locator('.partners-docs__item').count();
    let href = null, pdfOk = false;
    if (docs) {
      href = await web.locator('.partners-docs__link').first().getAttribute('href');
      const r = await web.request.get(B + href);
      pdfOk = r.status() === 200 && String(r.headers()['content-type']).includes('pdf');
    }
    note('12', 'Tài liệu PDF cho đại lý', docs > 0 && pdfOk,
      docs + ' tài liệu trên trang Hợp tác; tải thử "' + href + '" → ' + (pdfOk ? 'tải được, đúng định dạng PDF' : 'không tải được'));
  } catch (e) { note('12', 'Tài liệu PDF cho đại lý', false, 'lỗi: ' + e.message.split('\n')[0]); }

  console.log('\nLoi JS gap phai: ' + (jsErrors.length ? JSON.stringify(jsErrors.slice(0, 5)) : 'khong co'));
  console.log('TONG: ' + results.filter(r => r.ok).length + '/' + results.length + ' DAT');
  await br.close();
})();
