# Sprint Phase 1 — Stabilisation : Plan du Scrum Master

**Sprint** : Phase 1 — Stabilisation Technique  
**Date** : 2026-08-13  
**Objectif** : Rendre la codebase compilable, cohérente et prête pour le développement de features  
**Durée estimée** : 4-5h de travail Dev

---

## Board du Sprint

| ID | Story | Priorité | Estimation | Statut | Agent |
|----|-------|----------|------------|--------|-------|
| STAB-001 | Organiser et dédoublonner les DTOs Disponibilite | P0 | 1h | 🔴 TODO | Dev |
| STAB-002 | Fusionner le projet fantôme SanteSenegel.Application | P0 | 30min | 🔴 TODO | Dev |
| STAB-003 | Corriger CreatePatientHandler pour MediatR | P0 | 30min | 🔴 TODO | Dev |
| STAB-004 | Corriger la configuration EF Core RendezVous | P0 | 1h | 🔴 TODO | Dev |
| STAB-005 | Créer les enums StatutPaiement et StatutTransaction | P0 | 30min | 🔴 TODO | Dev |
| STAB-006 | Ajouter CreatedAt/UpdatedAt à BaseEntity | P0 | 1h | 🔴 TODO | Dev |

---

## Ordre d'exécution recommandé

```
Étape 1 ──▶ STAB-002 (déplacer le handler)
    │           └─> Libère le projet fantôme
    │
    ├───▶ STAB-001 (organiser DTOs)
    │       └─> Nécessite que le projet soit propre
    │
    ├───▶ STAB-003 (corriger Patient handler)
    │       └─> Indépendant mais rapide
    │
    ├───▶ STAB-005 (enums statuts)
    │       └─> Prépare les entités
    │
    ├───▶ STAB-006 (timestamps audit)
    │       └─> Touche BaseEntity → impact toutes les entités
    │
    └───▶ STAB-004 (corriger EF Core RendezVous)
            └─> FAIT EN DERNIER car nécessite une migration EF
                qui doit inclure tous les changements précédents
```

**Pourquoi STAB-004 en dernier ?** Parce que c'est la seule story qui nécessite une **migration EF Core**. Il faut d'abord que toutes les modifications de modèle (enums, timestamps) soient en place pour générer une migration unique et cohérente.

---

## Définition of Done (DoD) pour le Sprint

- [ ] Toutes les stories STAB-001 à STAB-006 sont implémentées
- [ ] La solution compile sans warning critique
- [ ] Les migrations EF Core se génèrent sans erreur
- [ ] La base de données se met à jour (`Update-Database`)
- [ ] Swagger affiche correctement tous les endpoints
- [ ] Au moins un test manuel réussi par endpoint critique :
  - [ ] POST /api/patients
  - [ ] POST /api/structures
  - [ ] POST /api/rendez-vous
  - [ ] GET /api/disponibilites
- [ ] Le dossier `SanteSenegel.Application` n'existe plus
- [ ] Aucun doublon de DTOs

---

## Notes pour le Dev Agent

### ⚡ Workflow recommandé

```
1. Lire la story STAB-XXX.md dans docs/bmad/stories/
2. Identifier les fichiers concernés (section "Fichiers concernés")
3. Implémenter les modifications
4. Vérifier la compilation : dotnet build
5. Si EF Core impliqué : dotnet ef migrations add ...
6. Vérifier avec Swagger ou test manuel
7. Cocher les critères d'acceptation
8. Passer à la story suivante
```

### 🚨 Points de vigilance

1. **Ne pas casser les endpoints existants** : Les endpoints `StructureEndpoints`, `RendezVousEndpoints`, etc. doivent continuer de fonctionner.

2. **Gérer les namespaces** : Après déplacement de fichiers, vérifier tous les `using`.

3. **Migrations EF** : Une seule migration finale suffit (`Add-Migration Phase1Stabilization`) plutôt qu'une par story.

4. **Tests** : Si des tests existent, les mettre à jour. Sinon, ce n'est pas le focus de ce sprint.

### 📋 Commandes utiles

```bash
# Build
 dotnet build SanteSenegal.sln

# EF Core migrations
cd SanteSenegal.Infrastructure
dotnet ef migrations add Phase1Stabilization --startup-project ../SanteSenegal.Api
dotnet ef database update --startup-project ../SanteSenegal.Api

# Run API
cd SanteSenegal.Api
dotnet run
```

---

**Prochaine étape** : Passer à l'**Agent Dev** pour l'implémentation.
