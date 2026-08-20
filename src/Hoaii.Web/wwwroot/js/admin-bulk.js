// Chọn nhiều dòng trong bảng admin: ô tick từng dòng, ô "chọn tất cả", và thanh hành động chỉ
// hiện khi đã chọn ít nhất một dòng. Nút Ẩn/Hiện đặt giá trị cho trường "active" rồi để form
// tự gửi — mỗi nút nói rõ nó đặt trạng thái nào, không đảo trạng thái từng dòng.
(function () {
  'use strict';

  const bar = document.querySelector('[data-bulkbar]');
  if (!bar) return;

  const items = [...document.querySelectorAll('[data-bulk-item]')];
  const all = document.querySelector('[data-bulk-all]');
  const count = bar.querySelector('[data-bulk-count]');
  const activeField = document.querySelector('[data-bulk-active]');

  function sync() {
    const chosen = items.filter(i => i.checked).length;
    count.textContent = chosen;
    bar.hidden = chosen === 0;
    if (all) {
      all.checked = chosen > 0 && chosen === items.length;
      all.indeterminate = chosen > 0 && chosen < items.length;
    }
  }

  items.forEach(i => i.addEventListener('change', sync));

  all && all.addEventListener('change', function () {
    items.forEach(i => { i.checked = all.checked; });
    sync();
  });

  bar.querySelector('[data-bulk-clear]')?.addEventListener('click', function () {
    items.forEach(i => { i.checked = false; });
    sync();
  });

  bar.querySelectorAll('[data-bulk-do]').forEach(function (btn) {
    btn.addEventListener('click', function () {
      if (activeField) activeField.value = btn.getAttribute('data-bulk-do');
    });
  });

  sync();
})();
