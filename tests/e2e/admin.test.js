// Functional tests for the admin area.
const { test, check, eq, has, notHas, summary, session } = require('./lib');

const ADMIN = { email: 'admin@hoaii.vn', password: 'Hoaii@2026' };
const rnd = () => Math.random().toString(36).slice(2, 8);

async function admin() {
  const s = session();
  await s.adminLogin(ADMIN.email, ADMIN.password);
  return s;
}

(async () => {
  console.log('===== ADMIN =====\n');

  await test('Đăng nhập admin: sai mật khẩu bị từ chối, đúng thì vào được', async () => {
    const bad = session();
    const t = await bad.token('/admin/dang-nhap');
    const r = await bad.post('/admin/dang-nhap', { Email: ADMIN.email, Password: 'sai-be-bet', __RequestVerificationToken: t });
    check(!(r.status === 302 && r.location === '/admin'), 'mật khẩu sai không được vào');

    const good = await admin();
    const dash = await good.get('/admin');
    eq(dash.status, 200, 'đăng nhập đúng -> vào dashboard');
  });

  await test('Tất cả trang admin trả 200', async () => {
    const s = await admin();
    const routes = ['/admin', '/admin/bao-cao', '/admin/don-hang', '/admin/san-pham', '/admin/danh-muc',
      '/admin/voucher', '/admin/khach-hang', '/admin/hop-thu', '/admin/hop-thu/ban-buon', '/admin/hop-thu/newsletter',
      '/admin/trang-chu', '/admin/blog', '/admin/chinh-sach', '/admin/trang/gioi-thieu', '/admin/trang/lien-he',
      '/admin/trang/hop-tac', '/admin/trang/khac', '/admin/menu', '/admin/thu-vien-anh', '/admin/cai-dat',
      '/admin/van-chuyen', '/admin/thanh-toan', '/admin/dia-gioi', '/admin/email', '/admin/tai-khoan', '/admin/nhat-ky'];
    for (const p of routes) eq((await s.get(p)).status, 200, `GET ${p}`);
  });

  // ---------- products ----------
  await test('Sản phẩm: tạo -> sửa -> ẩn -> xóa', async () => {
    const s = await admin();
    const slug = 'auto-test-' + rnd();
    let t = await s.token('/admin/san-pham/them');
    const cats = await s.get('/admin/san-pham/them');
    const catId = (cats.body.match(/<option value="(\d+)"/) || [])[1];
    check(!!catId, 'lấy được danh mục');

    let r = await s.post('/admin/san-pham/luu', {
      Id: 0, Name: 'Auto Test SP', Slug: slug, Price: 123000, CategoryId: catId,
      Badge: 'None', SortOrder: 0, IsActive: 'true', __RequestVerificationToken: t,
    });
    eq(r.status, 302, 'tạo sản phẩm -> redirect');

    const list = await s.get('/admin/san-pham?q=Auto Test SP');
    has(list.body, 'Auto Test SP', 'sản phẩm mới có trong danh sách');
    const id = (list.body.match(/\/admin\/san-pham\/(\d+)\/sua/) || [])[1];
    check(!!id, 'lấy được id sản phẩm');

    // sửa tên
    t = await s.token(`/admin/san-pham/${id}/sua`);
    r = await s.post('/admin/san-pham/luu', {
      Id: id, Name: 'Auto Test SP Sửa', Slug: slug, Price: 150000, CategoryId: catId,
      Badge: 'None', SortOrder: 0, IsActive: 'true', __RequestVerificationToken: t,
    });
    eq(r.status, 302, 'sửa -> redirect');
    has((await s.get(`/admin/san-pham/${id}/sua`)).body, 'Auto Test SP Sửa', 'tên đã đổi');

    // storefront thấy sản phẩm
    eq((await s.get('/san-pham/' + slug)).status, 200, 'storefront xem được');

    // ẩn -> storefront vẫn 200? (kiểm tra hành vi)
    t = await s.token('/admin/san-pham');
    await s.post(`/admin/san-pham/${id}/an-hien`, { __RequestVerificationToken: t });
    const hidden = await s.get('/san-pham/' + slug);
    check(hidden.status === 404, `sản phẩm ẩn phải 404 ở storefront (nhận ${hidden.status})`);

    // xóa
    t = await s.token('/admin/san-pham');
    r = await s.post(`/admin/san-pham/${id}/xoa`, { __RequestVerificationToken: t });
    eq(r.status, 302, 'xóa -> redirect');
    notHas((await s.get('/admin/san-pham?q=Auto Test SP')).body, 'Auto Test SP Sửa', 'đã xóa khỏi danh sách');
  });

  await test('Sản phẩm: slug trùng bị chặn', async () => {
    const s = await admin();
    const t = await s.token('/admin/san-pham/them');
    const catId = ((await s.get('/admin/san-pham/them')).body.match(/<option value="(\d+)"/) || [])[1];
    const r = await s.post('/admin/san-pham/luu', {
      Id: 0, Name: 'Trùng slug', Slug: 'thien-dieu-lac-hong', Price: 1000, CategoryId: catId,
      Badge: 'None', SortOrder: 0, IsActive: 'true', __RequestVerificationToken: t,
    });
    // Ô tìm kiếm echo lại từ khóa vào value="..." nên phải đếm link sửa trong bảng.
    const after = await s.get('/admin/san-pham?q=' + encodeURIComponent('Trùng slug'));
    const rows = (after.body.match(/admin-row-link" href="\/admin\/san-pham\/\d+\/sua"/g) || []).length;
    eq(rows, 0, 'không tạo được sản phẩm với slug đã tồn tại');
  });

  // ---------- order workflow ----------
  await test('Đơn hàng: luồng trạng thái Pending->Confirmed->Shipping->Delivered', async () => {
    // đặt 1 đơn mới từ storefront
    const c = session();
    const tp = await c.token('/san-pham/thien-dieu-lac-hong');
    await c.post('/gio-hang/them', { productId: 20, variantId: 100, quantity: 1, __RequestVerificationToken: tp });
    const tc = await c.token('/thanh-toan');
    const placed = await c.post('/thanh-toan/dat-hang', {
      Email: 'auto-flow@example.com', FirstName: 'Flow', LastName: 'Test', Address: '1 X',
      ProvinceDistrictWard: 'HN', Phone: '0900000002', ShippingMethod: 'InnerCity',
      PaymentMethod: 'CashOnDelivery', __RequestVerificationToken: tc,
    });
    const orderNo = (placed.location || '').split('/').pop();
    check(!!orderNo, 'đặt được đơn: ' + orderNo);

    const s = await admin();
    const list = await s.get('/admin/don-hang?q=' + orderNo);
    has(list.body, orderNo, 'admin thấy đơn mới');
    const id = (list.body.match(/\/admin\/don-hang\/(\d+)/) || [])[1];

    for (const to of ['Confirmed', 'Shipping', 'Delivered']) {
      const t = await s.token(`/admin/don-hang/${id}`);
      const r = await s.post(`/admin/don-hang/${id}/trang-thai`, { to, note: 'auto', __RequestVerificationToken: t });
      eq(r.status, 302, `đổi sang ${to}`);
      const d = await s.get(`/admin/don-hang/${id}`);
      check(!/AdminError/.test(d.body), `không lỗi khi sang ${to}`);
    }
    const final = await s.get(`/admin/don-hang/${id}`);
    has(final.body, 'Đã giao', 'đơn ở trạng thái Đã giao');
    has(final.body, 'Đã thanh toán', 'COD giao xong tự đánh dấu đã thanh toán');
  });

  await test('Đơn hàng: nhảy cóc trạng thái bị chặn (Pending -> Delivered)', async () => {
    const c = session();
    const tp = await c.token('/san-pham/thien-dieu-lac-hong');
    await c.post('/gio-hang/them', { productId: 20, variantId: 100, quantity: 1, __RequestVerificationToken: tp });
    const tc = await c.token('/thanh-toan');
    const placed = await c.post('/thanh-toan/dat-hang', {
      Email: 'auto-skip@example.com', FirstName: 'Skip', LastName: 'Test', Address: '1 X',
      ProvinceDistrictWard: 'HN', Phone: '0900000003', ShippingMethod: 'InnerCity',
      PaymentMethod: 'CashOnDelivery', __RequestVerificationToken: tc,
    });
    const orderNo = (placed.location || '').split('/').pop();
    const s = await admin();
    const list = await s.get('/admin/don-hang?q=' + orderNo);
    const id = (list.body.match(/\/admin\/don-hang\/(\d+)/) || [])[1];
    const t = await s.token(`/admin/don-hang/${id}`);
    await s.post(`/admin/don-hang/${id}/trang-thai`, { to: 'Delivered', note: '', __RequestVerificationToken: t });
    const d = await s.get(`/admin/don-hang/${id}`);
    notHas(d.body, 'Đã giao', 'không cho nhảy thẳng từ Chờ xác nhận sang Đã giao');
    has(d.body, 'Chờ xác nhận', 'đơn vẫn giữ nguyên trạng thái cũ');
  });

  // ---------- vouchers ----------
  await test('Voucher: tạo -> áp dụng được -> tắt -> hết áp dụng', async () => {
    const s = await admin();
    const code = 'AUTO' + rnd().toUpperCase();
    let t = await s.token('/admin/voucher/them');
    await s.post('/admin/voucher/luu', {
      id: 0, code, label: 'Auto voucher', tag: 'Test', type: 'FixedAmount', value: 50000,
      minOrderAmount: 0, usageLimit: '', startsAt: '', expiresAt: '', isActive: 'true', __RequestVerificationToken: t,
    });
    has((await s.get('/admin/voucher')).body, code, 'voucher mới có trong danh sách');

    const c = session();
    const tp = await c.token('/san-pham/thien-dieu-lac-hong');
    await c.post('/gio-hang/them', { productId: 20, variantId: 100, quantity: 1, __RequestVerificationToken: tp });
    let tc = await c.token('/thanh-toan');
    await c.post('/gio-hang/ap-dung-ma', { code, returnUrl: '/thanh-toan', __RequestVerificationToken: tc });
    has((await c.get('/thanh-toan')).body, '50.000', 'áp dụng được, giảm 50.000');

    // tắt voucher -> giỏ không còn giảm
    const vl = await s.get('/admin/voucher');
    const vid = (vl.body.match(new RegExp(`${code}[\\s\\S]{0,400}?/admin/voucher/(\\d+)/an-hien`)) || vl.body.match(/\/admin\/voucher\/(\d+)\/an-hien/) || [])[1];
    t = await s.token('/admin/voucher');
    await s.post(`/admin/voucher/${vid}/an-hien`, { __RequestVerificationToken: t });
    notHas((await c.get('/thanh-toan')).body, 'Đã áp dụng', 'voucher tắt -> giỏ hết giảm');

    t = await s.token('/admin/voucher');
    await s.post(`/admin/voucher/${vid}/xoa`, { __RequestVerificationToken: t });
  });

  await test('Voucher: mã trùng bị chặn', async () => {
    const s = await admin();
    const t = await s.token('/admin/voucher/them');
    await s.post('/admin/voucher/luu', {
      id: 0, code: 'GIAM20', label: 'trùng', tag: 'x', type: 'Percentage', value: 5,
      minOrderAmount: 0, usageLimit: '', startsAt: '', expiresAt: '', isActive: 'true', __RequestVerificationToken: t,
    });
    const list = await s.get('/admin/voucher');
    const count = (list.body.match(/>GIAM20</g) || []).length;
    check(count <= 1, `chỉ được 1 mã GIAM20 (đếm ${count})`);
  });

  // ---------- blog ----------
  await test('Blog: tạo nháp -> ẩn khỏi web -> đăng -> hiện', async () => {
    const s = await admin();
    const slug = 'auto-blog-' + rnd();
    let t = await s.token('/admin/blog/them');
    await s.post('/admin/blog/luu', {
      id: 0, title: 'Auto Blog Test', slug, category: 'Đời sống', author: 'Auto',
      excerpt: 'tóm tắt auto', content: 'nội dung auto', imageUrl: '',
      publishedAt: '2026-07-16', __RequestVerificationToken: t,  // không tick isPublished -> nháp
    });
    eq((await s.get('/blog/' + slug)).status, 404, 'bài nháp không hiện trên web');

    const list = await s.get('/admin/blog');
    const id = (list.body.match(new RegExp(`/admin/blog/(\\d+)/sua`)) || [])[1];
    t = await s.token('/admin/blog');
    await s.post(`/admin/blog/${id}/an-hien`, { __RequestVerificationToken: t });

    // tìm đúng bài vừa tạo rồi bật
    const l2 = await s.get('/admin/blog');
    const m = l2.body.match(new RegExp(`/admin/blog/(\\d+)/sua"[^>]*>\\s*Auto Blog Test`));
    const myId = m ? m[1] : null;
    if (myId) {
      t = await s.token('/admin/blog');
      const cur = await s.get(`/admin/blog/${myId}/sua`);
      const isPub = /name="isPublished"[^>]*checked/.test(cur.body);
      if (!isPub) { t = await s.token('/admin/blog'); await s.post(`/admin/blog/${myId}/an-hien`, { __RequestVerificationToken: t }); }
      const pub = await s.get('/blog/' + slug);
      eq(pub.status, 200, 'bật đăng -> bài hiện trên web');
      has(pub.body, 'nội dung auto', 'trang chi tiết render nội dung');
      t = await s.token('/admin/blog');
      await s.post(`/admin/blog/${myId}/xoa`, { __RequestVerificationToken: t });
    }
  });

  // ---------- media upload security ----------
  await test('Upload ảnh: từ chối file không phải ảnh (đổi đuôi .jpg)', async () => {
    const s = await admin();
    const t = await s.token('/admin/thu-vien-anh');
    const fd = new FormData();
    fd.append('files', new Blob([Buffer.from('MZ\x90\x00 fake exe payload')], { type: 'image/jpeg' }), 'virus.jpg');
    fd.append('json', 'true');
    fd.append('__RequestVerificationToken', t);
    const r = await s.post('/admin/thu-vien-anh/tai-len', fd, { headers: { 'x-requested-with': 'XMLHttpRequest' } });
    check(r.status !== 500, 'không 500');
    const data = JSON.parse(r.body || '{}');
    check((data.uploaded || []).length === 0, 'không nhận file giả mạo');
    check((data.errors || []).length > 0, 'có báo lỗi rõ ràng');
  });

  await test('Upload ảnh: chặn file quá 5MB', async () => {
    const s = await admin();
    const t = await s.token('/admin/thu-vien-anh');
    // 5.5MB — nằm giữa 5MB (giới hạn app) và 6MB (giới hạn request), để lấy thông báo thân thiện.
    const buf = Buffer.alloc(Math.floor(5.5 * 1024 * 1024));
    buf[0] = 0xFF; buf[1] = 0xD8; buf[2] = 0xFF; // JPEG magic — nhưng bị chặn theo cỡ trước khi giải mã
    const fd = new FormData();
    fd.append('files', new Blob([buf], { type: 'image/jpeg' }), 'big.jpg');
    fd.append('json', 'true');
    fd.append('__RequestVerificationToken', t);
    const r = await s.post('/admin/thu-vien-anh/tai-len', fd, { headers: { 'x-requested-with': 'XMLHttpRequest' } });
    const data = JSON.parse(r.body || '{}');
    check((data.uploaded || []).length === 0, 'file quá cỡ không được nhận');
    check((data.errors || []).some(e => e.includes('5MB')), 'báo lỗi "vượt quá 5MB"');
  });

  // ---------- settings / shipping / payment ----------
  await test('Cài đặt: sửa hotline -> storefront đổi -> trả lại', async () => {
    const s = await admin();
    let t = await s.token('/admin/cai-dat');
    await s.post('/admin/cai-dat', { 'settings_[hotline]': '0123.456.789', __RequestVerificationToken: t });
    has((await s.get('/')).body, '0123.456.789', 'hotline mới hiện trên web');
    t = await s.token('/admin/cai-dat');
    await s.post('/admin/cai-dat', { 'settings_[hotline]': '0941.686.682', __RequestVerificationToken: t });
    has((await s.get('/')).body, '0941.686.682', 'trả lại hotline cũ');
  });

  await test('Vận chuyển: phí nội thành áp vào checkout', async () => {
    const s = await admin();
    let t = await s.token('/admin/van-chuyen');
    await s.post('/admin/van-chuyen', {
      'settings_[shipping_inner_city]': '25000', 'settings_[shipping_intercity]': '45000',
      'settings_[free_ship_threshold]': '0', __RequestVerificationToken: t,
    });
    const c = session();
    const tp = await c.token('/san-pham/thien-dieu-lac-hong');
    await c.post('/gio-hang/them', { productId: 20, variantId: 100, quantity: 1, __RequestVerificationToken: tp });
    has((await c.get('/thanh-toan')).body, '25.000', 'checkout hiện phí ship 25.000');

    // ngưỡng miễn phí
    t = await s.token('/admin/van-chuyen');
    await s.post('/admin/van-chuyen', {
      'settings_[shipping_inner_city]': '25000', 'settings_[shipping_intercity]': '45000',
      'settings_[free_ship_threshold]': '100000', __RequestVerificationToken: t,
    });
    has((await c.get('/thanh-toan')).body, 'Miễn phí', 'đơn vượt ngưỡng -> miễn phí ship');

    // trả về 0
    t = await s.token('/admin/van-chuyen');
    await s.post('/admin/van-chuyen', {
      'settings_[shipping_inner_city]': '0', 'settings_[shipping_intercity]': '0',
      'settings_[free_ship_threshold]': '0', __RequestVerificationToken: t,
    });
  });

  await test('Thanh toán: tắt COD -> checkout không còn lựa chọn đó', async () => {
    const s = await admin();
    let t = await s.token('/admin/thanh-toan');
    await s.post('/admin/thanh-toan', { 'settings_[pay_bank_enabled]': 'true', __RequestVerificationToken: t }); // COD off
    const c = session();
    const tp = await c.token('/san-pham/thien-dieu-lac-hong');
    await c.post('/gio-hang/them', { productId: 20, variantId: 100, quantity: 1, __RequestVerificationToken: tp });
    notHas((await c.get('/thanh-toan')).body, 'value="CashOnDelivery"', 'tắt COD -> ẩn lựa chọn COD');
    t = await s.token('/admin/thanh-toan');
    await s.post('/admin/thanh-toan', { 'settings_[pay_cod_enabled]': 'true', 'settings_[pay_bank_enabled]': 'true', __RequestVerificationToken: t });
    has((await c.get('/thanh-toan')).body, 'value="CashOnDelivery"', 'bật lại -> COD trở lại');
  });

  // ---------- CMS ----------
  await test('CMS trang chủ: sửa tiêu đề mục -> web đổi -> trả lại', async () => {
    const s = await admin();
    let t = await s.token('/admin/trang-chu');
    await s.post('/admin/trang-chu/chu-khung', { 'f[customers_heading]': 'AUTOTEST KHACH HANG', __RequestVerificationToken: t });
    has((await s.get('/')).body, 'AUTOTEST KHACH HANG', 'web hiện tiêu đề mới');
    t = await s.token('/admin/trang-chu');
    await s.post('/admin/trang-chu/chu-khung', { 'f[customers_heading]': '', __RequestVerificationToken: t });
    has((await s.get('/')).body, 'KHÁCH HÀNG CỦA CHÚNG TÔI', 'để trống -> về mặc định');
  });

  await test('CMS chính sách: sửa nội dung -> trang chính sách đổi', async () => {
    const s = await admin();
    const idx = await s.get('/admin/chinh-sach');
    const id = (idx.body.match(/\/admin\/chinh-sach\/(\d+)\/sua/) || [])[1];
    const ed = await s.get(`/admin/chinh-sach/${id}/sua`);
    const slug = (ed.body.match(/name="slug" value="([^"]+)"/) || [])[1];
    const title = (ed.body.match(/name="title" value="([^"]+)"/) || [])[1];
    const nav = (ed.body.match(/name="navLabel" value="([^"]+)"/) || [])[1];
    const t = await s.token(`/admin/chinh-sach/${id}/sua`);
    const body = new URLSearchParams();
    body.set('id', id); body.set('title', title); body.set('navLabel', nav); body.set('slug', slug);
    body.set('breadcrumbLabel', ''); body.set('sortOrder', '0'); body.set('isPublished', 'true');
    body.append('blockKinds', 'Paragraph'); body.append('blockTexts', 'AUTOTEST CHINH SACH');
    body.set('__RequestVerificationToken', t);
    await s.post('/admin/chinh-sach/luu', Object.fromEntries(body));
    has((await s.get('/chinh-sach/' + slug)).body, 'AUTOTEST CHINH SACH', 'trang chính sách hiện nội dung mới');
  });

  // ---------- permissions ----------
  await test('Phân quyền: Nhân viên không vào được mục Chủ shop', async () => {
    const s = await admin();
    const email = `staff-${rnd()}@hoaii.vn`;
    let t = await s.token('/admin/tai-khoan');
    const r = await s.post('/admin/tai-khoan/luu', {
      id: 0, email, fullName: 'Staff Auto', password: 'Staff@2026', role: 'Staff', isActive: 'true', __RequestVerificationToken: t,
    });
    check(r.status === 302, 'tạo tài khoản nhân viên');

    const st = session();
    const lr = await st.adminLogin(email, 'Staff@2026');
    check(lr.status === 302, 'nhân viên đăng nhập được');
    eq((await st.get('/admin')).status, 200, 'nhân viên vào được dashboard');

    for (const p of ['/admin/tai-khoan', '/admin/nhat-ky']) {
      const res = await st.get(p);
      check(res.status !== 200, `nhân viên KHÔNG được vào ${p} (nhận ${res.status})`);
    }
    // nhân viên không được xóa sản phẩm
    const tt = await st.token('/admin/san-pham');
    const del = await st.post('/admin/san-pham/20/xoa', { __RequestVerificationToken: tt });
    check(del.status !== 302 || !/san-pham$/.test(del.location || ''), `nhân viên không xóa được sản phẩm (nhận ${del.status})`);
    eq((await s.get('/san-pham/thien-dieu-lac-hong')).status, 200, 'sản phẩm vẫn còn');

    // dọn
    const list = await s.get('/admin/tai-khoan');
    const uid = (list.body.match(new RegExp(`${email.replace(/[.*+?^${}()|[\]\\]/g, '\\$&')}[\\s\\S]{0,400}?/admin/tai-khoan/(\\d+)/xoa`)) || [])[1];
    if (uid) { const t2 = await s.token('/admin/tai-khoan'); await s.post(`/admin/tai-khoan/${uid}/xoa`, { __RequestVerificationToken: t2 }); }
  });

  // ---------- inbox ----------
  await test('Hộp thư: form web vào đúng hộp thư admin', async () => {
    const c = session();
    const marker = 'AUTOINBOX-' + rnd();
    const t = await c.token('/lien-he');
    await c.post('/lien-he', { FirstName: 'Inbox', LastName: 'Test', Email: 'auto-inbox@example.com', Phone: '0900', Message: marker, __RequestVerificationToken: t });
    const s = await admin();
    has((await s.get('/admin/hop-thu')).body, marker, 'admin thấy tin nhắn vừa gửi');
  });

  process.exit(summary('ADMIN') > 0 ? 1 : 0);
})();
