// Product edit form: image picker + reorder, and dynamic variant rows.
// The image picker modal opens/closes via the `is-open` class, so overlays.js gives it
// Escape / focus-trap / scroll-lock for free.
(function () {
  'use strict';

  const form = document.querySelector('[data-product-form]');
  if (!form) return;

  const token = form.querySelector('input[name="__RequestVerificationToken"]')?.value;
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

  // ---------- picker modal ----------
  const modal = document.querySelector('[data-picker-modal]');
  const backdrop = document.querySelector('[data-picker-backdrop]');
  const grid = document.querySelector('[data-picker-grid]');

  function openPicker() {
    modal.classList.add('is-open');
    backdrop.classList.add('is-open');
    modal.setAttribute('aria-hidden', 'false');
    loadLibrary();
  }
  function closePicker() {
    modal.classList.remove('is-open');
    backdrop.classList.remove('is-open');
    modal.setAttribute('aria-hidden', 'true');
  }

  form.querySelector('[data-open-picker]')?.addEventListener('click', openPicker);
  modal.querySelector('[data-picker-close]')?.addEventListener('click', closePicker);
  backdrop.addEventListener('click', closePicker);

  async function loadLibrary() {
    grid.innerHTML = '<p style="color:var(--color-grey-500);">Đang tải…</p>';
    try {
      const res = await fetch('/admin/thu-vien-anh/danh-sach', { headers: { 'X-Requested-With': 'XMLHttpRequest' } });
      const items = await res.json();
      renderLibrary(items);
    } catch {
      grid.innerHTML = '<p style="color:var(--color-brand-red);">Không tải được thư viện.</p>';
    }
  }

  function renderLibrary(items) {
    if (!items.length) {
      grid.innerHTML = '<p style="color:var(--color-grey-500);">Chưa có ảnh nào. Hãy tải lên ở trên.</p>';
      return;
    }
    grid.innerHTML = '';
    items.forEach(function (it) {
      const cell = document.createElement('button');
      cell.type = 'button';
      cell.className = 'admin-picker-cell';
      cell.title = it.name;
      cell.style.backgroundImage = "url('" + it.url + "')";
      cell.addEventListener('click', function () {
        addImage(it.url);
        closePicker();
      });
      grid.appendChild(cell);
    });
  }

  // Upload straight from the picker, then refresh + auto-add the new images.
  modal.querySelector('[data-picker-upload]')?.addEventListener('submit', async function (e) {
    e.preventDefault();
    const fd = new FormData(e.target);
    fd.append('json', 'true');
    // Antiforgery reads the token from a form field by default, so send it in the body.
    if (token) fd.append('__RequestVerificationToken', token);
    const btn = e.target.querySelector('button');
    btn.disabled = true;
    try {
      const res = await fetch('/admin/thu-vien-anh/tai-len', {
        method: 'POST',
        body: fd,
        headers: { 'X-Requested-With': 'XMLHttpRequest' },
      });
      const data = await res.json();
      (data.uploaded || []).forEach(function (u) { addImage(u.url); });
      if (data.errors && data.errors.length) {
        window.hoaiiToast && window.hoaiiToast(data.errors.join(' · '), 'error');
      }
      e.target.reset();
      loadLibrary();
    } catch {
      window.hoaiiToast && window.hoaiiToast('Tải ảnh thất bại.', 'error');
    } finally {
      btn.disabled = false;
    }
  });
})();
