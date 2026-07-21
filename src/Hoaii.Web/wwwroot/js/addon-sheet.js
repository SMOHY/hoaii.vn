// Variant picker for the "HOÀN CHỈNH VỚI" upsells in the mini-cart (Figma node 970:20686).
//
// A suggestion with more than one variant cannot be added blind, so its "Thêm" button opens a
// sheet over the drawer instead of posting. Products with a single variant keep their plain
// form and still work with JS off.
(function () {
  'use strict';

  function sheetFor(productId) {
    return document.querySelector('[data-addon-sheet="' + productId + '"]');
  }

  function open(sheet) {
    document.querySelectorAll('.addon-sheet.is-open').forEach(close);
    sheet.classList.add('is-open');
    sheet.setAttribute('aria-hidden', 'false');
    const first = sheet.querySelector('input[type="radio"]:checked') || sheet.querySelector('button, input');
    if (first) first.focus();
  }

  function close(sheet) {
    sheet.classList.remove('is-open');
    sheet.setAttribute('aria-hidden', 'true');
  }

  // The summary line and price under the product name follow the chosen variant, so the sheet
  // always shows what is actually about to be added.
  function sync(sheet) {
    const picked = sheet.querySelector('input[name="variantId"]:checked');
    if (!picked) return;
    const summary = sheet.querySelector('[data-addon-sheet-summary]');
    const price = sheet.querySelector('[data-addon-sheet-price]');
    if (summary) summary.textContent = picked.dataset.variantName || '';
    if (price) price.textContent = picked.dataset.variantPrice || '';
  }

  document.addEventListener('click', function (e) {
    const trigger = e.target.closest('[data-addon-pick]');
    if (trigger) {
      const sheet = sheetFor(trigger.dataset.addonPick);
      if (sheet) { e.preventDefault(); open(sheet); }
      return;
    }

    const closer = e.target.closest('[data-addon-sheet-close]');
    if (closer) { close(closer.closest('.addon-sheet')); return; }

    // Clicking the dimmed area behind the panel dismisses it.
    const sheet = e.target.closest('.addon-sheet');
    if (sheet && !e.target.closest('.addon-sheet__panel')) close(sheet);
  });

  document.addEventListener('change', function (e) {
    if (e.target.name === 'variantId') sync(e.target.closest('.addon-sheet'));
  });

  document.addEventListener('click', function (e) {
    const step = e.target.closest('[data-addon-qty]');
    if (!step) return;
    const sheet = step.closest('.addon-sheet');
    const input = sheet.querySelector('[data-addon-qty-input]');
    const label = sheet.querySelector('[data-addon-qty-value]');
    const next = Math.max(1, (parseInt(input.value, 10) || 1) + parseInt(step.dataset.addonQty, 10));
    input.value = next;
    label.textContent = next;
  });

  document.addEventListener('keydown', function (e) {
    if (e.key !== 'Escape') return;
    const open = document.querySelector('.addon-sheet.is-open');
    // Closes the sheet only; the mini-cart behind it stays open, which is what the shopper meant.
    if (open) { e.stopPropagation(); close(open); }
  }, true);
})();
