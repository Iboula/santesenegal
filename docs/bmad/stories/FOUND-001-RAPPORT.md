# Rapport — FOUND-001 : Authentification JWT

**Date** : 2026-08-13  
**Agent** : Dev  
**Statut** : Implémenté ✅

---

## ✅ Fichiers créés / modifiés

### Nouveaux fichiers

| Fichier | Description |
|---------|-------------|
| `Domain/Enums/UserRole.cs` | Enum Patient/Medecin/Admin |
| `Domain/Entities/User.cs` | Entité utilisateur |
| `Domain/Entities/RefreshToken.cs` | Entité refresh token |
| `Domain/Abstractions/IUserRepository.cs` | Interface repository |
| `Infrastructure/Persistence/UserRepository.cs` | Implémentation repository |
| `Infrastructure/Persistence/Configurations/UserConfiguration.cs` | Config EF User |
| `Infrastructure/Persistence/Configurations/RefreshTokenConfiguration.cs` | Config EF RefreshToken |
| `Infrastructure/Services/JwtService.cs` | Génération/validation JWT |
| `Infrastructure/Services/AuthService.cs` | Register/Login/Logout |
| `Application/DTOs/Auth/AuthDtos.cs` | DTOs Register/Login/Response |
| `Api/Endpoints/AuthEndpoints.cs` | Endpoints /api/auth/* |
| `Api/appsettings.Development.json` | Config JWT + DB |

### Fichiers modifiés

| Fichier | Changement |
|---------|-----------|
| `Infrastructure/Persistence/SanteDbContext.cs` | Ajout DbSet<User>, DbSet<RefreshToken> |
| `Infrastructure/DependencyInjection.cs` | Enregistrement IUserRepository, IJwtService, IAuthService |
| `Api/Program.cs` | Configuration JWT Bearer + Swagger auth + UseAuthentication/UseAuthorization |
| `Api/SanteSenegal.Api.csproj` | Package Microsoft.AspNetCore.Authentication.JwtBearer |
| `Infrastructure/SanteSenegal.Infrastructure.csproj` | Package BCrypt.Net-Next |

---

## 🔐 Endpoints Auth exposés

| Méthode | Endpoint | Auth | Description |
|---------|----------|------|-------------|
| POST | `/api/auth/register` | Anonymous | Inscription |
| POST | `/api/auth/login` | Anonymous | Connexion |
| GET | `/api/auth/me` | Bearer | Infos utilisateur connecté |
| POST | `/api/auth/logout` | Bearer | Déconnexion |

---

## 📋 Exemples d'utilisation

### Inscription
```bash
curl -X POST http://localhost:5000/api/auth/register \
  -H "Content-Type: application/json" \
  -d '{
    "email": "aminata@test.com",
    "password": "MonMotDePasse123!",
    "nom": "Diallo",
    "prenom": "Aminata",
    "telephone": "77 123 45 67",
    "role": "Patient"
  }'
```

### Connexion
```bash
curl -X POST http://localhost:5000/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{
    "email": "aminata@test.com",
    "password": "MonMotDePasse123!"
  }'
```

### Réponse
```json
{
  "accessToken": "eyJhbGciOiJIUzI1NiIs...",
  "refreshToken": "dG9rZW4...",
  "expiresIn": 3600,
  "user": {
    "id": 1,
    "email": "aminata@test.com",
    "nom": "Diallo",
    "prenom": "Aminata",
    "role": "Patient"
  }
}
```

### Appel protégé
```bash
curl http://localhost:5000/api/auth/me \
  -H "Authorization: Bearer eyJhbGciOiJIUzI1NiIs..."
```

---

## ⚠️ Prochaines étapes

1. **Générer la migration** :
   ```bash
   dotnet ef migrations add AddUserAndAuth --project SanteSenegal.Infrastructure --startup-project SanteSenegal.Api
   dotnet ef database update --project SanteSenegal.Infrastructure --startup-project SanteSenegal.Api
   ```

2. **Compiler** :
   ```bash
   dotnet build SanteSenegal.sln
   ```

3. **Tester via Swagger** :
   ```bash
   cd SanteSenegal.Api && dotnet run
   # Ouvrir https://localhost:7001/swagger
   # Cliquer sur "Authorize", entrer : Bearer eyJhbG...
   ```

---

## 🔒 Sécurisation des endpoints existants (à faire)

Les endpoints existants ne sont pas encore protégés. Pour les sécuriser, ajouter `[Authorize]` ou `[Authorize(Roles = "Admin")]` sur les endpoints concernés dans les fichiers `*Endpoints.cs`.

Exemple :
```csharp
// RendezVousEndpoints.cs
group.MapPost("/", [Authorize] async (CreateRendezVousDto createDto, IMediator mediator) =>
{
    ...
});

group.MapGet("/", [Authorize(Roles = "Admin,Medecin")] async (IMediator mediator) =>
{
    ...
});
```

---

*Rapport généré par l'Agent Dev — Workflow BMad*
