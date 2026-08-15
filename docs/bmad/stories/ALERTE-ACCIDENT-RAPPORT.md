# Rapport — Module Alertes Accidents de la Route

**Date** : 2026-08-13  
**Agent** : Dev  
**Statut** : Implémenté ✅

---

## 🚨 Contexte

Au Sénégal, les accidents de la route tuent plus de 700 personnes par an. Le retard de prise en charge est la principale cause de décès évitables. Ce module vise à réduire le temps entre l'accident et la prise en charge médicale.

---

## ✅ Fichiers créés / modifiés

### Nouveaux fichiers

| Fichier | Description |
|---------|-------------|
| `Domain/Entities/NavigSante/AccidentRoute.cs` | Entité accident avec statut, localisation, victimes |
| `Domain/Enums/StatutAccident.cs` | Enum Signale → Confirme → PriseEnCharge → VictimesEvacuees → Cloture |
| `Application/DTOs/NavigSante/AccidentRouteDtos.cs` | DTOs signalement, réponse, zones noires, stats |
| `Infrastructure/Services/NavigSante/AccidentRouteService.cs` | Logique métier complète |
| `Api/Endpoints/AccidentRouteEndpoints.cs` | Endpoints REST |
| `docs/bmad/stories/ALERTE-ACCIDENT-CONCEPTION.md` | Document de conception |

### Fichiers modifiés

| Fichier | Changement |
|---------|-----------|
| `Infrastructure/Persistence/SanteDbContext.cs` | Ajout DbSet<AccidentRoute> |
| `Infrastructure/DependencyInjection.cs` | Enregistrement IAccidentRouteService |
| `Api/Program.cs` | MapAccidentRouteEndpoints |

---

## 📡 Endpoints disponibles

| Méthode | Endpoint | Auth | Description |
|---------|----------|------|-------------|
| POST | `/api/accidents/signaler` | 🌍 Public | Signalement citoyen (30 sec) |
| GET | `/api/accidents/actifs` | 🌍 Public | Accidents en cours |
| GET | `/api/accidents/{id}` | 🌍 Public | Détail d'un accident |
| GET | `/api/accidents/zone` | 🌍 Public | Accidents proches (rayon km) |
| GET | `/api/accidents/aujourdhui` | 🌍 Public | Accidents du jour |
| GET | `/api/accidents/zones-noires` | 🌍 Public | Carte des zones dangereuses |
| GET | `/api/accidents/statistiques` | 🌍 Public | Stats (décès, blessés, routes...) |
| PUT | `/api/accidents/{id}/statut` | 🔒 Medecin/Admin | Mise à jour statut |
| GET | `/api/accidents/historique` | 🔒 Admin | Historique paginé |

---

## 🎯 Fonctionnalités clés

### 1. Signalement citoyen (30 secondes)
```bash
curl -X POST http://localhost:5000/api/accidents/signaler \
  -H "Content-Type: application/json" \
  -d '{
    "latitude": 14.7167,
    "longitude": -17.4677,
    "route": "N1",
    "pointKilometrique": "PK 45",
    "nombreVictimesEstime": 3,
    "typeAccident": "Collision",
    "typeVehicule": "Voiture",
    "risqueIncendie": false,
    "fuiteCarburant": true,
    "routeBloquee": true,
    "conditionsMeteo": "Pluie",
    "nomSignaleur": "Omar Faye",
    "telephoneSignaleur": "77 123 45 67"
  }'
```

**Réponse** :
```json
{
  "message": "🚨 Accident signalé avec succès. Les secours ont été notifiés.",
  "messageWolof": "🚨 Dëgg-dëggal na. Seen ngiran yi ñu jàppal.",
  "accidentId": 42,
  "reference": "ACC-000042",
  "conseil": "Ne bougez pas les blessés sauf danger immédiat. Couvrez-les."
}
```

### 2. Alertes temps réel aux urgences
Les structures dans un rayon de 20 km reçoivent automatiquement :
- Localisation GPS précise
- Nombre de victimes
- Risques (incendie, fuite)
- Recommandations de préparation

### 3. Zones noires (heat map)
Identification automatique des routes les plus dangereuses basée sur :
- Nombre d'accidents sur 12 mois
- Nombre de décès
- Niveau de risque (Rouge/Orange/Jaune)

### 4. Tableau de bord autorités
```json
{
  "totalAccidents": 342,
  "totalDeces": 78,
  "totalBlesses": 412,
  "tauxMortalite": 22.8,
  "accidentsAujourdhui": 3,
  "accidentsCetteSemaine": 18,
  "accidentsCeMois": 67,
  "routeLaPlusDangereuse": "N1 (Dakar-Thiès)",
  "heureLaPlusDangereuse": "18h-19h",
  "accidentsParRegion": [
    { "region": "Dakar", "nombreAccidents": 89, "nombreDeces": 21 },
    { "region": "Thiès", "nombreAccidents": 45, "nombreDeces": 12 }
  ]
}
```

---

## 🌍 Impact attendu

| Indicateur | Avant | Après (objectif) |
|------------|-------|-----------------|
| Temps alerte → secours | 15-30 min | < 5 min |
| Préparation des urgences | À l'arrivée | Avant l'arrivée |
| Connaissance zones dangereuses | Faible | Temps réel |
| Décès évitables | 0% | +30% |

---

## 📋 Prochaines étapes

1. **Générer la migration** :
   ```bash
   dotnet ef migrations add AddAccidentRoute --project SanteSenegal.Infrastructure --startup-project SanteSenegal.Api
   dotnet ef database update --project SanteSenegal.Infrastructure --startup-project SanteSenegal.Api
   ```

2. **Tester le signalement** :
   ```bash
   curl -X POST http://localhost:5000/api/accidents/signaler \
     -H "Content-Type: application/json" \
     -d '{"latitude":14.7,"longitude":-17.4,"route":"N1","nombreVictimesEstime":2}'
   ```

3. **Améliorations futures** :
   - [ ] Notification push vers app mobile des structures proches
   - [ ] Intégration WhatsApp pour signalement sans app
   - [ ] Prédiction ML des zones à risque
   - [ ] Dashboard carte interactive (Leaflet)
   - [ ] Intégration SAMU 15 / Gendarmerie

---

*Module Alertes Accidents de la Route — SanteSenegal*
