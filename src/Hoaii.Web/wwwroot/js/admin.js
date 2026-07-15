// Admin-only glue. The heavy lifting (async forms, toasts, validation, modal a11y) is reused
// from the storefront scripts already loaded on the page.
(function () {
  'use strict';

  // Mobile sidebar toggle.
  var sidebar = document.querySelector('[data-admin-sidebar]');
  var toggle = document.querySelector('[data-admin-menu-toggle]');
  toggle && toggle.addEventListener('click', function () {
    sidebar && sidebar.classList.toggle('is-open');
  });
  document.addEventListener('click', function (e) {
    if (!sidebar || !sidebar.classList.contains('is-open')) return;
    if (sidebar.contains(e.target) || (toggle && toggle.contains(e.target))) return;
    sidebar.classList.remove('is-open');
  });

  // Confirm before any destructive submit.
  document.addEventListener('submit', function (e) {
    var msg = e.target.getAttribute && e.target.getAttribute('data-confirm');
    if (msg && !window.confirm(msg)) {
      e.preventDefault();
      e.stopImmediatePropagation();
    }
  }, true);
})();
