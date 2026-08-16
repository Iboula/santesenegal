---
id: BMAD-VEC-GUIDE-001
title: Guide d'indexation RAG
status: accepted
tags:
  - bmad
  - rag
  - indexing
  - semantic-search
entities:
  - Chunking
  - Embeddings
  - Retrieval
  - Story
  - ADR
---

# Guide d'indexation RAG

Ce guide decrit comment transformer les documents BMAD en contenu exploitable par recherche semantique.

Il ne prescrit aucune base vectorielle, dependance externe ou fournisseur.

## Chunking

Chaque document doit etre decoupe en chunks autonomes.

Regles recommandees:

- conserver l'en-tete YAML avec chaque chunk sous forme de metadonnees;
- decouper par section Markdown de niveau `##` quand c'est possible;
- garder ensemble les listes de criteres, contraintes et validations;
- eviter de melanger plusieurs stories ou ADR dans un meme chunk;
- conserver les titres de section pour enrichir le contexte.

## Embeddings

Les embeddings doivent etre generes a partir du contenu utile et des metadonnees essentielles.

Champs a inclure dans le texte indexe:

- `id`;
- `title`;
- `status`;
- `tags`;
- `entities`;
- titre de section;
- corps du chunk.

Champs a conserver comme filtres:

- type de document;
- epic;
- dependencies;
- status;
- tags;
- entities.

## Recuperation contextuelle

La recuperation doit combiner recherche semantique et filtres metadonnees.

Exemples:

- filtrer `entities: FacilityCard` avant de chercher les usages;
- filtrer `status: accepted` pour eviter les documents brouillons;
- etendre les resultats avec les documents declares dans `dependencies`;
- ajouter les ADR lies quand une question concerne une decision technique.

## Recherche semantique

La recherche semantique doit repondre aux formulations naturelles, meme quand les mots exacts different.

Exemples de rapprochements attendus:

- "carte structure" peut retrouver `FacilityCard`;
- "accessibilite" peut retrouver `WCAG 2.2`;
- "urgence" peut retrouver `SOS`, `Triage` ou `StatusBadge` selon le contexte.

## Liens entre stories

Les stories doivent declarer leurs dependances dans `dependencies`.

Pour une question sur l'impact d'une story:

- retrouver la story par `id`;
- rechercher les documents qui citent cet `id` dans `dependencies`;
- inclure les epics et specs qui partagent les memes tags ou entites.

## Liens entre ADR

Les ADR doivent citer les stories, specs ou modules concernes dans `entities` ou `dependencies`.

Pour une question d'architecture:

- rechercher les ADR par tags et entites;
- recuperer les stories liees;
- presenter la decision et ses consequences avec le contexte metier.
