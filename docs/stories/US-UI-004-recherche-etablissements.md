---
id: US-UI-004
title: Recherche des établissements de santé
status: ready-for-review
epic: EPIC-002
dependencies:
  - US-UI-001
  - US-UI-002
  - US-UI-003
tags:
  - frontend
  - ui
  - search
  - healthcare
  - facilities
  - accessibility
entities:
  - FacilityCard
  - StatusBadge
  - DesignTextInput
  - LoadingState
  - EmptyState
  - ErrorState
---

# US-UI-004 — Recherche des établissements de santé

## Contexte métier

Un citoyen ne connaît pas forcément le nom ou l'organisation du système de santé.

Il doit pouvoir chercher à partir :

- d'un établissement ;
- d'un symptôme ;
- d'une spécialité ;
- d'un service.

## User Story

En tant que citoyen,

Je souhaite trouver rapidement un établissement adapté à mes besoins,

Afin d'obtenir des soins sans perdre de temps.

## Parcours utilisateur

```text
Accueil
  |
  v
Recherche
  |
  v
Filtres
  |
  v
Résultats
  |
  v
Fiche établissement
```

## Critères d'acceptation

Recherche libre :

- établissement ;
- symptôme ;
- spécialité ;
- service.

Filtres :

- distance ;
- région ;
- type d'établissement ;
- disponibilité ;
- spécialité.

Résultat :

- nom ;
- type ;
- adresse ;
- disponibilité ;
- distance ;
- temps d'attente ;
- actions.

## Hors scope

- géolocalisation ;
- GPS ;
- carte interactive ;
- itinéraire.
