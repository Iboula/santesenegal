---
id: SPEC-US-UI-004
title: Specification implementation recherche établissements
status: ready-for-review
dependencies:
  - US-UI-004
  - UX-US-UI-004
  - ADR-UI-004
  - US-UI-003
tags:
  - spec
  - frontend
  - blazor
  - facilities
  - search
entities:
  - SearchFacilities
  - SearchBar
  - SearchFilters
  - SearchResults
  - SearchResultsHeader
  - FacilityCard
  - StatusBadge
  - DesignTextInput
---

# Specification implementation recherche établissements

Ce document formalise le SPEC d'Ibrahima pour `US-UI-004`.

## Objectif

Permettre à un citoyen de rechercher un établissement avec :

- nom ;
- symptôme ;
- spécialité ;
- service.

## Scope

Créer plus tard pendant l'implémentation :

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

## Recherche

Utiliser `DesignTextInput`.

Le champ accepte notamment :

- fièvre ;
- radiographie ;
- pédiatre ;
- pharmacie ;
- maternité ;
- hôpital.

## Filtres

- distance ;
- région ;
- disponibilité ;
- type d'établissement ;
- spécialité.

## Résultats

Utiliser `FacilityCard`.

Afficher :

- nom ;
- adresse ;
- disponibilité ;
- distance ;
- temps d'attente.

## États

Utiliser :

- LoadingState ;
- EmptyState ;
- ErrorState.

## Responsive

Prévoir :

- 360 px ;
- 768 px ;
- 1024 px.

## Accessibilité

Respecter WCAG 2.2.

## Internationalisation

Préparer :

- français ;
- wolof.

Ne pas implémenter la traduction dans `US-UI-004`.

## Interdictions

- JavaScript externe ;
- bibliothèque cartographique ;
- GPS ;
- nouveau composant Design System sans justification ;
- modification API ;
- modification Domain ;
- modification Infrastructure.

## Tests attendus pour l'implémentation future

- recherche ;
- filtres ;
- résultats ;
- LoadingState ;
- EmptyState ;
- ErrorState.
