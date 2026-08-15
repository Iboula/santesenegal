# STAB-002 : Fusionner le projet fantôme SanteSenegel.Application

**Type** : Stabilisation technique  
**Priorité** : P0 (bloquant)  
**Estimation** : 30min  
**Agent** : Dev

---

## Contexte
Il existe un projet mal orthographié `SanteSenegel.Application` ("Senegel" au lieu de "Senegal") qui contient le handler `CreateStructureCommandHandler`. Ce fichier est dans le **mauvais projet** et avec un **namespace incorrect**.

## Fichiers concernés

| Action | Fichier source | Fichier cible |
|--------|---------------|---------------|
| 📦 DÉPLACER | `SanteSenegel.Application/Structures/Commands/Handlers/CreateStructureCommandHandler.cs` | `SanteSenegal.Application/Structures/Commands/Handlers/CreateStructureCommandHandler.cs` |
| ❌ SUPPRIMER | `SanteSenegel.Application/` (dossier entier) | — |
| 🔧 MODIFIER | `SanteSenegal.sln` | Retirer la référence au projet `SanteSenegel.Application` |

## Analyse du code à déplacer

```csharp
// Fichier actuel (mauvais emplacement)
// SanteSenegel.Application/Structures/Commands/Handlers/CreateStructureCommandHandler.cs

using MediatR;
using SanteSenegal.Application.DTOs;        // ✅ OK
using SanteSenegal.Domain.Abstractions;     // ✅ OK
using SanteSenegal.Domain.Entities;         // ✅ OK

namespace SanteSenegal.Application.Structures.Commands.Handlers;  // ✅ Namespace correct malgré le chemin

public class CreateStructureCommandHandler : IRequestHandler<CreateStructureCommand, StructureDto>
{
    private readonly IStructureRepository _structureRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateStructureCommandHandler(IStructureRepository structureRepository, IUnitOfWork unitOfWork)
    {
        _structureRepository = structureRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<StructureDto> Handle(CreateStructureCommand request, CancellationToken cancellationToken)
    {
        var structure = new Structure
        {
            Nom = request.Nom,
            Type = request.Type,
            Adresse = request.Adresse,
            Telephone = request.Telephone,
            Email = request.Email,
            Description = request.Description,
            ImageUrl = request.ImageUrl,
            HorairesOuverture = request.HorairesOuverture,
            EstActif = true
        };

        await _structureRepository.AddAsync(structure, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new StructureDto
        {
            Id = structure.Id,
            Nom = structure.Nom,
            Type = structure.Type,
            Adresse = structure.Adresse,
            Telephone = structure.Telephone,
            Email = structure.Email,
            Description = structure.Description,
            ImageUrl = structure.ImageUrl,
            HorairesOuverture = structure.HorairesOuverture,
            EstActif = structure.EstActif,
            Services = new List<ServiceBasicDto>()
        };
    }
}
```

## Vérifications à faire

1. **Le dossier cible existe-t-il ?**
   ```
   SanteSenegal.Application/Structures/Commands/Handlers/
   ```
   Oui, il existe déjà (contient `UpdateStructureCommandHandler.cs`, etc.)

2. **Y a-t-il un conflit de nom ?**
   Non, `CreateStructureCommandHandler.cs` n'existe pas encore dans le bon dossier.

3. **Le namespace est-il correct ?**
   Oui, `namespace SanteSenegal.Application.Structures.Commands.Handlers;` est correct.

## Critères d'acceptation

- [ ] Le fichier `CreateStructureCommandHandler.cs` est déplacé dans `SanteSenegal.Application/Structures/Commands/Handlers/`
- [ ] Le dossier `SanteSenegel.Application` est supprimé
- [ ] La solution `SanteSenegal.sln` ne référence plus `SanteSenegel.Application`
- [ ] La solution compile sans erreur
- [ ] L'endpoint `POST /api/structures` fonctionne correctement

## Notes pour le Dev Agent

⚠️ **Vérifier la solution (.sln)** : Le projet `SanteSenegel.Application` pourrait être référencé dans le fichier `.sln`. Ligne à supprimer :
```
Project("{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}") = "SanteSenegel.Application", ...
```

📝 **Vérifier aussi** : `SanteSenegal.Api.csproj` ne doit pas référencer `SanteSenegel.Application`.
