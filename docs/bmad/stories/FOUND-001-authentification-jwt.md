# FOUND-001 : Authentification JWT Backend

**Type** : Fondation sécurité  
**Priorité** : P0 (bloquant pour toute feature utilisateur)  
**Estimation** : 3-4h  
**Sprint** : Phase 2 — Fondations

---

## Contexte
Actuellement, l'API est **complètement ouverte**. N'importe qui peut créer, lire, modifier ou supprimer des données sans authentification. C'est critique pour une application de santé avec des données personnelles.

## Objectif
Implémenter un système d'authentification **JWT Bearer** avec rôles, permettant :
- L'inscription de nouveaux utilisateurs
- La connexion sécurisée
- La protection des endpoints sensibles
- La gestion des permissions par rôle

---

## Architecture technique (ADR-003 complément)

### Choix : JWT HMAC256 (symétrique)

| Option | Pour | Contre |
|--------|------|--------|
| **HMAC256 (symétrique)** | Simple, 1 seule clé, facile à rotate | Clé partagée serveur-only |
| RS256 (asymétrique) | Plus sécurisé, clé publique distribuable | Complexité, certificats |

**Décision** : HMAC256 pour l'instant (MVP). Migrer vers RS256 si besoin d'interopérabilité tierce.

### Stockage des mots de passe
- **Algorithme** : BCrypt (via BCrypt.Net-Next)
- **Work factor** : 12 (équilibre sécurité/performance)

### Structure du token JWT

```json
{
  "sub": "123",           // UserId
  "email": "user@test.com",
  "role": "Patient",       // ou "Medecin", "Admin"
  "jti": "uuid-unique",    // Token ID pour invalidation future
  "iat": 1691884800,
  "exp": 1691888400        // 1 heure
}
```

### Refresh Token
- **Stockage** : Base de données (table RefreshTokens)
- **Durée** : 7 jours
- **Rotation** : 1 refresh token = 1 utilisation (revocation)

---

## Découpage technique

### Tâche 1 : Modèle de données
- [ ] Créer `User` (hérite de BaseEntity)
- [ ] Créer `RefreshToken`
- [ ] Ajouter DbSet dans SanteDbContext
- [ ] Créer migration

### Tâche 2 : Services domaine
- [ ] Créer `IAuthService` interface
- [ ] Implémenter `AuthService` (register, login, refresh)
- [ ] Implémenter `JwtService` (génération, validation)
- [ ] Implémenter `PasswordHasher` (BCrypt wrapper)

### Tâche 3 : DTOs et Validators
- [ ] `RegisterRequest` (email, password, nom, prenom, telephone, role)
- [ ] `LoginRequest` (email, password)
- [ ] `AuthResponse` (accessToken, refreshToken, expiresIn, user)
- [ ] FluentValidation sur les DTOs

### Tâche 4 : API Endpoints
- [ ] `POST /api/auth/register`
- [ ] `POST /api/auth/login`
- [ ] `POST /api/auth/refresh`
- [ ] `POST /api/auth/logout` (revoke refresh token)

### Tâche 5 : Middleware & Protection
- [ ] Configurer JWT Bearer dans Program.cs
- [ ] Ajouter `[Authorize]` sur les endpoints sensibles
- [ ] Ajouter `[AllowAnonymous]` sur auth endpoints
- [ ] Créer policy par rôle (Patient, Medecin, Admin)

### Tâche 6 : Sécurisation des endpoints existants
- [ ] `POST /api/patients` → Patient ou Admin
- [ ] `POST /api/rendez-vous` → Patient (son propre)
- [ ] `GET /api/rendez-vous/patient/{id}` → Propriétaire ou Admin
- [ ] `POST /api/structures` → Admin
- [ ] `PUT /api/structures/{id}` → Admin ou propriétaire
- [ ] `GET /api/disponibilites` → Public (AllowAnonymous)

---

## Entités

### User
```csharp
public class User : BaseEntity
{
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string Nom { get; set; } = string.Empty;
    public string Prenom { get; set; } = string.Empty;
    public string? Telephone { get; set; }
    public UserRole Role { get; set; } = UserRole.Patient;
    public bool EstActif { get; set; } = true;
    
    // Navigation
    public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
}
```

### RefreshToken
```csharp
public class RefreshToken : BaseEntity
{
    public string Token { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
    public bool IsRevoked { get; set; } = false;
    public DateTime? RevokedAt { get; set; }
    public string? ReplacedByToken { get; set; }
    
    public int UserId { get; set; }
    public User User { get; set; } = null!;
    
    public bool IsExpired => DateTime.UtcNow >= ExpiresAt;
    public bool IsActive => !IsRevoked && !IsExpired;
}
```

### UserRole (enum)
```csharp
public enum UserRole
{
    Patient,
    Medecin,
    Admin
}
```

---

## Endpoints protégés (matrice)

| Endpoint | Patient | Medecin | Admin | Anonymous |
|----------|---------|---------|-------|-----------|
| GET /api/structures | ✅ | ✅ | ✅ | ✅ |
| POST /api/structures | ❌ | ❌ | ✅ | ❌ |
| GET /api/services | ✅ | ✅ | ✅ | ✅ |
| POST /api/rendez-vous | ✅ | ✅ | ✅ | ❌ |
| GET /api/rendez-vous/mes-rdv | ✅ (propriétaire) | ✅ (propriétaire) | ✅ | ❌ |
| GET /api/rendez-vous/patient/{id} | ❌ (sauf soi) | ✅ | ✅ | ❌ |
| POST /api/disponibilites | ❌ | ✅ (sa structure) | ✅ | ❌ |
| GET /api/patients | ❌ | ✅ | ✅ | ❌ |
| POST /api/patients | ✅ | ✅ | ✅ | ❌ |

---

## Packages NuGet à ajouter

```xml
<PackageReference Include="Microsoft.AspNetCore.Authentication.JwtBearer" Version="9.0.3" />
<PackageReference Include="BCrypt.Net-Next" Version="4.0.3" />
<PackageReference Include="FluentValidation" Version="11.11.0" />
<PackageReference Include="FluentValidation.DependencyInjectionExtensions" Version="11.11.0" />
```

---

## Notes pour le Dev Agent

⚠️ **Important** :
- La clé JWT doit être dans `appsettings.json` (développement) ou variables d'environnement (production)
- Stocker `JWT:Secret` avec au moins 32 caractères
- Ne jamais logger les tokens ou les passwords
- Le refresh token doit être stocké en httpOnly cookie (future amélioration) ou envoyé dans le body

📝 **appsettings.Development.json** à ajouter :
```json
{
  "Jwt": {
    "Secret": "votre-cle-super-secrete-de-32-caracteres!",
    "Issuer": "SanteSenegal.Api",
    "Audience": "SanteSenegal.Client",
    "ExpirationMinutes": 60
  }
}
```

---

## Critères d'acceptation

- [ ] Un utilisateur peut s'inscrire avec email/password
- [ ] Un utilisateur peut se connecter et recevoir accessToken + refreshToken
- [ ] Le accessToken permet d'appeler les endpoints protégés
- [ ] Un token expiré retourne 401
- [ ] Un refresh token valide génère un nouveau accessToken
- [ ] Les endpoints sensibles retournent 403 pour un rôle non autorisé
- [ ] Swagger affiche le bouton "Authorize" avec Bearer token
- [ ] La solution compile et les migrations se génèrent
