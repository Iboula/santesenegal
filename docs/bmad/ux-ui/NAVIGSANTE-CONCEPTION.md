---
id: UX-NAVIGSANTE-CONCEPTION
title: NavigSante Module d'Information et Orientation Sante
status: draft
tags:
  - ux
  - orientation
  - triage
  - geolocation
  - alerts
entities:
  - NavigSante
  - TriageEngine
  - Cartographe
  - Assistant
  - Alertes
created: 2026-08-13
updated: 2026-08-16
---

# NavigSante — Module d'Information et Orientation Santé

**Agent** : PM + Architect + UX/UI  
**Date** : 2026-08-13  
**Vision** : "Chaque Sénégalais, partout, tout le temps, informé et orienté en santé"

---

## 1. Problématique au Sénégal

| Problème | Impact |
|----------|--------|
| **Saturations des urgences** | 70% des visites aux urgences ne sont pas des urgences réelles |
| **Manque d'information** | Les patients ne savent pas où aller selon leurs symptômes |
| **Déserts médicaux** | Difficulté à trouver un spécialiste dans certaines régions |
| **Rumeurs sanitaires** | Fausses informations sur les maladies (ex: COVID, Ebola) |
| **Non-respect des traitements** | Mauvaise compréhension des ordonnances |
| **Alertes tardives** | Les épidémies sont détectées trop tard |

---

## 2. Vision NavigSante

```
┌─────────────────────────────────────────────────────────────────────┐
│                         NAVIGSANTE                                  │
│                                                                     │
│   🏥 ORIENTEUR          📍 CARTOGRAPHE         🤖 ASSISTANT        │
│                                                                     │
│   "Où dois-je aller ?"  "Quelle structure      "Je tousse,         │
│                         a des lits ?"          que faire ?"         │
│                                                                     │
├─────────────────────────────────────────────────────────────────────┤
│                                                                     │
│   📡 SURVEILLANCE        📢 ALERTES           📚 INFO-PUB          │
│                                                                     │
│   "Fievre jaune        "Epidemie de          "Campagne             │
│    à Kédougou"          cholera à          de vaccination         │
│                           Touba"             PMR en cours"          │
│                                                                     │
└─────────────────────────────────────────────────────────────────────┘
```

---

## 3. Composants du module

### 3.1 🏥 Moteur de Triage (TriageEngine)
**Objectif** : Orienter le patient vers le bon niveau de soins

**Algorithme** :
```
Patient décrit symptômes
        │
        ▼
┌───────────────────┐
│ 1. CRITIQUE ?     │───▶ Urgence immédiate (15/SAMU)
│ (respiration,     │     "Appelez le 15 maintenant !"
│  conscience,      │
│  hémorragie)      │
└───────────────────┘
        │ Non
        ▼
┌───────────────────┐
│ 2. URGENT ?       │───▶ Urgence relative (structure 24h)
│ (fièvre + enfant, │     "Rendez-vous dans les 4h"
│  douleur aiguë,   │
│  trauma)          │
└───────────────────┘
        │ Non
        ▼
┌───────────────────┐
│ 3. STANDARD ?     │───▶ Consultation programmée
│ (suivi, renouvel. │     "Prenez RDV dans la semaine"
│  ordonnance)      │
└───────────────────┘
        │ Non
        ▼
┌───────────────────┐
│ 4. CONSEIL        │───▶ Auto-soin / Pharmacie
│ (petit rhume,     │     "Reposez-vous, hydratez-vous"
│  griffure légère) │
└───────────────────┘
```

**Niveaux de soins au Sénégal** :
1. **Poste de santé** (soins de base, vaccination)
2. **Centre de santé** (consultations générales, petite chirurgie)
3. **Hôpital régional** (spécialités, urgences 24h)
4. **CHU / Cliniques** (haute spécialisation)
5. **Pharmacie** (conseil, médicaments sans ordonnance)

---

### 3.2 📍 Cartographie Sanitaire Temps Réel
**Objectif** : Savoir QUELLE structure a QUOI en temps réel

**Données collectées** :
- ✅ Lits disponibles (réanimation, hospitalisation, maternité)
- ✅ Stock de sang par groupe
- ✅ Médicaments essentiels en stock
- ✅ Spécialistes présents aujourd'hui
- ✅ Équipements fonctionnels (scanner, radio, labo)
- ✅ Temps d'attente estimé aux urgences

**Exemple d'utilisation** :
```
👤 "Ma femme est enceinte, elle a des contractions. 
     Où puis-je aller ?"

🤖 "Voici les structures avec maternité ouverte
    dans un rayon de 10 km :"

1. 🏥 Hôpital Principal (4 km)
   ✅ Maternité ouverte 24h
   ✅ 3 lits disponibles
   ✅ Salle d'accouchement libre
   ⏱️ Temps d'attente : 15 min
   📞 33 839 50 50

2. 🏥 Clinique Niang (8 km)
   ✅ Maternité ouverte
   ⚠️ 1 lit disponible
   ⏱️ Temps d'attente : 30 min
   📞 77 123 45 67
```

---

### 3.3 🤖 Assistant Santé Virtuel (SanteBot)
**Objectif** : Répondre aux questions santé basiques 24/7

**Canaux** :
- 💬 Chat dans l'app
- 📱 WhatsApp Business API
- 📲 SMS (pour les téléphones basiques)
- ☎️ Vocal (futur : assistant vocal Wolof/Français)

**Exemples d'interaction** :
```
👤 : "Mon bébé de 6 mois a de la fièvre à 38.5°C"
🤖 : "La fièvre chez un nourrisson nécessite une 
      consultation rapide. Ne donnez pas d'aspirine.
      Vous pouvez donner du paracétamol si vous en avez.
      
      Structures pédiatriques ouvertes près de vous :"
      [Liste avec distance]
      
      ⚠️ Allez aux urgences SI :
      - Fièvre > 39°C
      - Bébé refuse de manger
      - Bébé est très somnolent
      - Taches sur la peau
```

---

### 3.4 📡 Surveillance Épidémiologique Communautaire
**Objectif** : Détecter les épidémies avant qu'elles ne se propagent

**Mécanisme** :
```
┌─────────────┐     ┌─────────────┐     ┌─────────────┐
│ Agents de   │────▶│ Signalement │────▶│ Vérification│
│ santé       │     │ dans app    │     │ par labo    │
│ communautaire│    │             │     │             │
└─────────────┘     └─────────────┘     └──────┬──────┘
                                               │
                                        ┌──────┴──────┐
                                        │  Alerte     │
                                        │  confirmée  │
                                        └──────┬──────┘
                                               │
              ┌────────────────────────────────┼────────────────────────┐
              │                                │                        │
              ▼                                ▼                        ▼
        ┌──────────┐                    ┌──────────┐              ┌──────────┐
        │ Ministère│                    │ Populat. │              │ OMS/     │
        │ Santé    │                    │ (SMS)    │              │ Partenaires│
        └──────────┘                    └──────────┘              └──────────┘
```

**Maladies surveillées** (prioritaires Sénégal) :
- Paludisme (toute l'année, pics saisonniers)
- Fièvre jaune (zones forestières)
- Choléra (saison des pluies)
- Méningite (saison chaude)
- COVID-19 (veille permanente)
- Rougeole
- Fièvre hémorragique virale

---

### 3.5 📚 Hub Info-Santé Publique
**Objectif** : Centraliser l'information santé officielle

**Contenus** :
- 📅 Calendrier vaccinal (PNExpanded Programme on Immunization)
- 🩺 Conseils de prévention (paludisme, hygiene, nutrition)
- 📢 Campagnes en cours (vaccination, dépistage)
- 📊 Statistiques santé (taux de mortalité, couverture vaccinale)
- 🏥 Annuaire sanitaire (toutes les structures du Sénégal)
- 💊 Liste des médicaments essentiels

---

## 4. Architecture du module

```
┌─────────────────────────────────────────────────────────────────────┐
│                         NAVIGSANTE MODULE                           │
├─────────────────────────────────────────────────────────────────────┤
│                                                                     │
│  ┌─────────────┐  ┌─────────────┐  ┌─────────────┐  ┌───────────┐ │
│  │ TriageEngine│  │ Cartographie│  │ SanteBot    │  │ Surveillance│ │
│  │  Service    │  │  Service    │  │  Service    │  │  Service   │ │
│  └──────┬──────┘  └──────┬──────┘  └──────┬──────┘  └─────┬─────┘ │
│         │                │                │               │       │
│         └────────────────┴────────────────┴───────────────┘       │
│                              │                                     │
│                    ┌─────────┴─────────┐                          │
│                    │  NavigSanteDb     │                          │
│                    │  (SQLite/PostgreSQL)│                          │
│                    └───────────────────┘                          │
│                                                                     │
├─────────────────────────────────────────────────────────────────────┤
│  ENTITIES :                                                         │
│  - Symptome, Pathologie, Orientation, Recommandation                │
│  - RessourceSanitaire (lits, sang, médicaments)                    │
│  - AlerteEpidemiologique, SignalementCommunautaire                  │
│  - ArticleInfoSante, CampagneSante, FAQ                             │
│                                                                     │
│  SERVICES :                                                         │
│  - ITriageService, ICartographieService, ISanteBotService           │
│  - ISurveillanceService, IInfoSanteService                          │
│                                                                     │
└─────────────────────────────────────────────────────────────────────┘
```

---

## 5. User Stories

| ID | Story | Priorité |
|----|-------|----------|
| NAV-001 | En tant que patient, je veux décrire mes symptômes pour savoir où aller | P0 |
| NAV-002 | En tant que patient, je veux voir les structures ouvertes avec lits disponibles | P0 |
| NAV-003 | En tant qu'agent de santé communautaire, je veux signaler des cas suspects | P0 |
| NAV-004 | En tant que citoyen, je veux recevoir des alertes épidémiologiques | P1 |
| NAV-005 | En tant que patient, je veux discuter avec un assistant santé virtuel | P1 |
| NAV-006 | En tant qu'admin, je veux voir le tableau de bord épidémiologique | P1 |
| NAV-007 | En tant que citoyen, je veux consulter le calendrier vaccinal | P2 |
| NAV-008 | En tant que structure, je veux mettre à jour mes ressources en temps réel | P2 |

---

## 6. Innovation pour le Sénégal

### 🌍 Ce qui n'existe pas encore
1. **Triage intelligent en Wolof** : Premier moteur de triage symptomatique en langue locale
2. **Cartographie des ressources en temps réel** : Aucun système ne montre les lits disponibles en live
3. **Surveillance communautaire digitale** : Remplacer les rapports papier des ASC
4. **Alertes géolocalisées** : SMS ciblé par zone géographique

### 🚀 Impact attendu
- Réduction de 30% des visites non-urgentes aux urgences
- Détection des épidémies 7-10 jours plus tôt
- Meilleure répartition des patients entre structures
- Réduction des déserts médicaux informationnels

---

*Document de conception — Module NavigSante*
