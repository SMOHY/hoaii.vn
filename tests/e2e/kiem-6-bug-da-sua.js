// Kiểm 6 lỗi đã sửa ngày 21/08/2026, bằng ĐÚNG phép đo đã dùng để tái hiện chúng.
// Chạy: khởi động app ở cổng 5167 rồi `node tests/e2e/kiem-6-bug-da-sua.js`.
// Chi tiết từng lỗi và nguyên nhân gốc: warringFaild.md, WF-041 → WF-046.
const { chromium } = require('playwright');
const fs = require('fs');
const path = require('path');
const B = 'http://localhost:5167';
const TMP = 'c:/Users/longp/AppData/Local/Temp/claude/c--Users-longp-Desktop-Webmoi/e085ff4a-f385-4510-baf5-842ef5e090f0/scratchpad';

const kq = [];
function bao(ma, ten, dat, chiTiet) {
  kq.push(dat);
  console.log((dat ? 'DAT  | ' : 'HONG | ') + ma + ' ' + ten + ' — ' + chiTiet);
}

(async () => {
  const br = await chromium.launch();
  const p = await br.newPage({ viewport: { width: 1440, height: 1000 } });
  const loiJs = [];
  p.on('pageerror', e => loiJs.push(e.message));
  p.on('console', m => { if (m.type() === 'error' && !/404|Failed to load resource/.test(m.text())) loiJs.push('console: ' + m.text()); });

  await p.goto(B + '/admin/dang-nhap');
  await p.fill('input[name="Email"]', 'admin@hoaii.vn');
  await p.fill('input[name="Password"]', 'Hoaii@2026');
  await p.click('form button[type=submit]');
  await p.waitForLoadState('networkidle');

  // ===== ② tệp quá cỡ: trình duyệt chặn ngay, không treo =====
  const f70 = path.join(TMP, 'thu-70.mp4');
  if (!fs.existsSync(f70)) {
    const head = Buffer.from([0, 0, 0, 0x18, 0x66, 0x74, 0x79, 0x70, 0x69, 0x73, 0x6f, 0x6d]);
    fs.writeFileSync(f70, Buffer.concat([head, Buffer.alloc(70 * 1024 * 1024, 0)]));
  }
  await p.goto(B + '/admin/san-pham/20/sua');
  await p.waitForTimeout(500);
  let canhBao = null;
  p.once('dialog', async d => { canhBao = d.message(); await d.accept(); });
  await p.setInputFiles('input[name="VideoFile"]', f70);
  await p.waitForTimeout(1500);
  const oTepConGi = await p.evaluate(() => document.querySelector('input[name="VideoFile"]').files.length);
  const toast = await p.locator('.toast, [data-toast], .hoaii-toast').first().innerText().catch(() => '');
  bao('②', 'Video quá cỡ bị chặn ngay tại trình duyệt',
      oTepConGi === 0,
      'chọn tệp 70MB -> ô tệp còn ' + oTepConGi + ' tệp | báo: "' + String(canhBao || toast).slice(0, 70) + '"');

  // ===== ③ sai định dạng -> chữ đã gõ còn nguyên =====
  await p.goto(B + '/admin/san-pham/20/sua');
  await p.waitForTimeout(400);
  const tenGoc = await p.inputValue('input[name="Name"]');
  const CHU = 'CHU VUA GO ' + Date.now();
  await p.fill('input[name="Name"]', CHU);
  await p.fill('textarea[name="StoryBody"]', CHU + ' -- doan mo ta dai');
  const anhGia = path.join(TMP, 'khong-phai-video.mp4');
  if (!fs.existsSync(anhGia)) {
    fs.copyFileSync('c:/Users/longp/Desktop/Webmoi/hoaii.vn/src/Hoaii.Web/wwwroot/images/pdp/story.jpg', anhGia);
  }
  await p.setInputFiles('input[name="VideoFile"]', anhGia);
  await p.click('form[data-product-form] button[type=submit]');
  await p.waitForLoadState('networkidle');
  await p.waitForTimeout(700);
  const tenSau = await p.inputValue('input[name="Name"]').catch(() => '');
  const moTaSau = await p.inputValue('textarea[name="StoryBody"]').catch(() => '');
  const bcLoi = (await p.locator('body').innerText()).split('\n').filter(t => /Chỉ nhận video/i.test(t))[0] || '(khong thay)';
  bao('③', 'Sai định dạng video -> giữ nguyên chữ đã gõ',
      tenSau === CHU && moTaSau.startsWith(CHU),
      'tên: "' + tenSau.slice(0, 30) + '" | mô tả giữ: ' + moTaSau.startsWith(CHU) + ' | báo: "' + bcLoi.slice(0, 45) + '"');

  // trả lại tên gốc
  await p.goto(B + '/admin/san-pham/20/sua');
  await p.fill('input[name="Name"]', tenGoc);
  await p.click('form[data-product-form] button[type=submit]');
  await p.waitForLoadState('networkidle');

  // ===== ④ link m.youtube -> KHÔNG xoá link cũ =====
  await p.goto(B + '/admin/san-pham/20/sua');
  await p.fill('input[name="VideoEmbedUrl"]', 'https://www.youtube.com/watch?v=dQw4w9WgXcQ');
  await p.click('form[data-product-form] button[type=submit]');
  await p.waitForLoadState('networkidle');
  const linkTruoc = await p.inputValue('input[name="VideoEmbedUrl"]');

  await p.fill('input[name="VideoEmbedUrl"]', 'https://khong-phai-youtube.example.com/abc');
  await p.click('form[data-product-form] button[type=submit]');
  await p.waitForLoadState('networkidle');
  await p.waitForTimeout(600);
  const bcSai = (await p.locator('body').innerText()).split('\n').filter(t => /Không nhận ra link/i.test(t))[0] || '(khong thay)';
  await p.goto(B + '/admin/san-pham/20/sua');
  const linkSau = await p.inputValue('input[name="VideoEmbedUrl"]');
  bao('④', 'Link sai KHÔNG xoá mất link đang chạy',
      linkSau === linkTruoc && linkTruoc !== '',
      'trước: "' + linkTruoc.slice(-15) + '" sau: "' + (linkSau || '(TRONG)').slice(-15) + '" | báo: "' + bcSai.slice(0, 50) + '"');

  // link điện thoại m.youtube phải được NHẬN
  await p.fill('input[name="VideoEmbedUrl"]', 'https://m.youtube.com/watch?v=abc123XYZ');
  await p.click('form[data-product-form] button[type=submit]');
  await p.waitForLoadState('networkidle');
  await p.goto(B + '/admin/san-pham/20/sua');
  const linkMobile = await p.inputValue('input[name="VideoEmbedUrl"]');
  bao('④b', 'Link m.youtube.com được chấp nhận',
      linkMobile.includes('m.youtube.com'),
      'lưu được: "' + linkMobile + '"');

  // ===== ⑤ ảnh hỏng -> hiện chữ, không im lặng =====
  await p.goto(B + '/admin/thu-vien-anh');
  await p.waitForTimeout(2500);
  const nhanThieuTep = await p.locator('text=/Không tìm thấy tệp trên máy chủ/i').count();
  const epLoi = await p.evaluate(() => {
    const img = document.querySelector('.admin-card img');
    if (!img) return 'khong co img';
    img.setAttribute('src', '/uploads/chac-chan-khong-co-that.webp');
    return new Promise(r => setTimeout(() => {
      const span = img.nextElementSibling;
      r((document.body.innerText.includes('Tải ảnh hỏng') ? 'hien chu' : 'KHONG hien') +
        ' | img.hidden=' + img.hidden + ' | span.hidden=' + (span ? span.hidden : '?'));
    }, 1500));
  });
  bao('⑤', 'Ảnh tải hỏng có báo ra chữ',
      String(epLoi).startsWith('hien chu'),
      'nhãn "thiếu tệp" ở máy chủ: ' + nhanThieuTep + ' | ép lỗi: ' + epLoi);

  // ===== ⑥ tràn ngang =====
  const tran = [];
  for (const w of [1920, 1440, 1366, 1024, 900, 820, 768, 600, 430]) {
    const pg = await br.newPage({ viewport: { width: w, height: 900 } });
    let xau = 0;
    for (const t of ['/', '/san-pham/tinh-hoa-bac-bo', '/san-pham/thien-dieu-lac-hong', '/hop-tac', '/danh-muc/qua-tet', '/ve-chung-toi']) {
      await pg.goto(B + t, { waitUntil: 'domcontentloaded' });
      await pg.waitForTimeout(500);
      const d = await pg.evaluate(() => ({ c: document.documentElement.scrollWidth, k: document.documentElement.clientWidth }));
      if (d.c > d.k + 1) xau++;
    }
    if (xau) tran.push(w + 'px(' + xau + ' trang)');
    await pg.close();
  }
  bao('⑥', 'Không tràn ngang ở 9 bề rộng x 6 trang',
      tran.length === 0,
      tran.length ? 'CON TRAN: ' + tran.join(', ') : '54 phép đo, 0 trang tràn');

  console.log('\nloi JS gap phai: ' + (loiJs.length ? loiJs.slice(0, 4).join(' / ') : 'khong co'));
  console.log('TONG: ' + kq.filter(Boolean).length + '/' + kq.length + ' DAT');
  await br.close();
})();
