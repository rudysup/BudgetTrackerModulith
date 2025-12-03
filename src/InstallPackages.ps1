# Corrected InstallPackages.ps1 for BudgetTracker Modulith

# Run this script from the src folder:

# C:\ChatGptProjects\BudgetTrackerModulith\BudgetTrackerModulith\src

# --- CONFIG ---

$backendModules = @(
".\BudgetTracker.Modules.Auth",
".\BudgetTracker.Modules.Expenses",
".\BudgetTracker.Modules.Budget"
)
$frontendPath = ".\BudgetTracker.Frontend"   # Only if you have Angular frontend here
$mauiPath = ".\BudgetTracker.Maui"           # Only if you have MAUI project here
$startupProject = ".\BudgetTracker.Host\BudgetTracker.Host.csproj"
$dockerComposeFile = ".\docker-compose.yml"

# --- CLEANUP ---

Write-Host "Cleaning previous builds..." -ForegroundColor Cyan

foreach ($module in $backendModules) {
if (Test-Path "$module\bin") { Remove-Item "$module\bin" -Recurse -Force }
if (Test-Path "$module\obj") { Remove-Item "$module\obj" -Recurse -Force }
}

if (Test-Path "$frontendPath\dist") { Remove-Item "$frontendPath\dist" -Recurse -Force }
if (Test-Path "$frontendPath\node_modules") { Remove-Item "$frontendPath\node_modules" -Recurse -Force }

if (Test-Path "$mauiPath\bin") { Remove-Item "$mauiPath\bin" -Recurse -Force }
if (Test-Path "$mauiPath\obj") { Remove-Item "$mauiPath\obj" -Recurse -Force }

Write-Host "Removing old Docker containers and images..." -ForegroundColor Cyan
if (Test-Path $dockerComposeFile) {
docker compose -f $dockerComposeFile down --rmi all --volumes --remove-orphans
} else {
Write-Host "Docker compose file not found, skipping..." -ForegroundColor DarkGray
}

# --- DOTNET TOOL CHECK ---

if (-not (Get-Command "dotnet-ef" -ErrorAction SilentlyContinue)) {
Write-Host "dotnet-ef not found. Installing globally..." -ForegroundColor Yellow
dotnet tool install --global dotnet-ef
}

# --- EF.DESIGN PACKAGE CHECK ---

$efPackageInstalled = dotnet list $startupProject package | Select-String "Microsoft.EntityFrameworkCore.Design"
if (-not $efPackageInstalled) {
Write-Host "Installing Microsoft.EntityFrameworkCore.Design 8.x in startup project..." -ForegroundColor Yellow
dotnet add $startupProject package Microsoft.EntityFrameworkCore.Design --version 8.*
} else {
Write-Host "Microsoft.EntityFrameworkCore.Design already installed, skipping..." -ForegroundColor DarkGray
}

# --- BACKEND RESTORE & BUILD ---

Write-Host "Restoring and building backend modules..." -ForegroundColor Green
foreach ($module in $backendModules) {
dotnet restore $module
dotnet build $module -c Release --no-restore
}

# --- FRONTEND INSTALL & BUILD ---

if (Test-Path $frontendPath) {
Write-Host "Installing frontend dependencies and building Angular frontend..." -ForegroundColor Green
Push-Location $frontendPath
npm install
npm run build -- --prod
Pop-Location
} else {
Write-Host "Frontend folder not found, skipping..." -ForegroundColor DarkGray
}

# --- MAUI BUILD ---

if (Test-Path $mauiPath) {
Write-Host "Building MAUI app..." -ForegroundColor Green
dotnet restore $mauiPath
dotnet build $mauiPath -c Release --no-restore
} else {
Write-Host "MAUI folder not found, skipping..." -ForegroundColor DarkGray
}

# --- DOCKER BUILD & RUN ---

if (Test-Path $dockerComposeFile) {
Write-Host "Building and running Docker containers..." -ForegroundColor Green
docker compose -f $dockerComposeFile build --no-cache
docker compose -f $dockerComposeFile up -d
} else {
Write-Host "Docker compose file not found, skipping Docker step..." -ForegroundColor DarkGray
}

Write-Host "✅ InstallPackages.ps1 completed successfully!" -ForegroundColor Magenta
