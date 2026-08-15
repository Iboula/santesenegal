# QA Strategy BMAD - SanteSenegal

## Objectif

Donner un feedback fiable sans forcer un build complet mobile/web a chaque petite story.

## Niveaux de verification

| Niveau | Quand | Commande cible |
|---|---|---|
| L1 - Backend compile | Toute story API/Application/Domain/Infrastructure | `dotnet build SanteSenegal.Api/SanteSenegal.Api.csproj` |
| L2 - Tests unitaires | Toute logique metier/service | `dotnet test SanteSenegal.Tests/SanteSenegal.Tests.csproj` |
| L3 - Web compile | Story Blazor | `dotnet build SanteSenegal.Web/SanteSenegal.Web.csproj` |
| L4 - Mobile compile | Story MAUI | build cible mobile dedie, hors boucle rapide |
| L5 - Smoke API | Story endpoint | lancer API puis tester Swagger ou `.http` |

## Risques metier prioritaires

- sante: triage et orientation ne doivent pas donner une recommandation incoherente;
- paiement: aucune double confirmation, aucun remboursement non trace;
- rendez-vous: pas de double reservation du meme creneau;
- donnees personnelles: endpoints sensibles proteges;
- notifications: pas de fuite de token ou donnees sensibles dans les logs.

## Checklist QA par story

- [ ] Build cible execute.
- [ ] Tests unitaires ajoutes ou justifies.
- [ ] Cas erreur verifies.
- [ ] Autorisations verifiees si endpoint.
- [ ] Donnees sensibles non loggees.
- [ ] Documentation/story mise a jour avec resultat.

