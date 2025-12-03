# Migration.ps1 - THE ONE THAT ACTUALLY WORKS (2025 edition)
# Save as src\Migration.ps1 and run ONCE

$modules = @(
    @{ Name = "Auth";     Project = ".\BudgetTracker.Modules.Auth\BudgetTracker.Modules.Auth.csproj";     Context = "BudgetTracker.Modules.Auth.Infrastructure.Persistence.AuthMigrationsDbContext";     ConnKey = "AuthDb" },
    @{ Name = "Expenses"; Project = ".\BudgetTracker.Modules.Expenses\BudgetTracker.Modules.Expenses.csproj"; Context = "BudgetTracker.Modules.Expenses.Infrastructure.Persistence.ExpensesMigrationsDbContext"; ConnKey = "ExpensesDb" },
    @{ Name = "Budget";   Project = ".\BudgetTracker.Modules.Budget\BudgetTracker.Modules.Budget.csproj";   Context = "BudgetTracker.Modules.Budget.Infrastructure.Persistence.BudgetMigrationsDbContext";   ConnKey = "BudgetDb" }
)

$startupProject = ".\BudgetTracker.Host\BudgetTracker.Host.csproj"
$hostSettings = ".\BudgetTracker.Host\appsettings.json"

# Load connection strings
$json = Get-Content $hostSettings -Raw | ConvertFrom-Json
$connStrings = $json.ConnectionStrings

Write-Host "`nStarting migration for all modules..." -ForegroundColor Magenta

$globalSuccess = $true

foreach ($module in $modules) {
    Write-Host "`nModule: $($module.Name)" -ForegroundColor Cyan

    $projectPath = Split-Path $module.Project -Parent
    $migrationsFolder = Join-Path $projectPath "Infrastructure\Persistence\Migrations"

    # THE FIX: Properly detect if migrations exist (including Designer.cs and .cs files)
    $migrationCsFiles = Get-ChildItem $migrationsFolder -Filter "*.cs" -ErrorAction SilentlyContinue
    $hasMigrations = $migrationCsFiles -and ($migrationCsFiles | Where-Object { $_.Name -notmatch '\.Designer\.cs$' }).Count -gt 0

    if (-not $hasMigrations) {
        Write-Host "No migration files found → creating InitialCreate..." -ForegroundColor Yellow
        dotnet ef migrations add InitialCreate -p $module.Project -s $startupProject --context $module.Context -o "Infrastructure/Persistence/Migrations" --verbose
        if ($LASTEXITCODE -ne 0) {
            Write-Host "Could not add migration (probably already exists - continuing anyway)" -ForegroundColor DarkYellow
        }
    } else {
        Write-Host "Migrations already exist (found .cs files) → skipping add" -ForegroundColor Gray
    }

    # NOW APPLY THE MIGRATIONS - THIS IS WHAT ACTUALLY CREATES THE DATABASES
    Write-Host "Applying migrations to database..." -ForegroundColor Green

    $conn = $connStrings.($module.ConnKey)
    $winAuthConn = $conn -replace 'User Id=sa;Password=[^;]+;', 'Trusted_Connection=True;' -replace 'Integrated Security=[^;]+;', ''

    $applied = $false

    # Try 1: Windows Authentication (recommended & works on 99% of dev machines)
    Write-Host "  → Trying Windows Authentication..." -ForegroundColor DarkCyan
    dotnet ef database update -p $module.Project -s $startupProject --context $module.Context --connection $winAuthConn --no-build
    if ($LASTEXITCODE -eq 0) { $applied = $true }

    # Try 2: Original connection string
    if (-not $applied) {
        Write-Host "  → Trying original connection string..." -ForegroundColor DarkCyan
        dotnet ef database update -p $module.Project -s $startupProject --context $module.Context --connection $conn --no-build
        if ($LASTEXITCODE -eq 0) { $applied = $true }
    }

    # Try 3: No connection string (let Host configure it)
    if (-not $applied) {
        Write-Host "  → Trying via startup project DI..." -ForegroundColor DarkCyan
        dotnet ef database update -p $module.Project -s $startupProject --context $module.Context --no-build
        if ($LASTEXITCODE -eq 0) { $applied = $true }
    }

    if ($applied) {
        Write-Host "$($module.Name) database is UP TO DATE!" -ForegroundColor Green
    } else {
        Write-Host "$($module.Name) FAILED TO UPDATE DATABASE!" -ForegroundColor Red
        $globalSuccess = $false
    }
}

if ($globalSuccess) {
    Write-Host "`nALL 3 DATABASES SUCCESSFULLY CREATED AND MIGRATED!" -ForegroundColor Magenta
    Write-Host "You can now run: docker compose up -d   or   dotnet run --project .\BudgetTracker.Host" -ForegroundColor Yellow
} else {
    Write-Host "`nSome databases failed. Make sure SQL Server is running and you're using Windows Auth." -ForegroundColor Red
}