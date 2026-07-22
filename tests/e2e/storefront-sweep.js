const { chromium } = require('playwright');
const routes = ['/', '/qua-theo-dip', '/qua-tang-ca-nhan', '/danh-muc/qua-tet', '/danh-muc/qua-trung-thu',
  '/danh-muc/ngay-le-tinh-yeu', '/danh-muc/qua-tang-bo-me', '/danh-muc/san-pham-chon-loc',
  '/san-pham/ma-dao-thanh-cong', '/gio-hang', '/blog', '/ve-chung-toi', '/hop-tac', '/lien-he',
  '/tai-khoan/dang-nhap', '/tim-kiem?q=qua','/chinh-sach/trao-doi','/chinh-sach/giao-hang','/chinh-sach/bao-mat','/chinh-sach/dieu-khoan-su-dung','/blog/goi-y-chon-qua-tang-nguoi-than','/thanh-toan'];
(async () => {
  const b = await chromium.launch();
  let fails = 0;
  for (const w of [1920, 430]) {
    console.log('--- ' + w + 'px ---');
    const p = await b.newPage({ viewport: { width: w, height: w === 1920 ? 1080 : 932 } });
    for (const r of routes) {
      const bad = [], errs = [];
      const onResp = res => { if (res.status() >= 400) bad.push(res.status() + ' ' + res.url().replace('http://localhost:5167', '')); };
      const onErr = m => { if (m.type() === 'error') errs.push(m.text().slice(0, 80)); };
      p.on('response', onResp); p.on('console', onErr);
      let status = 0;
      try { const resp = await p.goto('http://localhost:5167' + r, { waitUntil: 'networkidle', timeout: 30000 }); status = resp.status(); }
      catch (e) { status = -1; errs.push('NAV ' + e.message.slice(0, 60)); }
      const m = await p.evaluate(() => ({
        ovf: document.documentElement.scrollWidth > window.innerWidth
          ? document.documentElement.scrollWidth + '>' + window.innerWidth : '',
        footers: document.querySelectorAll('footer').length,
        h1: document.querySelectorAll('h1').length,
      })).catch(() => ({ ovf: '?', footers: -1, h1: -1 }));
      p.off('response', onResp); p.off('console', onErr);
      const issues = [];
      if (status !== 200) issues.push('HTTP ' + status);
      if (m.ovf) issues.push('OVERFLOW ' + m.ovf);
      if (m.footers !== 1) issues.push('footers=' + m.footers);
      if (m.h1 !== 1) issues.push('h1=' + m.h1);
      if (bad.length) issues.push('req: ' + bad.slice(0, 2).join(', '));
      if (errs.length) issues.push('js: ' + errs.slice(0, 2).join(', '));
      if (issues.length) { fails++; console.log('  X ' + r.padEnd(34) + issues.join(' | ')); }
      else console.log('  . ' + r);
    }
    await p.close();
  }
  console.log(fails === 0 ? '\nTAT CA OK' : '\n' + fails + ' truong hop co van de');
  await b.close();
})();
