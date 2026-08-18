---
id: BMAD-VEC-RELATIONSHIPS-001
title: Matrice des relations documentaires
status: ready-for-review
tags:
  - bmad
  - vectorisation
  - relationships
  - dependencies
entities:
  - US-UI-001
  - US-UI-002
  - US-UI-003
  - US-UI-004
  - UX-US-UI-004
  - ADR-UI-004
  - SPEC-US-UI-004
  - FacilityCard
  - StatusBadge
  - PrimaryButton
  - SecondaryButton
  - BMAD-AGENTS-001
  - Serigne
  - Ousmane
created: 2026-08-16
updated: 2026-08-17
---

# Matrice des relations documentaires

Cette matrice documente les relations connues pour le corpus BMAD vectorisable.

Elle est documentaire uniquement et ne définit aucune implémentation technique de base vectorielle.

## Chaîne de dépendances UI

```text
US-UI-001
   |
   v
US-UI-002
   |
   v
US-UI-003
   |
   v
US-UI-004
```

## Relations par story

```text
US-UI-001
|-- Home
|-- HomeHero
|-- HomeSearchBar
|-- HomeQuickActions
|-- HomeNearbyStructures
`-- HomeHowItWorks
```

```text
US-UI-002
|-- PublicLayout
|-- PublicHeader
|-- BottomNavigation
`-- LanguageSelector
```

```text
US-UI-003
|-- FacilityCard
|-- StatusBadge
|-- PrimaryButton
|-- SecondaryButton
|-- LoadingState
|-- EmptyState
`-- ErrorState
```

```text
US-UI-004
|-- UX-US-UI-004
|-- ADR-UI-004
`-- SPEC-US-UI-004
```

```text
ADR-UI-004
|-- SearchFacilities
|-- SearchBar
|-- SearchFilters
|-- SearchResults
`-- FacilityCard
```

```text
SPEC-US-UI-004
|-- DesignTextInput
|-- FacilityCard
|-- StatusBadge
|-- LoadingState
|-- EmptyState
`-- ErrorState
```

## Relations UX

```text
UX-RESEARCH
|-- UX-WIREFRAMES
`-- UX-DESIGN-SYSTEM
```

```text
UX-NAVIGSANTE-CONCEPTION
|-- TriageEngine
|-- Cartographe
|-- Assistant
`-- Alertes
```

## Relations agents BMAD

```text
BMAD-AGENTS-001
|-- BMAD-AGENTS-WORKFLOW-001
|-- BMAD-AGENTS-RESPONSIBILITIES-001
|-- Mamadou
|-- Babacar
|-- Aïssatou
|-- Cheikh
|-- Ibrahima
|-- Ousmane
|-- Fatou
|-- Moustapha
|-- Khadim
`-- Serigne
```

```text
Mamadou
   |
   v
Babacar
   |
   v
Aïssatou
   |
   v
Cheikh
   |
   v
Ibrahima
   |
   v
Ousmane
   |
   v
Fatou
   |
   v
Moustapha
   |
   v
Khadim
   |
   v
Serigne
```
