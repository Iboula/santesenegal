# SanteSenegal — Product Requirements Document (PRD)

**Version** : 1.0  
**Date** : 2026-08-13  
**Auteur** : BMad Agent PM  
**Statut** : Draft → Review

---

## 1. Vision & Objectif

### 1.1 Problème
Au Sénégal, la prise de rendez-vous médicaux est souvent manuelle (téléphone, déplacement physique). Les patients perdent du temps, les structures de santé ont des taux d'absence élevés, et il n'existe pas de plateforme centralisée pour consulter la disponibilité des services de santé.

### 1.2 Solution
**SanteSenegal** est une plateforme numérique qui permet :
- Aux **patients** de trouver des structures de santé, consulter leurs services, voir les disponibilités en temps réel, et prendre rendez-vous en ligne.
- Aux **structures de santé** de gérer leurs services, leurs disponibilités, et leurs rendez-vous.
- Aux **administrateurs** de superviser la plateforme.

### 1.3 Public cible
| Persona | Besoin principal |
|---------|------------------|
| Patient citoyen | Prendre RDV facilement, recevoir des rappels |
| Médecin / Personnel soignant | Gérer son planning de consultations |
| Administrateur structure | Gérer les services, disponibilités, et personnels |
| Admin plateforme | Superviser les transactions et la santé du système |

---

## 2. User Stories — Backlog priorisé

### 🚨 P0 — Critique (lancement MVP)
| ID | Story | Acceptance Criteria |
|----|-------|---------------------|
| US-P0-001 | En tant que patient, je veux créer un compte et m'authentifier | JWT valide, rôles Patient/Admin, refresh token |
| US-P0-002 | En tant que patient, je veux rechercher une structure de santé | Filtres par type, localisation, spécialité |
| US-P0-003 | En tant que patient, je veux prendre un rendez-vous | Créneau verrouillé, confirmation immédiate, numéro de référence |
| US-P0-004 | En tant que structure, je veux gérer mes disponibilités | CRUD créneaux, visibilité publique/privée |
| US-P0-005 | En tant que patient, je veux payer ma consultation en ligne | Intégration Wave/Orange Money/Free Money, confirmation |

### 🟡 P1 — Important (v1.1)
| ID | Story | Acceptance Criteria |
|----|-------|---------------------|
| US-P1-001 | En tant que patient, je veux recevoir un SMS de rappel 24h avant mon RDV | Envoi automatique, statut livré/échoué |
| US-P1-002 | En tant que patient, je veux annuler ou reporter mon RDV | Annulation jusqu'à 2h avant, notification structure |
| US-P1-003 | En tant que patient, je veux sauvegarder mes structures favorites | Liste persistante, accès rapide |
| US-P1-004 | En tant que structure, je veux consulter mon agenda | Vue jour/semaine, filtres par praticien |

### 🟢 P2 — Amélioration (v1.2)
| ID | Story | Acceptance Criteria |
|----|-------|---------------------|
| US-P2-001 | En tant qu'admin, je veux un dashboard avec KPIs | Taux de présence, revenus, top structures |
| US-P2-002 | En tant que patient, je veux évaluer ma consultation | Note 1-5, commentaire optionnel |
| US-P2-003 | En tant que patient, je veux recevoir des recommandations | Basées sur l'historique et la localisation |

---

## 3. Spécifications non-fonctionnelles

| Critère | Exigence |
|---------|----------|
| **Performance** | Temps de réponse API < 200ms (p95) |
| **Disponibilité** | 99.5% uptime |
| **Sécurité** | JWT RS256, HTTPS obligatoire, données santé chiffrées |
| **RGPD/Sénégal** | Conformité à la loi sur la protection des données personnelles |
| **Localisation** | Français (principal), Wolof, Anglais |
| **Paiement** | Wave, Orange Money, Free Money |
| **SMS** | API Twilio ou équivalent local |

---

## 4. Règles métier (Business Rules)

### 4.1 Rendez-vous
- Un créneau de disponibilité a une capacité maximale (`MaxRendezVous`)
- Un patient ne peut pas avoir 2 RDV simultanés
- Annulation gratuite jusqu'à 2h avant le RDV
- Passé 2h, le paiement est conservé (sauf cas exceptionnel)

### 4.2 Paiement
- Le paiement est requis pour confirmer le RDV
- Si le paiement échoue, le RDV reste en statut "EnAttente" pendant 15 min
- Remboursement automatique en cas d'annulation > 2h

### 4.3 Disponibilités
- Une structure définit ses disponibilités par sous-service
- Les disponibilités peuvent être récurrentes (chaque semaine)
- Jours fériés : pas de disponibilités par défaut

---

## 5. KPIs Produit

| KPI | Cible | Mesure |
|-----|-------|--------|
| Taux de conversion (recherche → RDV) | > 15% | Analytics |
| Taux d'absence | < 10% | Comparé à ~25% actuel |
| Temps moyen de prise de RDV | < 3 min | Session tracking |
| NPS (Net Promoter Score) | > 50 | Enquête post-consultation |

---

## 6. Roadmap

```
Mois 1-2 : MVP (P0)
  ├── Authentification JWT
  ├── CRUD Structures/Services/Disponibilités
  ├── Prise de RDV + Paiement mobile
  └── API documentée (Swagger)

Mois 3 : v1.1 (P1)
  ├── Notifications SMS
  ├── Annulation/Report
  ├── Favoris
  └── Agenda structure

Mois 4-6 : v1.2 (P2)
  ├── Dashboard Admin
  ├── Avis patients
  ├── Recommandations ML
  └── Mobile App (MAUI/Flutter)
```

---

## 7. Hypothèses & Risques produit

| Hypothèse | Validation | Plan B |
|-----------|-----------|--------|
| Les Sénégalais veulent prendre RDV en ligne | Enquête pilote dans 3 structures | Pivot vers agenda interne pour structures |
| Les paiements mobiles sont suffisants | Taux d'adoption > 60% | Ajout paiement carte bancaire |
| Les structures ont un accès Internet fiable | Test avec 5 structures rurales | Mode offline/sync pour l'app structure |

---

**Prochaine étape** : Passer à l'**Agent Architect** pour les décisions techniques et l'ADR.
