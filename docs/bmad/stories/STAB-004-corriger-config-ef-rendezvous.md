# STAB-004 : Corriger la configuration EF Core RendezVous

**Type** : Stabilisation technique — Bug critique DB  
**Priorité** : P0 (bloquant)  
**Estimation** : 1h  
**Agent** : Dev

---

## Contexte
Il y a un **mismatch** entre l'entité `RendezVous` et sa configuration EF Core :

- **L'entité** a : `HeureDebut` + `HeureFin` (deux propriétés)
- **La configuration EF** a : `Heure` (une seule propriété)

Cela provoquera une erreur au runtime lors de l'exécution des requêtes ou de la création de la base de données.

## Fichiers concernés

| Fichier | Ligne problématique |
|---------|---------------------|
| `SanteSenegal.Domain/Entities/RendezVous.cs` | `public TimeSpan HeureDebut { get; set; }` + `public TimeSpan HeureFin { get; set; }` |
| `SanteSenegal.Infrastructure/Persistence/Configurations/RendezVousConfiguration.cs` | `builder.Property(r => r.Heure).IsRequired();` |

## Analyse détaillée

### Entité RendezVous (actuelle)
```csharp
public class RendezVous : BaseEntity
{
    public DateTime Date { get; set; }
    public TimeSpan HeureDebut { get; set; }    // ✅ Heure début
    public TimeSpan HeureFin { get; set; }      // ✅ Heure fin
    public StatutRendezVous Statut { get; set; } = StatutRendezVous.EnAttente;
    // ... autres propriétés
}
```

### Configuration EF (actuelle — BUG)
```csharp
public class RendezVousConfiguration : IEntityTypeConfiguration<RendezVous>
{
    public void Configure(EntityTypeBuilder<RendezVous> builder)
    {
        builder.HasKey(r => r.Id);

        builder.Property(r => r.Date)
            .IsRequired();

        builder.Property(r => r.Heure)    // ❌ ERREUR : 'Heure' n'existe pas dans l'entité !
            .IsRequired();

        // ... reste de la config
    }
}
```

## Correction à apporter

Remplacer dans `RendezVousConfiguration.cs` :

```csharp
// ❌ AVANT
builder.Property(r => r.Heure)
    .IsRequired();

// ✅ APRÈS
builder.Property(r => r.HeureDebut)
    .IsRequired();

builder.Property(r => r.HeureFin)
    .IsRequired();
```

## Vérification des impacts

| Élément à vérifier | Fichier | Action |
|--------------------|---------|--------|
| Command/Query qui utilisent `Heure` | `CreateRendezVousCommand.cs` | Vérifier si `Heure` est utilisé au lieu de `HeureDebut`/`HeureFin` |
| Handler qui mappe `Heure` | `CreateRendezVousCommandHandler.cs` | Corriger si nécessaire |
| DTO qui expose `Heure` | `RendezVousDto` (si existe) | Mettre à jour |
| Endpoint qui reçoit `Heure` | `RendezVousEndpoints.cs` | Vérifier |

### Vérification du handler (code actuel)
```csharp
var rendezVous = new Domain.Entities.RendezVous
{
    Date = request.Date,
    HeureDebut = request.HeureDebut,    // ✅ OK
    HeureFin = request.HeureFin,        // ✅ OK
    // ...
};
```

Le handler est correct. C'est uniquement la **configuration EF** qui est fausse.

## Critères d'acceptation

- [ ] `RendezVousConfiguration.cs` utilise `r.HeureDebut` et `r.HeureFin` au lieu de `r.Heure`
- [ ] Une nouvelle migration est créée (`Add-Migration FixRendezVousHeureColumns`)
- [ ] La migration se génère sans erreur
- [ ] La base de données se met à jour (`Update-Database`)
- [ ] La solution compile sans erreur
- [ ] Un test manuel de création de rendez-vous fonctionne

## Notes pour le Dev Agent

⚠️ **Si une migration existe déjà avec `Heure`** : Il faudra potentiellement une migration de correction qui :
1. Supprime la colonne `Heure` (si elle existe en DB)
2. Ajoute les colonnes `HeureDebut` et `HeureFin`

📝 **Commandes EF Core** :
```bash
dotnet ef migrations add FixRendezVousHeureColumns --project SanteSenegal.Infrastructure --startup-project SanteSenegal.Api
dotnet ef database update --project SanteSenegal.Infrastructure --startup-project SanteSenegal.Api
```
