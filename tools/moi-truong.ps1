# =====================================================================
#  DO MOI TRUONG — chay TRUOC dong code dau tien
#  Rut tu 5/7 du an tung tac vi moi truong. Tat ca deu phat hien duoc
#  trong 10 phut dau bang script nay.
#  Chay:  powershell -ExecutionPolicy Bypass -File tools\moi-truong.ps1
# =====================================================================

$loi = @()

Write-Host "`n=== O DIA ===" -ForegroundColor Cyan
$c = Get-PSDrive C
$freeGB = [math]::Round($c.Free / 1GB, 2)
Write-Host "  O C con trong: $freeGB GB"
if ($freeGB -lt 20) {
    Write-Host "  !! CANH BAO: duoi 20GB. Da tung chan cung ca du an 2 lan." -ForegroundColor Red
    $loi += "O dia duoi 20GB"
} else {
    Write-Host "  OK" -ForegroundColor Green
}

Write-Host "`n=== .NET SDK ===" -ForegroundColor Cyan
dotnet --list-sdks
Write-Host "`n--- ASP.NET Core runtime ---"
dotnet --list-runtimes | Select-String "Microsoft.AspNetCore.App"
Write-Host "  Neu thieu dung ban target -> dat DOTNET_ROLL_FORWARD=LatestMajor"
Write-Host "  va tao run-demo.bat cho may demo cua khach."

Write-Host "`n=== POWERSHELL ===" -ForegroundColor Cyan
$psv = $PSVersionTable.PSVersion
Write-Host "  Phien ban: $psv"
if ($psv.Major -le 5) {
    Write-Host "  !! PowerShell 5.1 — hai bay bat buoc nho:" -ForegroundColor Yellow
    Write-Host "     1. git commit -F <file>   (KHONG dung -m voi chuoi nhieu dong,"
    Write-Host "        moi dong se thanh mot pathspec)"
    Write-Host "     2. Tham so nhan mang phai khai [string[]], KHONG [string]"
    Write-Host "        (mang 1 phan tu bung thanh chuoi, `$x[0] tra ve KY TU dau tien)"
}

Write-Host "`n=== SQL LOCALDB ===" -ForegroundColor Cyan
try {
    sqllocaldb info
    Write-Host "  Neu DB khong len, exit 82 'file already exists':" -ForegroundColor Yellow
    Write-Host "    -> File .mdf mo coi. CREATE DATABASE [X] ON (FILENAME='...mdf'),"
    Write-Host "       (FILENAME='...ldf') FOR ATTACH"
    Write-Host "    -> TUYET DOI khong xoa file .mdf (da tung cuu 75MB du lieu that)"
} catch {
    Write-Host "  Khong tim thay sqllocaldb (co the du an nay khong dung LocalDB)" -ForegroundColor Yellow
}

Write-Host "`n=== CONG DANG BAN ===" -ForegroundColor Cyan
$ports = Get-NetTCPConnection -State Listen -ErrorAction SilentlyContinue |
         Where-Object LocalPort -in 5000,5001,5170,7000,7001 |
         Select-Object LocalPort, OwningProcess
if ($ports) {
    $ports | Format-Table -AutoSize
    Write-Host "  !! Co cong dang ban — kiem tra co du an khac dang chay khong" -ForegroundColor Yellow
} else {
    Write-Host "  Khong cong nao ban. OK" -ForegroundColor Green
}

Write-Host "`n=== TIEN TRINH DOTNET/NODE TON DONG ===" -ForegroundColor Cyan
$procs = Get-Process dotnet,node -ErrorAction SilentlyContinue
if ($procs) {
    $procs | Select-Object Name, Id, @{n='RAM_MB';e={[math]::Round($_.WS/1MB)}} | Format-Table -AutoSize
    Write-Host "  Tien trinh rac tich luy lam moi truong cham dan theo phien." -ForegroundColor Yellow
    Write-Host "  Don bang: Get-Process dotnet,node | Stop-Process -Force"
} else {
    Write-Host "  Sach. OK" -ForegroundColor Green
}

Write-Host "`n=== DONG HO HE THONG ===" -ForegroundColor Cyan
Write-Host "  $(Get-Date)"
Write-Host "  (Da tung co du an commit lech 3 tuan vi dong ho sai)"

Write-Host "`n=====================================================" -ForegroundColor Cyan
if ($loi.Count -gt 0) {
    Write-Host "CAN XU LY TRUOC KHI BAT DAU:" -ForegroundColor Red
    $loi | ForEach-Object { Write-Host "  - $_" -ForegroundColor Red }
} else {
    Write-Host "MOI TRUONG OK — bat dau duoc." -ForegroundColor Green
}

Write-Host "`n>> SAU KHI BUILD LAN DAU, kiem them 1 viec (bug nay dinh 2 du an):" -ForegroundColor Cyan
Write-Host '   curl -s -H "Accept-Encoding: gzip" http://localhost:5000/css/site.css --output nul -w "%{size_download}\n"'
Write-Host "   Tra ve 0  ->  chay strip-compressed-assets.js sau MOI lan build."
Write-Host "   Trieu chung khi dinh: trang mat sach style, nhung curl thuong van binh thuong."
Write-Host ""
