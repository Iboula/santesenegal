---
id: BMAD-VEC-CATALOG-001
title: Catalogue documentaire vectorisable
status: ready-for-review
tags:
  - bmad
  - vectorisation
  - catalog
  - rag
entities:
  - US-UI-001
  - US-UI-002
  - US-UI-003
  - UX-DESIGN-SYSTEM
  - UX-WIREFRAMES
  - UX-RESEARCH
  - UX-NAVIGSANTE-CONCEPTION
  - BMAD-AGENTS-001
created: 2026-08-16
updated: 2026-08-16
---

# Catalogue documentaire vectorisable

Ce catalogue liste les documents migres dans le cadre de `BMAD-VEC-002`.

| ID | Type | Titre | Statut | Document | Dependances principales |
| --- | --- | --- | --- | --- | --- |
| US-UI-001 | Story | Refonte complete de la page Home | accepted | `docs/stories/US-UI-001-refonte-home.md` | - |
| US-UI-002 | Story | Refonte de la navigation publique | ready-for-review | `docs/stories/US-UI-002-refonte-navigation-publique.md` | US-UI-001 |
| US-UI-003 | Story | Design System | ready-for-review | `docs/stories/US-UI-003-design-system.md` | US-UI-001, US-UI-002 |
| UX-RESEARCH | UX | SanteSenegal UX Research and Personas | draft | `docs/bmad/ux-ui/UX_RESEARCH.md` | - |
| UX-WIREFRAMES | UX | SanteSenegal Wireframes | draft | `docs/bmad/ux-ui/WIREFRAMES.md` | - |
| UX-DESIGN-SYSTEM | UX | SanteSenegal Design System | draft | `docs/bmad/ux-ui/DESIGN_SYSTEM.md` | UX-RESEARCH, UX-WIREFRAMES |
| UX-NAVIGSANTE-CONCEPTION | UX | NavigSante Module d'Information et Orientation Sante | draft | `docs/bmad/ux-ui/NAVIGSANTE-CONCEPTION.md` | - |
| BMAD-AGENTS-README-001 | Agent | Index des agents BMAD | accepted | `docs/agents/README.md` | - |
| BMAD-AGENTS-001 | Agent | Catalogue des agents BMAD | accepted | `docs/agents/agents.md` | BMAD-AGENTS-README-001 |
| BMAD-AGENTS-WORKFLOW-001 | Agent | Workflow des agents BMAD | accepted | `docs/agents/workflow.md` | BMAD-AGENTS-001 |
| BMAD-AGENTS-RESPONSIBILITIES-001 | Agent | Matrice des responsabilites des agents BMAD | accepted | `docs/agents/responsibilities.md` | BMAD-AGENTS-001 |

## Notes d'indexation

- Les stories `US-UI-001`, `US-UI-002` et `US-UI-003` constituent la premiere chaine de dependances UI.
- Les documents UX sont conserves dans leur emplacement historique `docs/bmad/ux-ui` et enrichis par front matter.
- Aucun ADR ou SPEC specifique aux stories UI n'a ete cree artificiellement.
- Les documents agents officialisent les roles nommes du workflow BMAD.
