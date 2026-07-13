// OTP resend countdown timer.
(function () {
  const card = document.querySelector('[data-resend-after]');
  if (!card) return;

  let remaining = parseInt(card.getAttribute('data-resend-after'), 10) || 0;
  const timerEl = card.querySelector('[data-otp-timer]');
  const resendForm = card.querySelector('[data-resend-form]');

  function render() {
    if (remaining <= 0) {
      timerEl.hidden = true;
      resendForm.hidden = false;
      return;
    }
    const mm = String(Math.floor(remaining / 60)).padStart(2, '0');
    const ss = String(remaining % 60).padStart(2, '0');
    timerEl.textContent = 'Gửi lại sau ' + mm + ':' + ss;
  }

  render();
  const interval = setInterval(function () {
    remaining -= 1;
    render();
    if (remaining <= 0) clearInterval(interval);
  }, 1000);
})();
