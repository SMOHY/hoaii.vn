(function () {
  // Gallery: thumbnail / dot click swaps the main image.
  var mainImage = document.getElementById('pdp-main-image');
  var thumbs = document.querySelectorAll('.pdp-gallery__thumb');
  var dots = document.querySelectorAll('.pdp-dot');

  function setActiveGalleryIndex(index) {
    thumbs.forEach(function (t, i) { t.classList.toggle('active', i === index); });
    dots.forEach(function (d, i) { d.classList.toggle('active', i === index); });
    var url = thumbs[index] && thumbs[index].getAttribute('data-image');
    if (mainImage) {
      mainImage.style.backgroundImage = url ? "url('" + url + "')" : '';
    }
  }

  thumbs.forEach(function (thumb, index) {
    thumb.addEventListener('click', function () { setActiveGalleryIndex(index); });
  });
  dots.forEach(function (dot, index) {
    dot.addEventListener('click', function () { setActiveGalleryIndex(index); });
  });

  // Color swatch selection
  var swatches = document.querySelectorAll('.pdp-swatch');
  var selectedColorLabel = document.getElementById('pdp-selected-color');
  swatches.forEach(function (swatch) {
    swatch.addEventListener('click', function () {
      swatches.forEach(function (s) { s.classList.remove('selected'); });
      swatch.classList.add('selected');
      if (selectedColorLabel) {
        selectedColorLabel.textContent = swatch.getAttribute('data-color-name') || '';
      }
    });
  });

  // Box type selection — keep the hidden variantId input (posted to /gio-hang/them) in sync.
  var boxOptions = document.querySelectorAll('.pdp-box-option');
  var variantIdInput = document.getElementById('pdp-variant-id-input');
  boxOptions.forEach(function (btn) {
    btn.addEventListener('click', function () {
      boxOptions.forEach(function (b) { b.classList.remove('selected'); });
      btn.classList.add('selected');
      if (variantIdInput) {
        variantIdInput.value = btn.getAttribute('data-box-id') || '';
      }
    });
  });

  // Quantity stepper — keeps the visible count and the hidden quantity input in sync.
  var qtyValue = document.getElementById('pdp-qty-value');
  var qtyInput = document.getElementById('pdp-qty-input');
  var decreaseBtn = document.querySelector('[data-qty-decrease]');
  var increaseBtn = document.querySelector('[data-qty-increase]');

  function getQty() { return parseInt(qtyValue.textContent, 10) || 1; }
  function setQty(value) {
    var qty = Math.max(1, value);
    qtyValue.textContent = String(qty);
    if (qtyInput) {
      qtyInput.value = String(qty);
    }
  }

  decreaseBtn && decreaseBtn.addEventListener('click', function () { setQty(getQty() - 1); });
  increaseBtn && increaseBtn.addEventListener('click', function () { setQty(getQty() + 1); });
})();
