# STAB-001 : Organiser et dédoublonner les DTOs Disponibilite

**Type** : Stabilisation technique  
**Priorité** : P0 (bloquant)  
**Estimation** : 1h  
**Agent** : Dev

---

## Contexte
Il existe actuellement **deux ensembles de DTOs** pour `Disponibilite` :
1. `SanteSenegal.Application.DTOs.DisponibiliteDto` (ancien, dans `DisponibiliteDtos.cs`)
2. `SanteSenegal.Application.DTOs.Disponibilites.DisponibiliteDto` (nouveau, dans dossier `Disponibilites/`)

Cela crée des ambiguïtés de compilation et de maintenance. L'objectif est de **conserver uniquement le dossier `Disponibilites/`** (approche par feature) et de supprimer l'ancien fichier.

## Fichiers concernés

| Action | Fichier | Détail |
|--------|---------|--------|
| ❌ SUPPRIMER | `SanteSenegal.Application/DTOs/DisponibiliteDtos.cs` | Contient `DisponibiliteDto`, `DisponibiliteCreateDto`, `DisponibiliteUpdateDto`, `DisponibiliteSearchDto` |
| ✅ GARDER | `SanteSenegal.Application/DTOs/Disponibilites/DisponibiliteDto.cs` | Nouveau DTO |
| ✅ GARDER | `SanteSenegal.Application/DTOs/Disponibilites/DisponibiliteCreateDto.cs` | Nouveau DTO |
| ✅ GARDER | `SanteSenegal.Application/DTOs/Disponibilites/DisponibiliteUpdateDto.cs` | Nouveau DTO |
| 🔧 MODIFIER | `SanteSenegal.Application/Disponibilites/Commands/CreateDisponibiliteCommand.cs` | Vérifier le `using` |
| 🔧 MODIFIER | `SanteSenegal.Application/Disponibilites/Queries/GetAllDisponibilitesQuery.cs` | Vérifier le `using` |
| 🔧 MODIFIER | Tous les handlers Disponibilite | Mettre à jour les `using` |
| 🔧 MODIFIER | `SanteSenegal.Application.csproj` | Si besoin d'ajuster les références |

## Analyse des différences

### DisponibiliteDto (ancien vs nouveau)

**Ancien (`DTOs/DisponibiliteDtos.cs`)** :
```csharp
public record DisponibiliteDto
{
    public int Id { get; init; }
    public DateTime Date { get; init; }
    public TimeSpan HeureDebut { get; init; }
    public TimeSpan HeureFin { get; init; }
    public bool EstDisponible { get; init; }
    public string? Notes { get; init; }
    public int? MaxRendezVous { get; init; }
    public int NombreRendezVousPris { get; init; }
}
```

**Nouveau (`DTOs/Disponibilites/DisponibiliteDto.cs`)** :
```csharp
public class DisponibiliteDto
{
    public int Id { get; set; }
    public DateTime Date { get; set; }
    public TimeSpan HeureDebut { get; set; }
    public TimeSpan HeureFin { get; set; }
    public bool EstDisponible { get; set; }
    public int MaxRendezVous { get; set; }          // ⚠️ plus nullable
    public int NombreRendezVousPris { get; set; }
    public string Notes { get; set; }               // ⚠️ plus nullable
}
```

**Décision** : Garder le nouveau (`class`) mais ajouter les `?` pour la nullabilité cohérente avec le domaine.

### DisponibiliteCreateDto (ancien vs nouveau)

**Ancien** : `MaxRendezVous` est `int?`  
**Nouveau** : `MaxRendezVous` est `int` (non nullable)

**Décision** : Garder `int?` dans le DTO de création pour permettre la valeur par défaut côté serveur.

## Critères d'acceptation

- [ ] Le fichier `SanteSenegal.Application/DTOs/DisponibiliteDtos.cs` est supprimé
- [ ] Tous les `using SanteSenegal.Application.DTOs;` relatifs à Disponibilite sont remplacés par `using SanteSenegal.Application.DTOs.Disponibilites;`
- [ ] La solution compile sans erreur
- [ ] Les tests (s'ils existent) passent
- [ ] Swagger affiche correctement les schémas Disponibilite

## Notes pour le Dev Agent

⚠️ **Attention aux références croisées** :
- `CreateDisponibiliteCommand.cs` utilise `SanteSenegal.Application.DTOs.Disponibilites.DisponibiliteCreateDto`
- `CreateDisponibiliteCommandHandler.cs` retourne `DisponibiliteDto` — vérifier quel namespace il utilise
- `DisponibiliteEndpoints.cs` reçoit les DTOs en paramètre

📝 **Après suppression**, vérifier que `DisponibiliteSearchDto` n'est pas utilisé ailleurs. S'il est utilisé, le déplacer dans `DTOs/Disponibilites/`.
