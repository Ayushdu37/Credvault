# ============================================================
#  start-all.ps1  --  Start the full CredVault microservice stack
#  Place this file in your ROOT project folder and run:
#  PS> .\start-all.ps1
# ============================================================

# --- CONFIGURATION ------------------------------------------
$rootDir = $PSScriptRoot   # Folder where this script lives

$services = @(
    @{ Name = "Identity Service";     Path = "src\services\identity-service\IdentityService.API" },
    @{ Name = "Card Service";         Path = "src\services\card-service\CardService.API" },
    @{ Name = "Billing Service";      Path = "src\services\billing-services\BillingService.API" },
    @{ Name = "Payment Service";      Path = "src\services\payment-service\PaymentService.API" },
    @{ Name = "Notification Service"; Path = "src\services\notification-service\NotificationService.API" },
    @{ Name = "Ocelot API Gateway";   Path = "src\services\ocelot-gateway\OcelotGateway.API" }
)

$frontendPath = "src\angular-spa"   # Relative path to Angular app
$composeFileName = "docker-compose.infra.yml"
# ------------------------------------------------------------


# --- Helpers ------------------------------------------------
function Write-Header($text) {
    Write-Host ""
    Write-Host "  $text" -ForegroundColor Cyan
    Write-Host ("  " + ("-" * $text.Length)) -ForegroundColor DarkCyan
}

function Open-Terminal($title, $workDir, $command) {
    $fullPath = Join-Path $rootDir $workDir
    if (-not (Test-Path $fullPath)) {
        Write-Host "  [SKIP] Folder not found: $fullPath" -ForegroundColor Yellow
        return
    }
    Start-Process "wt.exe" `
        -ArgumentList "new-tab --title `"$title`" --startingDirectory `"$fullPath`" powershell.exe -NoExit -Command `"$command`"" `
        -ErrorAction SilentlyContinue

    # Fallback to plain PowerShell window if Windows Terminal (wt) isn't installed
    if ($LASTEXITCODE -ne 0) {
        Start-Process "powershell.exe" `
            -ArgumentList "-NoExit -Command `"Set-Location '$fullPath'; $command`"" `
            -WorkingDirectory $fullPath
    }
}
# ------------------------------------------------------------


Clear-Host
Write-Host ""
Write-Host "  +================================================+" -ForegroundColor DarkCyan
Write-Host "  |   CredVault Microservice Stack -- Starting...   |" -ForegroundColor Cyan
Write-Host "  +================================================+" -ForegroundColor DarkCyan


# --- Step 1: Docker infrastructure --------------------------
Write-Header "Step 1/3 -- Docker containers (SQL Server, RabbitMQ, Redis)"

# Auto-start Docker Desktop if the engine isn't running
$dockerRunning = docker info 2>&1
if ($LASTEXITCODE -ne 0) {
    Write-Host "  Docker Engine not running. Launching Docker Desktop..." -ForegroundColor Yellow
    Start-Process "C:\Program Files\Docker\Docker\Docker Desktop.exe" -ErrorAction SilentlyContinue

    $timeout = 60
    $elapsed = 0
    while ($elapsed -lt $timeout) {
        Start-Sleep -Seconds 3
        $elapsed += 3
        $check = docker info 2>&1
        if ($LASTEXITCODE -eq 0) {
            Write-Host "  Docker Engine is ready. (waited ${elapsed}s)" -ForegroundColor Green
            break
        }
        Write-Host "  Waiting for Docker Engine... (${elapsed}s / ${timeout}s)" -ForegroundColor DarkGray
    }
    if ($elapsed -ge $timeout) {
        Write-Host "  Docker Engine did not start within ${timeout}s. Please start Docker Desktop manually." -ForegroundColor Red
        exit 1
    }
} else {
    Write-Host "  Docker Engine already running." -ForegroundColor Green
}

$composeFile = Join-Path $rootDir $composeFileName
if (Test-Path $composeFile) {
    Write-Host "  Starting docker-compose..." -ForegroundColor Gray
    docker compose -f $composeFile up -d
    if ($LASTEXITCODE -eq 0) {
        Write-Host "  Docker containers started." -ForegroundColor Green
        Write-Host "    - SQL Server  : localhost:1434  (SA / CredVault@2025)" -ForegroundColor DarkGray
        Write-Host "    - RabbitMQ    : localhost:15672 (guest / guest)" -ForegroundColor DarkGray
        Write-Host "    - Redis       : localhost:6379" -ForegroundColor DarkGray
    } else {
        Write-Host "  docker-compose failed. Is Docker Desktop running?" -ForegroundColor Red
        exit 1
    }
} else {
    Write-Host "  $composeFileName not found -- skipping." -ForegroundColor Yellow
}


# --- Step 2: .NET microservices -----------------------------
Write-Header "Step 2/3 -- .NET microservices (dotnet run)"

Write-Host "  Waiting 15s for SQL Server / RabbitMQ / Redis to be ready..." -ForegroundColor Gray
Start-Sleep -Seconds 15

# Build the entire solution first (just like Visual Studio does)
Write-Host "  Building solution (credvault.slnx)..." -ForegroundColor Gray
Push-Location $rootDir
dotnet build credvault.slnx --configuration Debug 2>&1 | Out-Null
$buildExit = $LASTEXITCODE
Pop-Location

if ($buildExit -ne 0) {
    Write-Host "  Build failed! Running dotnet build with full output..." -ForegroundColor Red
    Push-Location $rootDir
    dotnet build credvault.slnx --configuration Debug
    Pop-Location
    exit 1
}
Write-Host "  Solution built successfully." -ForegroundColor Green

foreach ($svc in $services) {
    Write-Host "  Starting: $($svc.Name)..." -ForegroundColor Gray
    Open-Terminal $svc.Name $svc.Path "dotnet run --launch-profile http --no-build"
    Start-Sleep -Milliseconds 500   # small stagger to avoid port conflicts
}
Write-Host "  All services launched in separate tabs." -ForegroundColor Green
Write-Host "    - Identity Service     : http://localhost:5032" -ForegroundColor DarkGray
Write-Host "    - Card Service         : http://localhost:5033" -ForegroundColor DarkGray
Write-Host "    - Billing Service      : http://localhost:5034" -ForegroundColor DarkGray
Write-Host "    - Payment Service      : http://localhost:5035" -ForegroundColor DarkGray
Write-Host "    - Notification Service : http://localhost:5036" -ForegroundColor DarkGray
Write-Host "    - Ocelot API Gateway   : http://localhost:5100" -ForegroundColor DarkGray


# --- Step 3: Angular frontend ------------------------------
Write-Header "Step 3/3 -- Angular frontend (ng serve)"

$fullFrontendPath = Join-Path $rootDir $frontendPath
if (-not (Test-Path (Join-Path $fullFrontendPath "node_modules"))) {
    Write-Host "  Installing npm dependencies (first-time setup)..." -ForegroundColor Gray
    Push-Location $fullFrontendPath
    npm install
    Pop-Location
}

Write-Host "  Starting Angular dev server..." -ForegroundColor Gray
Open-Terminal "CredVault Angular" $frontendPath "npm start"
Write-Host "  Angular launched : http://localhost:4200" -ForegroundColor Green


# --- Done ---------------------------------------------------
Write-Host ""
Write-Host "  [OK] CredVault stack is up! Check each terminal tab for logs." -ForegroundColor Green
Write-Host "  Run .\stop-all.ps1 to shut everything down." -ForegroundColor DarkGray
Write-Host ""
