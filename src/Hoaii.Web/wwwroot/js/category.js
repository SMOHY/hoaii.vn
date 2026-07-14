// Category hero carousel (Figma node 1519:33997) — a coverflow strip where the centre slide
// is taller than its neighbours, with the product name below.
//
// Figma always shows slides running off both edges. A plain track can't: on the first slide
// there is nothing to its left, so the strip sat in the middle of the band with the left half
// empty. The slides are cloned either side and the strip is scrolled back to the middle copy
// when it reaches an edge, so it reads as an endless run of product shots.
(function () {
    const root = document.querySelector('[data-cat-carousel]');
    if (!root) return;

    const track = root.querySelector('[data-cat-track]');
    const originals = Array.from(root.querySelectorAll('[data-cat-slide]'));
    const counter = root.querySelector('[data-cat-counter]');
    const title = root.querySelector('[data-cat-title]');
    const prev = root.querySelector('[data-cat-prev]');
    const next = root.querySelector('[data-cat-next]');

    const count = originals.length;
    if (count === 0) return;

    const looped = count > 1;
    if (looped) {
        // One copy before, one after. Clones are inert: no links, no tab stops.
        const before = originals.map(s => s.cloneNode(true));
        const after = originals.map(s => s.cloneNode(true));
        [...before, ...after].forEach(clone => {
            clone.removeAttribute('href');
            clone.setAttribute('aria-hidden', 'true');
            clone.setAttribute('tabindex', '-1');
        });
        before.reverse().forEach(clone => track.prepend(clone));
        after.forEach(clone => track.append(clone));
    }

    const slides = Array.from(root.querySelectorAll('[data-cat-slide]'));
    const offset = looped ? count : 0; // where the real slides start
    let index = 0; // index into the originals

    function centre(slide, smooth) {
        const left = slide.offsetLeft + slide.offsetWidth / 2 - track.clientWidth / 2;
        track.scrollTo({ left, behavior: smooth ? 'smooth' : 'auto' });
    }

    function render(smooth = true) {
        const active = offset + index;
        slides.forEach((slide, i) => slide.classList.toggle('is-active', i === active));
        centre(slides[active], smooth);

        if (counter) counter.textContent = `${index + 1}/${count}`;
        if (title) title.textContent = (originals[index].dataset.name || '').toUpperCase();
    }

    function step(delta) {
        index = (index + delta + count) % count;
        render();
    }

    prev?.addEventListener('click', () => step(-1));
    next?.addEventListener('click', () => step(1));

    slides.forEach((slide, i) => {
        slide.addEventListener('click', e => {
            // First click centres the slide; a second click follows the link.
            const target = ((i - offset) % count + count) % count;
            if (target !== index) {
                e.preventDefault();
                index = target;
                render();
            }
        });
    });

    // Swiping used to leave the counter and the title describing a different product than the
    // one on screen — follow the scroll and re-sync. This is also where the loop wraps.
    let scrollTimer = null;
    track?.addEventListener('scroll', () => {
        clearTimeout(scrollTimer);
        scrollTimer = setTimeout(() => {
            const mid = track.scrollLeft + track.clientWidth / 2;
            let nearest = 0;
            let best = Infinity;
            slides.forEach((slide, i) => {
                const d = Math.abs(slide.offsetLeft + slide.offsetWidth / 2 - mid);
                if (d < best) { best = d; nearest = i; }
            });

            index = ((nearest - offset) % count + count) % count;
            const active = offset + index;
            slides.forEach((slide, i) => slide.classList.toggle('is-active', i === active));
            if (counter) counter.textContent = `${index + 1}/${count}`;
            if (title) title.textContent = (originals[index].dataset.name || '').toUpperCase();

            // Landed on a clone — jump silently back onto the real one.
            if (looped && nearest !== active) centre(slides[active], false);
        }, 120);
    }, { passive: true });

    render(false);
    window.addEventListener('resize', () => render(false));
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
