# STAB-006 : Ajouter CreatedAt/UpdatedAt à BaseEntity

**Type** : Stabilisation technique — Amélioration  
**Priorité** : P0  
**Estimation** : 1h  
**Agent** : Dev

---

## Contexte
L'entité de base `BaseEntity` ne contient que l'`Id`. Il n'y a aucune traçabilité temporelle (quand une entité a été créée ou modifiée). C'est essentiel pour :
- L'audit
- Le debugging
- Le reporting
- La synchronisation future

## Fichiers concernés

| Action | Fichier | Détail |
|--------|---------|--------|
| 🔧 MODIFIER | `SanteSenegal.Domain/Entities/BaseEntity.cs` | Ajouter `CreatedAt`, `UpdatedAt` |
| 🔧 MODIFIER | `SanteSenegal.Infrastructure/Persistence/SanteDbContext.cs` | Ajouter `SaveChanges` override pour auto-set timestamps |
| 🔧 MODIFIER | `SanteSenegal.Infrastructure/Persistence/Configurations/*.cs` | Ajouter `.ValueGeneratedOnAdd()` si nécessaire |
| ✅ CRÉER | Migration EF Core | `Add-Migration AddAuditTimestamps` |

## Spécification technique

### BaseEntity.cs (modifié)
```csharp
namespace SanteSenegal.Domain.Entities;

public abstract class BaseEntity
{
    public int Id { get; set; }
    
    public DateTime CreatedAt { get; set; }
    
    public DateTime UpdatedAt { get; set; }
}
```

> **Note** : On utilise `abstract class` pour empêcher l'instanciation directe de `BaseEntity`.

### SanteDbContext.cs (override SaveChanges)
```csharp
public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
{
    var entries = ChangeTracker.Entries<BaseEntity>();
    
    foreach (var entry in entries)
    {
        switch (entry.State)
        {
            case EntityState.Added:
                entry.Entity.CreatedAt = DateTime.UtcNow;
                entry.Entity.UpdatedAt = DateTime.UtcNow;
                break;
                
            case EntityState.Modified:
                entry.Entity.UpdatedAt = DateTime.UtcNow;
                // Empêcher la modification manuelle de CreatedAt
                entry.Property(nameof(BaseEntity.CreatedAt)).IsModified = false;
                break;
        }
    }
    
    return await base.SaveChangesAsync(cancellationToken);
}
```

### Explication du pattern

```
┌─────────────────┐     ┌─────────────────────┐     ┌─────────────────┐
│  Command        │────▶│  _context.SaveChanges│────▶│  BaseEntity     │
│  Handler        │     │  (override)         │     │  timestamps     │
│  modifie entity │     │                     │     │  auto-set       │
└─────────────────┘     └─────────────────────┘     └─────────────────┘
                              │
                              ▼
                        ┌─────────────┐
                        │  DateTime   │
                        │  UtcNow     │
                        └─────────────┘
```

## Avantages de cette approche

| Avantage | Détail |
|----------|--------|
| **Transparent** | Les handlers n'ont pas à gérer les timestamps |
| **Cohérent** | Toutes les entités ont les mêmes champs d'audit |
| **Sécurisé** | `CreatedAt` ne peut pas être modifié après création |
| **UTC** | Pas de problème de fuseau horaire |

## Configuration EF Core optionnelle

Si on veut que les colonnes soient non-nullable avec une valeur par défaut en DB :

```csharp
// Dans chaque configuration (ou via un apply global)
builder.Property(e => e.CreatedAt)
    .IsRequired()
    .HasDefaultValueSql("CURRENT_TIMESTAMP");

builder.Property(e => e.UpdatedAt)
    .IsRequired()
    .HasDefaultValueSql("CURRENT_TIMESTAMP");
```

**Recommandation** : Ne pas mettre de `HasDefaultValueSql` pour éviter les conflits avec le code C#. Le code C# gère déjà tout.

## Vérification des impacts

Toutes les entités héritent de `BaseEntity` :
- ✅ `Patient`
- ✅ `RendezVous`
- ✅ `Paiement`
- ✅ `Disponibilite`
- ✅ `Service`
- ✅ `SousService`
- ✅ `Structure`
- ✅ `Transaction`
- ✅ `PatientStructureFavorite`

Donc **toutes** auront automatiquement `CreatedAt` et `UpdatedAt`.

## Critères d'acceptation

- [ ] `BaseEntity` contient `CreatedAt` et `UpdatedAt`
- [ ] `BaseEntity` est déclaré `abstract`
- [ ] `SanteDbContext.SaveChangesAsync` est surchargé pour auto-set les timestamps
- [ ] `CreatedAt` est protégé en modification (cas `Modified`)
- [ ] Une migration `AddAuditTimestamps` est créée
- [ ] La migration s'applique sans erreur
- [ ] Test manuel : créer un patient → vérifier que `CreatedAt` et `UpdatedAt` sont remplis
- [ ] Test manuel : modifier un patient → vérifier que seul `UpdatedAt` change

## Notes pour le Dev Agent

⚠️ **Attention** : Si des données existent déjà en DB, la migration doit gérer les valeurs par défaut pour les enregistrements existants :

```csharp
// Dans la migration Up():
migrationBuilder.Sql("UPDATE \"Patients\" SET \"CreatedAt\" = NOW(), \"UpdatedAt\" = NOW()");
// Répéter pour chaque table...
```

📝 **Alternative future** : Pour un audit complet (qui a modifié quoi), envisager ultérieurement :
- `CreatedBy` / `UpdatedBy` (string ou int, lié à l'user)
- Table d'audit séparée (`AuditLog`)

Mais pour l'instant, les timestamps suffisent.
