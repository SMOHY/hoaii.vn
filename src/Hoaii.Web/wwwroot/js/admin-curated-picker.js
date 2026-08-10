// Search-and-pick products for a fixed-size chip list. Originally built for the 8 hand-curated
// mega-menu columns (Bán chạy nhất / Phiên bản giới hạn / Hoài gợi ý / Nổi bật) — each
// [data-curated-slot] there is its own <form>, submitted separately from the rest of the menu
// editor. Reused (Products/Edit.cshtml, "Sản phẩm liên quan") as a plain <div data-curated-slot>
// nested inside the product's own form instead, submitting together with it — the script only
// ever calls form.querySelector/contains, never form-specific APIs, so either tag works. Field
// name and search endpoint are configurable per slot (data-curated-field /
// data-curated-search-url) so the two call sites don't collide; both default to the menu editor's
// original values for backward compatibility.
(function () {
  'use strict';

  const MAX_PICKS = 4;

  document.querySelectorAll('[data-curated-slot]').forEach(function (form) {
    const chips = form.querySelector('[data-curated-chips]');
    const emptyHint = form.querySelector('[data-curated-empty-hint]');
    const search = form.querySelector('[data-curated-search]');
    const results = form.querySelector('[data-curated-results]');
    const fieldName = form.dataset.curatedField || 'productIds';
    const searchUrl = form.dataset.curatedSearchUrl || '/admin/menu/tim-san-pham';
    let debounceTimer = null;

    function pickedIds() {
      return [...chips.querySelectorAll('input[name="' + fieldName + '"]')].map(function (i) { return i.value; });
    }

    function syncEmptyHint() {
      if (emptyHint) emptyHint.style.display = chips.children.length > 0 ? 'none' : '';
    }

    function addChip(product) {
      if (pickedIds().includes(String(product.id)) || pickedIds().length >= MAX_PICKS) return;
      const chip = document.createElement('span');
      chip.className = 'admin-curated-chip';
      chip.setAttribute('data-curated-chip', '');
      chip.innerHTML =
        '<input type="hidden" name="' + fieldName + '" value="' + product.id + '">' +
        product.name +
        '<button type="button" data-curated-remove aria-label="Xoá">✕</button>';
      chips.appendChild(chip);
      syncEmptyHint();
    }

    chips.addEventListener('click', function (e) {
      if (e.target.hasAttribute('data-curated-remove')) {
        e.target.closest('[data-curated-chip]').remove();
        syncEmptyHint();
      }
    });

    function closeResults() {
      results.hidden = true;
      results.innerHTML = '';
    }

    async function runSearch(q) {
      try {
        const sep = searchUrl.indexOf('?') === -1 ? '?' : '&';
        const res = await fetch(searchUrl + sep + 'q=' + encodeURIComponent(q), { headers: { 'X-Requested-With': 'XMLHttpRequest' } });
        const items = await res.json();
        renderResults(items);
      } catch {
        results.innerHTML = '<p class="admin-field__hint">Không tìm được — thử lại.</p>';
        results.hidden = false;
      }
    }

    function renderResults(items) {
      if (pickedIds().length >= MAX_PICKS) {
        results.innerHTML = '<p class="admin-field__hint">Đã chọn đủ ' + MAX_PICKS + ' sản phẩm — xoá 1 mục trước khi thêm.</p>';
        results.hidden = false;
        return;
      }
      if (!items.length) {
        results.innerHTML = '<p class="admin-field__hint">Không thấy sản phẩm nào.</p>';
        results.hidden = false;
        return;
      }
      results.innerHTML = '';
      items.forEach(function (it) {
        const row = document.createElement('button');
        row.type = 'button';
        row.className = 'admin-curated-result';
        row.innerHTML =
          (it.imageUrl ? '<span class="admin-curated-result__thumb" style="background-image:url(\'' + it.imageUrl + '\')"></span>' : '<span class="admin-curated-result__thumb"></span>') +
          '<span><strong>' + it.name + '</strong><br><span class="admin-field__hint">' + it.category + '</span></span>';
        row.addEventListener('click', function () {
          addChip(it);
          search.value = '';
          closeResults();
        });
        results.appendChild(row);
      });
      results.hidden = false;
    }

    search.addEventListener('input', function () {
      clearTimeout(debounceTimer);
      const q = search.value.trim();
      if (!q) { closeResults(); return; }
      debounceTimer = setTimeout(function () { runSearch(q); }, 250);
    });
    search.addEventListener('focus', function () {
      if (search.value.trim()) runSearch(search.value.trim());
    });
    document.addEventListener('click', function (e) {
      if (!form.contains(e.target)) closeResults();
    });
  });
})();
