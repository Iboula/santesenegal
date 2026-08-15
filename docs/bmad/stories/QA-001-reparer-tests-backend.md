# QA-001 : Reparer la compilation des tests backend

**Epic**: EPIC-STAB
**Priorite**: P0
**Statut**: Ready
**Source PRD**: qualite transverse
**Source architecture/ADR**: `../tests/QA-STRATEGY.md`

## Contexte

Le backend API compile correctement, mais le projet de tests ne compile pas. Cela bloque la boucle BMAD Dev -> QA.

## Objectif

Remettre `SanteSenegal.Tests` en compilation et executer la suite de tests.

## Erreurs observees

Commande:

```bash
dotnet test SanteSenegal.Tests/SanteSenegal.Tests.csproj --no-restore
```

Resultat:

- warning `MSB3277`: conflit `Microsoft.EntityFrameworkCore.Relational` entre `9.0.1` et `9.0.9`;
- `AccidentRouteServiceTests.cs(35,32)`: argument 5 attendu `string?`, recu `int`;
- `AccidentRouteServiceTests.cs(128,37)`: conversion `int` vers `string`;
- `AuthServiceTests.cs(26,34)`, `(27,34)`, `(43,34)`, `(69,34)`, `(95,34)`: Moq/expression tree avec arguments optionnels;
- `PaiementServiceTests.cs(117,28)` et `(118,50)`: `Result<(Paiement Paiement, bool EstPaye)>` n'a pas de propriete `Data`.

## Criteres d'acceptation

- [ ] Harmoniser les versions EF Core dans tests/infrastructure.
- [ ] Corriger `AccidentRouteServiceTests` selon les signatures actuelles.
- [ ] Corriger les setups Moq de `AuthServiceTests` sans arguments optionnels implicites.
- [ ] Adapter `PaiementServiceTests` au type `Result<T>` actuel.
- [ ] `dotnet test SanteSenegal.Tests/SanteSenegal.Tests.csproj --no-restore` passe.

## Notes Dev

Ne pas modifier le comportement metier pour satisfaire les tests sans verifier les signatures reelles des services. Les tests doivent suivre l'API actuelle si le code applicatif est coherent.

