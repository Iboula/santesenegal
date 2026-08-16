---
id: UX-RESEARCH
title: SanteSenegal UX Research and Personas
status: draft
tags:
  - ux
  - research
  - personas
  - accessibility
entities:
  - Aminata
  - Dr Ndiaye
  - Patient
  - Rendez-vous
  - Mobile Money
created: 2026-08-13
updated: 2026-08-16
---

# SanteSenegal — UX Research & Personas

**Agent** : UX/UI  
**Date** : 2026-08-13  
**Version** : 1.0

---

## 1. Contexte utilisateur — Sénégal

### Réalités du terrain
| Facteur | Impact UX |
|---------|-----------|
| **Connectivité** | 4G instable en zone urbaine, 3G/EDGE en rural |
| **Appareils** | Android bas de gamme dominants (>80%) |
| **Digital literacy** | Variable — du très à l'aise au premier smartphone |
| **Langues** | Français (officiel), Wolof (langue véhiculaire), autres langues locales |
| **Paiement** | Mobile Money dominant (Wave, Orange Money, Free Money) |
| **Confiance numérique** | Méfiance initiale, besoin de réassurance forte |

### Contraintes UX déduites
- ✅ **Progressive Disclosure** : ne pas submerger l'utilisateur
- ✅ **Offline-first** : certaines actions doivent fonctionner sans connexion
- ✅ **Lightweight** : app < 15 Mo, chargement rapide
- ✅ **Large touch targets** : boutons > 48x48 dp
- ✅ **Haute lisibilité** : contrastes AAA, polices > 16px

---

## 2. Personas

### 👤 Persona 1 : Aminata — Patient citoyen

```
┌─────────────────────────────────────────────────────────────┐
│  Aminata Diallo, 34 ans                                     │
│  Mère de 2 enfants, vendeuse au marché Sandaga              │
│  Smartphone : Samsung Galaxy A13 (Android 13)               │
│  Connexion : 4G Orange, forfait 2 Go/mois                   │
│  Langue : Wolof (principale), Français (basique)            │
│  Compte Mobile Money : Orange Money (quotidien)             │
└─────────────────────────────────────────────────────────────┘
```

**Objectifs** :
- Prendre rendez-vous chez le pédiatre pour son fils sans se déplacer 3x
- Recevoir un rappel pour ne pas oublier le RDV
- Payer directement depuis son téléphone

**Frustrations** :
- Les applis trop compliquées avec trop de texte
- Ne pas comprendre pourquoi un paiement a échoué
- Avoir à se rendre physiquement pour un simple renouvellement

**Comportement** :
- Utilise WhatsApp quotidiennement
- Fait ses courses sur Jumia
- Appelle plutôt qu'elle n'écrit (préfère vocal)

**Besoins UX** :
- 🟢 Interface en Wolof possible
- 🟢 Parcours de prise de RDV en 3 étapes max
- 🟢 Confirmation vocale/SMS après réservation
- 🟢 Paiement Orange Money intégré en 1 clic

---

### 👤 Persona 2 : Dr. Ndiaye — Médecin structure

```
┌─────────────────────────────────────────────────────────────┐
│  Dr. Abdou Ndiaye, 45 ans                                   │
│  Médecin généraliste, Centre de Santé Liberté 6             │
│  Smartphone : iPhone 13                                     │
│  Connexion : Fibre + 4G Free                                │
│  Langue : Français (courant), Wolof (courant)               │
│  Outils : Ordinateur au cabinet, tablette pour déplacements │
└─────────────────────────────────────────────────────────────┘
```

**Objectifs** :
- Gérer son emploi du temps efficacement
- Voir les patients enregistrés pour la journée
- Réduire les absences (perte de revenus)

**Frustrations** :
- Les patients qui oublient leurs RDV (30% d'absence)
- Double réservation sur le même créneau
- Difficulté à communiquer les disponibilités

**Besoins UX** :
- 🟢 Vue agenda claire (jour/semaine)
- 🟢 Notifications push pour nouveaux RDV
- 🟢 Statistiques simples (taux de présence, revenus)
- 🟢 Blocage rapide d'un créneau (pause déjeuner, urgence)

---

### 👤 Persona 3 : Fatou — Administratrice structure

```
┌─────────────────────────────────────────────────────────────┐
│  Fatou Sall, 28 ans                                         │
│  Responsable admin, Clinique Niang                            │
│  Laptop : Dell Latitude (Windows 11)                        │
│  Connexion : Fibre                                          │
│  Langue : Français (courant), Anglais (notions)             │
└─────────────────────────────────────────────────────────────┘
```

**Objectifs** :
- Gérer les praticiens et leurs spécialités
- Superviser les revenus et les remboursements
- Configurer les créneaux de disponibilité

**Besoins UX** :
- 🟢 Dashboard avec KPIs clairs
- 🟢 Gestion des droits (qui peut quoi)
- 🟢 Export des données (Excel/PDF)
- 🟢 Gestion des tarifs par service

---

## 3. User Flows critiques

### Flow 1 : Prise de rendez-vous (Patient)

```
┌──────────┐    ┌─────────────┐    ┌──────────────┐    ┌─────────────┐    ┌──────────┐
│  Accueil │───▶│ Recherche   │───▶│ Choix        │───▶│ Choix       │───▶│  Paiement│
│          │    │ Structure   │    │ Service      │    │ Créneau     │    │          │
└──────────┘    └─────────────┘    └──────────────┘    └─────────────┘    └─────┬────┘
                                                                                 │
┌──────────┐    ┌─────────────┐    ┌──────────────┐                             │
│  Succès  │◀───│ Confirmation│◀───│ Paiement     │◀────────────────────────────┘
│  (SMS)   │    │ Récapitulatif│   │ Mobile Money │
└──────────┘    └─────────────┘    └──────────────┘

Métrique clé : < 3 minutes de bout en bout
```

### Flow 2 : Gestion disponibilités (Médecin)

```
┌──────────┐    ┌─────────────┐    ┌──────────────┐    ┌──────────┐
│  Login   │───▶│ Dashboard   │───▶│ Agenda       │───▶│  Créneau │
│          │    │ Médecin     │    │ Jour/Semaine │    │  +/-     │
└──────────┘    └─────────────┘    └──────────────┘    └──────────┘
```

---

## 4. Matrice de priorité UX

| Fonctionnalité | Impact utilisateur | Facilité technique | Priorité |
|----------------|-------------------|-------------------|----------|
| Prise RDV en 3 clics | ⭐⭐⭐⭐⭐ | ⭐⭐⭐ | P0 |
| Confirmation SMS | ⭐⭐⭐⭐⭐ | ⭐⭐⭐⭐ | P0 |
| Agenda médecin | ⭐⭐⭐⭐⭐ | ⭐⭐⭐ | P0 |
| Paiement intégré | ⭐⭐⭐⭐⭐ | ⭐⭐ | P0 |
| Support Wolof | ⭐⭐⭐⭐ | ⭐⭐ | P1 |
| Mode hors-ligne | ⭐⭐⭐⭐ | ⭐ | P1 |
| Dark mode | ⭐⭐ | ⭐⭐⭐ | P2 |

---

## 5. Principes UX définis

1. **Mobile First** : 80% des utilisateurs seront sur mobile
2. **3-Tap Rule** : Toute action critique en 3 taps maximum
3. **Fail Gracefully** : Messages d'erreur clairs avec solution
4. **Trust Signals** : Badges vérifiés, avis patients, photos réelles
5. **Voice Friendly** : Support vocal pour les utilisateurs peu à l'aise avec le texte

---

**Prochain livrable** : Wireframes des écrans clés
