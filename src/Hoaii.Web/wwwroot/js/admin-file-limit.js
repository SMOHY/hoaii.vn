// Chặn tệp quá cỡ NGAY tại trình duyệt, trước khi tải lên.
//
// Máy chủ vẫn có chốt chặn riêng, nhưng chốt đó chỉ trả lời được sau khi trình duyệt đã gửi xong
// toàn bộ tệp — với một video 70MB thì người dùng ngồi chờ rất lâu rồi mới biết là hỏng, và nếu
// máy chủ trả lời sớm giữa chừng thì trình duyệt lại báo đứt kết nối chứ không hiện thông báo.
// Kiểm ở đây cho người dùng biết ngay lập tức.
//
// Con số lấy từ thuộc tính data-max-bytes do máy chủ in ra, nên chỉ có đúng một nguồn: hằng số
// trong MediaService.
(function () {
  'use strict';

  document.querySelectorAll('input[type=file][data-max-bytes]').forEach(function (o) {
    const tran = parseInt(o.getAttribute('data-max-bytes'), 10);
    const nhan = o.getAttribute('data-max-label') || (Math.round(tran / 1024 / 1024) + 'MB');
    if (!tran) return;

    o.addEventListener('change', function () {
      const qua = [...o.files].filter(f => f.size > tran);
      if (!qua.length) return;

      const ten = qua.map(f => f.name + ' (' + (f.size / 1024 / 1024).toFixed(1) + 'MB)').join(', ');
      const loi = 'Tệp vượt quá ' + nhan + ': ' + ten;
      if (window.hoaiiToast) window.hoaiiToast(loi, 'error');
      else window.alert(loi);
      o.value = '';
    });
  });
})();
