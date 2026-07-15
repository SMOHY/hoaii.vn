// Shared behaviour for every overlay on the site: the mobile drawer, the mini-cart, the
// voucher modal, the Zalo popup and the account modals.
//
// None of them could be closed with Escape, none moved focus into the panel or gave it back
// on close, and the page behind them kept scrolling and kept its links in the tab order.
// Each overlay still owns its own open/close (they toggle an `is-open` class); this watches
// for that class and layers the shared behaviour on top, so nothing had to be rewired.
(function () {
  'use strict';

  const OVERLAYS = [
    '[data-nav-drawer]',
    '[data-minicart-panel]',
    '[data-voucher-modal]',
    '[data-contact-popup]',
    '[data-edit-modal]',
    '[data-picker-modal]',
  ];

  const FOCUSABLE = 'a[href], button:not([disabled]), input:not([disabled]), select, textarea, [tabindex]:not([tabindex="-1"])';

  let openPanel = null;
  let lastFocused = null;

  function visible(el) {
    return el.offsetWidth > 0 || el.offsetHeight > 0 || el.getClientRects().length > 0;
  }

  function focusables(panel) {
    return Array.from(panel.querySelectorAll(FOCUSABLE)).filter(visible);
  }

  function onOpened(panel) {
    if (openPanel === panel) return;
    openPanel = panel;
    lastFocused = document.activeElement;

    document.body.style.overflow = 'hidden';

    const first = focusables(panel)[0];
    // Panels that hold no controls at all still need to take focus, or Escape would be
    // handled while focus sits behind the overlay.
    if (first) {
      first.focus();
    } else {
      panel.setAttribute('tabindex', '-1');
      panel.focus();
    }
  }

  function onClosed() {
    if (!openPanel) return;
    openPanel = null;

    document.body.style.overflow = '';
    if (lastFocused instanceof HTMLElement) lastFocused.focus();
    lastFocused = null;
  }

  // Clicking the panel's own close button, or the backdrop, removes `is-open` — so rather than
  // hooking every close path, watch the class itself.
  const observer = new MutationObserver(function (records) {
    records.forEach(function (record) {
      const el = record.target;
      if (el.classList.contains('is-open')) onOpened(el);
      else if (el === openPanel) onClosed();
    });
  });

  OVERLAYS.forEach(function (selector) {
    document.querySelectorAll(selector).forEach(function (panel) {
      observer.observe(panel, { attributes: true, attributeFilter: ['class'] });
      if (panel.classList.contains('is-open')) onOpened(panel);
    });
  });

  function close(panel) {
    // Prefer the panel's own close button so its bespoke logic (aria-hidden, backdrop) runs.
    const btn = panel.querySelector('[data-minicart-close], [data-voucher-modal-close], [data-contact-popup-close], [data-nav-drawer-close], [data-modal-close], [data-picker-close]');
    if (btn) btn.click();
    else panel.classList.remove('is-open');
  }

  document.addEventListener('keydown', function (e) {
    if (!openPanel) return;

    if (e.key === 'Escape') {
      e.preventDefault();
      close(openPanel);
      return;
    }

    if (e.key !== 'Tab') return;

    // Keep Tab inside the panel: without this you tabbed straight out into the page behind it.
    const items = focusables(openPanel);
    if (items.length === 0) return;

    const first = items[0];
    const last = items[items.length - 1];

    if (e.shiftKey && document.activeElement === first) {
      e.preventDefault();
      last.focus();
    } else if (!e.shiftKey && document.activeElement === last) {
      e.preventDefault();
      first.focus();
    }
  });
})();
