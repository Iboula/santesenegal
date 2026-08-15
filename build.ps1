#!/usr/bin/env pwsh
# Script de build et migration pour SanteSenegal (Windows PowerShell)

$ErrorActionPreference = "Stop"
$ProjectRoot = Split-Path -Parent $MyInvocation.MyCommand.Definition

function Write-Header($text) {
    Write-Host "`n========================================" -ForegroundColor Cyan
    Write-Host $text -ForegroundColor Cyan
    Write-Host "========================================`n" -ForegroundColor Cyan
}

function Write-Success($text) {
    Write-Host "  OK: $text" -ForegroundColor Green
}

function Write-Error($text) {
    Write-Host "  ERREUR: $text" -ForegroundColor Red
}

Set-Location $ProjectRoot

Write-Header "1/6 - Restauration des packages NuGet"
dotnet restore
if ($LASTEXITCODE -eq 0) { Write-Success "Packages restaurés" } else { Write-Error "Échec de la restauration"; exit 1 }

Write-Header "2/6 - Build de la solution"
dotnet build --no-restore
if ($LASTEXITCODE -eq 0) { Write-Success "Build réussi" } else { Write-Error "Échec du build"; exit 1 }

Write-Header "3/6 - Génération de la migration EF Core"
Set-Location "$ProjectRoot\SanteSenegal.Infrastructure"
$MigrationExists = Test-Path "$ProjectRoot\SanteSenegal.Infrastructure\Persistence\Migrations"
if (-not $MigrationExists) {
    dotnet ef migrations add InitialCreate --startup-project "$ProjectRoot\SanteSenegal.Api" --output-dir Persistence/Migrations
    if ($LASTEXITCODE -eq 0) { Write-Success "Migration créée" } else { Write-Error "Échec de la création de la migration"; exit 1 }
} else {
    Write-Host "  Migration déjà existante, passage..." -ForegroundColor Yellow
}

Write-Header "4/6 - Mise à jour de la base de données"
dotnet ef database update --startup-project "$ProjectRoot\SanteSenegal.Api"
if ($LASTEXITCODE -eq 0) { Write-Success "Base de données à jour" } else { Write-Error "Échec de la mise à jour de la BDD"; exit 1 }

Write-Header "5/6 - Exécution des tests unitaires"
Set-Location "$ProjectRoot\SanteSenegal.Tests"
dotnet test --no-build --verbosity normal
if ($LASTEXITCODE -eq 0) { Write-Success "Tous les tests passent" } else { Write-Host "  Attention: certains tests ont échoué" -ForegroundColor Yellow }

Write-Header "6/6 - Démarrage de l'API"
Set-Location "$ProjectRoot\SanteSenegal.Api"
Write-Host "  L'API démarre sur https://localhost:7001" -ForegroundColor Cyan
Write-Host "  Swagger: https://localhost:7001/swagger" -ForegroundColor Cyan
Write-Host "  Appuyez sur Ctrl+C pour arrêter`n" -ForegroundColor Gray
dotnet run --no-build
