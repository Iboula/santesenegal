---
id: UX-US-UI-004
title: Wireframes recherche des établissements
status: ready-for-review
dependencies:
  - US-UI-004
tags:
  - ux
  - mobile-first
  - facilities
  - search
  - accessibility
entities:
  - FacilityCard
  - SearchFilters
  - SearchResults
  - SearchBar
---

# Wireframes recherche des établissements

Ce document formalise les décisions UX d'Aïssatou pour `US-UI-004`.

## Variante A — Recherche guidée

Que recherchez-vous ?

- Un médecin
- Un symptôme
- Une pharmacie
- Un établissement

## Variante B — Recherche libre

Champ principal :

```text
Médecin, symptôme, service, établissement...
```

Filtres :

- distance ;
- spécialité ;
- type d'établissement ;
- disponibilité.

## Variante C — Résultats

Exemple :

```text
Hôpital Principal
Dakar
Ouvert
Maternité
15 min d'attente
2,4 km
Action : Consulter
```

## Décision UX

Approche hybride :

- recherche guidée pour les nouveaux utilisateurs ;
- recherche libre pour les utilisateurs expérimentés ;
- résultats sous forme de cartes.

## Mobile

Les filtres doivent être masqués par défaut et ouverts via une action "Filtres".

## États

- Idle
- Searching
- ResultsFound
- NoResults
- Error

## Accessibilité

- WCAG 2.2 AA ;
- focus visible ;
- navigation clavier ;
- zones tactiles >= 44 px ;
- français / wolof préparé ;
- usage acceptable sur connexion lente.
