---
id: BMAD-VEC-001
title: Base documentaire vectorisable
status: accepted
tags:
  - bmad
  - vectorisation
  - rag
  - documentation
entities:
  - Vision
  - Epic
  - Story
  - ADR
  - SPEC
  - UX
  - Test
---

# Base documentaire vectorisable

Ce repertoire definit la convention documentaire pour rendre les artefacts BMAD de Sante Senegal indexables par un moteur RAG.

La methode cible le flux suivant:

```text
BMAD -> SPEC -> VECTORISATION -> IMPLEMENTATION -> REVIEW -> MERGE
```

## Objectifs

- standardiser les documents avec un en-tete YAML;
- rendre les artefacts retrouvables par type, statut, tag, entite et dependance;
- faciliter le chunking et la recherche semantique;
- conserver les liens entre vision, epics, stories, ADR, specs, UX et tests;
- rester independant de tout fournisseur ou moteur vectoriel.

## Documents de reference

- `vectorization-schema.md`: schema documentaire et metadonnees;
- `indexing-guidelines.md`: regles de chunking, embeddings et recuperation;
- `query-examples.md`: exemples de requetes RAG.
