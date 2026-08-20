// Ô video trong dải ảnh sản phẩm: bấm ô video thì video phủ lên khung ảnh chính, bấm một ảnh
// bất kỳ thì quay lại ảnh. Iframe YouTube/Vimeo chỉ được nạp khi người xem thực sự mở video —
// nạp sẵn thì mỗi lượt vào trang sản phẩm đều gọi sang máy chủ của họ.
(function () {
  'use strict';

  const panel = document.querySelector('[data-video-panel]');
  // Hai lối vào: ô video trong dải ảnh nhỏ (desktop) và nút nổi trên ảnh (điện thoại).
  const openBtns = [...document.querySelectorAll('[data-video-open]')];
  if (!panel || !openBtns.length) return;

  const iframe = panel.querySelector('iframe[data-video-src]');
  const video = panel.querySelector('video');

  function show() {
    if (iframe && !iframe.getAttribute('src')) {
      iframe.setAttribute('src', iframe.getAttribute('data-video-src'));
    }
    panel.hidden = false;
    document.querySelectorAll('.pdp-gallery__thumb').forEach(t => t.classList.remove('active'));
    openBtns.forEach(b => b.classList.add('active'));
  }

  function hide() {
    panel.hidden = true;
    if (video) video.pause();
    // Dừng hẳn video nhúng: ẩn iframe không làm nó ngừng phát tiếng.
    if (iframe && iframe.getAttribute('src')) iframe.removeAttribute('src');
    openBtns.forEach(b => b.classList.remove('active'));
  }

  openBtns.forEach(b => b.addEventListener('click', show));

  document.querySelectorAll('.pdp-gallery__thumb:not([data-video-open])').forEach(function (thumb) {
    thumb.addEventListener('click', hide);
  });
  document.querySelectorAll('.pdp-dot').forEach(function (dot) {
    dot.addEventListener('click', hide);
  });
})();
