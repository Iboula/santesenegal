using SanteSenegal.Application.Abstractions.NavigSante;
using SanteSenegal.Domain.Entities.NavigSante;
using SanteSenegal.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace SanteSenegal.Infrastructure.Services.NavigSante;

public class TriageService : ITriageService
{
    private readonly SanteDbContext _context;

    public TriageService(SanteDbContext context)
    {
        _context = context;
    }

    public async Task<OrientationResult> TrierAsync(string[] symptomes, int? age, string? sexe)
    {
        // Moteur de triage simplifié basé sur des règles
        var gravite = CalculerGravite(symptomes);
        var (niveauSoins, message, messageWolof) = GenererOrientation(gravite, symptomes);
        
        var pathologies = await IdentifierPathologies(symptomes);
        
        var structures = gravite >= 3 
            ? await GetStructuresUrgenceProches() 
            : await GetStructuresConsultationProches();

        return new OrientationResult(
            gravite,
            niveauSoins,
            message,
            messageWolof,
            pathologies.Select(p => p.Nom).ToList(),
            gravite >= 3,
            structures
        );
    }

    public async Task<List<Symptome>> GetSymptomesAsync()
        => await _context.Symptomes.ToListAsync();

    public async Task<List<Pathologie>> GetPathologiesAsync()
        => await _context.Pathologies.ToListAsync();

    private int CalculerGravite(string[] symptomes)
    {
        var symptomesCritiques = new[] { 
            "difficulte_respirer", "perte_conscience", "saignement_abondant", 
            "douleur_poitrine", "convulsions", "trauma_grave", "brulures_graves",
            "fievre_bebe_3mois", "grossesse_douleur", "etouffement" 
        };
        
        var symptomesUrgents = new[] { 
            "fievre_elevee", "vomissements_persistants", "diarrhee_sanglante",
            "douleur_abdominale_aigue", "maux_de_tete_intense", "faiblesse_bras_jambe",
            "vision_trouble", "confusion", "douleur_oreille_bebe", "fracture" 
        };

        var symptomesModeres = new[] { 
            "toux", "fievre", "maux_de_tete", "mal_de_gorge", 
            "nez_bouche", "douleur_oreille", "eruption_cutanee", "brulure_legere" 
        };

        var symptomesLegers = new[] { 
            "fatigue", "eternuements", "yeux_rouges", "griffure", 
            "piqre_insecte", "renvoi", "constipation" 
        };

        if (symptomes.Any(s => symptomesCritiques.Contains(s.ToLower())))
            return 4; // CRITIQUE
        
        if (symptomes.Any(s => symptomesUrgents.Contains(s.ToLower())))
            return 3; // URGENT
        
        if (symptomes.Any(s => symptomesModeres.Contains(s.ToLower())))
            return 2; // MODÉRÉ
        
        return 1; // LÉGER
    }

    private (string niveau, string msg, string? msgWolof) GenererOrientation(int gravite, string[] symptomes)
    {
        return gravite switch
        {
            4 => (
                "URGENCE_IMMEDIATE",
                "🚨 URGENCE VITALE ! Appelez immédiatement le SAMU au 15 ou rendez-vous aux urgences la plus proche. Ne perdez pas de temps !",
                "🚨 KEMU BU MAG ! Bëggal SAMU 15 bi ci loolu. Bul wàññi sa waxtu !"
            ),
            3 => (
                "URGENCE_RELATIVE",
                "⚠️ Votre état nécessite une consultation urgente dans les 4 heures. Rendez-vous à un centre de santé ou hôpital avec urgences.",
                "⚠️ Danga amoon na loo bëgg seen doktër ci 4 waxtu yi. Demal dëkk bi am gis-gis."
            ),
            2 => (
                "CONSULTATION_PROGRAMMEE",
                "📅 Vos symptômes nécessitent une consultation médicale. Prenez rendez-vous dans la semaine à un centre de santé.",
                "📅 Danga bëgg gis seen doktër. Jëlal benn fanaan ci ayubis bi."
            ),
            _ => (
                "AUTO_SOIN",
                "💚 Vos symptômes semblent légers. Reposez-vous, buvez beaucoup d'eau, et surveillez l'évolution. Consultez si les symptômes persistent plus de 3 jours.",
                "💚 Danga fiif. Nelawal, naan ndox, te koolu ci sa yaram. Gisal doktër su fekkee ne duma dara ci 3 fan yi."
            )
        };
    }

    private async Task<List<Pathologie>> IdentifierPathologies(string[] symptomes)
    {
        // Logique simplifiée - en production, utiliser un vrai moteur d'inférence
        var pathologies = new List<Pathologie>();
        
        if (symptomes.Contains("fievre") && symptomes.Contains("maux_de_tete") && symptomes.Contains("fatigue"))
            pathologies.Add(new Pathologie { Nom = "Paludisme (suspect)", NiveauGravite = 3 });
        
        if (symptomes.Contains("toux") && symptomes.Contains("fievre") && symptomes.Contains("difficulte_respirer"))
            pathologies.Add(new Pathologie { Nom = "Pneumonie (suspect)", NiveauGravite = 3 });
        
        if (symptomes.Contains("vomissements") && symptomes.Contains("diarrhee"))
            pathologies.Add(new Pathologie { Nom = "Gastro-entérite", NiveauGravite = 2 });
        
        if (symptomes.Contains("douleur_poitrine") || symptomes.Contains("difficulte_respirer"))
            pathologies.Add(new Pathologie { Nom = "Problème cardiaque/respiratoire", NiveauGravite = 4 });

        if (pathologies.Count == 0)
            pathologies.Add(new Pathologie { Nom = "Symptômes non spécifiques", NiveauGravite = 1 });

        return pathologies;
    }

    private async Task<List<StructureProcheDto>> GetStructuresUrgenceProches()
    {
        return await _context.Structures
            .Where(s => s.Type == Domain.Enums.TypeStructure.Hopital || s.Type == Domain.Enums.TypeStructure.CentreDeSante)
            .Where(s => s.EstActif)
            .Select(s => new StructureProcheDto(
                s.Id,
                s.Nom,
                s.Type.ToString(),
                s.Adresse,
                s.Telephone,
                null,
                null
            ))
            .Take(3)
            .ToListAsync();
    }

    private async Task<List<StructureProcheDto>> GetStructuresConsultationProches()
    {
        return await _context.Structures
            .Where(s => s.EstActif)
            .Select(s => new StructureProcheDto(
                s.Id,
                s.Nom,
                s.Type.ToString(),
                s.Adresse,
                s.Telephone,
                null,
                null
            ))
            .Take(5)
            .ToListAsync();
    }
}
