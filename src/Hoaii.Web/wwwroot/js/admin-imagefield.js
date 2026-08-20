// Reusable image field for CMS forms: one shared media-picker modal drives any number of
// [data-open-picker] buttons on the page. Each button names its target input via data-target;
// selecting or uploading an image fills that input and updates its preview.
(function () {
  'use strict';

  const modal = document.querySelector('[data-picker-modal]');
  if (!modal) return;

  const backdrop = document.querySelector('[data-picker-backdrop]');
  const grid = modal.querySelector('[data-picker-grid]');
  const token = document.querySelector('input[name="__RequestVerificationToken"]')?.value;
  let targetId = null;

  function preview(input) {
    const box = document.querySelector('[data-preview-for="' + input.id + '"]');
    if (box) box.style.backgroundImage = input.value ? "url('" + input.value + "')" : '';
  }

  // With a target input, fill it directly. Without one (e.g. a gallery's "+ Thêm ảnh"
  // button, which has no single field to fill), let the page decide what to do via this event.
  function setValue(url) {
    if (!targetId) {
      document.dispatchEvent(new CustomEvent('picker:append', { detail: { url: url } }));
      return;
    }
    const input = document.getElementById(targetId);
    if (!input) return;
    input.value = url || '';
    preview(input);
  }

  // Keep previews in sync with manual edits.
  document.querySelectorAll('[data-image-input]').forEach(function (input) {
    preview(input);
    input.addEventListener('change', function () { preview(input); });
  });

  function open(id) {
    targetId = id;
    modal.classList.add('is-open');
    backdrop && backdrop.classList.add('is-open');
    modal.setAttribute('aria-hidden', 'false');
    load();
  }
  function close() {
    modal.classList.remove('is-open');
    backdrop && backdrop.classList.remove('is-open');
    modal.setAttribute('aria-hidden', 'true');
  }

  document.querySelectorAll('[data-open-picker]').forEach(function (btn) {
    btn.addEventListener('click', function () { open(btn.getAttribute('data-target')); });
  });
  modal.querySelector('[data-picker-close]')?.addEventListener('click', close);
  backdrop && backdrop.addEventListener('click', close);

  async function load() {
    grid.innerHTML = '<p style="color:var(--color-grey-500);">Đang tải…</p>';
    try {
      const res = await fetch('/admin/thu-vien-anh/danh-sach', { headers: { 'X-Requested-With': 'XMLHttpRequest' } });
      render(await res.json());
    } catch {
      grid.innerHTML = '<p style="color:var(--color-brand-red);">Không tải được thư viện.</p>';
    }
  }

  function render(items) {
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
      cell.addEventListener('click', function () { setValue(it.url); close(); });
      grid.appendChild(cell);
    });
  }

  modal.querySelector('[data-picker-upload]')?.addEventListener('submit', async function (e) {
    e.preventDefault();
    // Instant feedback instead of a wasted round-trip; the server enforces the same 5MB cap.
    var tooBig = [...e.target.querySelectorAll('input[type=file]')]
      .flatMap(function (i) { return [...i.files]; })
      .filter(function (f) { return f.size > 5 * 1024 * 1024; });
    if (tooBig.length) {
      window.hoaiiToast && window.hoaiiToast('Ảnh vượt quá 5MB: ' + tooBig.map(function (f) { return f.name; }).join(', '), 'error');
      return;
    }
    const fd = new FormData(e.target);
    fd.append('json', 'true');
    if (token) fd.append('__RequestVerificationToken', token);
    const btn = e.target.querySelector('button');
    btn.disabled = true;
    try {
      const res = await fetch('/admin/thu-vien-anh/tai-len', { method: 'POST', body: fd, headers: { 'X-Requested-With': 'XMLHttpRequest' } });
      const data = await res.json();
      if (data.uploaded && data.uploaded.length) {
        if (targetId) {
          // Single field: only the first upload makes sense.
          setValue(data.uploaded[0].url);
        } else {
          data.uploaded.forEach(function (u) { setValue(u.url); });
        }
      }
      if (data.errors && data.errors.length) window.hoaiiToast && window.hoaiiToast(data.errors.join(' · '), 'error');
      e.target.reset();
      load();
    } catch {
      window.hoaiiToast && window.hoaiiToast('Tải ảnh thất bại.', 'error');
    } finally {
      btn.disabled = false;
    }
  });
})();

// Image slot: the Desktop/Mobile tab pair and the focal-point picker. Kept separate from the
// picker modal above so a page without the modal (or a slot without either extra) still works.
(function () {
  'use strict';

  function parseFocal(value) {
    var m = /^\s*(-?[\d.]+)%\s+(-?[\d.]+)%\s*$/.exec(value || '');
    return m ? { x: parseFloat(m[1]), y: parseFloat(m[2]) } : { x: 50, y: 50 };
  }

  function clamp(n) { return Math.max(0, Math.min(100, n)); }

  document.querySelectorAll('[data-imgslot]').forEach(function (slot) {
    // --- Desktop / Mobile tabs -------------------------------------------------
    var tabs = slot.querySelectorAll('[data-imgtab]');
    tabs.forEach(function (tab) {
      tab.addEventListener('click', function () {
        var want = tab.getAttribute('data-imgtab');
        tabs.forEach(function (t) {
          var on = t === tab;
          t.classList.toggle('is-active', on);
          t.setAttribute('aria-selected', on ? 'true' : 'false');
        });
        slot.querySelectorAll('[data-imgpane]').forEach(function (pane) {
          pane.classList.toggle('is-active', pane.getAttribute('data-imgpane') === want);
        });
      });
    });

    // --- Mobile override: share <-> own image ----------------------------------
    var shared = slot.querySelector('[data-imgslot-shared]');
    var box = slot.querySelector('[data-imgslot-override-box]');
    var badge = slot.querySelector('[data-imgslot-badge]');
    var mobileInput = box && box.querySelector('[data-image-input]');

    function showOverride(on) {
      if (!shared || !box) return;
      shared.style.display = on ? 'none' : '';
      box.style.display = on ? '' : 'none';
      if (badge) badge.style.display = on ? '' : 'none';
    }

    slot.querySelector('[data-imgslot-override]')?.addEventListener('click', function () {
      showOverride(true);
      // Open the library straight away — the only reason to click this button is to pick an image.
      box.querySelector('[data-open-picker]')?.click();
    });

    slot.querySelector('[data-imgslot-share]')?.addEventListener('click', function () {
      // Clearing the field is what makes the storefront fall back to the desktop image.
      if (mobileInput) {
        mobileInput.value = '';
        mobileInput.dispatchEvent(new Event('change'));
      }
      showOverride(false);
    });

    // --- Focal point -----------------------------------------------------------
    var thumb = slot.querySelector('[data-focal-target]');
    var focalInput = slot.querySelector('[data-focal-input]');
    var dot = slot.querySelector('[data-focal-dot]');
    if (!thumb || !focalInput) return;

    function paint() {
      var p = parseFocal(focalInput.value);
      thumb.style.backgroundPosition = p.x + '% ' + p.y + '%';
      if (dot) {
        dot.style.left = p.x + '%';
        dot.style.top = p.y + '%';
      }
    }

    thumb.addEventListener('click', function (e) {
      var r = thumb.getBoundingClientRect();
      if (!r.width || !r.height) return;
      var x = clamp(Math.round(((e.clientX - r.left) / r.width) * 100));
      var y = clamp(Math.round(((e.clientY - r.top) / r.height) * 100));
      focalInput.value = x + '% ' + y + '%';
      paint();
    });

    slot.querySelector('[data-focal-reset]')?.addEventListener('click', function () {
      focalInput.value = '';
      paint();
    });

    paint();
  });
})();
