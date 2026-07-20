// Mobile nav drawer
(function () {
  const drawer = document.querySelector('[data-nav-drawer]');
  const openBtn = document.querySelector('[data-nav-drawer-open]');
  const closeBtn = document.querySelector('[data-nav-drawer-close]');
  const backdrop = drawer?.querySelector('.nav-drawer__backdrop');

  function open() {
    drawer?.classList.add('is-open');
    openBtn?.setAttribute('aria-expanded', 'true');
  }
  function close() {
    drawer?.classList.remove('is-open');
    openBtn?.setAttribute('aria-expanded', 'false');
  }

  openBtn?.addEventListener('click', open);
  closeBtn?.addEventListener('click', close);
  backdrop?.addEventListener('click', close);

  // Category accordions — the only way into the mega-menu links on a phone.
  drawer?.querySelectorAll('[data-drawer-toggle]').forEach(function (toggle) {
    const group = toggle.closest('[data-drawer-group]');
    const sub = group?.querySelector('[data-drawer-sub]');
    toggle.addEventListener('click', function () {
      const willOpen = toggle.getAttribute('aria-expanded') !== 'true';
      toggle.setAttribute('aria-expanded', String(willOpen));
      if (sub) sub.hidden = !willOpen;
    });
  });
})();

// Mini-cart drawer
(function () {
  const panel = document.querySelector('[data-minicart-panel]');
  const backdrop = document.querySelector('[data-minicart-backdrop]');
  const openBtns = document.querySelectorAll('[data-minicart-open]');
  // The close button is re-rendered by cart-live.js, so it is resolved on each click.

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

  openBtns.forEach(function (btn) {
    btn.addEventListener('click', function (e) {
      e.preventDefault(); // JS users get the drawer; without JS the href still navigates to /gio-hang
      open();
    });
  });

  // Delegated: the drawer's contents are replaced whenever the cart changes.
  panel?.addEventListener('click', function (e) {
    if (e.target.closest('[data-minicart-close]')) close();
  });
  backdrop?.addEventListener('click', close);

  document.addEventListener('minicart:open', open);
})();

// Bulk-order Zalo contact popup (global — triggered from any [data-contact-popup-open] button)
(function () {
  const panel = document.querySelector('[data-contact-popup]');
  const backdrop = document.querySelector('[data-contact-popup-backdrop]');
  const openBtns = document.querySelectorAll('[data-contact-popup-open]');
  const closeBtn = document.querySelector('[data-contact-popup-close]');
  const copyBtn = panel?.querySelector('[data-copy]');

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

  openBtns.forEach(function (btn) {
    btn.addEventListener('click', function (e) {
      e.preventDefault();
      open();
    });
  });
  closeBtn?.addEventListener('click', close);
  backdrop?.addEventListener('click', close);

  // Used to copy silently — you could not tell whether it had worked.
  copyBtn?.addEventListener('click', async function () {
    var value = copyBtn.getAttribute('data-copy') || '';
    try {
      await navigator.clipboard.writeText(value);
      window.hoaiiToast?.('Đã sao chép số Zalo: ' + value, 'ok');
    } catch {
      window.hoaiiToast?.('Không sao chép được. Số Zalo: ' + value, 'error');
    }
  });
})();

// Mega-menu open/close. All four panels are click-only — every Figma prototype specifies
// ON_CLICK (nodes 1287:56680, 56777, 56871, 56965) — and only one is ever open at a time.
(function () {
  const triggers = document.querySelectorAll('[data-menu-trigger]');

  function panelFor(trigger) {
    return document.querySelector('[data-menu-panel="' + trigger.dataset.menuTrigger + '"]');
  }

  /**
   * @param instant Close without the open animation playing in reverse. Used when the user
   *   switches straight to another menu: the panels are stacked in the same spot, so letting the
   *   outgoing one animate shut would leave two dropdowns overlapping (and "Quà tết" closes over
   *   0.8s, far longer than the other three take to open). Dismissing the menu outright — cross,
   *   Escape, click outside — keeps the normal animated close.
   */
  function closeAll(instant) {
    triggers.forEach(function (t) {
      if (!t.classList.contains('is-open')) return;

      const panel = instant ? panelFor(t) : null;
      if (panel) panel.classList.add('is-closing-instantly');

      t.classList.remove('is-open');
      t.setAttribute('aria-expanded', 'false');

      if (panel) {
        void panel.offsetHeight; // commit the collapsed height while transitions are off
        panel.classList.remove('is-closing-instantly');
      }
    });
  }

  triggers.forEach(function (trigger) {
    trigger.addEventListener('click', function () {
      const willOpen = !trigger.classList.contains('is-open');
      closeAll(willOpen); // opening a different menu replaces the current one outright
      trigger.classList.toggle('is-open', willOpen);
      trigger.setAttribute('aria-expanded', willOpen ? 'true' : 'false');
    });
  });

  // The cross inside a panel. One listener per button, bound once at load — the panels are
  // server-rendered and never replaced, so nothing accumulates.
  document.querySelectorAll('[data-menu-close]').forEach(function (btn) {
    btn.addEventListener('click', function () {
      closeAll();
    });
  });

  document.addEventListener('click', function (e) {
    if (!e.target.closest('[data-mega-menu-nav]') && !e.target.closest('.mega-menu')) {
      closeAll();
    }
  });

  // Escape closes the open panel and hands focus back to the trigger that opened it.
  document.addEventListener('keydown', function (e) {
    if (e.key !== 'Escape') return;
    const open = document.querySelector('[data-menu-trigger].is-open');
    if (!open) return;
    closeAll();
    open.focus();
  });
})();

// Chat widget expand/collapse
(function () {
  const widget = document.querySelector('[data-chat-widget]');
  const toggle = widget?.querySelector('.toggle');

  toggle?.addEventListener('click', function () {
    widget.classList.toggle('is-collapsed');
  });
})();

// Back-to-top button
(function () {
  const btn = document.querySelector('[data-back-to-top]');
  if (!btn) return;

  window.addEventListener('scroll', function () {
    const shouldShow = window.scrollY > 600;
    btn.hidden = !shouldShow;
    btn.classList.toggle('is-visible', shouldShow);
  }, { passive: true });

  btn.addEventListener('click', function () {
    window.scrollTo({ top: 0, behavior: 'smooth' });
  });
})();
