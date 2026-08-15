# Backlog BMAD - SanteSenegal

Date de cadrage: 2026-08-15
Source produit: `prd/PRD.md`
Source architecture: `architecture/ARCHITECTURE.md`

## Etat produit

SanteSenegal couvre deja plusieurs blocs du MVP:

- authentification JWT;
- structures, services, sous-services, disponibilites;
- rendez-vous;
- paiements mobiles;
- notifications;
- module NavigSante avec triage, ressources sanitaires, accidents et temps reel;
- web Blazor, API ASP.NET Core, infrastructure EF Core, mobile MAUI, scene Godot.

## Story map

| Epic | Objectif | Stories existantes | Etat |
|---|---|---|---|
| EPIC-FOUND | Fondations techniques et securite | `FOUND-001-authentification-jwt.md` | A verifier contre le code |
| EPIC-STAB | Stabilisation Clean Architecture / EF / DTO | `STAB-001` a `STAB-006` | Rapport indique termine, sprint plan obsolescent |
| EPIC-NAVIG | NavigSante, triage, alertes accident | `ALERTE-ACCIDENT-*`, rapports finaux | A consolider en stories dev-ready |
| EPIC-RDV | Recherche, disponibilites, prise de RDV | PRD P0 | Stories detaillees manquantes |
| EPIC-PAY | Paiement mobile et remboursement | PRD P0 + SQL ajout remboursement | Stories detaillees manquantes |
| EPIC-NOTIF | SMS, rappels, outbox | PRD P1 + services existants | Stories detaillees manquantes |
| EPIC-UX | Portail patient/structure | UX docs + Blazor | Stories ecran par ecran manquantes |
| EPIC-MOBILE | App mobile terrain | MAUI + offline accident | Stories detaillees manquantes |

## Prochaines stories recommandees

| ID | Titre | Statut | Pourquoi maintenant |
|---|---|---|---|
| BMAD-001 | Assainir le pilotage BMAD et verifier backend | Done | Structure creee dans cette passe |
| QA-001 | Build et test backend cible | Ready | Le build complet timeout; il faut isoler API/Application/Infrastructure/Tests |
| WEB-001 | Debloquer le build Blazor | Blocked | DLL verrouillee par Visual Studio Insiders |
| RDV-001 | Parcours prise de rendez-vous bout en bout | Ready | Coeur MVP selon PRD P0 |
| PAY-001 | Verifier paiement mobile + remboursement | Ready | Risque metier et financier |
| NAV-001 | Re-decouper NavigSante en stories dev-ready | Ready | Les rapports existent mais peu exploitables par iteration |
| UX-001 | Aligner Blazor sur le design system BMAD | Backlog | Ameliore la continuite produit |
| OPS-001 | Initialiser Git et nettoyer artefacts `bin/obj` | Ready | Base saine avant refactoring important |

## Definition of Ready

Une story est `Ready` si elle contient:

- lien vers exigence PRD ou justification brownfield;
- contexte architecture ou ADR;
- fichiers touches probables;
- criteres d'acceptation testables;
- plan de verification;
- risques et rollback;
- statut initial.

## Definition of Done

Une story est `Done` si:

- les criteres d'acceptation sont coches;
- les tests pertinents sont passes ou l'ecart est documente;
- la story contient une note de completion;
- les docs impactees sont mises a jour;
- aucun secret ni artefact build inutile n'a ete ajoute.

## Traceabilite PRD -> implementation

| PRD ID | Besoin | Code/docs existants | Gap |
|---|---|---|---|
| US-P0-001 | Authentification patient/admin | `AuthEndpoints`, `AuthService`, `JwtService`, tests auth | Verifier roles et refresh tokens |
| US-P0-002 | Recherche structure | `StructureEndpoints`, queries structures | Tester filtres et UX |
| US-P0-003 | Prise RDV | `RendezVousEndpoints`, handlers RDV | Verifier verrouillage creneau |
| US-P0-004 | Gestion disponibilites | `DisponibiliteEndpoints`, handlers dispo | Verifier droits medecin/admin |
| US-P0-005 | Paiement consultation | services paiement mobile | Verifier providers et remboursements |
| US-P1-001 | SMS rappel | services notifications/SMS | Planifier worker/outbox |
| US-P1-002 | Annulation/report | endpoints RDV + paiement | Story manquante |
| US-P1-003 | Favoris structures | handlers favorites | Tester UX et API |
| US-P1-004 | Agenda structure | non clair | Story a creer |
