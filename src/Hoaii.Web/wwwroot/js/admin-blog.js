// Blog edit form: single cover-image picker. Reuses the media-library endpoints and the same
// modal markup as the product form, but binds one URL into a single text input + preview.
(function () {
  'use strict';

  const form = document.querySelector('[data-blog-form]');
  if (!form) return;

  const token = form.querySelector('input[name="__RequestVerificationToken"]')?.value;
  const urlInput = form.querySelector('[data-blog-image-url]');
  const preview = form.querySelector('[data-blog-image-preview]');

  function setImage(url) {
    urlInput.value = url || '';
    preview.style.backgroundImage = url ? "url('" + url + "')" : '';
  }

  // Keep the preview in sync if the URL is typed/pasted by hand.
  urlInput.addEventListener('change', function () { setImage(urlInput.value.trim()); });

  // ---------- picker modal ----------
  const modal = document.querySelector('[data-picker-modal]');
  const backdrop = document.querySelector('[data-picker-backdrop]');
  const grid = document.querySelector('[data-picker-grid]');
  if (!modal) return;

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
      renderLibrary(await res.json());
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
        setImage(it.url);
        closePicker();
      });
      grid.appendChild(cell);
    });
  }

  // Upload straight from the picker, then pick the first new image.
  modal.querySelector('[data-picker-upload]')?.addEventListener('submit', async function (e) {
    e.preventDefault();
    const fd = new FormData(e.target);
    fd.append('json', 'true');
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
      if (data.uploaded && data.uploaded.length) {
        setImage(data.uploaded[0].url);
      }
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
