// Product edit form: dynamic variant rows.
// Bộ chọn ảnh nằm ở admin-imagefield.js, dải ảnh ở admin-imagelist.js — trang này chỉ còn
// phần biến thể (loại hộp).
(function () {
  'use strict';

  const form = document.querySelector('[data-product-form]');
  if (!form) return;

  // Danh sách ảnh do admin-imagelist.js lo — xem file đó.


  // ---------- variant rows ----------
  const variantBody = form.querySelector('[data-variant-body]');
  form.querySelector('[data-add-variant]')?.addEventListener('click', function () {
    const tr = document.createElement('tr');
    tr.setAttribute('data-variant-row', '');
    tr.innerHTML =
      '<td><input type="hidden" name="VariantIds" value="0"><input type="text" name="VariantNames" placeholder="VD: 4 Bánh" style="width:100%;padding:6px 8px;border:1px solid var(--color-grey-300);border-radius:4px;"></td>' +
      '<td class="num"><input type="number" name="VariantPrices" value="0" step="1000" style="width:120px;padding:6px 8px;border:1px solid var(--color-grey-300);border-radius:4px;"></td>' +
      '<td><input type="text" name="VariantSkus" style="width:120px;padding:6px 8px;border:1px solid var(--color-grey-300);border-radius:4px;"></td>' +
      '<td class="num"><input type="number" name="VariantStocks" value="0" min="0" style="width:90px;padding:6px 8px;border:1px solid var(--color-grey-300);border-radius:4px;"></td>' +
      '<td class="num"><button type="button" class="admin-btn admin-btn--danger admin-btn--sm" data-remove-variant>✕</button></td>';
    variantBody.appendChild(tr);
  });
  variantBody?.addEventListener('click', function (e) {
    if (e.target.hasAttribute('data-remove-variant')) {
      e.target.closest('[data-variant-row]').remove();
    }
  });

})();
