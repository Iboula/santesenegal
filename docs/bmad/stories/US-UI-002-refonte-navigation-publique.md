# US-UI-002 : Refonte de la navigation publique

**Epic**: EPIC-UX
**Priorite**: P0
**Statut**: Ready for BMAD Review
**Source PRD**: navigation publique patient/visiteur
**Source architecture/ADR**: Blazor WASM, Clean Architecture

## Contexte

La navigation publique utilisait le layout historique avec sidebar et liens conditionnels vers des zones privees ou operationnelles. Pour le parcours public, la navigation devait etre simplifiee, responsive et accessible.

## Objectif

Creer une navigation publique composee d'un header desktop horizontal et d'une bottom navigation mobile fixe.

## Scope

Inclus:

- `PublicLayout.razor`;
- `PublicHeader.razor`;
- `BottomNavigation.razor`;
- `LanguageSelector.razor`;
- styles isoles des composants;
- branchement du layout public comme layout par defaut;
- tests bUnit.

Exclus:

- modification API;
- acces direct a la base de donnees;
- logique metier;
- changement des pages ciblees par les liens;
- suppression physique de l'ancien `MainLayout` ou `NavMenu`.

## Navigation publique retenue

Liens conserves:

- Accueil: `/`;
- Structures: `/structures`;
- Triage: `/triage`;
- Urgence: `/accident`.

Liens retires de la navigation publique:

- Dashboard;
- Dispatch;
- Visualisation 3D;
- Paiements.

## Accessibilite

- Navigation desktop avec `aria-label="Navigation principale"`.
- Navigation mobile avec `aria-label="Navigation mobile"`.
- Skip link vers le contenu principal.
- Focus visibles sur liens et boutons.
- Boutons du selecteur de langue avec `aria-pressed`.
- Navigation clavier native via liens et boutons HTML.

## Fichiers touches

- `SanteSenegal.Web/App.razor`
- `SanteSenegal.Web/Layout/PublicLayout.razor`
- `SanteSenegal.Web/Layout/PublicLayout.razor.css`
- `SanteSenegal.Web/Layout/PublicHeader.razor`
- `SanteSenegal.Web/Layout/PublicHeader.razor.css`
- `SanteSenegal.Web/Layout/BottomNavigation.razor`
- `SanteSenegal.Web/Layout/BottomNavigation.razor.css`
- `SanteSenegal.Web/Layout/LanguageSelector.razor`
- `SanteSenegal.Web/Layout/LanguageSelector.razor.css`
- `SanteSenegal.Web.Tests/PublicNavigationTests.cs`

## Criteres d'acceptation

- [x] Header horizontal desktop cree.
- [x] Bottom navigation mobile fixe creee.
- [x] Navigation publique limitee a Accueil, Structures, Triage, Urgence.
- [x] Dashboard, Dispatch, Visualisation 3D et Paiements absents de la navigation publique.
- [x] Selecteur de langue FR/WO cree.
- [x] Aucun JavaScript externe ajoute.
- [x] Aucun acces base ajoute.
- [x] Aucune modification API.
- [x] Tests bUnit ajoutes et verts.
- [x] Build Web OK avec 0 warning.

## Verification

```bash
dotnet build SanteSenegal.Web/SanteSenegal.Web.csproj -c CodexUi002 -p:OutputPath=..\artifacts\us-ui-002-web-build\
dotnet test SanteSenegal.Web.Tests/SanteSenegal.Web.Tests.csproj -c CodexUi002Tests -p:OutputPath=..\artifacts\us-ui-002-web-tests\ -p:WasmEnableWebcil=false
```

Resultats:

- Build Web: OK, 0 warning, 0 erreur.
- Tests UI: OK, 6 passed.

## Notes

Les sorties de build utilisent des dossiers `artifacts/us-ui-002-*` pour eviter les verrous locaux de Visual Studio Insiders sur `bin/Debug` et `obj/Debug`.
