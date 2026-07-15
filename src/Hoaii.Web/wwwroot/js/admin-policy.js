// Policy page editor: add / remove / reorder content blocks. Each row is a (kind, text) pair
// posted as parallel arrays blockKinds[] / blockTexts[], so DOM order == document order on save.
(function () {
  'use strict';

  const form = document.querySelector('[data-policy-form]');
  if (!form) return;

  const list = form.querySelector('[data-block-list]');

  function makeRow() {
    const row = document.createElement('div');
    row.className = 'admin-block-row';
    row.setAttribute('data-block-row', '');
    row.innerHTML =
      '<select name="blockKinds" class="admin-block-row__kind">' +
        '<option value="Paragraph">Đoạn văn</option>' +
        '<option value="Heading">Tiêu đề mục</option>' +
        '<option value="Bullet">Gạch đầu dòng</option>' +
      '</select>' +
      '<textarea name="blockTexts" rows="2" class="admin-block-row__text"></textarea>' +
      '<div class="admin-block-row__controls">' +
        '<button type="button" data-move="-1" aria-label="Lên">▲</button>' +
        '<button type="button" data-move="1" aria-label="Xuống">▼</button>' +
        '<button type="button" data-remove-block aria-label="Xóa">✕</button>' +
      '</div>';
    return row;
  }

  form.querySelector('[data-add-block]')?.addEventListener('click', function () {
    const row = makeRow();
    list.appendChild(row);
    row.querySelector('textarea').focus();
  });

  list.addEventListener('click', function (e) {
    const row = e.target.closest('[data-block-row]');
    if (!row) return;

    if (e.target.hasAttribute('data-remove-block')) {
      row.remove();
    } else if (e.target.hasAttribute('data-move')) {
      const dir = parseInt(e.target.getAttribute('data-move'), 10);
      if (dir < 0 && row.previousElementSibling) {
        row.parentNode.insertBefore(row, row.previousElementSibling);
      } else if (dir > 0 && row.nextElementSibling) {
        row.parentNode.insertBefore(row.nextElementSibling, row);
      }
    }
  });
})();
