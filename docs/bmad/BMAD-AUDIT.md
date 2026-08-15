# Audit BMAD - SanteSenegal

Date: 2026-08-15
Projet audite: `C:\Users\iboul\Documents\PROJETS\SANTESENEGAL\SanteSenegal`

## Conclusion

Le projet ne part pas de zero: il contient deja des artefacts BMAD utiles (`PRD`, `ARCHITECTURE`, stories, UX, rapports). Il n'etait toutefois pas pleinement exploitable comme framework BMAD continu, car les documents etaient disperses, certains statuts etaient obsoletes, et la trace entre besoins, architecture, stories et tests n'etait pas formalisee.

Verdict: **partiellement conforme BMAD**.

Action prise: ajout d'une couche de gouvernance BMAD locale pour permettre de continuer le projet proprement.

## Matrice de conformite

| Domaine BMAD | Etat observe | Conformite | Action |
|---|---|---:|---|
| Product context | `docs/bmad/prd/PRD.md` existe | Moyen | Ajouter trace vers epics/stories dans `BACKLOG.md` |
| Architecture context | `docs/bmad/architecture/ARCHITECTURE.md` existe | Moyen | Ajouter index ADR et decisions futures |
| UX context | `docs/bmad/ux-ui/*` existe | Bon | Garder comme source UX pour les stories web/mobile |
| Story context | Plusieurs stories existent | Moyen | Normaliser via template et statuts |
| QA context | Dossier `tests` vide | Faible | Ajouter strategie QA et template de plan de test |
| ADR | Dossier `adr` vide | Faible | Ajouter index ADR |
| Backlog courant | Pas de fichier unique | Faible | Ajouter `BACKLOG.md` |
| Continuation | Pas de README BMAD | Faible | Ajouter `README.md` |
| Outillage officiel | Pas de `.bmad-core` installe | Faible | Garder docs locales; installation officielle possible plus tard |
| Git | Pas de depot Git detecte | Risque | Initialiser Git avant gros refactoring code |

## Points a corriger avant nouvelles grosses features

1. Initialiser ou rattacher le projet a un depot Git.
2. Rejouer un build cible backend, puis noter le resultat dans `BACKLOG.md`.
3. Mettre a jour les anciennes stories `STAB-*` qui sont encore marquees TODO dans le sprint plan.
4. Decider si l'installation officielle BMAD (`npx bmad-method install`) doit etre faite plus tard, dans une session dediee.
5. Ajouter une story par prochain increment, plutot que coder depuis un rapport global.

## Risques principaux

| Risque | Impact | Mitigation BMAD |
|---|---|---|
| Pas de Git | Impossible de revenir proprement apres refactor | Initialiser Git avant refactoring |
| Build complet lent ou bloque par MAUI | Feedback dev trop long | Separer checks backend, web, mobile |
| Statuts docs incoherents | Mauvaise priorisation | Maintenir `BACKLOG.md` comme source de pilotage |
| PRD et code divergent | Features hors besoin | Lier chaque story a PRD + architecture |
| Tests insuffisants pour sante/paiement | Regression metier | Appliquer `tests/QA-STRATEGY.md` |

