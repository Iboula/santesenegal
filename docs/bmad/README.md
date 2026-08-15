# BMAD Framework - SanteSenegal

Ce dossier est le centre de pilotage BMAD du projet SanteSenegal.

BMAD garde les decisions produit et techniques explicites, puis les transforme en stories implementees une par une avec contexte, criteres d'acceptation et verification. Pour un projet existant comme SanteSenegal, le framework local doit permettre de reprendre le travail sans redecouvrir le projet a chaque session.

## Etat rapide

Statut global: **partiellement conforme, maintenant restructure pour continuer**.

Le projet avait deja un PRD, une architecture, des stories et des rapports. Il manquait surtout:

- un index de navigation BMAD;
- un audit de conformite;
- une trace PRD -> architecture -> stories;
- des templates pour produire des stories homogenes;
- une strategie QA;
- un backlog courant qui dit quoi faire ensuite;
- des statuts coherents entre stories et rapports.

## Structure

```text
docs/bmad/
  README.md
  BMAD-AUDIT.md
  BACKLOG.md
  prd/
    PRD.md
  architecture/
    ARCHITECTURE.md
  adr/
    ADR-000-index.md
  stories/
    *.md
  tests/
    QA-STRATEGY.md
  templates/
    STORY_TEMPLATE.md
    ADR_TEMPLATE.md
    TEST_PLAN_TEMPLATE.md
  ux-ui/
    *.md
```

## Workflow recommande

1. **Analyst / PM**: maintenir le besoin dans `prd/PRD.md` et dans `BACKLOG.md`.
2. **Architect**: documenter les decisions dans `architecture/ARCHITECTURE.md` et `adr/`.
3. **Scrum Master**: creer une story depuis `templates/STORY_TEMPLATE.md`.
4. **Dev**: implementer une seule story a la fois, en mettant a jour son statut.
5. **QA**: verifier selon `tests/QA-STRATEGY.md`, puis consigner les resultats dans la story.

## Regle de continuation

Avant de coder une nouvelle feature:

- lire `BACKLOG.md`;
- choisir la premiere story en statut `Ready`;
- verifier que les liens PRD, architecture et tests sont presents;
- mettre le statut a `In Progress`;
- implementer;
- executer les checks;
- mettre la story a `Done` ou `Blocked` avec une note claire.

