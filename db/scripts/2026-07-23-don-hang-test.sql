/*
    Dọn 67 đơn hàng do bộ kiểm thử sinh ra.

    Bối cảnh: chạy các bộ test (storefront / admin / e2e / user-journey) lần nào cũng đặt đơn
    thật vào DB. Đến 23/07/2026 có 67 đơn, và **không đơn nào là đơn thật** — toàn bộ email đều
    thuộc miền test: test-auto@example.com, test@hoaii.test, auto-flow@example.com,
    auto-skip@example.com, journey@example.com. Đã kiểm:

        SELECT COUNT(*) FROM Orders
        WHERE Email NOT LIKE '%example.com' AND Email NOT LIKE '%hoaii.test';   -- = 0

    Script này KHÔNG tự chạy. Chọn một trong hai phương án rồi chạy phần đó.

    ---------------------------------------------------------------------------------------
    PHƯƠNG ÁN A — Trải trạng thái cho đẹp khi demo (khuyên dùng lúc bàn giao)

    Giữ nguyên đơn, chỉ rải trạng thái để mỗi tab trong `/tai-khoan/don-hang` và mỗi bộ lọc
    trong `/admin/don-hang` đều có nội dung. Hiện 49 đơn ở Chờ xác nhận, 7 ở Chờ lấy hàng,
    11 ở Đã giao — ba tab Đang giao / Trả hàng / Đã huỷ trống trơn, nhìn như tính năng hỏng.

    Không xoá gì, chạy lại được nhiều lần.
    ---------------------------------------------------------------------------------------
*/

-- === PHƯƠNG ÁN A ===
-- Bỏ dấu chú thích khối dưới đây để chạy.
/*
BEGIN TRANSACTION;

;WITH DanhSo AS (
    SELECT Id, ROW_NUMBER() OVER (ORDER BY CreatedAt DESC) AS Hang
    FROM Orders
    WHERE Status = 0            -- chỉ động vào đơn đang ở Chờ xác nhận
)
UPDATE o
SET o.Status = CASE d.Hang % 6
                   WHEN 0 THEN 2   -- Shipping   — Đang giao
                   WHEN 1 THEN 4   -- Returned   — Trả hàng
                   WHEN 2 THEN 5   -- Cancelled  — Đã huỷ
                   ELSE o.Status   -- ba phần còn lại giữ nguyên Chờ xác nhận
               END,
    o.UpdatedAt = SYSUTCDATETIME()
FROM Orders o
JOIN DanhSo d ON d.Id = o.Id;

SELECT Status, COUNT(*) AS So FROM Orders GROUP BY Status ORDER BY Status;

COMMIT;
*/


/*
    ---------------------------------------------------------------------------------------
    PHƯƠNG ÁN B — Xoá sạch đơn test, bắt đầu từ số 0

    Dùng khi khách muốn nhận một hệ thống trắng, tự đặt đơn đầu tiên của mình.
    KHÔNG HOÀN TÁC ĐƯỢC. Sao lưu DB trước khi chạy.
    ---------------------------------------------------------------------------------------
*/

-- === PHƯƠNG ÁN B ===
-- Bỏ dấu chú thích khối dưới đây để chạy.
/*
BEGIN TRANSACTION;

DECLARE @DonTest TABLE (Id INT PRIMARY KEY);
INSERT INTO @DonTest (Id)
SELECT Id FROM Orders
WHERE Email LIKE '%@example.com' OR Email LIKE '%@hoaii.test';

-- Chốt an toàn: nếu vô tình khớp cả đơn thật thì dừng lại, không xoá gì.
IF EXISTS (SELECT 1 FROM Orders
           WHERE Id IN (SELECT Id FROM @DonTest)
             AND Email NOT LIKE '%@example.com' AND Email NOT LIKE '%@hoaii.test')
BEGIN
    ROLLBACK;
    THROW 50001, N'Có đơn không phải đơn test lọt vào danh sách — đã huỷ, không xoá gì.', 1;
END

DELETE FROM OrderStatusHistories WHERE OrderId IN (SELECT Id FROM @DonTest);
DELETE FROM OrderItems           WHERE OrderId IN (SELECT Id FROM @DonTest);
DELETE FROM Orders               WHERE Id      IN (SELECT Id FROM @DonTest);

SELECT COUNT(*) AS DonConLai FROM Orders;

COMMIT;
*/
