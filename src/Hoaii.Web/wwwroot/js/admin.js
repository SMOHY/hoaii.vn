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

  // Block over-5MB uploads on plain (non-async) upload forms before they hit the wire, so the
  // shop owner sees a clear message instead of the framework's bare "413 Payload Too Large" page.
  // The JS pickers do their own check; this covers the standalone media library form.
  var MAX = 5 * 1024 * 1024;
  document.addEventListener('submit', function (e) {
    var form = e.target;
    if (!form.matches || !form.matches('[data-media-upload]')) return;
    var tooBig = [];
    form.querySelectorAll('input[type=file]').forEach(function (input) {
      Array.prototype.forEach.call(input.files, function (f) { if (f.size > MAX) tooBig.push(f.name); });
    });
    if (tooBig.length) {
      e.preventDefault();
      e.stopImmediatePropagation();
      window.alert('Ảnh vượt quá 5MB, không tải lên được:\n' + tooBig.join('\n'));
    }
  }, true);
})();
