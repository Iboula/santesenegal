# STAB-005 : Créer les enums pour les statuts Paiement et Transaction

**Type** : Stabilisation technique — Refactoring  
**Priorité** : P0  
**Estimation** : 30min  
**Agent** : Dev

---

## Contexte
Les entités `Paiement` et `Transaction` utilisent des `string` pour leurs statuts :

```csharp
// Paiement.cs
public string Statut { get; set; } = "EnCours"; // EnCours, Reçu, Remboursé

// Transaction.cs
public string Statut { get; set; } = "EN_ATTENTE"; // EN_ATTENTE, SUCCES, ECHEC
```

Cela est dangereux car :
- Pas d'intellisense
- Risque de fautes de frappe
- Valeurs possibles non documentées dans le code
- Comparaisons fragiles (casse, accents)

## Fichiers concernés

| Action | Fichier |
|--------|---------|
| ✅ CRÉER | `SanteSenegal.Domain/Enums/StatutPaiement.cs` |
| ✅ CRÉER | `SanteSenegal.Domain/Enums/StatutTransaction.cs` |
| 🔧 MODIFIER | `SanteSenegal.Domain/Entities/Paiement.cs` |
| 🔧 MODIFIER | `SanteSenegal.Domain/Entities/Transaction.cs` |
| 🔧 MODIFIER | `SanteSenegal.Infrastructure/Persistence/Configurations/` (si mapping enum spécifique) |
| 🔧 MODIFIER | Tous les handlers/services qui assignent/lisent ces statuts |

## Spécifications des enums

### StatutPaiement
```csharp
namespace SanteSenegal.Domain.Enums;

public enum StatutPaiement
{
    EnAttente,      // Paiement initié, en attente de confirmation
    Recu,           // Paiement confirmé et reçu
    Rembourse,      // Paiement remboursé au patient
    Echoue,         // Paiement échoué
    Annule          // Paiement annulé par le patient
}
```

### StatutTransaction
```csharp
namespace SanteSenegal.Domain.Enums;

public enum StatutTransaction
{
    EnAttente,      // Transaction initiée
    Succes,         // Transaction réussie
    Echec,          // Transaction échouée
    Annulee         // Transaction annulée
}
```

## Modifications des entités

### Paiement.cs
```csharp
// ❌ AVANT
public string Statut { get; set; } = "EnCours";

// ✅ APRÈS
public StatutPaiement Statut { get; set; } = StatutPaiement.EnAttente;
```

### Transaction.cs
```csharp
// ❌ AVANT
public string Statut { get; set; } = "EN_ATTENTE";

// ✅ APRÈS
public StatutTransaction Statut { get; set; } = StatutTransaction.EnAttente;
```

## Vérification des usages

Rechercher toutes les assignations de statut :
```bash
grep -r "Statut.*=" --include="*.cs" SanteSenegal/
grep -r "\"EnCours\"" --include="*.cs" SanteSenegal/
grep -r "\"EN_ATTENTE\"" --include="*.cs" SanteSenegal/
grep -r "\"Reçu\"" --include="*.cs" SanteSenegal/
grep -r "\"Remboursé\"" --include="*.cs" SanteSenegal/
grep -r "\"SUCCES\"" --include="*.cs" SanteSenegal/
grep -r "\"ECHEC\"" --include="*.cs" SanteSenegal/
```

## Configuration EF Core (PostgreSQL)

Pour PostgreSQL, les enums nécessitent une configuration spécifique :

Option 1 : Mapping en `varchar` (recommandé pour compatibilité)
```csharp
builder.Property(p => p.Statut)
    .HasConversion<string>()   // Stocke le nom de l'enum en DB
    .HasMaxLength(50);
```

Option 2 : Enum natif PostgreSQL (plus complexe)
```csharp
// Nécessite Npgsql.EntityFrameworkCore.PostgreSQL configuration
// modelBuilder.HasPostgresEnum<StatutPaiement>();
```

**Recommandation** : Option 1 (`HasConversion<string>`) pour garder la DB lisible et portable.

## Critères d'acceptation

- [ ] Les enums `StatutPaiement` et `StatutTransaction` sont créées
- [ ] Les entités `Paiement` et `Transaction` utilisent ces enums
- [ ] La configuration EF mappe les enums en `varchar`
- [ ] Toutes les assignations de statut utilisent les enums (pas de string magic)
- [ ] Une migration est créée et appliquée
- [ ] La solution compile sans erreur

## Notes pour le Dev Agent

⚠️ **Attention à la casse** : PostgreSQL avec `HasConversion<string>` stocke exactement le nom de l'enum (`"EnAttente"`, `"Succes"`). Si des données existent en DB avec d'autres formats (`"EN_ATTENTE"`), il faudra une migration de données.

📝 **Vérifier aussi** : `MethodePaiement.cs` existe déjà comme enum. S'assurer de la cohérence de style.
