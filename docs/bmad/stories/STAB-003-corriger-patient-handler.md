# STAB-003 : Corriger CreatePatientHandler pour MediatR

**Type** : Stabilisation technique — Bug  
**Priorité** : P0 (bloquant)  
**Estimation** : 30min  
**Agent** : Dev

---

## Contexte
Le `CreatePatientHandler` n'implémente **pas** l'interface `IRequestHandler<,>` de MediatR. Par conséquent, il n'est pas découvert automatiquement par le DI container et la commande `CreatePatientCommand` ne peut pas être exécutée via `_mediator.Send()`.

## Fichier concerné

```
SanteSenegal.Application/Patients/Handlers/CreatePatientHandler.cs
```

## Code actuel (❌ incorrect)

```csharp
using SanteSenegal.Application.Abstractions;
using SanteSenegal.Application.Patients.Commands;
using SanteSenegal.Domain.Entities;

namespace SanteSenegal.Application.Patients.Handlers;

public class CreatePatientHandler
{
    private readonly IPatientRepository _repository;
    private readonly IUnitOfWork _uow;

    public CreatePatientHandler(IPatientRepository repository, IUnitOfWork uow)
    {
        _repository = repository;
        _uow = uow;
    }

    public async Task<int> Handle(CreatePatientCommand command, CancellationToken cancellationToken)
    {
        var patient = new Patient
        {
            Nom = command.Nom,
            Prenom = command.Prenom,
            DateNaissance = command.DateNaissance,
            Telephone = command.Telephone
        };

        await _repository.AddAsync(patient, cancellationToken);
        await _uow.SaveChangesAsync(cancellationToken);
        return patient.Id;
    }
}
```

## Problèmes identifiés

| # | Problème | Impact |
|---|----------|--------|
| 1 | Pas d'implémentation de `IRequestHandler<CreatePatientCommand, int>` | MediatR ne peut pas router la commande |
| 2 | Pas de `using MediatR;` | Impossible d'implémenter l'interface |
| 3 | La commande `CreatePatientCommand` doit hériter de `IRequest<int>` | Vérifier le fichier de la commande |

## Code cible (✅ correct)

```csharp
using MediatR;
using SanteSenegal.Application.Abstractions;
using SanteSenegal.Application.Patients.Commands;
using SanteSenegal.Domain.Entities;

namespace SanteSenegal.Application.Patients.Handlers;

public class CreatePatientHandler : IRequestHandler<CreatePatientCommand, int>
{
    private readonly IPatientRepository _repository;
    private readonly IUnitOfWork _uow;

    public CreatePatientHandler(IPatientRepository repository, IUnitOfWork uow)
    {
        _repository = repository;
        _uow = uow;
    }

    public async Task<int> Handle(CreatePatientCommand command, CancellationToken cancellationToken)
    {
        var patient = new Patient
        {
            Nom = command.Nom,
            Prenom = command.Prenom,
            DateNaissance = command.DateNaissance,
            Telephone = command.Telephone
        };

        await _repository.AddAsync(patient, cancellationToken);
        await _uow.SaveChangesAsync(cancellationToken);
        return patient.Id;
    }
}
```

## Vérification de la Command

Vérifier que `CreatePatientCommand` implémente bien `IRequest<int>` :

```csharp
// SanteSenegal.Application/Patients/Commands/CreatePatientCommand.cs

using MediatR;

namespace SanteSenegal.Application.Patients.Commands;

public record CreatePatientCommand(
    string Nom,
    string Prenom,
    DateTime DateNaissance,
    string Telephone
) : IRequest<int>;
```

Si ce n'est pas le cas, la corriger aussi.

## Critères d'acceptation

- [ ] `CreatePatientHandler` implémente `IRequestHandler<CreatePatientCommand, int>`
- [ ] `CreatePatientCommand` hérite de `IRequest<int>`
- [ ] La solution compile sans erreur
- [ ] L'endpoint `POST /api/patients` fonctionne via MediatR
- [ ] Un test manuel avec Swagger crée bien un patient

## Notes pour le Dev Agent

⚠️ **Vérifier aussi** `GetPatientByIdHandler` pour s'assurer qu'il implémente bien `IRequestHandler<,>` aussi.

📝 **Pattern à suivre** : Utiliser les autres handlers comme référence (ex: `CreateRendezVousCommandHandler`).
