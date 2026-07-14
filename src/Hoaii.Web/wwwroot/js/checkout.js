// Toggle the `.is-selected` visual state on custom radio option rows
// (shipping method / payment method groups).
(function () {
  document.querySelectorAll('.option-group').forEach(function (group) {
    var rows = group.querySelectorAll('.option-row');
    rows.forEach(function (row) {
      var radio = row.querySelector('input[type="radio"]');
      radio && radio.addEventListener('change', function () {
        rows.forEach(function (r) { r.classList.remove('is-selected'); });
        row.classList.add('is-selected');
      });
    });
  });
})();

// Voucher modal open/close
(function () {
  const panel = document.querySelector('[data-voucher-modal]');
  const backdrop = document.querySelector('[data-voucher-modal-backdrop]');
  const openBtns = document.querySelectorAll('[data-voucher-modal-open]');
  const closeBtn = document.querySelector('[data-voucher-modal-close]');

  function open() {
    panel?.classList.add('is-open');
    backdrop?.classList.add('is-open');
    panel?.setAttribute('aria-hidden', 'false');
  }
  function close() {
    panel?.classList.remove('is-open');
    backdrop?.classList.remove('is-open');
    panel?.setAttribute('aria-hidden', 'true');
  }

  openBtns.forEach(function (btn) { btn.addEventListener('click', open); });
  closeBtn?.addEventListener('click', close);
  backdrop?.addEventListener('click', close);

  // The modal used to close by virtue of the page reloading; the apply now happens in the
  // background, so it has to close itself.
  document.addEventListener('voucher:applied', close);

  // Selecting a voucher card visually marks it selected too (server also re-marks on reload).
  document.querySelectorAll('.voucher-item__input').forEach(function (input) {
    input.addEventListener('change', function () {
      document.querySelectorAll('.voucher-item').forEach(function (item) { item.classList.remove('is-selected'); });
      input.closest('.voucher-item')?.classList.add('is-selected');
    });
  });
})();
