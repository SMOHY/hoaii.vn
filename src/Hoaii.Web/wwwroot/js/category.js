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

    // Swiping the track used to leave the counter and the title describing a different
    // product than the one on screen. Follow the scroll and re-sync.
    let scrollTimer = null;
    track?.addEventListener('scroll', () => {
        clearTimeout(scrollTimer);
        scrollTimer = setTimeout(() => {
            const mid = track.scrollLeft + track.clientWidth / 2;
            let nearest = 0;
            let best = Infinity;
            slides.forEach((slide, i) => {
                const distance = Math.abs(slide.offsetLeft + slide.offsetWidth / 2 - mid);
                if (distance < best) { best = distance; nearest = i; }
            });
            if (nearest !== index) {
                index = nearest;
                slides.forEach((slide, i) => slide.classList.toggle('is-active', i === index));
                if (counter) counter.textContent = `${index + 1}/${slides.length}`;
                if (title) title.textContent = (slides[index].dataset.name || '').toUpperCase();
            }
        }, 90);
    }, { passive: true });

    render();
    window.addEventListener('resize', render);
})();

// Sort / filter popovers. The links inside work on their own; this only saves a page load
// worth of hunting by showing the options in place.
(function () {
    const menus = Array.from(document.querySelectorAll('[data-filter-menu]'));
    if (menus.length === 0) return;

    function closeAll(except) {
        menus.forEach(menu => {
            if (menu === except) return;
            menu.classList.remove('is-open');
            menu.querySelector('[data-filter-toggle]')?.setAttribute('aria-expanded', 'false');
        });
    }

    menus.forEach(menu => {
        const toggle = menu.querySelector('[data-filter-toggle]');
        toggle?.addEventListener('click', e => {
            e.stopPropagation();
            const willOpen = !menu.classList.contains('is-open');
            closeAll(menu);
            menu.classList.toggle('is-open', willOpen);
            toggle.setAttribute('aria-expanded', String(willOpen));
        });
    });

    document.addEventListener('click', () => closeAll());
    document.addEventListener('keydown', e => { if (e.key === 'Escape') closeAll(); });
})();
