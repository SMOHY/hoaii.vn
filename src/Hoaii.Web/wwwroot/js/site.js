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

// Mega-menu open/close. Figma's own reactions (nodes 1287:56680, 56777, 56871, 56965) fire on
// BOTH ON_CLICK and ON_HOVER, so real mouse users can open a panel just by hovering the trigger;
// click still works too (touch, keyboard, and mouse users who'd rather not rely on hover). Only
// one panel is ever open at a time.
(function () {
  const triggers = document.querySelectorAll('[data-menu-trigger]');
  const canHover = window.matchMedia('(hover: hover) and (pointer: fine)').matches;
  let closeTimer = null;

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
    pinned = null;
    triggers.forEach(function (t) {
      if (!t.classList.contains('is-open')) return;

      const panel = panelFor(t);
      if (instant && panel) panel.classList.add('is-closing-instantly');

      t.classList.remove('is-open');
      t.setAttribute('aria-expanded', 'false');
      if (panel) panel.classList.remove('is-open');

      if (instant && panel) {
        void panel.offsetHeight; // commit the collapsed height while transitions are off
        panel.classList.remove('is-closing-instantly');
      }
    });
  }

  function open(trigger, instant) {
    if (trigger.classList.contains('is-open')) return;
    closeAll(instant);
    trigger.classList.add('is-open');
    trigger.setAttribute('aria-expanded', 'true');
    panelFor(trigger)?.classList.add('is-open');
    document.dispatchEvent(new CustomEvent('nav-flyout:open', { detail: 'mega-menu' }));
  }

  // The search flyout sits at the same spot under the nav — close this one out if that opens.
  document.addEventListener('nav-flyout:open', function (e) {
    if (e.detail !== 'mega-menu') closeAll();
  });

  // Trên máy có chuột, con trỏ luôn đi qua mục nav trước khi bấm nên hover đã mở panel; cú click
  // ngay sau đó thấy is-open đang bật và toggle nó tắt — bấm vào mục nav thành ra đóng menu vừa
  // hiện ra. Click giờ luôn mở và ghim panel lại; đóng bằng dấu X, Escape, hoặc bấm ra ngoài.
  let pinned = null;

  triggers.forEach(function (trigger) {
    trigger.addEventListener('click', function () {
      const already = trigger.classList.contains('is-open');
      if (!already) {
        closeAll(true); // mở menu khác thì thay thẳng menu đang mở
        trigger.classList.add('is-open');
        trigger.setAttribute('aria-expanded', 'true');
        panelFor(trigger)?.classList.add('is-open');
        document.dispatchEvent(new CustomEvent('nav-flyout:open', { detail: 'mega-menu' }));
      }
      // Ghim sau closeAll, vì closeAll tự xoá ghim.
      pinned = trigger;
    });
  });

  if (canHover) {
    // A gap of real pixels separates the nav row from the dropdown panel below it, so leaving the
    // trigger to cross that gap into the panel must not read as "left the menu" — hence the short
    // grace delay before actually closing, cancelled if the pointer lands on either piece.
    function cancelClose() { clearTimeout(closeTimer); }
    function scheduleClose() { cancelClose(); closeTimer = setTimeout(function () { closeAll(); }, 150); }

    triggers.forEach(function (trigger) {
      trigger.addEventListener('mouseenter', function () { cancelClose(); open(trigger, true); });
      trigger.addEventListener('mouseleave', function () { if (pinned !== trigger) scheduleClose(); });

      const panel = panelFor(trigger);
      if (panel) {
        panel.addEventListener('mouseenter', cancelClose);
        panel.addEventListener('mouseleave', function () { if (pinned !== trigger) scheduleClose(); });
      }
    });
  }

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
    const openTrigger = document.querySelector('[data-menu-trigger].is-open');
    if (!openTrigger) return;
    closeAll();
    openTrigger.focus();
  });
})();

// Search flyout — Figma node 940:19274: hover or click the search icon to reveal a full-width
// bar under the nav (see nav.css .search-flyout-wrap) instead of jumping straight to /tim-kiem.
(function () {
  const trigger = document.querySelector('[data-search-trigger]');
  const wrap = document.querySelector('[data-search-flyout]');
  const input = wrap?.querySelector('[data-search-input]');
  const closeBtn = wrap?.querySelector('[data-search-close]');
  if (!trigger || !wrap) return;

  const canHover = window.matchMedia('(hover: hover) and (pointer: fine)').matches;
  let closeTimer = null;

  function open() {
    wrap.hidden = false;
    trigger.setAttribute('aria-expanded', 'true');
    input?.focus();
    document.dispatchEvent(new CustomEvent('nav-flyout:open', { detail: 'search' }));
  }
  function close() {
    wrap.hidden = true;
    trigger.setAttribute('aria-expanded', 'false');
    pinned = false;
  }
  function cancelClose() { clearTimeout(closeTimer); }
  function scheduleClose() { cancelClose(); closeTimer = setTimeout(close, 150); }

  // A mega-menu opening at the same spot under the nav should close this one out.
  document.addEventListener('nav-flyout:open', function (e) {
    if (e.detail !== 'search' && !wrap.hidden) close();
  });

  // Trên máy có chuột, con trỏ luôn đi qua icon trước khi bấm, nên hover đã mở sẵn ô tìm kiếm
  // và cú click ngay sau đó lại đóng nó — bấm vào kính lúp thành ra không mở được gì.
  // Click giờ luôn mở và ghim lại; muốn đóng thì bấm nút X, Escape, hoặc bấm ra ngoài.
  let pinned = false;

  trigger.addEventListener('click', function () {
    cancelClose();
    pinned = true;
    open();
  });

  if (canHover) {
    trigger.addEventListener('mouseenter', function () { cancelClose(); open(); });
    trigger.addEventListener('mouseleave', function () { if (!pinned) scheduleClose(); });
    wrap.addEventListener('mouseenter', cancelClose);
    wrap.addEventListener('mouseleave', function () { if (!pinned) scheduleClose(); });
  }

  closeBtn?.addEventListener('click', close);

  document.addEventListener('click', function (e) {
    if (wrap.hidden) return;
    if (!e.target.closest('[data-search-flyout]') && !e.target.closest('[data-search-trigger]')) close();
  });

  document.addEventListener('keydown', function (e) {
    if (e.key === 'Escape' && !wrap.hidden) { close(); trigger.focus(); }
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
