# Script chạy migration cho hoaii.vn
Set-Location $PSScriptRoot

# Thiết lập biến môi trường
$env:DOTNET_ROLL_FORWARD = 'LatestMajor'
$env:DOTNET_ROLL_FORWARD_TO_PRERELEASE = '1'
$env:ConnectionStrings__DefaultConnection = 'Server=(localdb)\MSSQLLocalDB;Database=HoaiiDb;Trusted_Connection=True;MultipleActiveResultSets=true'

# Chạy migration
Write-Host "=== RUNNING MIGRATIONS ==="
dotnet ef database update --project "src\Hoaii.Infrastructure\Hoaii.Infrastructure.csproj" --startup-project "src\Hoaii.Web\Hoaii.Web.csproj"