---
id: US-UI-001
title: Refonte complete de la page Home
status: accepted
epic: EPIC-UX
tags:
  - frontend
  - ui
  - home
  - accessibility
  - blazor
entities:
  - Home
  - HomeHero
  - HomeSearchBar
  - HomeQuickActions
  - HomeNearbyStructures
  - HomeHowItWorks
created: 2026-08-16
updated: 2026-08-16
---

# US-UI-001 : Refonte complete de la page Home

**Epic**: EPIC-UX
**Priorite**: P0
**Statut**: Done
**Source PRD**: parcours d'accueil patient/structure
**Source architecture/ADR**: Clean Architecture, Blazor WASM

## Contexte

La page Home etait encore le template Blazor par defaut. Elle devait devenir un vrai point d'entree produit pour Sante Senegal, sans modifier la logique metier, l'API ou les acces donnees.

## Objectif

Refondre `Home.razor` et creer `Home.razor.css` avec une experience mobile-first, responsive et accessible.

## Scope

Inclus:

- Hero;
- barre de recherche;
- actions rapides;
- structures proches;
- section "Comment ca marche";
- composants reutilisables sous `SanteSenegal.Web/Shared/Home`;
- tests UI bUnit dedies;
- documentation BMAD.

Exclus:

- modification API;
- acces direct a la base de donnees;
- injection de donnees fictives de structures;
- modification de logique metier;
- refonte des pages ciblees par les liens.

## Fichiers touches

- `SanteSenegal.Web/Pages/Home.razor`
- `SanteSenegal.Web/Pages/Home.razor.css`
- `SanteSenegal.Web/Shared/Home/HomeHero.razor`
- `SanteSenegal.Web/Shared/Home/HomeSearchBar.razor`
- `SanteSenegal.Web/Shared/Home/HomeQuickActions.razor`
- `SanteSenegal.Web/Shared/Home/HomeQuickActionCard.razor`
- `SanteSenegal.Web/Shared/Home/HomeNearbyStructures.razor`
- `SanteSenegal.Web/Shared/Home/HomeHowItWorks.razor`
- `SanteSenegal.Web/Shared/Home/HomeSectionHeader.razor`
- `SanteSenegal.Web/_Imports.razor`
- `SanteSenegal.Web.Tests/SanteSenegal.Web.Tests.csproj`
- `SanteSenegal.Web.Tests/HomePageTests.cs`
- `SanteSenegal.sln`

## Decisions

- La section "Structures proches" n'affiche aucune fausse structure. Elle fournit un etat vide et un lien vers `/structures` jusqu'au branchement d'un vrai service applicatif.
- La recherche est un formulaire GET vers `/structures`; elle ne consomme pas l'API et n'accede pas a la base.
- Les textes principaux sont en francais avec un signal Wolof dans le hero et dans la recherche.
- Les liens rapides utilisent les routes Blazor existantes: `/structures`, `/triage`, `/accident`, `/alertes-temps-reel`.
- Les styles sont portes par `Home.razor.css` avec une approche mobile-first et des focus visibles.

## Criteres d'acceptation

- [x] `Home.razor` remplace le template par une vraie page d'accueil.
- [x] `Home.razor.css` existe et porte la mise en page responsive.
- [x] Les sections obligatoires sont presentes.
- [x] Aucun acces direct a la base n'est ajoute.
- [x] Aucun appel API n'est ajoute.
- [x] Aucun mock de structures proches n'est rendu.
- [x] Les composants sont reutilisables.
- [x] Les tests UI couvrent les sections, la recherche et l'absence de mock.

## Verification

```bash
dotnet build SanteSenegal.Web/SanteSenegal.Web.csproj --no-restore -p:OutputPath=..\artifacts\web-build\
dotnet test SanteSenegal.Web.Tests/SanteSenegal.Web.Tests.csproj --no-restore -p:OutputPath=..\artifacts\web-tests\
```

Resultats:

- Build Web: OK, 0 warning, 0 erreur.
- Tests UI: OK, 3 passed.

## Notes

Le `OutputPath` isole les sorties de build parce que Visual Studio Insiders verrouille parfois les DLL dans `SanteSenegal.Web/bin/Debug/net9.0`.
