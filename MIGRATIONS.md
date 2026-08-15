# 🗄️ Guide Migrations EF Core + Build

## Prérequis

- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0) installé
- [PostgreSQL](https://www.postgresql.org/download/) en local ou accessible
- Outil EF Core CLI : `dotnet tool install --global dotnet-ef` (si pas déjà installé)

---

## 🚀 Étapes rapides

### 1. Restaurer les packages NuGet

```bash
cd C:\Users\iboul\Documents\PROJETS\SANTESENEGAL\SanteSenegal
dotnet restore
```

### 2. Générer la migration initiale

```bash
cd SanteSenegal.Infrastructure
dotnet ef migrations add InitialCreate --startup-project ../SanteSenegal.Api --output-dir Persistence/Migrations
```

### 3. Mettre à jour la base de données

```bash
dotnet ef database update --startup-project ../SanteSenegal.Api
```

> ⚠️ **Vérifiez votre `appsettings.Development.json`** : assurez-vous que la `ConnectionStrings:DefaultConnection` pointe vers votre instance PostgreSQL.

---

## 🧪 Build et Tests

### Build complet de la solution

```bash
cd ..
dotnet build
```

### Exécuter les tests unitaires

```bash
cd SanteSenegal.Tests
dotnet test
```

### Exécuter l'API en mode développement

```bash
cd SanteSenegal.Api
dotnet run
```

L'API sera disponible sur :
- `https://localhost:7001` (HTTPS)
- `http://localhost:5001` (HTTP)
- Swagger UI : `https://localhost:7001/swagger`

---

## 📋 Commandes utiles

| Commande | Description |
|----------|-------------|
| `dotnet ef migrations add <Nom>` | Créer une nouvelle migration |
| `dotnet ef database update` | Appliquer toutes les migrations |
| `dotnet ef database update <NomMigration>` | Migrer jusqu'à une migration spécifique |
| `dotnet ef migrations list` | Lister toutes les migrations |
| `dotnet ef database drop --force` | Supprimer la base de données |
| `dotnet test --verbosity normal` | Tests avec détails |

---

## 🔑 Comptes par défaut (seed)

Après le premier démarrage en mode `Development`, le seeder crée automatiquement :

| Email | Mot de passe | Rôle |
|-------|-------------|------|
| `admin@santesenegal.sn` | `Admin@2026` | Admin |
| `dr.ndiaye@hpd.sn` | `Medecin@2026` | Médecin |
| `dr.fall@fann.sn` | `Medecin@2026` | Médecin |
| `agent@thiessante.sn` | `Agent@2026` | AgentSante |

---

## 🐛 Dépannage

### Erreur "dotnet-ef n'est pas reconnu"

```bash
dotnet tool install --global dotnet-ef
dotnet tool update --global dotnet-ef
```

### Erreur de connexion PostgreSQL

Vérifiez que PostgreSQL est démarré et que la connection string dans `appsettings.Development.json` est correcte :

```json
"ConnectionStrings": {
  "DefaultConnection": "Host=localhost;Database=SanteSenegal;Username=postgres;Password=VOTRE_MOT_DE_PASSE"
}
```

### Erreur "The entity type requires a primary key"

Si vous ajoutez une nouvelle entité, assurez-vous qu'elle hérite de `BaseEntity` ou qu'elle a une propriété `Id` marquée comme clé primaire.

---

## 📝 Prochaines migrations

Si vous modifiez les entités, créez une nouvelle migration :

```bash
cd SanteSenegal.Infrastructure
dotnet ef migrations add <NomDescriptif> --startup-project ../SanteSenegal.Api
```

Exemples de noms de migration :
- `AddUserProfilePicture`
- `UpdateRendezVousIndexes`
- `AddNotificationRetryCount`
