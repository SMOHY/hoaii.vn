// Custom services tab switcher — see design-specs/custom-services.md
// Auto-advance timing (4s per tab, looping In khắc -> Lựa chọn gói quà -> Thiết kế -> ...)
// is read straight from the Figma prototype's AFTER_TIMEOUT reactions on nodes 1234:40239/
// 40241/40290, not something invented here. Paused on hover/focus so it doesn't fight a
// visitor mid-read, and stopped for prefers-reduced-motion.
(function () {
  var AUTO_ADVANCE_MS = 4000;
  var reduced = window.matchMedia('(prefers-reduced-motion: reduce)');

  document.querySelectorAll('[data-component="custom-services"]').forEach(function (root) {
    var tabs = Array.prototype.slice.call(root.querySelectorAll('.custom-services__tab'));
    var panels = root.querySelectorAll('.custom-services__panel-item');
    var timer = null;

    // Figma's own reactions use two different crossfade speeds for the same swap: 0.5s ease-out
    // when a person clicks a tab, 0.1s ease-out for the unattended auto-advance — set as a CSS
    // var so .custom-services__panel-item's transition picks up whichever one applies.
    function activate(tab, fadeMs) {
      root.style.setProperty('--cs-fade-duration', (fadeMs / 1000) + 's');
      var key = tab.getAttribute('data-tab');
      tabs.forEach(function (t) {
        t.classList.toggle('is-active', t === tab);
        t.setAttribute('aria-selected', t === tab ? 'true' : 'false');
      });
      panels.forEach(function (p) {
        p.classList.toggle('is-active', p.getAttribute('data-panel') === key);
      });
    }

    function stopAuto() {
      if (timer) { clearInterval(timer); timer = null; }
    }

    function startAuto() {
      if (reduced.matches || tabs.length < 2) return;
      stopAuto();
      timer = setInterval(function () {
        var current = tabs.findIndex(function (t) { return t.classList.contains('is-active'); });
        activate(tabs[(current + 1) % tabs.length], 100);
      }, AUTO_ADVANCE_MS);
    }

    tabs.forEach(function (tab) {
      tab.addEventListener('click', function () {
        activate(tab, 500);
        startAuto(); // manual pick restarts the 4s window instead of jumping mid-cycle
      });
    });

    root.addEventListener('mouseenter', stopAuto);
    root.addEventListener('mouseleave', startAuto);
    root.addEventListener('focusin', stopAuto);
    root.addEventListener('focusout', startAuto);

    startAuto();
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

// Story banner video — plays only while the section is hovered/focused, so a 1.6MB file is
// never fetched for visitors who scroll past. The grow-to-fill motion itself lives in CSS.
(function () {
  const banner = document.querySelector('[data-story-banner]');
  const video = banner?.querySelector('.story-banner__video');
  if (!banner || !video) return;

  const reduced = window.matchMedia('(prefers-reduced-motion: reduce)');

  function start() {
    if (reduced.matches) return;
    // preload="none" means the first hover has to fetch before it can play.
    video.play().catch(function () { /* autoplay blocked — the poster stays up */ });
  }
  function stop() {
    video.pause();
    video.currentTime = 0;
  }

  banner.addEventListener('mouseenter', start);
  banner.addEventListener('mouseleave', stop);
  banner.addEventListener('focusin', start);
  banner.addEventListener('focusout', stop);
})();
