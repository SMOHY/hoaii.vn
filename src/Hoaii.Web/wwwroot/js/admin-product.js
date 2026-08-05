// Product edit form: gallery reorder + dynamic variant rows.
// The media picker itself (modal, upload, library grid) is shared — see admin-imagefield.js.
// The gallery's "+ Thêm ảnh" button has no data-target, so picks/uploads land here via
// the picker:append event instead of filling a single input.
(function () {
  'use strict';

  const form = document.querySelector('[data-product-form]');
  if (!form) return;

  const imageList = form.querySelector('[data-image-list]');

  // ---------- image list: add / remove / reorder ----------
  function makeImageItem(url) {
    const item = document.createElement('div');
    item.className = 'admin-image-item';
    item.setAttribute('data-image-item', '');
    item.innerHTML =
      '<input type="hidden" name="ImageUrls">' +
      '<div class="admin-image-item__thumb"></div>' +
      '<div class="admin-image-item__controls">' +
        '<button type="button" data-move="-1" aria-label="Trái">◀</button>' +
        '<button type="button" data-move="1" aria-label="Phải">▶</button>' +
        '<button type="button" data-remove aria-label="Xóa">✕</button>' +
      '</div>';
    item.querySelector('input').value = url;
    item.querySelector('.admin-image-item__thumb').style.backgroundImage = "url('" + url + "')";
    return item;
  }

  function addImage(url) {
    if (!url) return;
    // Skip duplicates.
    const exists = [...imageList.querySelectorAll('input[name="ImageUrls"]')].some(i => i.value === url);
    if (exists) return;
    imageList.appendChild(makeImageItem(url));
  }

  document.addEventListener('picker:append', function (e) { addImage(e.detail.url); });

  imageList.addEventListener('click', function (e) {
    const item = e.target.closest('[data-image-item]');
    if (!item) return;
    if (e.target.hasAttribute('data-remove')) {
      item.remove();
    } else if (e.target.hasAttribute('data-move')) {
      const dir = parseInt(e.target.getAttribute('data-move'), 10);
      if (dir < 0 && item.previousElementSibling) {
        item.parentNode.insertBefore(item, item.previousElementSibling);
      } else if (dir > 0 && item.nextElementSibling) {
        item.parentNode.insertBefore(item.nextElementSibling, item);
      }
    }
  });

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
