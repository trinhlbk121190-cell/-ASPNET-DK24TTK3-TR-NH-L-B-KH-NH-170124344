# Script ki?m tra môi tru?ng tru?c khi cài d?t MedForum
Write-Host "=== MedForum Environment Check ===" -ForegroundColor Cyan

# Ki?m tra .NET
try {
    $dotnet = dotnet --version
    Write-Host ".NET SDK: $dotnet" -ForegroundColor Green
} catch { Write-Host ".NET SDK: NOT FOUND" -ForegroundColor Red }

# Ki?m tra SQL Server
try {
    $sql = Invoke-Sqlcmd -Query "SELECT @@VERSION" -ErrorAction Stop
    Write-Host "SQL Server: OK" -ForegroundColor Green
} catch { Write-Host "SQL Server: NOT FOUND or not accessible" -ForegroundColor Yellow }

Write-Host "Check complete." -ForegroundColor Cyan
