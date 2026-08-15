# SanteSenegal — Architecture Decision Record (ADR)

**Projet** : SanteSenegal  
**Date** : 2026-08-13  
**Auteur** : BMad Agent Architect  
**Statut** : Approved

---

## ADR-001 : Architecture globale — Clean Architecture + CQRS

### Contexte
Le projet SanteSenegal nécessite une architecture qui supporte :
- La scalabilité future (microservices potentiels)
- La testabilité unitaire élevée
- La séparation des responsabilités (métier vs infrastructure)

### Décision
Adopter une **Clean Architecture** avec le pattern **CQRS** via MediatR.

```
┌─────────────────────────────────────────────────────────┐
│                    Présentation Layer                    │
│  (Blazor WASM / MAUI / React — future)                 │
├─────────────────────────────────────────────────────────┤
│                    API Layer                             │
│  SanteSenegal.Api (Minimal API + Carter/Endpoints)     │
├─────────────────────────────────────────────────────────┤
│                    Application Layer                     │
│  SanteSenegal.Application (CQRS: Commands/Queries)     │
├─────────────────────────────────────────────────────────┤
│                    Domain Layer                          │
│  SanteSenegal.Domain (Entities, Value Objects, Rules)  │
├─────────────────────────────────────────────────────────┤
│                    Infrastructure Layer                  │
│  SanteSenegal.Infrastructure (EF Core, Repos, Services)│
├─────────────────────────────────────────────────────────┤
│                    Shared Layer                          │
│  SanteSenegal.Shared (cross-cutting, DTOs communs)     │
└─────────────────────────────────────────────────────────┘
```

### Conséquences
✅ Dépendances dirigées vers l'intérieur (Domain ne dépend de rien)  
✅ Facile à tester avec des mocks  
✅ CQRS permet de scaler lectures/écritures indépendamment  
⚠️ Verbose — plus de fichiers que MVC classique  
⚠️ Courbe d'apprentissage pour les nouveaux développeurs

---

## ADR-002 : Base de données — PostgreSQL + EF Core Code First

### Contexte
Besoin d'une base relationnelle robuste avec support JSON (pour extensibilité future), bonne gestion des transactions, et hébergement cloud accessible au Sénégal.

### Décision
**PostgreSQL 15+** avec **Entity Framework Core 9** (Code First).

### Justification
| Option | Pour | Contre |
|--------|------|--------|
| PostgreSQL | Open source, JSONB, performances, hébergement local (Sénégal) | Nécessite expertise DBA |
| SQL Server | Intégration .NET | Coût licence, moins de flexibilité |
| MongoDB | Flexible | Pas de transactions ACID complexes, moins mature en .NET |

### Conséquences
✅ Migrations versionnées pour le déploiement  
✅ Support natif des enums PostgreSQL (mapping EF Core)  
⚠️ Nécessite `pgAdmin` ou outils équivalents pour le DBA

---

## ADR-003 : Authentification — JWT Bearer (RS256)

### Contexte
API stateless nécessitant authentification multi-plateformes (web, mobile, future intégration tiers).

### Décision
**JWT Bearer** avec algorithmes **RS256** (asymétrique).

```
┌─────────────┐    ┌─────────────┐    ┌─────────────┐
│   Client    │───▶│  /api/auth  │───▶│  JWT Token  │
│             │◀───│   (login)   │◀───│  (RS256)    │
└─────────────┘    └─────────────┘    └─────────────┘
       │                                    │
       └─────────────▶ API (Authorization)   │
                     (validate with Public Key)
```

### Conséquences
✅ Tokens signés de façon sécurisée  
✅ Validation côté API sans appel DB  
⚠️ Nécessite gestion du refresh token  
⚠️ Key rotation à planifier

---

## ADR-004 : Paiement — Abstraction avec Providers

### Contexte
Plusieurs providers de paiement mobile au Sénégal (Wave, Orange Money, Free Money). L'architecture doit supporter l'ajout futur de nouveaux providers.

### Décision
Pattern **Strategy** avec abstraction `IPaiementProvider`.

```
┌─────────────────────────────────────────┐
│         IPaiementProvider               │
│  + InitierPaiement()                    │
│  + VerifierStatut()                     │
│  + Rembourser()                         │
└─────────────────────────────────────────┘
       ▲              ▲              ▲
       │              │              │
┌──────────┐  ┌──────────┐  ┌──────────┐
│ Wave     │  │ Orange   │  │ Free     │
│ Provider │  │ Money    │  │ Money    │
│          │  │ Provider │  │ Provider │
└──────────┘  └──────────┘  └──────────┘
```

### Conséquences
✅ Ajout d'un provider = nouvelle implémentation uniquement  
✅ Tests unitaires facilités avec des mocks  
⚠️ Nécessite unifier les modèles de requête/réponse

---

## ADR-005 : Notifications — Outbox Pattern + Queue

### Contexte
Envoi de SMS et emails qui ne doit pas bloquer le flux principal (prise de RDV).

### Décision
**Outbox Pattern** avec stockage en DB + worker/process background.

```
┌─────────────┐    ┌─────────────┐    ┌─────────────┐
│   Command   │───▶│   Outbox    │───▶│  Worker     │
│   Handler   │    │   Table     │    │  (poll)     │
└─────────────┘    └─────────────┘    └──────┬──────┘
                                             │
                                    ┌────────┴────────┐
                                    │  SMS Provider   │
                                    │  Email Service  │
                                    └─────────────────┘
```

### Conséquences
✅ Fiabilité — pas de perte de notification  
✅ Résilience — retry automatique  
⚠️ Complexité supplémentaire (worker à maintenir)

---

## ADR-006 : Frontend — Blazor WASM (recommandation future)

### Contexte
L'équipe est déjà sur .NET. Pour minimiser la stack technique et réutiliser les DTOs.

### Décision
**Blazor WASM** pour le portail patient/structure.  
**MAUI** pour l'app mobile native (iOS/Android).

### Conséquences
✅ Partage de code C# entre API et UI  
✅ Écosystème .NET unifié  
⚠️ Blazor WASM = téléchargement initial lourd  
⚠️ MAUI = moins mature que Flutter/React Native

---

# Architecture Cible — Diagramme de déploiement

```
┌──────────────────────────────────────────────────────────────┐
│                         CLIENT                                │
│  ┌─────────────┐  ┌─────────────┐  ┌─────────────────────┐  │
│  │ Blazor WASM │  │ MAUI Mobile │  │ 3rd Party (futur)   │  │
│  │  (Patient)  │  │ (Structure) │  │                     │  │
│  └──────┬──────┘  └──────┬──────┘  └──────────┬──────────┘  │
└─────────┼────────────────┼────────────────────┼─────────────┘
          │                │                    │
          └────────────────┴────────────────────┘
                             │
                    HTTPS / JWT Bearer
                             │
┌──────────────────────────────────────────────────────────────┐
│                      CLOUD / SERVEUR                         │
│  ┌────────────────────────────────────────────────────────┐  │
│  │              Azure / AWS / Local (Sénégal)             │  │
│  │                                                        │  │
│  │  ┌─────────────┐    ┌─────────────┐    ┌───────────┐  │  │
│  │  │  ASP.NET    │    │  Worker     │    │ PostgreSQL│  │  │
│  │  │  Core API   │◀──▶│  Service    │◀──▶│   + EF    │  │  │
│  │  │  (.NET 9)   │    │  (Outbox)   │    │           │  │  │
│  │  └─────────────┘    └─────────────┘    └───────────┘  │  │
│  │         │                                           │   │  │
│  │  ┌──────┴──────┐    ┌─────────────┐                 │   │  │
│  │  │  Swagger    │    │  Redis      │                 │   │  │
│  │  │  / OpenAPI  │    │  (Cache)    │                 │   │  │
│  │  └─────────────┘    └─────────────┘                 │   │  │
│  └────────────────────────────────────────────────────────┘  │
│                           │                                  │
│  ┌────────────────────────┴──────────────────────────────┐  │
│  │              EXTERNAL SERVICES                        │  │
│  │  ┌───────────┐  ┌───────────┐  ┌─────────────────┐   │  │
│  │  │ Wave API  │  │ Orange $  │  │  SMS Provider   │   │  │
│  │  │           │  │  API      │  │  (Twilio/local) │   │  │
│  │  └───────────┘  └───────────┘  └─────────────────┘   │  │
│  └────────────────────────────────────────────────────────┘  │
└──────────────────────────────────────────────────────────────┘
```

---

**Prochaine étape** : Passer au **Scrum Master** pour découper la Phase 1 (Stabilisation) en stories Dev-ready.
