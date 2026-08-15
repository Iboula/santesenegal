using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using SanteSenegal.Api.Hubs;
using SanteSenegal.Domain.Entities.NavigSante;
using SanteSenegal.Infrastructure.Persistence;

namespace SanteSenegal.Api.Endpoints.NavigSante;

public static class RessourceSanitaireEndpoints
{
    public static void MapRessourceSanitaireEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/ressources");

        // ── PUBLIC — Ressources d'une structure ──
        group.MapGet("/structure/{structureId}", async (int structureId, SanteDbContext db) =>
        {
            var ressources = await db.RessourcesSanitaires
                .Where(r => r.StructureId == structureId)
                .OrderByDescending(r => r.DerniereMiseAJour)
                .FirstOrDefaultAsync();

            return ressources == null ? Results.NotFound() : Results.Ok(ressources);
        });

        // ── PUBLIC — Toutes les ressources avec lits disponibles ──
        group.MapGet("/disponibles", async (SanteDbContext db) =>
        {
            var ressources = await db.RessourcesSanitaires
                .Where(r => r.LitsDisponibles > 0 || r.LitsReanimationDisponibles > 0)
                .Select(r => new
                {
                    r.StructureId,
                    r.LitsTotal,
                    r.LitsDisponibles,
                    r.LitsReanimationTotal,
                    r.LitsReanimationDisponibles,
                    r.StockSangOPlus,
                    r.ScannerFonctionnel,
                    r.RadioFonctionnel,
                    r.LaboFonctionnel,
                    r.BlocOperatoireFonctionnel,
                    r.TempsAttenteUrgencesMinutes,
                    r.DerniereMiseAJour
                })
                .ToListAsync();

            return Results.Ok(ressources);
        });

        // ── AUTH — Mettre à jour les ressources (Admin/AgentSante) ──
        group.MapPut("/structure/{structureId}", [Authorize(Roles = "Admin,AgentSante")] async (
            int structureId,
            MettreAJourRessourcesRequest request,
            SanteDbContext db,
            IHubContext<AlertesHub> hub) =>
        {
            var ressources = await db.RessourcesSanitaires
                .Where(r => r.StructureId == structureId)
                .OrderByDescending(r => r.DerniereMiseAJour)
                .FirstOrDefaultAsync();

            if (ressources == null)
            {
                ressources = new RessourceSanitaire { StructureId = structureId };
                db.RessourcesSanitaires.Add(ressources);
            }

            // Mise à jour conditionnelle
            if (request.LitsDisponibles.HasValue) ressources.LitsDisponibles = request.LitsDisponibles.Value;
            if (request.LitsReanimationDisponibles.HasValue) ressources.LitsReanimationDisponibles = request.LitsReanimationDisponibles.Value;
            if (request.StockSangOPlus.HasValue) ressources.StockSangOPlus = request.StockSangOPlus.Value;
            if (request.TempsAttenteUrgencesMinutes.HasValue) ressources.TempsAttenteUrgencesMinutes = request.TempsAttenteUrgencesMinutes.Value;
            if (request.ScannerFonctionnel.HasValue) ressources.ScannerFonctionnel = request.ScannerFonctionnel.Value;
            if (request.RadioFonctionnel.HasValue) ressources.RadioFonctionnel = request.RadioFonctionnel.Value;
            if (request.LaboFonctionnel.HasValue) ressources.LaboFonctionnel = request.LaboFonctionnel.Value;
            if (request.BlocOperatoireFonctionnel.HasValue) ressources.BlocOperatoireFonctionnel = request.BlocOperatoireFonctionnel.Value;

            ressources.DerniereMiseAJour = DateTime.UtcNow;
            await db.SaveChangesAsync();

            // Notifier en temps réel
            await AlertesHub.MettreAJourRessources(hub, structureId, ressources);

            return Results.Ok(ressources);
        });

        // ── AUTH — Rechercher du sang par type ──
        group.MapGet("/sang/{groupeSanguin}", [Authorize] async (string groupeSanguin, SanteDbContext db) =>
        {
            var result = groupeSanguin.ToUpper() switch
            {
                "A+" => await db.RessourcesSanitaires.Where(r => r.StockSangAPlus > 0).Select(r => new StockSanguinResponse(r.StructureId, r.StockSangAPlus)).ToListAsync(),
                "A-" => await db.RessourcesSanitaires.Where(r => r.StockSangAMoins > 0).Select(r => new StockSanguinResponse(r.StructureId, r.StockSangAMoins)).ToListAsync(),
                "B+" => await db.RessourcesSanitaires.Where(r => r.StockSangBPlus > 0).Select(r => new StockSanguinResponse(r.StructureId, r.StockSangBPlus)).ToListAsync(),
                "B-" => await db.RessourcesSanitaires.Where(r => r.StockSangBMoins > 0).Select(r => new StockSanguinResponse(r.StructureId, r.StockSangBMoins)).ToListAsync(),
                "AB+" => await db.RessourcesSanitaires.Where(r => r.StockSangABPlus > 0).Select(r => new StockSanguinResponse(r.StructureId, r.StockSangABPlus)).ToListAsync(),
                "AB-" => await db.RessourcesSanitaires.Where(r => r.StockSangABMoins > 0).Select(r => new StockSanguinResponse(r.StructureId, r.StockSangABMoins)).ToListAsync(),
                "O+" => await db.RessourcesSanitaires.Where(r => r.StockSangOPlus > 0).Select(r => new StockSanguinResponse(r.StructureId, r.StockSangOPlus)).ToListAsync(),
                "O-" => await db.RessourcesSanitaires.Where(r => r.StockSangOMoins > 0).Select(r => new StockSanguinResponse(r.StructureId, r.StockSangOMoins)).ToListAsync(),
                _ => null
            };

            return result == null ? Results.BadRequest("Groupe sanguin invalide") : Results.Ok(result);
        });
    }
}

public record MettreAJourRessourcesRequest(
    int? LitsDisponibles,
    int? LitsReanimationDisponibles,
    int? StockSangOPlus,
    int? TempsAttenteUrgencesMinutes,
    bool? ScannerFonctionnel,
    bool? RadioFonctionnel,
    bool? LaboFonctionnel,
    bool? BlocOperatoireFonctionnel
);

public record StockSanguinResponse(int StructureId, int Stock);
