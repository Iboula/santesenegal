using Microsoft.EntityFrameworkCore;
using SanteSenegal.Domain.Entities.NavigSante;
using SanteSenegal.Infrastructure.Persistence;

namespace SanteSenegal.Api.Endpoints.NavigSante;

public static class TriageEndpoints
{
    public static void MapTriageEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/triage");

        // ── PUBLIC — Évaluer les symptômes ──
        group.MapPost("/evaluer", async (EvaluerSymptomesRequest request, SanteDbContext db) =>
        {
            var scoreGravite = CalculerScoreGravite(request.Symptomes, request.Age);
            var orientation = DeterminerOrientation(scoreGravite, request.Symptomes);
            var niveau = scoreGravite switch
            {
                >= 8 => "rouge",
                >= 5 => "orange",
                >= 3 => "jaune",
                _ => "vert"
            };

            // Structures recommandées selon la gravité
            var structures = new List<object>();
            if (scoreGravite >= 5)
            {
                structures = await db.Structures
                    .Where(s => s.Type == Domain.Enums.TypeStructure.Hopital && s.EstActif)
                    .Take(3)
                    .Select(s => new { s.Id, s.Nom, s.Adresse, s.Telephone })
                    .ToListAsync<object>();
            }
            else if (scoreGravite >= 3)
            {
                structures = await db.Structures
                    .Where(s => (s.Type == Domain.Enums.TypeStructure.Hopital || s.Type == Domain.Enums.TypeStructure.CentreDeSante) && s.EstActif)
                    .Take(3)
                    .Select(s => new { s.Id, s.Nom, s.Adresse, s.Telephone })
                    .ToListAsync<object>();
            }

            var resultat = new
            {
                Score = scoreGravite,
                Niveau = niveau,
                Titre = orientation.Titre,
                Message = orientation.Message,
                Delai = orientation.Delai,
                SpecialiteRecommandee = orientation.Specialite,
                StructuresProches = structures,
                Conseils = orientation.Conseils,
                AlerteUrgence = scoreGravite >= 8
            };

            return Results.Ok(resultat);
        });

        // ── PUBLIC — Liste des symptômes référencés ──
        group.MapGet("/symptomes", async (SanteDbContext db) =>
        {
            var symptomes = await db.Symptomes
                .Select(s => new { s.Nom, s.NomWolof, s.Description, s.Gravite })
                .ToListAsync();
            return Results.Ok(symptomes);
        });
    }

    private static int CalculerScoreGravite(List<string> symptomes, int age)
    {
        var scores = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase)
        {
            ["Difficulté à respirer"] = 4,
            ["Douleur thoracique"] = 4,
            ["Saignement"] = 4,
            ["Fièvre"] = 2,
            ["Toux"] = 2,
            ["Vomissements"] = 2,
            ["Diarrhée"] = 2,
            ["Maux de tête"] = 1,
            ["Éruption cutanée"] = 1,
            ["Fatigue"] = 1
        };

        int score = symptomes.Sum(s => scores.GetValueOrDefault(s, 1));

        // Bonus âge
        if (age < 2 || age > 65) score += 2;
        else if (age < 5 || age > 50) score += 1;

        return Math.Min(score, 10);
    }

    private static (string Titre, string Message, string Delai, string Specialite, List<string> Conseils) DeterminerOrientation(int score, List<string> symptomes)
    {
        return score switch
        {
            >= 8 => (
                "URGENCE IMMÉDIATE",
                "Vos symptômes sont très graves. Appelez le 800 00 50 50 ou rendez-vous immédiatement aux urgences.",
                "Immédiat",
                "Urgences",
                new List<string> { "Ne vous déplacez pas seul", "Appelez une ambulance si possible", "Ayez vos documents d'identité" }
            ),
            >= 5 => (
                "Consultation urgente recommandée",
                "Vos symptômes nécessitent une évaluation médicale dans les 2 heures.",
                "< 2 heures",
                "Médecin généraliste ou urgentiste",
                new List<string> { "Rendez-vous à l'hôpital le plus proche", "Hydratez-vous", "Reposez-vous" }
            ),
            >= 3 => (
                "Consultation recommandée",
                "Vos symptômes nécessitent une évaluation médicale dans les 24 heures.",
                "< 24 heures",
                "Médecin généraliste",
                new List<string> { "Prenez rendez-vous dans un centre de santé", "Surveillez l'évolution", "Hydratez-vous" }
            ),
            _ => (
                "Auto-soins recommandés",
                "Vos symptômes sont légers. Surveillez leur évolution et consultez si ils persistent plus de 3 jours.",
                "3 jours",
                "Pharmacie",
                new List<string> { "Reposez-vous", "Buvez beaucoup d'eau", "Consultez un pharmacien pour des conseils" }
            )
        };
    }
}

public record EvaluerSymptomesRequest(List<string> Symptomes, int Age, string? Sexe);
