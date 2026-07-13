// Generic edit-modal open/close (Profile page field-edit modals).
(function () {
  const backdrop = document.querySelector('[data-edit-modal-backdrop]');
  const openTriggers = document.querySelectorAll('[data-modal-open]');

  function closeAll() {
    document.querySelectorAll('[data-edit-modal]').forEach(function (m) {
      m.classList.remove('is-open');
      m.setAttribute('aria-hidden', 'true');
    });
    backdrop?.classList.remove('is-open');
  }

  openTriggers.forEach(function (trigger) {
    trigger.addEventListener('click', function () {
      const modal = document.getElementById(trigger.getAttribute('data-modal-open'));
      modal?.classList.add('is-open');
      modal?.setAttribute('aria-hidden', 'false');
      backdrop?.classList.add('is-open');
    });
  });

  document.querySelectorAll('[data-modal-close]').forEach(function (btn) {
    btn.addEventListener('click', closeAll);
  });
  backdrop?.addEventListener('click', closeAll);
})();

// Custom radio control visual state (Giới tính modal).
(function () {
  document.querySelectorAll('.radio-option').forEach(function (option) {
    const input = option.querySelector('input[type="radio"]');
    const control = option.querySelector('.radio-control');
    function sync() { control?.classList.toggle('is-selected', input.checked); }
    input?.addEventListener('change', function () {
      document.querySelectorAll('input[name="' + input.name + '"]').forEach(function (i) {
        i.closest('.radio-option')?.querySelector('.radio-control')?.classList.toggle('is-selected', i.checked);
      });
    });
    sync();
  });
})();

// Vietnam Province -> Ward cascading select (Saved Addresses form).
// NOTE: seed data only covers a handful of provinces/wards for demo purposes —
// swap window.__allWards for a real dataset/API before production.
(function () {
  const provinceSelect = document.getElementById('province-select');
  const wardSelect = document.getElementById('ward-select');
  if (!provinceSelect || !wardSelect || !window.__allWards) return;

  function populateWards(provinceId, selectedWardId) {
    wardSelect.innerHTML = '<option value="">-- Chọn phường/xã --</option>';
    const wards = window.__allWards.filter(function (w) { return w.provinceId === provinceId; });
    wards.forEach(function (w) {
      const opt = document.createElement('option');
      opt.value = w.id;
      opt.textContent = w.name;
      if (selectedWardId && w.id === selectedWardId) opt.selected = true;
      wardSelect.appendChild(opt);
    });
    wardSelect.disabled = wards.length === 0;
  }

  provinceSelect.addEventListener('change', function () {
    populateWards(parseInt(provinceSelect.value, 10), null);
  });

  if (window.__selectedProvinceId) {
    populateWards(window.__selectedProvinceId, window.__selectedWardId);
  }
})();
