using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using SanteSenegal.Application.Abstractions.NavigSante;
using SanteSenegal.Application.DTOs.NavigSante;
using SanteSenegal.Domain.Entities.NavigSante;
using SanteSenegal.Infrastructure.Persistence;

namespace SanteSenegal.Api.Endpoints;

public static class NavigSanteEndpoints
{
    public static void MapNavigSanteEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/navigsante");

        // ─── TRIAGE SYMPTOMATIQUE ───

        // POST /api/navigsante/triage
        group.MapPost("/triage", [AllowAnonymous] async (TriageRequest request, ITriageService triageService) =>
        {
            var result = await triageService.TrierAsync(request.Symptomes, request.Age, request.Sexe);
            return Results.Ok(new TriageResponse(
                result.Gravite,
                result.NiveauSoins,
                result.Message,
                result.MessageWolof,
                result.PathologiesPossibles,
                result.EstUrgence,
                result.StructuresRecommandees?.Select(s => new StructureRecommandeeDto(
                    s.Id, s.Nom, s.Type, s.Adresse, s.Telephone, s.DistanceKm, s.TempsAttenteMinutes
                )).ToList()
            ));
        })
        .WithName("TriageSymptomatique")
        .WithOpenApi();

        // GET /api/navigsante/symptomes
        group.MapGet("/symptomes", [AllowAnonymous] async (ITriageService triageService) =>
        {
            var symptomes = await triageService.GetSymptomesAsync();
            return Results.Ok(symptomes.Select(s => new
            {
                s.Id,
                s.Nom,
                s.NomWolof,
                s.Description,
                s.Gravite,
                s.ZoneCorporelle
            }));
        })
        .WithName("GetSymptomes")
        .WithOpenApi();

        // ─── ALERTES ÉPIDÉMIOLOGIQUES ───

        // GET /api/navigsante/alertes
        group.MapGet("/alertes", [AllowAnonymous] async (SanteDbContext context, string? region, bool? actives) =>
        {
            var query = context.AlertesEpidemiologiques.AsQueryable();
            
            if (!string.IsNullOrEmpty(region))
                query = query.Where(a => a.Region == region);
            
            if (actives.HasValue)
                query = query.Where(a => a.EstActive == actives.Value);

            var alertes = await query
                .OrderByDescending(a => a.DateDebut)
                .Take(20)
                .ToListAsync();

            return Results.Ok(alertes.Select(a => new AlerteResponse(
                a.Id,
                a.Titre,
                a.Description,
                a.Maladie,
                a.NiveauAlerte,
                a.Region,
                a.CasConfirmes,
                a.CasSuspects,
                a.EstActive,
                a.DateDebut
            )));
        })
        .WithName("GetAlertesEpidemiologiques")
        .WithOpenApi();

        // GET /api/navigsante/alertes/{id}
        group.MapGet("/alertes/{id}", [AllowAnonymous] async (int id, SanteDbContext context) =>
        {
            var alerte = await context.AlertesEpidemiologiques.FindAsync(id);
            if (alerte == null) return Results.NotFound();

            return Results.Ok(new AlerteResponse(
                alerte.Id,
                alerte.Titre,
                alerte.Description,
                alerte.Maladie,
                alerte.NiveauAlerte,
                alerte.Region,
                alerte.CasConfirmes,
                alerte.CasSuspects,
                alerte.EstActive,
                alerte.DateDebut
            ));
        })
        .WithName("GetAlerteById")
        .WithOpenApi();

        // POST /api/navigsante/alertes (Admin seulement)
        group.MapPost("/alertes", [Authorize(Roles = "Admin")] async (AlerteEpidemiologique alerte, SanteDbContext context) =>
        {
            await context.AlertesEpidemiologiques.AddAsync(alerte);
            await context.SaveChangesAsync();
            return Results.Created($"/api/navigsante/alertes/{alerte.Id}", alerte);
        })
        .WithName("CreateAlerte")
        .WithOpenApi();

        // ─── ARTICLES INFO-SANTÉ ───

        // GET /api/navigsante/articles
        group.MapGet("/articles", [AllowAnonymous] async (SanteDbContext context, string? categorie, string? recherche) =>
        {
            var query = context.ArticlesInfoSante
                .Where(a => a.EstPublie)
                .AsQueryable();

            if (!string.IsNullOrEmpty(categorie))
                query = query.Where(a => a.Categorie == categorie);

            if (!string.IsNullOrEmpty(recherche))
                query = query.Where(a => a.Titre.Contains(recherche) || (a.Resume != null && a.Resume.Contains(recherche)));

            var articles = await query
                .OrderByDescending(a => a.DatePublication)
                .Take(20)
                .ToListAsync();

            return Results.Ok(articles.Select(a => new ArticleResponse(
                a.Id,
                a.Titre,
                a.Resume ?? "",
                a.Categorie,
                a.ImageUrl,
                a.DatePublication,
                a.NombreVues,
                a.Tags
            )));
        })
        .WithName("GetArticlesInfoSante")
        .WithOpenApi();

        // GET /api/navigsante/articles/{id}
        group.MapGet("/articles/{id}", [AllowAnonymous] async (int id, SanteDbContext context) =>
        {
            var article = await context.ArticlesInfoSante.FindAsync(id);
            if (article == null || !article.EstPublie) return Results.NotFound();

            article.NombreVues++;
            await context.SaveChangesAsync();

            return Results.Ok(new ArticleResponse(
                article.Id,
                article.Titre,
                article.Resume ?? "",
                article.Categorie,
                article.ImageUrl,
                article.DatePublication,
                article.NombreVues,
                article.Tags
            ));
        })
        .WithName("GetArticleById")
        .WithOpenApi();

        // ─── RESSOURCES SANITAIRES ───

        // GET /api/navigsante/ressources
        group.MapGet("/ressources", [AllowAnonymous] async (SanteDbContext context, int? structureId) =>
        {
            var query = context.RessourcesSanitaires
                .Include(r => r.Structure)
                .AsQueryable();

            if (structureId.HasValue)
                query = query.Where(r => r.StructureId == structureId.Value);

            var ressources = await query.ToListAsync();

            return Results.Ok(ressources.Select(r => new RessourceSanitaireResponse(
                r.StructureId,
                r.Structure.Nom,
                r.LitsDisponibles,
                r.LitsReanimationDisponibles,
                r.LitsMaterniteDisponibles,
                r.StockSangOPlus,
                r.ScannerFonctionnel,
                r.RadioFonctionnel,
                r.LaboFonctionnel,
                r.TempsAttenteUrgencesMinutes,
                r.DerniereMiseAJour
            )));
        })
        .WithName("GetRessourcesSanitaires")
        .WithOpenApi();

        // PUT /api/navigsante/ressources/{structureId} (Admin/Medecin)
        group.MapPut("/ressources/{structureId}", [Authorize(Roles = "Medecin,Admin")] async (int structureId, RessourceSanitaire ressource, SanteDbContext context) =>
        {
            var existante = await context.RessourcesSanitaires
                .FirstOrDefaultAsync(r => r.StructureId == structureId);

            if (existante == null)
            {
                ressource.StructureId = structureId;
                ressource.DerniereMiseAJour = DateTime.UtcNow;
                await context.RessourcesSanitaires.AddAsync(ressource);
            }
            else
            {
                existante.LitsDisponibles = ressource.LitsDisponibles;
                existante.LitsReanimationDisponibles = ressource.LitsReanimationDisponibles;
                existante.LitsMaterniteDisponibles = ressource.LitsMaterniteDisponibles;
                existante.StockSangOPlus = ressource.StockSangOPlus;
                existante.ScannerFonctionnel = ressource.ScannerFonctionnel;
                existante.RadioFonctionnel = ressource.RadioFonctionnel;
                existante.LaboFonctionnel = ressource.LaboFonctionnel;
                existante.TempsAttenteUrgencesMinutes = ressource.TempsAttenteUrgencesMinutes;
                existante.DerniereMiseAJour = DateTime.UtcNow;
            }

            await context.SaveChangesAsync();
            return Results.Ok();
        })
        .WithName("UpdateRessourceSanitaire")
        .WithOpenApi();
    }
}
