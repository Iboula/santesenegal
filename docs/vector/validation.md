---
id: BMAD-VEC-VALIDATION-001
title: Validation automatique du corpus documentaire
status: ready-for-review
tags:
  - bmad
  - vectorisation
  - validation
  - tooling
entities:
  - SanteSenegal.BmadValidator
  - Front Matter
  - Catalog
  - Relationships
created: 2026-08-16
updated: 2026-08-16
---

# Validation automatique du corpus documentaire

Le validateur `SanteSenegal.BmadValidator` controle localement la qualite du corpus documentaire BMAD avant commit, revue ou indexation RAG future.

Il ne cree aucune base vectorielle, ne genere aucun embedding et n'appelle aucun service externe.

## Role

Le validateur scanne les fichiers Markdown sous `docs/` et controle les documents vectorisables qui commencent par un front matter YAML.

Il verifie:

- la presence des delimiters `---`;
- le parsing du front matter YAML attendu;
- les metadonnees obligatoires;
- l'unicite des IDs;
- les statuts autorises;
- les tags non vides et en minuscules;
- les dependances vers des IDs existants;
- les chemins et IDs references par `docs/vector/catalog.md`;
- les IDs explicites mentionnes dans `docs/vector/relationships.md`.

## Commande locale

```bash
dotnet run --project tools/bmad-vector-validator
```

## Regles

Les statuts autorises sont:

- `draft`;
- `ready-for-review`;
- `accepted`;
- `deprecated`.

Les metadonnees obligatoires sont:

- `id`;
- `title`;
- `status`;
- `tags`;
- `entities`.

## Codes de sortie

- `0`: corpus valide;
- autre que `0`: corpus invalide.

## Sortie attendue

En succes:

```text
BMAD corpus validation

Documents: 21
IDs: 21
Broken dependencies: 0
Invalid metadata: 0
Invalid catalog entries: 0

VALID
```

En erreur, le validateur groupe les problemes par document:

```text
INVALID

US-UI-005
  - dependency US-UI-999 does not exist
```

## Integration CI future

Le validateur peut etre ajoute a un workflow CI avec:

```bash
dotnet build
dotnet test
dotnet run --project tools/bmad-vector-validator
```

Le code de sortie non nul bloque naturellement le pipeline si le corpus devient invalide.
