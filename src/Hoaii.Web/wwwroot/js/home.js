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

// Hero carousel. The arrows and dots are only rendered when there is more than one slide,
// so with the single hero image delivered so far this does nothing at all.
(function () {
  var root = document.querySelector('[data-hero-carousel]');
  if (!root) return;

  var slides = Array.prototype.slice.call(root.querySelectorAll('[data-hero-slide]'));
  var dots = Array.prototype.slice.call(root.querySelectorAll('[data-hero-dot]'));
  if (slides.length < 2) return;

  var titleEl = root.querySelector('[data-hero-title]');
  var mobileTitleEl = root.querySelector('[data-hero-mobile-title]');
  var subtitleEl = root.querySelector('[data-hero-subtitle]');
  var mobileSubtitleEl = root.querySelector('[data-hero-mobile-subtitle]');
  var index = 0;

  function show(next) {
    index = (next + slides.length) % slides.length;
    slides.forEach(function (slide, i) { slide.classList.toggle('is-active', i === index); });
    dots.forEach(function (dot, i) { dot.classList.toggle('dot--active', i === index); });

    var copy = slides[index].dataset;
    if (titleEl && copy.title) titleEl.textContent = copy.title;
    if (mobileTitleEl && copy.mobileTitle) mobileTitleEl.textContent = copy.mobileTitle;
    if (subtitleEl && copy.subtitle) subtitleEl.textContent = copy.subtitle;
    if (mobileSubtitleEl && copy.mobileSubtitle) mobileSubtitleEl.textContent = copy.mobileSubtitle;
  }

  root.querySelector('[data-hero-prev]')?.addEventListener('click', function () { show(index - 1); });
  root.querySelector('[data-hero-next]')?.addEventListener('click', function () { show(index + 1); });
  dots.forEach(function (dot, i) { dot.addEventListener('click', function () { show(i); }); });
})();
