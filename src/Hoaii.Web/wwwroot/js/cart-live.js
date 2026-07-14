// Keeps the cart interactive without full page reloads.
//
// Every cart form still works with JS off — it posts, the server redirects, the page renders.
// With JS on we post the same form in the background, then re-fetch the current page and swap
// only the regions that show cart state. That way there is one source of truth for the markup
// (Razor) and typing in the checkout form is never thrown away by a quantity change.
(function () {
  'use strict';

  const CART_ACTION = /^\/gio-hang\//;

  // ---------- toast ----------
  let toastHost = null;

  function toast(message, kind) {
    if (!toastHost) {
      toastHost = document.createElement('div');
      toastHost.className = 'toast-host';
      toastHost.setAttribute('role', 'status');
      toastHost.setAttribute('aria-live', 'polite');
      document.body.appendChild(toastHost);
    }

    const el = document.createElement('div');
    el.className = 'toast' + (kind ? ' toast--' + kind : '');
    el.textContent = message;
    toastHost.appendChild(el);

    // Let the element land in the DOM before transitioning it in.
    requestAnimationFrame(function () { el.classList.add('is-in'); });

    setTimeout(function () {
      el.classList.remove('is-in');
      el.addEventListener('transitionend', function () { el.remove(); }, { once: true });
    }, 3200);
  }

  window.hoaiiToast = toast;

  // ---------- region refresh ----------
  // Swaps innerHTML rather than the elements themselves, so open/closed state (e.g. the
  // mini-cart's .is-open class) survives the update.
  const REGIONS = ['[data-minicart-panel]', '[data-cart-region="cart-page"]', '[data-cart-region="checkout-summary"]'];

  async function refreshCartRegions() {
    const res = await fetch(location.href, { headers: { 'X-Requested-With': 'XMLHttpRequest' } });
    const html = await res.text();
    const doc = new DOMParser().parseFromString(html, 'text/html');

    REGIONS.forEach(function (selector) {
      const fresh = doc.querySelector(selector);
      const current = document.querySelector(selector);
      if (fresh && current) {
        current.innerHTML = fresh.innerHTML;
        if (fresh.dataset.cartCount !== undefined) {
          current.dataset.cartCount = fresh.dataset.cartCount;
        }
      }
    });

    syncBadges();
    document.dispatchEvent(new CustomEvent('cart:updated'));
  }

  function syncBadges() {
    const panel = document.querySelector('[data-minicart-panel]');
    const count = parseInt(panel?.dataset.cartCount || '0', 10);
    document.querySelectorAll('[data-cart-badge]').forEach(function (badge) {
      badge.textContent = String(count);
      badge.hidden = count <= 0;
    });
  }

  // ---------- submit interception ----------
  function busy(form, on) {
    form.querySelectorAll('button[type="submit"]').forEach(function (btn) {
      btn.disabled = on;
      btn.classList.toggle('is-busy', on);
    });
  }

  async function post(form, submitter) {
    const body = new FormData(form);
    // A submit button's own name/value (the quantity steppers rely on this) is not in FormData.
    if (submitter && submitter.name) {
      body.set(submitter.name, submitter.value);
    }

    return fetch(form.action, {
      method: 'POST',
      body: body,
      headers: { 'X-Requested-With': 'XMLHttpRequest' },
    });
  }

  document.addEventListener('submit', async function (e) {
    const form = e.target;
    if (!(form instanceof HTMLFormElement)) return;

    const action = new URL(form.action, location.origin).pathname;
    const isCart = CART_ACTION.test(action);
    const isAsync = form.hasAttribute('data-async-form');
    if (!isCart && !isAsync) return;
    if (!form.checkValidity()) return; // let the browser show its own message

    e.preventDefault();
    busy(form, true);

    try {
      const res = await post(form, e.submitter);

      if (isAsync) {
        const data = await res.json().catch(function () { return {}; });
        toast(data.message || (res.ok ? 'Đã gửi.' : 'Có lỗi xảy ra, vui lòng thử lại.'), res.ok ? 'ok' : 'error');
        if (res.ok) form.reset();
        return;
      }

      if (!res.ok) {
        toast('Không cập nhật được giỏ hàng, vui lòng thử lại.', 'error');
        return;
      }

      await refreshCartRegions();

      if (action === '/gio-hang/them') {
        toast('Đã thêm vào giỏ hàng.', 'ok');
        // Only pop the drawer open on pages that aren't already showing the cart.
        if (!document.querySelector('[data-cart-region="cart-page"], [data-cart-region="checkout-summary"]')) {
          document.dispatchEvent(new CustomEvent('minicart:open'));
        }
      } else if (action === '/gio-hang/xoa') {
        toast('Đã xóa khỏi giỏ hàng.', 'ok');
      } else if (action === '/gio-hang/ap-dung-ma') {
        toast('Đã áp dụng ưu đãi.', 'ok');
        document.dispatchEvent(new CustomEvent('voucher:applied'));
      }
    } catch {
      toast('Không kết nối được máy chủ.', 'error');
    } finally {
      busy(form, false);
    }
  });

  syncBadges();
})();
