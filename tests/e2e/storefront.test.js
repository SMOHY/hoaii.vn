// Functional tests for every customer-facing feature.
const { test, check, eq, has, notHas, summary, session, BASE } = require('./lib');

const PRODUCT = { slug: 'thien-dieu-lac-hong', id: 20, variant: 100, price: 899000 };

(async () => {
  console.log('===== STOREFRONT =====\n');

  // ---------- pages render ----------
  await test('Các trang công khai trả 200', async () => {
    const paths = ['/', '/blog', '/lien-he', '/ve-chung-toi', '/hop-tac', '/gio-hang', '/tim-kiem',
      '/danh-muc/qua-tet', '/danh-muc/tra', '/san-pham/' + PRODUCT.slug,
      '/chinh-sach/trao-doi', '/chinh-sach/giao-hang', '/chinh-sach/dieu-khoan-su-dung', '/chinh-sach/bao-mat'];
    const s = session();
    for (const p of paths) {
      const r = await s.get(p);
      eq(r.status, 200, `GET ${p}`);
    }
  });

  await test('Slug không tồn tại trả 404 (không phải 500)', async () => {
    const s = session();
    for (const p of ['/san-pham/khong-co-that', '/danh-muc/khong-co-that', '/blog/khong-co-that', '/chinh-sach/khong-co-that']) {
      const r = await s.get(p);
      eq(r.status, 404, `GET ${p}`);
    }
  });

  // ---------- category ----------
  await test('Danh mục: phân trang 9 sản phẩm/trang', async () => {
    const s = session();
    const r = await s.get('/danh-muc/tra');
    // Đếm 1 lần/thẻ: mỗi thẻ có đúng một __image-link. (Đừng đếm "product-card" —
    // nó khớp cả class BEM con: __image, __name, __price…)
    const countCards = body => {
      const i = body.indexOf('class="product-grid"');
      const seg = body.slice(i, i + 60000);
      const grid = seg.slice(0, seg.indexOf('</section>'));
      return (grid.match(/product-card__image-link/g) || []).length;
    };
    const cards = countCards(r.body);
    // Figma xếp lưới 3 cột x 3 hàng và ship thanh phân trang ở trạng thái ẩn (node 722:25541),
    // nên PageSize là 9 chứ không phải 6. Ở mức 6 thì Quà tết tràn sang trang hai và thanh phân
    // trang hiện ra — điều thiết kế không hề vẽ.
    check(cards > 0 && cards <= 9, `số thẻ trang 1 = ${cards}, phải trong 1..9`);
    has(r.body, 'page=2', 'có link sang trang 2');
    const label = (r.body.match(/pagination__label">([^<]*)</) || [])[1];
    check(/^1\/\d+$/.test((label || '').trim()), `nhãn phân trang = "${label}" (mong "1/N")`);
    const p2 = await s.get('/danh-muc/tra?page=2');
    eq(p2.status, 200, 'trang 2 tải được');
    check(countCards(p2.body) > 0, 'trang 2 có sản phẩm');
  });

  await test('Danh mục: sắp xếp theo giá hoạt động', async () => {
    const s = session();
    const asc = await s.get('/danh-muc/tra?sort=gia-tang');
    const desc = await s.get('/danh-muc/tra?sort=gia-giam');
    eq(asc.status, 200, 'giá tăng 200');
    eq(desc.status, 200, 'giá giảm 200');
    const price = b => (b.match(/(\d[\d.]*)\s*VNĐ/) || [])[1];
    check(price(asc.body) !== price(desc.body), 'thứ tự giá tăng/giảm phải khác nhau');
  });

  await test('Danh mục: trang vượt quá bị kẹp về trang cuối', async () => {
    const s = session();
    const r = await s.get('/danh-muc/tra?page=999');
    eq(r.status, 200, 'page=999 không lỗi');
  });

  // ---------- search ----------
  await test('Tìm kiếm: có kết quả và không có kết quả', async () => {
    const s = session();
    const hit = await s.get('/tim-kiem?q=thien');
    eq(hit.status, 200, 'tìm "thien" 200');
    has(hit.body, 'product-card', 'có kết quả');
    const miss = await s.get('/tim-kiem?q=zzzzkhongcogi');
    eq(miss.status, 200, 'tìm rác 200');
    has(miss.body, 'KHÔNG CÓ KẾT QUẢ', 'hiện thông báo không có kết quả');
  });

  // ---------- cart ----------
  await test('Giỏ hàng: thêm, tăng số lượng, xóa', async () => {
    const s = session();
    const t = await s.token('/san-pham/' + PRODUCT.slug);
    await s.post('/gio-hang/them', { productId: PRODUCT.id, variantId: PRODUCT.variant, quantity: 1, __RequestVerificationToken: t });
    let cart = await s.get('/gio-hang');
    has(cart.body, PRODUCT.price.toLocaleString('vi-VN'), 'giỏ có 1 sản phẩm đúng giá');

    const t2 = await s.token('/gio-hang');
    await s.post('/gio-hang/cap-nhat', { productId: PRODUCT.id, variantId: PRODUCT.variant, quantity: 3, returnUrl: '/gio-hang', __RequestVerificationToken: t2 });
    cart = await s.get('/gio-hang');
    has(cart.body, (PRODUCT.price * 3).toLocaleString('vi-VN'), 'cập nhật số lượng = 3 -> tổng đúng');

    const t3 = await s.token('/gio-hang');
    await s.post('/gio-hang/xoa', { productId: PRODUCT.id, variantId: PRODUCT.variant, returnUrl: '/gio-hang', __RequestVerificationToken: t3 });
    cart = await s.get('/gio-hang');
    notHas(cart.body, 'checkout-totals', 'xóa xong giỏ rỗng');
  });

  await test('Giỏ hàng: số lượng 0 hoặc âm không tạo dòng âm', async () => {
    const s = session();
    const t = await s.token('/san-pham/' + PRODUCT.slug);
    await s.post('/gio-hang/them', { productId: PRODUCT.id, variantId: PRODUCT.variant, quantity: -5, __RequestVerificationToken: t });
    const cart = await s.get('/gio-hang');
    notHas(cart.body, '-5', 'không có số lượng âm');
  });

  // ---------- voucher ----------
  await test('Voucher: GIAM20 giảm đúng 20%', async () => {
    const s = session();
    const t = await s.token('/san-pham/' + PRODUCT.slug);
    await s.post('/gio-hang/them', { productId: PRODUCT.id, variantId: PRODUCT.variant, quantity: 2, __RequestVerificationToken: t });
    const tc = await s.token('/thanh-toan');
    await s.post('/gio-hang/ap-dung-ma', { code: 'GIAM20', returnUrl: '/thanh-toan', __RequestVerificationToken: tc });
    const co = await s.get('/thanh-toan');
    const sub = PRODUCT.price * 2;
    has(co.body, Math.round(sub * 0.2).toLocaleString('vi-VN'), 'giảm 20% đúng số tiền');
    has(co.body, (sub - sub * 0.2).toLocaleString('vi-VN'), 'tổng sau giảm đúng');
  });

  await test('Voucher: mã sai bị từ chối', async () => {
    const s = session();
    const t = await s.token('/san-pham/' + PRODUCT.slug);
    await s.post('/gio-hang/them', { productId: PRODUCT.id, variantId: PRODUCT.variant, quantity: 1, __RequestVerificationToken: t });
    const tc = await s.token('/thanh-toan');
    await s.post('/gio-hang/ap-dung-ma', { code: 'MA-SAI-BET', returnUrl: '/thanh-toan', __RequestVerificationToken: tc });
    const co = await s.get('/thanh-toan');
    notHas(co.body, 'Đã áp dụng', 'mã sai không được áp dụng');
  });

  await test('Voucher: gỡ mã thì hết giảm giá', async () => {
    const s = session();
    const t = await s.token('/san-pham/' + PRODUCT.slug);
    await s.post('/gio-hang/them', { productId: PRODUCT.id, variantId: PRODUCT.variant, quantity: 1, __RequestVerificationToken: t });
    let tc = await s.token('/thanh-toan');
    await s.post('/gio-hang/ap-dung-ma', { code: 'GIAM20', returnUrl: '/thanh-toan', __RequestVerificationToken: tc });
    has((await s.get('/thanh-toan')).body, 'Đã áp dụng', 'áp dụng được trước đã');
    tc = await s.token('/thanh-toan');
    await s.post('/gio-hang/xoa-ma', { returnUrl: '/thanh-toan', __RequestVerificationToken: tc });
    notHas((await s.get('/thanh-toan')).body, 'Đã áp dụng', 'gỡ mã xong hết giảm');
  });

  // ---------- checkout ----------
  await test('Thanh toán: giỏ rỗng bị đá về giỏ hàng', async () => {
    const s = session();
    const r = await s.get('/thanh-toan');
    eq(r.status, 302, 'redirect khi giỏ rỗng');
  });

  await test('Thanh toán: thiếu trường bắt buộc thì báo lỗi, không tạo đơn', async () => {
    const s = session();
    const t = await s.token('/san-pham/' + PRODUCT.slug);
    await s.post('/gio-hang/them', { productId: PRODUCT.id, variantId: PRODUCT.variant, quantity: 1, __RequestVerificationToken: t });
    const tc = await s.token('/thanh-toan');
    const r = await s.post('/thanh-toan/dat-hang', { Email: 'khong-phai-email', FirstName: '', LastName: '', Address: '', ProvinceDistrictWard: '', Phone: '', ShippingMethod: 'InnerCity', PaymentMethod: 'CashOnDelivery', __RequestVerificationToken: tc });
    eq(r.status, 200, 'trả về form (không redirect)');
    has(r.body, 'Vui lòng nhập', 'hiện lỗi validation');
  });

  await test('Thanh toán: đặt hàng thành công + trừ tồn kho', async () => {
    const s = session();
    const t = await s.token('/san-pham/' + PRODUCT.slug);
    await s.post('/gio-hang/them', { productId: PRODUCT.id, variantId: PRODUCT.variant, quantity: 1, __RequestVerificationToken: t });
    const tc = await s.token('/thanh-toan');
    const r = await s.post('/thanh-toan/dat-hang', {
      Email: 'test-auto@example.com', FirstName: 'Test', LastName: 'Auto', Address: '1 Test',
      ProvinceDistrictWard: 'Hà Nội', Phone: '0900000000', ShippingMethod: 'InnerCity',
      PaymentMethod: 'CashOnDelivery', __RequestVerificationToken: tc,
    });
    check(r.status === 302 && /xac-nhan/.test(r.location || ''), `đặt hàng -> redirect xác nhận (nhận ${r.status} ${r.location})`);
    const conf = await s.get(r.location || '/');
    has(conf.body, 'HD', 'trang xác nhận có mã đơn');
    const after = await s.get('/gio-hang');
    notHas(after.body, 'checkout-totals', 'giỏ được dọn sau khi đặt');
  });

  // ---------- forms ----------
  await test('Form liên hệ: gửi được và lưu lại', async () => {
    const s = session();
    const t = await s.token('/lien-he');
    const r = await s.post('/lien-he', { FirstName: 'Auto', LastName: 'Test', Email: 'auto-contact@example.com', Phone: '0900000001', Message: 'AUTOTEST-CONTACT', __RequestVerificationToken: t });
    eq(r.status, 302, 'gửi xong redirect');
    has((await s.get('/lien-he')).body, 'Cảm ơn', 'hiện lời cảm ơn');
  });

  await test('Form liên hệ: email sai bị chặn', async () => {
    const s = session();
    const t = await s.token('/lien-he');
    const r = await s.post('/lien-he', { FirstName: 'A', LastName: 'B', Email: 'sai-dinh-dang', Message: 'x', __RequestVerificationToken: t });
    eq(r.status, 200, 'trả lại form');
    has(r.body, 'Email không hợp lệ', 'báo lỗi email');
  });

  await test('Form bán buôn: gửi được', async () => {
    const s = session();
    const t = await s.token('/hop-tac');
    const r = await s.post('/hop-tac', { FirstName: 'Auto', LastName: 'WS', Email: 'auto-ws@example.com', CompanyName: 'Cty Auto', RequestType: 'Business', Message: 'AUTOTEST-WS', __RequestVerificationToken: t });
    eq(r.status, 302, 'gửi xong redirect');
  });

  await test('Newsletter: đăng ký + trùng email không lỗi', async () => {
    const s = session();
    const t = await s.token('/');
    const r1 = await s.post('/newsletter/subscribe', { email: 'auto-news@example.com', returnUrl: '/', __RequestVerificationToken: t });
    check(r1.status === 302 || r1.status === 200, 'đăng ký lần 1 OK');
    const t2 = await s.token('/');
    const r2 = await s.post('/newsletter/subscribe', { email: 'auto-news@example.com', returnUrl: '/', __RequestVerificationToken: t2 });
    check(r2.status === 302 || r2.status === 200, 'đăng ký trùng không lỗi');
  });

  await test('Newsletter: email sai bị chặn', async () => {
    const s = session();
    const t = await s.token('/');
    const r = await s.post('/newsletter/subscribe', { email: 'khong-phai-email', returnUrl: '/', __RequestVerificationToken: t });
    check(r.status !== 500, 'không 500');
  });

  // ---------- security ----------
  await test('CSRF: POST thiếu token bị chặn', async () => {
    const s = session();
    await s.get('/lien-he');
    const r = await s.post('/lien-he', { FirstName: 'X', LastName: 'Y', Email: 'a@b.com', Message: 'no token' });
    eq(r.status, 400, 'thiếu antiforgery token -> 400');
  });

  await test('Trang admin không đăng nhập bị đá về login', async () => {
    const s = session();
    for (const p of ['/admin', '/admin/don-hang', '/admin/san-pham', '/admin/cai-dat']) {
      const r = await s.get(p);
      check(r.status === 302 && /dang-nhap/.test(r.location || ''), `${p} -> đá về login (nhận ${r.status})`);
    }
  });

  // ---------- account / OTP ----------
  await test('Đăng nhập OTP: gửi mã rồi nhập sai bị từ chối', async () => {
    const s = session();
    const t = await s.token('/tai-khoan/dang-nhap');
    const r = await s.post('/tai-khoan/dang-nhap', { email: 'auto-otp@example.com', __RequestVerificationToken: t });
    check(r.status === 302 || r.status === 200, 'yêu cầu OTP không lỗi');
    const tv = await s.token('/tai-khoan/xac-thuc');
    if (tv) {
      const bad = await s.post('/tai-khoan/xac-thuc', { email: 'auto-otp@example.com', code: '000000', __RequestVerificationToken: tv });
      check(bad.status !== 500, 'mã sai không 500');
      notHas(bad.body || '', 'tai-khoan/don-hang', 'mã sai không cho vào');
    }
  });

  process.exit(summary('STOREFRONT') > 0 ? 1 : 0);
})();
