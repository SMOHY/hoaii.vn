/*
  Tạm vá cho máy dev đang thiếu .NET 10.

  Máy này chỉ còn SDK/runtime .NET 11 preview, trong khi dự án target net10.0. SDK preview
  sinh ra static-asset manifest có các endpoint nén trỏ tới "<route>.gz" — đường dẫn không hề
  tồn tại (file nén thật nằm trong thư mục obj compressed, với tên đã băm). Kết quả: mọi request
  kèm "Accept-Encoding: gzip" (tức là mọi trình duyệt) nhận HTTP 200 với body RỖNG, nên toàn
  bộ CSS/JS về trắng và trang hiện ra không có định dạng.

  Script này xoá các endpoint có Content-Encoding khỏi manifest trong bin/, buộc app phục vụ
  bản không nén (vẫn hoạt động bình thường). Manifest bị sinh lại sau mỗi lần build, nên chạy
  lại script sau khi build.

  CÁCH SỬA TRIỆT ĐỂ: cài .NET 10 runtime/SDK, rồi xoá file này đi.

  Dùng: node tools/strip-compressed-assets.js
*/
const fs = require('fs');
const path = require('path');

const manifest = path.join(
  __dirname, '..', 'src', 'Hoaii.Web', 'bin', 'Debug', 'net10.0',
  'Hoaii.Web.staticwebassets.endpoints.json'
);

if (!fs.existsSync(manifest)) {
  console.error('Không thấy manifest — build dự án trước đã:\n  ' + manifest);
  process.exit(1);
}

const data = JSON.parse(fs.readFileSync(manifest, 'utf8'));
const before = data.Endpoints.length;

data.Endpoints = data.Endpoints.filter(
  (e) => !(e.Selectors || []).some((s) => s.Name === 'Content-Encoding')
);

const removed = before - data.Endpoints.length;
fs.writeFileSync(manifest, JSON.stringify(data));
console.log(`Đã bỏ ${removed}/${before} endpoint nén. App sẽ phục vụ file tĩnh không nén.`);
