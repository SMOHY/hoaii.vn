// Danh sách ảnh sắp thứ tự được, dùng chung cho mọi form admin có nhiều ảnh (dải ảnh sản phẩm,
// ảnh chạy luân phiên của dịch vụ...). Bộ chọn ảnh nằm ở admin-imagefield.js; nút "+ Thêm ảnh"
// không có data-target nên ảnh chọn/tải lên rơi về đây qua sự kiện picker:append.
//
// Mỗi danh sách khai báo tên trường của nó bằng data-image-list="TenTruong", nhờ vậy cùng một
// đoạn mã phục vụ được cả "ImageUrls" của sản phẩm lẫn "imageUrls" của dịch vụ.
(function () {
  'use strict';

  const lists = [...document.querySelectorAll('[data-image-list]')];
  if (!lists.length) return;

  function fieldName(list) {
    return list.getAttribute('data-image-list') || 'ImageUrls';
  }

  function makeItem(list, url) {
    const item = document.createElement('div');
    item.className = 'admin-image-item';
    item.setAttribute('data-image-item', '');
    item.innerHTML =
      '<input type="hidden">' +
      '<div class="admin-image-item__thumb"></div>' +
      '<div class="admin-image-item__controls">' +
        '<button type="button" data-move="-1" aria-label="Trái">◀</button>' +
        '<button type="button" data-move="1" aria-label="Phải">▶</button>' +
        '<button type="button" data-remove aria-label="Xóa">✕</button>' +
      '</div>';
    const input = item.querySelector('input');
    input.name = fieldName(list);
    input.value = url;
    item.querySelector('.admin-image-item__thumb').style.backgroundImage = "url('" + url + "')";
    return item;
  }

  function add(list, url) {
    if (!url) return;
    const already = [...list.querySelectorAll('input')].some(i => i.value === url);
    if (already) return;
    list.appendChild(makeItem(list, url));
  }

  // Ảnh mới rơi vào danh sách vừa được bấm "+ Thêm ảnh"; chỉ có một danh sách thì khỏi cần nhớ.
  let target = lists.length === 1 ? lists[0] : null;

  document.querySelectorAll('[data-image-list-add]').forEach(function (btn) {
    btn.addEventListener('click', function () {
      target = document.querySelector('[data-image-list="' + btn.getAttribute('data-image-list-add') + '"]') || target;
    });
  });

  document.addEventListener('picker:append', function (e) {
    if (target) add(target, e.detail.url);
  });

  lists.forEach(function (list) {
    list.addEventListener('click', function (e) {
      const item = e.target.closest('[data-image-item]');
      if (!item) return;
      if (e.target.hasAttribute('data-remove')) {
        item.remove();
      } else if (e.target.hasAttribute('data-move')) {
        const dir = parseInt(e.target.getAttribute('data-move'), 10);
        if (dir < 0 && item.previousElementSibling) {
          item.parentNode.insertBefore(item, item.previousElementSibling);
        } else if (dir > 0 && item.nextElementSibling) {
          item.parentNode.insertBefore(item.nextElementSibling, item);
        }
      }
    });
  });
})();
