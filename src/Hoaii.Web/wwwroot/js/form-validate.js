// Client-side validation for the checkout / contact / wholesale forms.
//
// The views already render an empty error span next to each field (asp-validation-for), which
// only ever filled in after a server round-trip. This fills the same spans immediately, so a
// missing phone number costs a keystroke rather than a page load. The server still validates —
// this is only there to save the trip.
(function () {
  'use strict';

  const MESSAGES = {
    valueMissing: 'Vui lòng điền thông tin này.',
    typeMismatch: 'Định dạng chưa đúng.',
    tooShort: 'Nội dung quá ngắn.',
    patternMismatch: 'Định dạng chưa đúng.',
  };

  function messageFor(input) {
    const v = input.validity;
    if (v.valueMissing) return MESSAGES.valueMissing;
    if (v.typeMismatch) return input.type === 'email' ? 'Email chưa đúng định dạng.' : MESSAGES.typeMismatch;
    if (v.tooShort) return MESSAGES.tooShort;
    if (v.patternMismatch) return MESSAGES.patternMismatch;
    return input.validationMessage;
  }

  // Checkout renders an empty span per field; the contact and wholesale forms don't, so one
  // is created on demand rather than editing every field in those views.
  function errorSlot(input) {
    const field = input.closest('.form-field, .simple-field');
    if (!field) return null;

    let slot = field.querySelector('.form-field__error, .field-error');
    if (!slot) {
      slot = document.createElement('span');
      slot.className = 'field-error';
      field.appendChild(slot);
    }
    return slot;
  }

  function show(input) {
    const slot = errorSlot(input);
    input.setAttribute('aria-invalid', 'true');
    input.classList.add('is-invalid');
    if (slot) slot.textContent = messageFor(input);
  }

  function clear(input) {
    const slot = errorSlot(input);
    input.removeAttribute('aria-invalid');
    input.classList.remove('is-invalid');
    if (slot) slot.textContent = '';
  }

  document.querySelectorAll('form').forEach(function (form) {
    const fields = form.querySelectorAll('input[required], input[type="email"], textarea[required], select[required]');
    if (fields.length === 0) return;

    // Take over from the browser's own bubbles so the message lands next to the field and the
    // page scrolls to it. The `required` attributes stay, so with JS off the browser still
    // blocks the submit.
    form.setAttribute('novalidate', '');

    // Only re-check on input once the field has already been flagged, so the form doesn't
    // shout at someone who is still halfway through typing.
    fields.forEach(function (input) {
      input.addEventListener('input', function () {
        if (input.classList.contains('is-invalid') && input.checkValidity()) clear(input);
      });
      input.addEventListener('blur', function () {
        if (input.value !== '' && !input.checkValidity()) show(input);
      });
    });

    form.addEventListener('submit', function (e) {
      let firstBad = null;
      fields.forEach(function (input) {
        if (input.checkValidity()) {
          clear(input);
        } else {
          show(input);
          if (!firstBad) firstBad = input;
        }
      });

      if (firstBad) {
        e.preventDefault();
        e.stopPropagation(); // keep cart-live.js from posting an invalid form
        firstBad.focus();
        firstBad.scrollIntoView({ block: 'center', behavior: 'smooth' });
      }
    }, true); // capture: run before the async-submit handler
  });
})();
