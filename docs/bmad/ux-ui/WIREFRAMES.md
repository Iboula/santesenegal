---
id: UX-WIREFRAMES
title: SanteSenegal Wireframes
status: draft
tags:
  - ux
  - ui
  - wireframes
  - mobile-first
entities:
  - Accueil Patient
  - SearchBar
  - CategoryChip
  - StructureCard
  - BottomNav
created: 2026-08-13
updated: 2026-08-16
---

# SanteSenegal — Wireframes

**Agent** : UX/UI  
**Date** : 2026-08-13  
**Format** : Wireframes textuels (ASCII) — Mobile first (375px)

---

## Écran 1 : Accueil Patient

```
┌─────────────────────────┐
│ ≡  SanteSenegal    🔔   │  ← Header (logo + notif)
├─────────────────────────┤
│                         │
│  Bonjour Aminata 👋     │  ← Personalisation
│  Comment allez-vous ?   │
│                         │
│  ┌───────────────────┐  │
│  │ 🔍 Rechercher une │  │  ← Barre de recherche
│  │    structure...   │  │
│  └───────────────────┘  │
│                         │
│  Prendre rendez-vous    │  ← Section principale
│                         │
│  ┌────┐ ┌────┐ ┌────┐   │
│  │ 🏥 │ │ 🩺 │ │ 🔬 │   │  ← Filtres rapides
│  │Hôpi│ │Cons│ │Anal│   │
│  └────┘ └────┘ └────┘   │
│                         │
│  Structures proches     │  ← Section géolocalisée
│  ┌───────────────────┐  │
│  │ 🏥 Clinique Niang   │  ← Card structure
│  │ 📍 Médina, Dakar    │
│  │ ⭐ 4.8 · 2 km       │
│  │ Pédiatrie, Général  │
│  └───────────────────┘  │
│  ┌───────────────────┐  │
│  │ 🏥 Centre Santé L6  │  ← Card structure
│  │ 📍 Liberté 6        │
│  │ ⭐ 4.5 · 5 km       │
│  └───────────────────┘  │
│                         │
│  [⭐]  [📅]  [👤]  [⚙️] │  ← Tab Bar
│   Fav    RDV   Prof   + │
└─────────────────────────┘
```

**Composants** :
- SearchBar (h=48, radius=12, icon loupe)
- CategoryChip (scroll horizontal, icon + label)
- StructureCard (image, nom, adresse, distance, note, spécialités)
- BottomNav (4 onglets, icon + label)

---

## Écran 2 : Recherche avancée

```
┌─────────────────────────┐
│ ←  Rechercher        ✕  │
├─────────────────────────┤
│                         │
│  ┌───────────────────┐  │
│  │ 🔍 Centre de santé│  │  ← Search input avec texte
│  └───────────────────┘  │
│                         │
│  Filtres                │
│                         │
│  Type de structure      │
│  ┌────┐┌────┐┌────┐    │
│  │🏥  ││🏨  ││🩺  │    │  ← Chips sélectionnables
│  │Tout││Hôpi││Cabi│    │
│  └────┘└────┘└────┘    │
│                         │
│  Spécialité             │
│  ┌────┐┌────┐┌────┐    │
│  │👶  ││🫀  ││🦷  │    │
│  │Pédi││Card││Dent│    │
│  └────┘└────┘└────┘    │
│                         │
│  Distance max           │
│  [─────●────────] 5 km  │  ← Slider
│                         │
│  Disponible aujourd'hui │
│  [==============●====]  │  ← Toggle
│                         │
│  ┌───────────────────┐  │
│  │    🔍 Rechercher   │  │  ← Bouton primaire
│  └───────────────────┘  │
│                         │
└─────────────────────────┘
```

---

## Écran 3 : Détail Structure

```
┌─────────────────────────┐
│ ←                 [♡]   │
├─────────────────────────┤
│                         │
│  ┌───────────────────┐  │
│  │                   │  │  ← Image principale
│  │   [PHOTO CABINET] │  │
│  │                   │  │
│  └───────────────────┘  │
│                         │
│  Clinique Niang         │  ← Nom
│  ⭐ 4.8 (127 avis)      │  ← Note
│  📍 Médina, Rue 10      │  ← Adresse
│  📞 77 123 45 67        │  ← Téléphone
│                         │
│  Horaires               │
│  Lun-Ven : 08h-18h      │
│  Sam     : 09h-13h      │
│                         │
│  Services               │
│  ┌───────────────────┐  │
│  │ 👶 Pédiatrie       │  │  ← Service card
│  │ Dr. Ndiaye · 5 000F│  │
│  │ [📅 Voir créneaux] │  │
│  └───────────────────┘  │
│  ┌───────────────────┐  │
│  │ 🩺 Généraliste     │  │
│  │ Dr. Diallo · 3 000F│  │
│  │ [📅 Voir créneaux] │  │
│  └───────────────────┘  │
│                         │
│  Avis patients          │
│  "Très professionnel..."│
│  — Aminata D. ⭐⭐⭐⭐⭐  │
│                         │
└─────────────────────────┘
```

---

## Écran 4 : Choix du créneau

```
┌─────────────────────────┐
│ ←  Pédiatrie             │
├─────────────────────────┤
│                         │
│  📅 Août 2026           │
│  ┌─────────────────────┐│
│  │ Lun Mar Mer Jeu Ven ││  ← Sélecteur de semaine
│  │ 10  11  12  13  14  ││
│  │ ●   ○   ○   ○   ○   ││  ← 10 sélectionné
│  └─────────────────────┘│
│                         │
│  Mardi 12 août          │
│                         │
│  Matin                  │
│  ┌────┐┌────┐┌────┐    │
│  │09:00││09:30││10:00│  │  ← Créneaux
│  │ ✅  ││ ✅  ││ ❌  │  │  ✅ dispo / ❌ indispo
│  └────┘└────┘└────┘    │
│                         │
│  Après-midi             │
│  ┌────┐┌────┐┌────┐    │
│  │14:00││14:30││15:00│  │
│  │ ✅  ││ ✅  ││ ✅  │  │
│  └────┘└────┘└────┘    │
│                         │
│  Créneau sélectionné :  │
│  Mardi 12/08 à 09:00    │
│                         │
│  ┌───────────────────┐  │
│  │  Confirmer 3 000F  │  │  ← Bouton prix
│  └───────────────────┘  │
│                         │
└─────────────────────────┘
```

---

## Écran 5 : Paiement Mobile Money

```
┌─────────────────────────┐
│ ←  Paiement              │
├─────────────────────────┤
│                         │
│  Récapitulatif          │
│  ┌───────────────────┐  │
│  │ Pédiatrie          │  │
│  │ Dr. Ndiaye         │  │
│  │ Mardi 12/08 09:00  │  │
│  │ ─────────────────  │  │
│  │ Total : 3 000 FCFA │  │
│  └───────────────────┘  │
│                         │
│  Choisir mode de paiement│
│                         │
│  ┌───────────────────┐  │
│  │ [🌊] Wave          │  │  ← Option 1
│  │                    │  │
│  └───────────────────┘  │
│  ┌───────────────────┐  │
│  │ [🟠] Orange Money  │  │  ← Option 2
│  │                    │  │
│  └───────────────────┘  │
│  ┌───────────────────┐  │
│  │ [🔵] Free Money    │  │  ← Option 3
│  │                    │  │
│  └───────────────────┘  │
│                         │
│  🔒 Paiement sécurisé   │  ← Trust signal
│  par encryption SSL     │
│                         │
└─────────────────────────┘
```

---

## Écran 6 : Confirmation

```
┌─────────────────────────┐
│                         │
│                         │
│          ✅             │  ← Success icon (animé)
│                         │
│    RDV confirmé !       │
│                         │
│    Pédiatrie            │
│    Dr. Ndiaye           │
│    Mardi 12/08 à 09:00  │
│                         │
│    N° : SS-2026-0812-001│  ← Numéro de référence
│                         │
│    📍 Clinique Niang    │
│    Médina, Rue 10       │
│                         │
│    Un SMS de rappel     │
│    vous sera envoyé     │
│    24h avant le RDV     │
│                         │
│  ┌───────────────────┐  │
│  │  📅 Voir mon RDV   │  │
│  └───────────────────┘  │
│                         │
│  ┌───────────────────┐  │
│  │  🏠 Retour accueil │  │
│  └───────────────────┘  │
│                         │
└─────────────────────────┘
```

---

## Écran 7 : Dashboard Médecin

```
┌─────────────────────────┐
│  SanteSenegal Pro   🔔  │
├─────────────────────────┤
│                         │
│  Bonjour Dr. Ndiaye 👋  │
│                         │
│  Aujourd'hui, 12 août   │
│                         │
│  ┌────┐ ┌────┐ ┌────┐   │
│  │ 5  │ │ 2  │ │1.5k│   │  ← KPI cards
│  │RDV │ │Abs │ │FCFA│   │
│  └────┘ └────┘ └────┘   │
│                         │
│  Prochains rendez-vous  │
│  ┌───────────────────┐  │
│  │ 09:00             │  │
│  │ 👶 Aminata D.     │  │  ← RDV card
│  │ Pédiatrie · 1er RDV│ │
│  │ [✅ Confirmer]     │  │
│  └───────────────────┘  │
│  ┌───────────────────┐  │
│  │ 09:30             │  │
│  │ 🩺 Omar F.        │  │
│  │ Généraliste        │  │
│  │ [✅ Confirmé]      │  │
│  └───────────────────┘  │
│  ┌───────────────────┐  │
│  │ 10:00             │  │
│  │ 👶 Sophie N.      │  │
│  │ Pédiatrie · Rappel │  │
│  │ [❌ Annulé]        │  │
│  └───────────────────┘  │
│                         │
│  [📅]  [👥]  [📊]  [⚙️] │  ← Tab Bar
│  Agenda Pat.  Stats  +  │
└─────────────────────────┘
```

---

## Écran 8 : Gestion créneaux (Médecin)

```
┌─────────────────────────┐
│ ←  Mes disponibilités    │
├─────────────────────────┤
│                         │
│  📅 Semaine du 10/08    │
│  [←]         [→]        │
│                         │
│  Lundi 10/08            │
│  ┌────┐┌────┐┌────┐┌────┐│
│  │08:00││09:00││10:00││+  │  ← Créneaux + bouton ajouter
│  │ ✅ ││ ✅ ││ ❌ ││   │
│  └────┘└────┘└────┘└────┘│
│                         │
│  Mardi 11/08            │
│  ┌────┐┌────┐┌────┐     │
│  │09:00││10:00││+   │     │
│  │ ✅ ││ ✅ ││    │
│  └────┘└────┘└────┘     │
│                         │
│  [+ Ajouter une         │
│   disponibilité         │
│   récurrente]           │
│                         │
└─────────────────────────┘
```

---

## 📝 Spécifications communes à tous les écrans

### Marges et espacements
- **Safe area** : top=44px (notch), bottom=34px (home indicator)
- **Marge latérale** : 16px (mobile), 24px (tablet)
- **Espacement entre sections** : 24px
- **Espacement entre éléments** : 12px

### Boutons
| Type | Hauteur | Radius | Police |
|------|---------|--------|--------|
| Primaire | 48px | 12px | 16px SemiBold |
| Secondaire | 44px | 8px | 14px Medium |
| Ghost | 40px | 8px | 14px Regular |

### Touch targets
- **Minimum** : 48x48 dp (Android Material)
- **Idéal** : 56x56 dp pour les actions principales

---

**Prochain livrable** : Design System (couleurs, typographie, composants)
