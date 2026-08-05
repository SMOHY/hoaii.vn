# Script khởi động hoaii.vn
# Chuyển vào thư mục chứa script
Set-Location $PSScriptRoot

# Dừng process cũ nếu có
Get-Process Hoaii.Web -ErrorAction SilentlyContinue | Stop-Process -Force

# Build dự án
Write-Host "=== BUILD ==="
dotnet build "src\Hoaii.Web\Hoaii.Web.csproj"

# Chạy script strip-compressed-assets (bắt buộc sau mỗi build)
Write-Host "=== STRIP COMPRESSED ASSETS ==="
node "tools\strip-compressed-assets.js"

# Thiết lập biến môi trường
$env:DOTNET_ROLL_FORWARD = 'LatestMajor'
$env:DOTNET_ROLL_FORWARD_TO_PRERELEASE = '1'
$env:ASPNETCORE_URLS = 'http://localhost:5167'
$env:ConnectionStrings__DefaultConnection = 'Server=(localdb)\MSSQLLocalDB;Database=HoaiiDb;Trusted_Connection=True;MultipleActiveResultSets=true'
$env:ASPNETCORE_CONTENTROOT = Join-Path $PSScriptRoot 'src\Hoaii.Web'
$env:ASPNETCORE_WEBROOT = Join-Path $PSScriptRoot 'src\Hoaii.Web\wwwroot'

# Chạy ứng dụng
Write-Host "=== STARTING APP ==="
Write-Host "Web khach: http://localhost:5167"
Write-Host "Admin: http://localhost:5167/admin"
& ".\src\Hoaii.Web\bin\Debug\net10.0\Hoaii.Web.exe"