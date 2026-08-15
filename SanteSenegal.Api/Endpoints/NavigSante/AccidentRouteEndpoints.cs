using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using SanteSenegal.Api.Hubs;
using SanteSenegal.Domain.Entities.NavigSante;
using SanteSenegal.Infrastructure.Persistence;

namespace SanteSenegal.Api.Endpoints.NavigSante;

public static class AccidentRouteEndpoints
{
    public static void MapAccidentsTempsReelEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/accidents");

        // ── PUBLIC — Signaler un accident ──
        group.MapPost("/signaler", async (
            SignalerAccidentRequest request,
            SanteDbContext db,
            IHubContext<AlertesHub> hub) =>
        {
            var accident = new AccidentRoute
            {
                Latitude = request.Latitude,
                Longitude = request.Longitude,
                AdresseApproximative = request.Adresse,
                Route = request.Route,
                PointKilometrique = request.PointKilometrique,
                Description = request.Description,
                NombreVictimesEstime = request.NombreVictimes,
                NombreBlessesGraves = request.NombreBlessesGraves ?? 0,
                NombreDeces = request.NombreDeces ?? 0,
                TypeAccident = request.TypeAccident,
                TypeVehicule = request.TypeVehicule,
                RisqueIncendie = request.RisqueIncendie,
                FuiteCarburant = request.FuiteCarburant,
                RouteBloquee = request.RouteBloquee,
                ConditionsMeteo = request.ConditionsMeteo,
                EtatRoute = request.EtatRoute,
                NomSignaleur = request.NomSignaleur,
                TelephoneSignaleur = request.TelephoneSignaleur,
                PhotoUrl = request.PhotoUrl,
                Statut = StatutAccident.Signale,
                DateAccident = DateTime.UtcNow
            };

            db.AccidentsRoute.Add(accident);
            await db.SaveChangesAsync();

            // Notifier en temps réel tous les agents connectés
            var hopitauxProches = await TrouverHopitauxProches(db, request.Latitude, request.Longitude);
            await AlertesHub.DiffuserAlerteAccident(hub, accident, hopitauxProches);

            // Envoyer SMS aux hôpitaux les plus proches
            // TODO: intégrer SMS provider

            return Results.Created($"/api/accidents/{accident.Id}", new
            {
                AccidentId = accident.Id,
                NumeroSuivi = $"ACC-{accident.Id:D6}",
                Statut = accident.Statut.ToString(),
                HopitauxProches = hopitauxProches
            });
        })
        .WithName("SignalerAccident")
        .WithOpenApi();

        // ── AUTH — Liste des accidents actifs ──
        group.MapGet("/actifs", [Authorize(Roles = "Admin,AgentSante,Medecin")] async (SanteDbContext db) =>
        {
            var accidents = await db.AccidentsRoute
                .Where(a => a.Statut != StatutAccident.Cloture)
                .OrderByDescending(a => a.DateAccident)
                .ToListAsync();

            return Results.Ok(accidents);
        });

        // ── AUTH — Détails d'un accident ──
        group.MapGet("/{id}", [Authorize] async (int id, SanteDbContext db) =>
        {
            var accident = await db.AccidentsRoute.FindAsync(id);
            return accident == null ? Results.NotFound() : Results.Ok(accident);
        });

        // ── AUTH — Mettre à jour le statut ──
        group.MapPut("/{id}/statut", [Authorize(Roles = "Admin,AgentSante,Medecin")] async (
            int id,
            MettreAJourStatutRequest request,
            SanteDbContext db,
            IHubContext<AlertesHub> hub) =>
        {
            var accident = await db.AccidentsRoute.FindAsync(id);
            if (accident == null) return Results.NotFound();

            accident.Statut = request.NouveauStatut;
            accident.DatePriseEnCharge = request.DatePriseEnCharge ?? DateTime.UtcNow;
            accident.StructureIdIntervention = request.StructureIdIntervention;
            accident.NotesIntervention = request.NotesIntervention;

            if (request.NouveauStatut == StatutAccident.Cloture)
                accident.DateCloture = DateTime.UtcNow;

            await db.SaveChangesAsync();

            // Notifier les clients connectés
            var hopital = await db.Structures.FindAsync(request.StructureIdIntervention);
            await AlertesHub.NotifierPriseEnCharge(hub, id, hopital?.Nom ?? "Inconnu");

            return Results.Ok(accident);
        });

        // ── AUTH — Clôturer un accident ──
        group.MapPost("/{id}/cloturer", [Authorize(Roles = "Admin,Medecin")] async (
            int id,
            CloturerAccidentRequest request,
            SanteDbContext db) =>
        {
            var accident = await db.AccidentsRoute.FindAsync(id);
            if (accident == null) return Results.NotFound();

            accident.Statut = StatutAccident.Cloture;
            accident.DateCloture = DateTime.UtcNow;
            accident.RapportIntervention = request.Rapport;

            await db.SaveChangesAsync();
            return Results.Ok(accident);
        });

        // ── PUBLIC — Zones noires (stats) ──
        group.MapGet("/zones-noires", async (SanteDbContext db) =>
        {
            var sixDerniersMois = DateTime.UtcNow.AddMonths(-6);

            var zones = await db.AccidentsRoute
                .Where(a => a.DateAccident >= sixDerniersMois && a.Statut == StatutAccident.Cloture)
                .GroupBy(a => new { a.Route, a.PointKilometrique })
                .Select(g => new
                {
                    Route = g.Key.Route,
                    PointKilometrique = g.Key.PointKilometrique,
                    NombreAccidents = g.Count(),
                    NombreDeces = g.Sum(a => a.NombreDeces),
                    NombreBlessesGraves = g.Sum(a => a.NombreBlessesGraves),
                    Latitude = g.Average(a => a.Latitude),
                    Longitude = g.Average(a => a.Longitude)
                })
                .Where(z => z.NombreAccidents >= 3)
                .OrderByDescending(z => z.NombreAccidents)
                .Take(20)
                .ToListAsync();

            return Results.Ok(zones);
        });

        // ── AUTH — Statistiques ──
        group.MapGet("/statistiques", [Authorize(Roles = "Admin,AgentSante")] async (
            SanteDbContext db,
            [FromQuery] DateTime? debut,
            [FromQuery] DateTime? fin) =>
        {
            var dateDebut = debut ?? DateTime.UtcNow.AddMonths(-12);
            var dateFin = fin ?? DateTime.UtcNow;

            var stats = new
            {
                TotalAccidents = await db.AccidentsRoute.CountAsync(a => a.DateAccident >= dateDebut && a.DateAccident <= dateFin),
                TotalDeces = await db.AccidentsRoute.Where(a => a.DateAccident >= dateDebut && a.DateAccident <= dateFin).SumAsync(a => a.NombreDeces),
                TotalBlessesGraves = await db.AccidentsRoute.Where(a => a.DateAccident >= dateDebut && a.DateAccident <= dateFin).SumAsync(a => a.NombreBlessesGraves),
                ParType = await db.AccidentsRoute
                    .Where(a => a.DateAccident >= dateDebut && a.DateAccident <= dateFin)
                    .GroupBy(a => a.TypeAccident)
                    .Select(g => new { Type = g.Key, Count = g.Count() })
                    .ToListAsync(),
                ParMois = await db.AccidentsRoute
                    .Where(a => a.DateAccident >= dateDebut && a.DateAccident <= dateFin)
                    .GroupBy(a => new { a.DateAccident.Year, a.DateAccident.Month })
                    .Select(g => new { Annee = g.Key.Year, Mois = g.Key.Month, Count = g.Count() })
                    .OrderBy(x => x.Annee).ThenBy(x => x.Mois)
                    .ToListAsync()
            };

            return Results.Ok(stats);
        });
    }

    private static async Task<List<StructureBasicInfo>> TrouverHopitauxProches(SanteDbContext db, double lat, double lng)
    {
        // Recherche simplifiée — en prod utiliser PostGIS pour la distance réelle
        var hopitaux = await db.Structures
            .Where(s => s.Type == Domain.Enums.TypeStructure.Hopital || s.Type == Domain.Enums.TypeStructure.Clinique)
            .Where(s => s.EstActif)
            .Take(5)
            .Select(s => new StructureBasicInfo(
                s.Id,
                s.Nom,
                s.Adresse,
                0, // TODO: lits réa depuis RessourceSanitaire
                0  // TODO: distance réelle
            ))
            .ToListAsync();

        return hopitaux;
    }
}

// DTOs
public record SignalerAccidentRequest(
    double Latitude,
    double Longitude,
    string? Adresse,
    string? Route,
    string? PointKilometrique,
    string? Description,
    int NombreVictimes,
    int? NombreBlessesGraves,
    int? NombreDeces,
    string? TypeAccident,
    string? TypeVehicule,
    bool RisqueIncendie,
    bool FuiteCarburant,
    bool RouteBloquee,
    string? ConditionsMeteo,
    string? EtatRoute,
    string? NomSignaleur,
    string? TelephoneSignaleur,
    string? PhotoUrl
);

public record MettreAJourStatutRequest(
    StatutAccident NouveauStatut,
    DateTime? DatePriseEnCharge,
    int? StructureIdIntervention,
    string? NotesIntervention
);

public record CloturerAccidentRequest(string Rapport);
