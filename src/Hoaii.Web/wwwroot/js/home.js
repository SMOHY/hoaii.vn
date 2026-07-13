// Custom services tab switcher — see design-specs/custom-services.md
(function () {
  document.querySelectorAll('[data-component="custom-services"]').forEach(function (root) {
    var tabs = root.querySelectorAll('.custom-services__tab');
    var panels = root.querySelectorAll('.custom-services__panel-item');

    tabs.forEach(function (tab) {
      tab.addEventListener('click', function () {
        var key = tab.getAttribute('data-tab');

        tabs.forEach(function (t) {
          t.classList.toggle('is-active', t === tab);
          t.setAttribute('aria-selected', t === tab ? 'true' : 'false');
        });
        panels.forEach(function (p) {
          p.classList.toggle('is-active', p.getAttribute('data-panel') === key);
        });
      });
    });
  });
})();
