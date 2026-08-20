# =====================================================================
#  CHAY APP — 1 lenh duy nhat
#  Vi sao phai script hoa: cac buoc moi truong co dinh (roll-forward,
#  stop process truoc build, strip-assets sau build, --no-build) tung bi
#  QUEN LAP LAI RAT NHIEU LAN trong cung mot phien du da co ghi chu.
#  Root cause khong phai tri nho — la chua gop thanh mot script.
#
#  Sua bien $Duan ben duoi cho dung du an, roi:  tools\chay.ps1
# =====================================================================

$Duan = "src\<Ten>.Web"          # <<< SUA DUONG DAN NAY
$StripAssets = $true             # dat $false neu khong dung .NET preview

# --- 1. Runtime roll-forward (may co .NET 9 + 11-preview, thieu ban giua) ---
$env:DOTNET_ROLL_FORWARD = "LatestMajor"

# --- 2. Go build lock: kill tien trinh dotnet dang giu file ---
Write-Host "[1/4] Dung tien trinh dotnet cu..." -ForegroundColor Cyan
Get-Process dotnet -ErrorAction SilentlyContinue | Stop-Process -Force
Start-Sleep -Milliseconds 500

# --- 3. Build ---
Write-Host "[2/4] Build..." -ForegroundColor Cyan
dotnet build -c Debug
if ($LASTEXITCODE -ne 0) {
    Write-Host "BUILD THAT BAI — dung lai." -ForegroundColor Red
    exit 1
}

# --- 4. Strip manifest nen hong (.NET preview + MapStaticAssets) ---
#     Khong lam buoc nay: CSS/JS tra RONG khi trinh duyet xin gzip
#     -> trang mat sach style, nhung curl thuong van binh thuong.
if ($StripAssets -and (Test-Path "tools\strip-compressed-assets.js")) {
    Write-Host "[3/4] Strip compressed assets..." -ForegroundColor Cyan
    node tools\strip-compressed-assets.js
} else {
    Write-Host "[3/4] Bo qua strip-assets" -ForegroundColor DarkGray
}

# --- 5. Run ---
#     LUON tach build va run --no-build. Server chay dai phai chay NGOAI
#     sandbox — neu khong, app khoi dong thanh cong roi chet ngay exit 255
#     (sandbox giet ca cay tien trinh khi lenh cha ket thuc).
Write-Host "[4/4] Chay app..." -ForegroundColor Cyan
dotnet run --no-build --project $Duan
