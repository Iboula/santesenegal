# ADR Index - SanteSenegal

Les decisions d'architecture historiques sont dans `../architecture/ARCHITECTURE.md`.

## Decisions existantes

| ADR | Sujet | Statut | Source |
|---|---|---|---|
| ADR-001 | Clean Architecture + CQRS | Approved | `architecture/ARCHITECTURE.md` |
| ADR-002 | PostgreSQL + EF Core Code First | Approved | `architecture/ARCHITECTURE.md` |
| ADR-003 | JWT Bearer | Approved, a clarifier HMAC vs RS256 | `architecture/ARCHITECTURE.md`, `stories/FOUND-001-authentification-jwt.md` |
| ADR-004 | Paiement par providers | Approved | `architecture/ARCHITECTURE.md` |
| ADR-005 | Notifications avec Outbox Pattern | Proposed/partial | `architecture/ARCHITECTURE.md` |
| ADR-006 | Blazor WASM + MAUI | Approved | `architecture/ARCHITECTURE.md` |

## Regle

Toute decision structurante future doit avoir son propre fichier `ADR-XXX-titre.md`, base sur `../templates/ADR_TEMPLATE.md`.

