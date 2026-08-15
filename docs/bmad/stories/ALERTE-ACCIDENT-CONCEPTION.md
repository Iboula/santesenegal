# Module Alertes Accidents de la Route — Conception

**Agent** : PM + Architect + Dev  
**Date** : 2026-08-13  
**Contexte** : 700+ décès/an au Sénégal, retard de prise en charge = mortalité

---

## 🚨 Problématique

| Statistique | Source |
|-------------|--------|
| 700+ décès/an | OMS Sénégal |
| 50% des décès évitables | Si prise en charge < 1h |
| Temps moyen d'alerte | 15-30 min (témoin appelle, info relayée...) |
| Structures manquant de sang | Fréquent lors d'accidents multiples |

---

## 💡 Vision du module

```
┌─────────────────────────────────────────────────────────────────┐
│              🚨 ALERTE ACCIDENT — SENEGAL                       │
│                                                                 │
│   ┌──────────┐     ┌──────────┐     ┌──────────┐              │
│   │ SIGNAlER │────▶│ ALERTER  │────▶│ SAUVER   │              │
│   │ (30 sec) │     │ (instant)│     │ (vite)   │              │
│   └──────────┘     └──────────┘     └──────────┘              │
│                                                                 │
│   📱 Appui sur bouton                                           │
│   📍 GPS envoie localisation                                    │
│   📸 Photo du lieu                                              │
│   🏥 Urgences pré-alerte                                        │
│   🚑 SAMU notifié                                               │
│   💉 Sang réservé                                               │
└─────────────────────────────────────────────────────────────────┘
```

---

## 🎯 Fonctionnalités proposées

### 1. 🚨 Signalement citoyen (30 secondes)

**Parcours utilisateur** :
```
1. Témoin ouvre l'app → Bouton rouge "ACCIDENT"
2. Appui → GPS capturé automatiquement
3. Questions rapides :
   - Combien de victimes ? (1 / 2-3 / 4+)
   - Blessures visibles ? (oui/non)
   - Feu ? Fuite de carburant ? (oui/non)
   - Photo du lieu (optionnel)
4. Envoi → Alerte transmise en 5 secondes
```

**Données collectées** :
- 📍 Localisation GPS précise (latitude, longitude)
- 📸 Photo du lieu
- 👥 Nombre de victimes estimé
- 🚗 Type de véhicule impliqué
- 🔥 Risque incendie / fuite
- ⏰ Heure exacte
- 📞 Numéro du témoin (pour rappel)

---

### 2. 🏥 Alerte temps réel aux structures de santé

**Notification automatique** envoyée aux structures dans un rayon de 20 km :

```
🚨 ALERTE ACCIDENT — SENEGAL

📍 Route : N1, PK 45 (près de Thiès)
⏰ 14h32
👥 3 victimes estimées
🏥 Structure la plus proche : Hôpital de Thiès (8 km)

ACTIONS RECOMMANDÉES :
✅ Préparez le bloc d'urgence
✅ Vérifiez stock sang (groupe inconnu)
✅ Activez équipe chirurgicale
✅ Préparez 3 brancards

🚑 SAMU Dakar notifié (EN ROUTE)
```

---

### 3. 🗺️ Cartographie des zones à risque ("Zones Noires")

**Identification** des routes les plus dangereuses basée sur :
- Historique des accidents (12 derniers mois)
- Heures critiques (nuit, pluie, week-end)
- Types de véhicules impliqués
- Conditions météo

**Exemple de heatmap** :
```
🔴 ROUTE N1 (Dakar-Thiès) — 45 accidents/an
🟠 ROUTE N2 (Dakar-Mbour) — 32 accidents/an
🟡 AUTOROUTE A1 — 18 accidents/an
🟢 ROUTE VERS FATICK — 5 accidents/an
```

---

### 4. 📊 Tableau de bord autorités

Pour le **Ministère de la Santé**, **Gendarmerie**, **SAMU** :

| KPI | Valeur |
|-----|--------|
| Accidents aujourd'hui | 12 |
| Victimes | 8 blessés, 2 décès |
| Temps moyen alerte → prise en charge | 18 min |
| Structures surchargées | 2 |
| Zones noires cette semaine | N1 PK45, N2 PK23 |

---

### 5. 🛡️ Prévention et sensibilisation

- **Campagnes ciblées** sur les zones noires
- **Conseils sécurité** (vitesse, fatigue, téléphone)
- **Statistiques visibles** par région
- **Rappels** avant les périodes à risque (Tabaski, fin d'année)

---

## 🏗️ Architecture du module

```
┌─────────────────────────────────────────────────────────────┐
│                    MODULE ALERTE ACCIDENT                   │
├─────────────────────────────────────────────────────────────┤
│                                                             │
│  ┌──────────┐   ┌──────────┐   ┌──────────┐              │
│  │ Signale- │   │ Alerte   │   │ Prépa-   │              │
│  │ ment     │──▶│ Temps    │──▶│ ration   │              │
│  │ Citoyen  │   │ Réel     │   │ Urgences │              │
│  └──────────┘   └──────────┘   └──────────┘              │
│                                                             │
│  ┌──────────┐   ┌──────────┐   ┌──────────┐              │
│  │ Zones    │   │ Stats    │   │ Préven-  │              │
│  │ Noires   │   │ Auth-    │   │ tion     │              │
│  │          │   │ orités   │   │          │              │
│  └──────────┘   └──────────┘   └──────────┘              │
│                                                             │
└─────────────────────────────────────────────────────────────┘
```

---

## 📋 User Stories

| ID | Story | Priorité |
|----|-------|----------|
| ACC-001 | En tant que témoin, je veux signaler un accident en 30 secondes | P0 |
| ACC-002 | En tant qu'urgentiste, je veux être alerté avant l'arrivée des victimes | P0 |
| ACC-003 | En tant que SAMU, je veux connaître le nombre de victimes et la gravité | P0 |
| ACC-004 | En tant qu'autorité, je veux voir les zones noires sur une carte | P1 |
| ACC-005 | En tant que citoyen, je veux éviter les routes dangereuses | P1 |
| ACC-006 | En tant que structure, je veux mettre à jour mes ressources en temps réel | P1 |

---

*Conception — Module Alertes Accidents de la Route*
