# WEB-001 : Debloquer le build Blazor

**Epic**: EPIC-UX
**Priorite**: P1
**Statut**: Blocked
**Source PRD**: portail patient/structure
**Source architecture/ADR**: ADR-006 Blazor WASM + MAUI

## Contexte

Le build du projet web ne peut pas terminer car la DLL de sortie est verrouillee par Visual Studio Insiders.

## Commande

```bash
dotnet build SanteSenegal.Web/SanteSenegal.Web.csproj --no-restore
```

## Blocage observe

`SanteSenegal.Web\bin\Debug\net9.0\SanteSenegal.Web.dll` est utilise par `Microsoft Visual Studio Insiders (7256)`.

## Criteres de reprise

- [ ] Fermer ou arreter l'instance qui verrouille la DLL.
- [ ] Relancer `dotnet build SanteSenegal.Web/SanteSenegal.Web.csproj --no-restore`.
- [ ] Si le build passe, mettre cette story a `Done`.
- [ ] Si d'autres erreurs apparaissent, les consigner ici.

