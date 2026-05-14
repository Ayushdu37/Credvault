# ============================================================
#  stop-all.ps1  --  Stop the full CredVault microservice stack
#  PS> .\stop-all.ps1
# ============================================================

$rootDir = $PSScriptRoot
$composeFileName = "docker-compose.infra.yml"

function Write-Header($text) {
    Write-Host ""
    Write-Host "  $text" -ForegroundColor Cyan
    Write-Host ("  " + ("-" * $text.Length)) -ForegroundColor DarkCyan
}

Clear-Host
Write-Host ""
Write-Host "  +================================================+" -ForegroundColor DarkCyan
Write-Host "  |   CredVault Microservice Stack -- Stopping...   |" -ForegroundColor Cyan
Write-Host "  +================================================+" -ForegroundColor DarkCyan


# --- Stop .NET processes ------------------------------------
Write-Header "Killing dotnet run processes"
$dotnetProcs = Get-Process -Name "dotnet" -ErrorAction SilentlyContinue
if ($dotnetProcs) {
    $dotnetProcs | Stop-Process -Force
    Write-Host "  dotnet processes stopped." -ForegroundColor Green
} else {
    Write-Host "  No dotnet processes found." -ForegroundColor Gray
}


# --- Stop Node (Angular) -----------------------------------
Write-Header "Killing Angular / Node processes"
$nodeProcs = Get-Process -Name "node" -ErrorAction SilentlyContinue
if ($nodeProcs) {
    $nodeProcs | Stop-Process -Force
    Write-Host "  Node processes stopped." -ForegroundColor Green
} else {
    Write-Host "  No Node processes found." -ForegroundColor Gray
}


# --- Stop Docker containers --------------------------------
Write-Header "Stopping Docker containers (SQL Server, RabbitMQ, Redis)"
$composeFile = Join-Path $rootDir $composeFileName
if (Test-Path $composeFile) {
    docker compose -f $composeFile down
    if ($LASTEXITCODE -eq 0) {
        Write-Host "  Docker containers stopped." -ForegroundColor Green
    } else {
        Write-Host "  docker-compose down failed." -ForegroundColor Red
    }
} else {
    Write-Host "  $composeFileName not found -- skipping." -ForegroundColor Yellow
}


Write-Host ""
Write-Host "  [OK] All CredVault services stopped." -ForegroundColor Green
Write-Host ""
