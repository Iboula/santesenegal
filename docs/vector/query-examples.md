---
id: BMAD-VEC-QUERIES-001
title: Exemples de requetes RAG
status: accepted
tags:
  - bmad
  - rag
  - queries
  - examples
entities:
  - FacilityCard
  - US-UI-003
  - WCAG 2.2
  - Geolocation
  - ADR
---

# Exemples de requetes RAG

Ces exemples guident les recherches sur la base documentaire BMAD vectorisable.

## Stories et composants

Question:

```text
Quelles stories utilisent FacilityCard ?
```

Strategie de recherche:

- filtrer `entities: FacilityCard`;
- limiter aux documents de type `Story`;
- retourner les `id`, `title`, `status` et dependances.

## Dependances de story

Question:

```text
Quelles stories dependent de US-UI-003 ?
```

Strategie de recherche:

- rechercher `US-UI-003` dans `dependencies`;
- limiter aux documents de type `Story`;
- presenter les stories dependantes par statut.

## Accessibilite

Question:

```text
Quels composants sont compatibles WCAG 2.2 ?
```

Strategie de recherche:

- rechercher `WCAG 2.2`, `accessibility` et `accessibilite`;
- croiser avec les entites de composants;
- privilegier les stories et documents UX au statut `accepted`.

## Geolocalisation

Question:

```text
Quels ADR concernent la geolocalisation ?
```

Strategie de recherche:

- filtrer les documents de type `ADR`;
- rechercher les tags et entites lies a `geolocation`, `carte`, `distance`, `structures proches`;
- retourner la decision, les consequences et les stories liees.
