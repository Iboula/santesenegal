using Microsoft.AspNetCore.Authorization;
using SanteSenegal.Application.DTOs.NavigSante;
using SanteSenegal.Domain.Entities.NavigSante;
using SanteSenegal.Infrastructure.Services.NavigSante;

namespace SanteSenegal.Api.Endpoints;

public static class AccidentRouteEndpoints
{
    public static void MapAccidentRouteEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/accidents");

        // ─── SIGNAGEMENT CITOYEN ───

        // POST /api/accidents/signaler
        // 🌍 PUBLIC — N'importe qui peut signaler un accident
        group.MapPost("/signaler", async (SignalerAccidentRequest request, IAccidentRouteService service) =>
        {
            var accident = await service.SignalerAccidentAsync(request);
            
            return Results.Created($"/api/accidents/{accident.Id}", new
            {
                Message = "🚨 Accident signalé avec succès. Les secours ont été notifiés.",
                MessageWolof = "🚨 Dëgg-dëggal na. Seen ngiran yi ñu jàppal.",
                AccidentId = accident.Id,
                Reference = $"ACC-{accident.Id:D6}",
                HeureSignalement = accident.DateAccident,
                Conseil = accident.NombreVictimesEstime >= 3 
                    ? "Ne bougez pas les blessés sauf danger immédiat. Couvrez-les. Ne donnez rien à boire."
                    : "Assurez votre sécurité avant d'aider. Mettez les warnings."
            });
        })
        .AllowAnonymous()
        .WithName("SignalerAccident")
        .WithOpenApi();

        // ─── CONSULTATION ───

        // GET /api/accidents/actifs
        // 🌍 PUBLIC — Accidents en cours
        group.MapGet("/actifs", async (IAccidentRouteService service) =>
        {
            var accidents = await service.GetAccidentsActifsAsync();
            return Results.Ok(accidents);
        })
        .AllowAnonymous()
        .WithName("GetAccidentsActifs")
        .WithOpenApi();

        // GET /api/accidents/{id}
        // 🌍 PUBLIC — Détail d'un accident
        group.MapGet("/{id}", async (int id, IAccidentRouteService service) =>
        {
            var accident = await service.GetByIdAsync(id);
            if (accident == null) return Results.NotFound();

            return Results.Ok(new AccidentResponse(
                accident.Id,
                accident.Latitude,
                accident.Longitude,
                accident.AdresseApproximative,
                accident.Route,
                accident.DateAccident,
                accident.NombreVictimesEstime,
                accident.NombreBlessesGraves,
                accident.NombreDeces,
                accident.TypeAccident,
                accident.TypeVehicule,
                accident.RisqueIncendie,
                accident.FuiteCarburant,
                accident.RouteBloquee,
                accident.ConditionsMeteo,
                accident.Statut.ToString(),
                accident.DatePriseEnCharge,
                accident.DateCloture
            ));
        })
        .AllowAnonymous()
        .WithName("GetAccidentById")
        .WithOpenApi();

        // GET /api/accidents/zone?lat=...&lng=...&rayon=20
        // 🌍 PUBLIC — Accidents proches
        group.MapGet("/zone", async (double lat, double lng, double rayonKm, IAccidentRouteService service) =>
        {
            var accidents = await service.GetAccidentsParZoneAsync(lat, lng, rayonKm);
            return Results.Ok(accidents);
        })
        .AllowAnonymous()
        .WithName("GetAccidentsParZone")
        .WithOpenApi();

        // GET /api/accidents/aujourdhui
        // 🌍 PUBLIC — Accidents du jour
        group.MapGet("/aujourdhui", async (IAccidentRouteService service) =>
        {
            var accidents = await service.GetAccidentsAujourdhuiAsync();
            return Results.Ok(new
            {
                Date = DateTime.UtcNow.Date,
                Nombre = accidents.Count,
                Victimes = accidents.Sum(a => a.NombreVictimesEstime),
                Deces = accidents.Sum(a => a.NombreDeces),
                Accidents = accidents
            });
        })
        .AllowAnonymous()
        .WithName("GetAccidentsAujourdhui")
        .WithOpenApi();

        // ─── ZONES NOIRES ───

        // GET /api/accidents/zones-noires
        // 🌍 PUBLIC — Carte des zones dangereuses
        group.MapGet("/zones-noires", async (IAccidentRouteService service) =>
        {
            var zones = await service.GetZonesNoiresAsync();
            return Results.Ok(zones);
        })
        .AllowAnonymous()
        .WithName("GetZonesNoires")
        .WithOpenApi();

        // ─── STATISTIQUES ───

        // GET /api/accidents/statistiques
        // 🌍 PUBLIC — Stats générales
        group.MapGet("/statistiques", async (IAccidentRouteService service, DateTime? dateDebut, DateTime? dateFin) =>
        {
            var stats = await service.GetStatistiquesAsync(dateDebut, dateFin);
            return Results.Ok(stats);
        })
        .AllowAnonymous()
        .WithName("GetStatistiquesAccidents")
        .WithOpenApi();

        // ─── GESTION (SAMU / ADMIN) ───

        // PUT /api/accidents/{id}/statut
        // 🔒 ADMIN / SAMU — Mise à jour du statut
        group.MapPut("/{id}/statut", [Authorize(Roles = "Admin,Medecin")] async (int id, MettreAJourStatutAccidentRequest request, IAccidentRouteService service) =>
        {
            var accident = await service.MettreAJourStatutAsync(id, request);
            if (accident == null) return Results.NotFound();

            return Results.Ok(new
            {
                Message = $"Statut mis à jour : {accident.Statut}",
                AccidentId = accident.Id,
                NouveauStatut = accident.Statut.ToString(),
                DateMiseAJour = DateTime.UtcNow
            });
        })
        .WithName("MettreAJourStatutAccident")
        .WithOpenApi();

        // GET /api/accidents/historique
        // 🔒 ADMIN — Historique paginé
        group.MapGet("/historique", [Authorize(Roles = "Admin")] async (int page, int pageSize, IAccidentRouteService service) =>
        {
            var accidents = await service.GetHistoriqueAsync(page, pageSize);
            return Results.Ok(accidents);
        })
        .WithName("GetHistoriqueAccidents")
        .WithOpenApi();
    }
}
