# BMAD-001 : Framework BMAD de continuation

**Epic**: EPIC-STAB
**Priorite**: P0
**Statut**: Done
**Source PRD**: besoin brownfield de continuation projet
**Source architecture/ADR**: `../architecture/ARCHITECTURE.md`, `../BMAD-AUDIT.md`

## Contexte

Le projet contenait deja des documents BMAD, mais il manquait un systeme de pilotage pour continuer le projet de facon iterative. Les anciens rapports et stories ne suffisaient pas a savoir quoi faire ensuite, avec quels criteres et quels checks.

## Objectif

Mettre en place une structure BMAD locale minimale pour cadrer les prochaines iterations sans modifier le code applicatif.

## Scope

Inclus:

- audit BMAD;
- README de navigation;
- backlog trace PRD -> implementation;
- index ADR;
- strategie QA;
- templates de story, ADR et plan de test.

Exclus:

- installation officielle `.bmad-core`;
- refactoring applicatif;
- initialisation Git;
- correction du build global.

## Fichiers touches

- `docs/bmad/README.md`
- `docs/bmad/BMAD-AUDIT.md`
- `docs/bmad/BACKLOG.md`
- `docs/bmad/adr/ADR-000-index.md`
- `docs/bmad/tests/QA-STRATEGY.md`
- `docs/bmad/templates/STORY_TEMPLATE.md`
- `docs/bmad/templates/ADR_TEMPLATE.md`
- `docs/bmad/templates/TEST_PLAN_TEMPLATE.md`

## Criteres d'acceptation

- [x] Le projet a un index BMAD lisible.
- [x] Les gaps BMAD sont documentes.
- [x] Le backlog courant existe avec prochaines stories.
- [x] La QA a une strategie cible.
- [x] Les prochaines stories peuvent etre creees depuis un template.

## Completion

Structure BMAD ajoutee le 2026-08-15.

Verification executee:

- `dotnet build SanteSenegal.Api/SanteSenegal.Api.csproj --no-restore`: OK, 0 warning, 0 erreur.
- `dotnet build SanteSenegal.sln`: timeout apres plus de 2 minutes.
- `dotnet test SanteSenegal.Tests/SanteSenegal.Tests.csproj --no-restore`: KO, compilation tests cassee.
- `dotnet build SanteSenegal.Web/SanteSenegal.Web.csproj --no-restore`: KO, DLL verrouillee par Visual Studio Insiders PID 7256.

Les prochains checks doivent cibler backend, tests ou web separement selon `../tests/QA-STRATEGY.md`.

