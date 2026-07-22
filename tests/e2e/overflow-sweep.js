// Quet tran ngang that su: bo qua phan tu bi cha clip, dang an, hoac position:fixed (drawer dong).
const { chromium } = require('playwright');
const R = ['/','/danh-muc/qua-tet','/danh-muc/tra','/qua-theo-dip','/qua-tang-ca-nhan',
  '/san-pham/tinh-hoa-bac-bo','/gio-hang','/thanh-toan','/lien-he','/hop-tac','/blog',
  '/ve-chung-toi','/tim-kiem?q=tra','/chinh-sach/trao-doi'];
(async () => {
  const b = await chromium.launch();
  const loi = [];
  let n = 0;
  for (const vw of [2560,1920,1600,1440,1280,1024,900,768,430,375]) {
    const p = await b.newPage({ viewport: { width: vw, height: 900 } });
    for (const r of R) {
      n++;
      await p.goto('http://localhost:5167' + r, { waitUntil: 'networkidle' });
      const d = await p.evaluate(() => {
        const de = document.documentElement;
        const thua = de.scrollWidth - de.clientWidth;
        if (thua <= 1) return null;
        const res = [];
        document.querySelectorAll('body *').forEach(e => {
          const bb = e.getBoundingClientRect();
          if (bb.right <= de.clientWidth + 1 || bb.width < 1) return;
          const cs = getComputedStyle(e);
          if (cs.visibility === 'hidden' || cs.position === 'fixed' || e.offsetParent === null) return;
          let par = e.parentElement, clip = false;
          while (par && par !== document.body) {
            const o = getComputedStyle(par);
            if (o.position === 'fixed') { clip = true; break; }
            if (['hidden','clip','auto','scroll'].includes(o.overflowX)) { clip = true; break; }
            par = par.parentElement;
          }
          if (!clip) res.push(e.tagName + '.' + (e.className || '').toString().split(' ')[0] + '(' + Math.round(bb.right) + ')');
        });
        return res.length ? { thua, ai: res.slice(0, 2) } : null;
      });
      if (d) loi.push(vw + 'px ' + r + '  thua ' + d.thua + 'px  ← ' + d.ai.join(', '));
    }
    await p.close();
  }
  console.log(loi.length ? 'CON TRAN:\n' + loi.join('\n') : 'OK — ' + n + ' truong hop (14 trang x 10 do rong), khong cho nao tran ngang');
  await b.close();
})();
