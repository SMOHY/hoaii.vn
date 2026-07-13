// Mobile nav drawer
(function () {
  const drawer = document.querySelector('[data-nav-drawer]');
  const openBtn = document.querySelector('[data-nav-drawer-open]');
  const closeBtn = document.querySelector('[data-nav-drawer-close]');
  const backdrop = drawer?.querySelector('.nav-drawer__backdrop');

  function open() { drawer?.classList.add('is-open'); }
  function close() { drawer?.classList.remove('is-open'); }

  openBtn?.addEventListener('click', open);
  closeBtn?.addEventListener('click', close);
  backdrop?.addEventListener('click', close);
})();

// Mini-cart drawer
(function () {
  const panel = document.querySelector('[data-minicart-panel]');
  const backdrop = document.querySelector('[data-minicart-backdrop]');
  const openBtns = document.querySelectorAll('[data-minicart-open]');
  const closeBtn = document.querySelector('[data-minicart-close]');
  const badges = document.querySelectorAll('[data-cart-badge]');

  // Reflect the current cart count (rendered server-side into the panel) onto the nav badges.
  const count = parseInt(panel?.getAttribute('data-cart-count') || '0', 10);
  badges.forEach(function (badge) {
    badge.textContent = String(count);
    badge.hidden = count <= 0;
  });

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
  closeBtn?.addEventListener('click', close);
  backdrop?.addEventListener('click', close);
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

  copyBtn?.addEventListener('click', function () {
    var value = copyBtn.getAttribute('data-copy') || '';
    if (navigator.clipboard) {
      navigator.clipboard.writeText(value);
    }
  });
})();

// Mega-menu click-to-toggle (progressive enhancement — CSS :hover already shows/hides the
// panels with zero JS; this just adds a click affordance for touch/keyboard users).
(function () {
  const triggers = document.querySelectorAll('[data-menu-trigger]');
  const closeBtns = document.querySelectorAll('[data-menu-close]');

  function closeAll(except) {
    triggers.forEach(function (t) {
      if (t !== except) {
        t.classList.remove('is-open');
        t.setAttribute('aria-expanded', 'false');
      }
    });
  }

  triggers.forEach(function (trigger) {
    trigger.addEventListener('click', function () {
      const willOpen = !trigger.classList.contains('is-open');
      closeAll();
      trigger.classList.toggle('is-open', willOpen);
      trigger.setAttribute('aria-expanded', willOpen ? 'true' : 'false');
    });
  });

  closeBtns.forEach(function (btn) {
    btn.addEventListener('click', function () { closeAll(); });
  });

  document.addEventListener('click', function (e) {
    if (!e.target.closest('[data-mega-menu-nav]') && !e.target.closest('.mega-menu')) {
      closeAll();
    }
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
