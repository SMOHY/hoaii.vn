// Ảnh dịch vụ chạy luân phiên trong khung ảnh của từng tab.
//
// Nhịp 2,5 giây là cố ý: home.js tự chuyển sang tab kế tiếp sau mỗi 4 giây (theo prototype
// Figma), nên mỗi tab chỉ hiện khoảng 4 giây mỗi vòng. Nhịp 4 giây trùng khít với nhịp đổi tab
// và người xem sẽ không bao giờ thấy ảnh thứ hai. 2,5 giây bảo đảm mỗi lần tab được mở là thấy
// ảnh đổi ít nhất một lần.
//
// Không dừng khi tab đang ẩn: ẩn rồi thì đổi ảnh chẳng tốn gì mà lần sau quay lại người xem
// gặp một ảnh khác, đúng ý "thay phiên nhau" hơn là mỗi lần mở tab lại thấy đúng ảnh cũ.
(function () {
  'use strict';

  const reduced = window.matchMedia && window.matchMedia('(prefers-reduced-motion: reduce)').matches;
  if (reduced) return;

  document.querySelectorAll('[data-service-slides]').forEach(function (box) {
    const slides = [...box.querySelectorAll('.custom-services__panel-img')];
    if (slides.length < 2) return;

    let i = 0;
    setInterval(function () {
      slides[i].classList.remove('is-active');
      i = (i + 1) % slides.length;
      slides[i].classList.add('is-active');
    }, 2500);
  });
})();
