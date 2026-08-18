# US-UI-003 : Design System

**Epic**: EPIC-UX
**Priorite**: P0
**Statut**: Ready for BMAD Review
**Source PRD**: socle UI transversal
**Source architecture/ADR**: Blazor, CSS isole, Clean Architecture

## Contexte

Les stories `US-UI-001` et `US-UI-002` ont introduit les premieres bases de l'experience citoyenne. Avant d'ajouter de nouvelles fonctionnalites, l'application a besoin d'un Design System reutilisable pour les modules Structures, Triage, Rendez-vous, SOS, Patient et Administration.

## Objectif

Creer un socle de composants et de tokens CSS reutilisables dans la couche presentation uniquement.

## Scope

Inclus:

- tokens CSS globaux;
- typographie;
- styles de composants;
- boutons primaires et secondaires;
- carte `FacilityCard`;
- badge `StatusBadge`;
- etats `LoadingState`, `EmptyState`, `ErrorState`;
- composants de base `Inputs` et `Layout` pour suivre l'arborescence demandee;
- tests bUnit;
- documentation BMAD.

Exclus:

- modification API;
- modification Application;
- modification Domain;
- modification Infrastructure;
- appels API;
- acces direct a la base;
- donnees simulees;
- logique metier.

## Arborescence creee

```text
SanteSenegal.Web/
  Components/
    Buttons/
    Cards/
    Badges/
    Feedback/
    Inputs/
    Layout/
  wwwroot/
    css/
      tokens.css
      typography.css
      components.css
      app.css
```

## Accessibilite

- variables de contraste avec texte sombre sur surfaces claires;
- `focus-visible` explicite;
- zones tactiles minimales de 44 px sur boutons et champs;
- `aria-label` sur boutons et badges;
- `role="status"` pour chargement et etat vide;
- `role="alert"` pour erreur.

## Breakpoints

- Mobile: `360px`;
- Tablette: `768px`;
- Desktop: `1024px`.

## Verification

Commandes a executer:

```bash
dotnet build SanteSenegal.Web/SanteSenegal.Web.csproj -c CodexUi003 -p:OutputPath=..\artifacts\us-ui-003-web-build\
dotnet test SanteSenegal.Web.Tests/SanteSenegal.Web.Tests.csproj -c CodexUi003Tests -p:OutputPath=..\artifacts\us-ui-003-web-tests\ -p:WasmEnableWebcil=false
git diff --check
```

Resultats:

- Build Web: OK, 0 warning, 0 erreur.
- Tests bUnit: OK, 11 tests passes, 0 echec, 0 ignore.
- `git diff --check`: OK, aucun whitespace error. Git signale uniquement des avertissements CRLF sur des fichiers texte suivis.
