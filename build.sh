#!/bin/bash
# Script de build et migration pour SanteSenegal (Linux/Mac)

set -e

PROJECT_ROOT="$(cd "$(dirname "$0")" && pwd)"

header() {
    echo ""
    echo "========================================"
    echo "$1"
    echo "========================================"
    echo ""
}

success() {
    echo "  OK: $1"
}

error() {
    echo "  ERREUR: $1"
    exit 1
}

cd "$PROJECT_ROOT"

header "1/6 - Restauration des packages NuGet"
dotnet restore || error "Échec de la restauration"
success "Packages restaurés"

header "2/6 - Build de la solution"
dotnet build --no-restore || error "Échec du build"
success "Build réussi"

header "3/6 - Génération de la migration EF Core"
cd "$PROJECT_ROOT/SanteSenegal.Infrastructure"
if [ ! -d "$PROJECT_ROOT/SanteSenegal.Infrastructure/Persistence/Migrations" ]; then
    dotnet ef migrations add InitialCreate --startup-project "$PROJECT_ROOT/SanteSenegal.Api" --output-dir Persistence/Migrations || error "Échec de la création de la migration"
    success "Migration créée"
else
    echo "  Migration déjà existante, passage..."
fi

header "4/6 - Mise à jour de la base de données"
dotnet ef database update --startup-project "$PROJECT_ROOT/SanteSenegal.Api" || error "Échec de la mise à jour de la BDD"
success "Base de données à jour"

header "5/6 - Exécution des tests unitaires"
cd "$PROJECT_ROOT/SanteSenegal.Tests"
dotnet test --no-build --verbosity normal || echo "  Attention: certains tests ont échoué"

header "6/6 - Démarrage de l'API"
cd "$PROJECT_ROOT/SanteSenegal.Api"
echo "  L'API démarre sur https://localhost:7001"
echo "  Swagger: https://localhost:7001/swagger"
echo "  Appuyez sur Ctrl+C pour arrêter"
echo ""
dotnet run --no-build
