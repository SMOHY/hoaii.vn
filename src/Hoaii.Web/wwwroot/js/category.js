// Category hero carousel (Figma node 1519:33997) — a coverflow strip where the
// centre slide is taller than its neighbours, with the product name below.
(function () {
    const root = document.querySelector('[data-cat-carousel]');
    if (!root) return;

    const track = root.querySelector('[data-cat-track]');
    const slides = Array.from(root.querySelectorAll('[data-cat-slide]'));
    const counter = root.querySelector('[data-cat-counter]');
    const title = root.querySelector('[data-cat-title]');
    const prev = root.querySelector('[data-cat-prev]');
    const next = root.querySelector('[data-cat-next]');

    if (slides.length === 0) return;

    let index = 0;

    function render() {
        slides.forEach((slide, i) => {
            slide.classList.toggle('is-active', i === index);
        });

        // Centre the active slide inside the track.
        const active = slides[index];
        const offset = active.offsetLeft + active.offsetWidth / 2 - track.clientWidth / 2;
        track.scrollTo({ left: offset, behavior: 'smooth' });

        if (counter) counter.textContent = `${index + 1}/${slides.length}`;
        if (title) title.textContent = (active.dataset.name || '').toUpperCase();
    }

    function step(delta) {
        index = (index + delta + slides.length) % slides.length;
        render();
    }

    prev?.addEventListener('click', () => step(-1));
    next?.addEventListener('click', () => step(1));

    slides.forEach((slide, i) => {
        slide.addEventListener('click', e => {
            // First click centres the slide; a second click follows the link.
            if (i !== index) {
                e.preventDefault();
                index = i;
                render();
            }
        });
    });

    render();
    window.addEventListener('resize', render);
})();
