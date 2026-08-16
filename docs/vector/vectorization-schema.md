---
id: BMAD-VEC-SCHEMA-001
title: Schema de vectorisation documentaire
status: accepted
tags:
  - bmad
  - schema
  - vectorisation
  - metadata
entities:
  - Vision
  - Epic
  - Story
  - ADR
  - SPEC
  - UX
  - Test
---

# Schema de vectorisation documentaire

Tous les documents vectorisables doivent commencer par un en-tete YAML.

## Types de documents

Les types supportes sont:

- `Vision`: vision produit, metier ou strategique;
- `Epic`: regroupement fonctionnel de stories;
- `Story`: besoin utilisateur ou technique suivi dans BMAD;
- `ADR`: decision d'architecture;
- `SPEC`: specification fonctionnelle ou technique;
- `UX`: experience utilisateur, accessibilite, design system, parcours;
- `Test`: strategie, plan, cas ou resultat de validation.

## Metadonnees obligatoires

- `id`: identifiant stable et unique du document;
- `title`: titre court et explicite;
- `status`: statut du document;
- `tags`: mots-cles normalises;
- `entities`: objets, modules, composants ou concepts cites.

## Metadonnees optionnelles

- `epic`: epic rattachee au document;
- `dependencies`: documents ou stories requis avant lecture ou implementation;
- `authors`: auteurs ou responsables;
- `created`: date de creation au format `YYYY-MM-DD`;
- `updated`: date de derniere mise a jour au format `YYYY-MM-DD`.

## Exemple

```yaml
---
id: US-UI-003
epic: EPIC-011
title: Design System
status: accepted
dependencies:
  - US-UI-001
  - US-UI-002
tags:
  - frontend
  - ui
  - accessibility
  - design-system
entities:
  - FacilityCard
  - StatusBadge
  - PrimaryButton
  - SecondaryButton
---
```

## Regles de nommage

- utiliser des identifiants stables: `US-UI-003`, `ADR-001`, `SPEC-TRIAGE-001`;
- conserver les tags en minuscules;
- preferer les entites nommees comme dans le code ou la documentation;
- ne pas changer un `id` apres publication.
