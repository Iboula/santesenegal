---
id: ADR-UI-004
title: Architecture de la recherche des établissements
status: accepted
dependencies:
  - US-UI-004
  - US-UI-003
tags:
  - architecture
  - frontend
  - search
  - facilities
entities:
  - SearchFacilities
  - SearchBar
  - SearchFilters
  - SearchResults
  - FacilityCard
---

# Architecture de la recherche des établissements

Ce document formalise les décisions de Cheikh pour `US-UI-004`.

## Décision 1 — Séparer recherche et géolocalisation

`US-UI-004` couvre uniquement :

```text
Recherche
  |
  v
Filtres
  |
  v
Résultats
```

La géolocalisation sera traitée dans `US-UI-006`.

## Décision 2 — Recherche orientée besoins

Le moteur doit accepter des entrées telles que :

- fièvre ;
- radiographie ;
- pharmacie ;
- maternité ;
- hôpital ;
- pédiatre.

## Décision 3 — Réutiliser le Design System

Composants existants à réutiliser :

- FacilityCard
- StatusBadge
- PrimaryButton
- SecondaryButton
- DesignTextInput
- LoadingState
- EmptyState
- ErrorState

## Décision 4 — Architecture frontend cible

```text
Pages/
  SearchFacilities.razor

Components/
  Search/
    SearchBar.razor
    SearchFilters.razor
    SearchResults.razor
    SearchResultsHeader.razor
```

## États

- Idle
- Searching
- ResultsFound
- NoResults
- Error

## Contraintes

Autorisé :

- Blazor ;
- CSS isolé ;
- Design System existant.

Interdit dans cette story :

- JavaScript externe ;
- Leaflet ;
- Google Maps ;
- OpenStreetMap ;
- GPS ;
- bibliothèques cartographiques.
