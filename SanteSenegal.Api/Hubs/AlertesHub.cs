using Microsoft.AspNetCore.SignalR;
using SanteSenegal.Domain.Entities.NavigSante;

namespace SanteSenegal.Api.Hubs;

/// <summary>
/// Hub SignalR pour les alertes temps réel (accidents, épidémies, ressources)
/// </summary>
public class AlertesHub : Hub
{
    // Groupe par région pour les alertes géolocalisées
    public async Task RejoindreGroupeRegion(string region)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, region);
    }

    public async Task QuitterGroupeRegion(string region)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, region);
    }

    // Groupe par rôle (Admin, Medecin, AgentSante)
    public async Task RejoindreGroupeRole(string role)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, $"role:{role}");
    }

    // ── Diffusion alertes ──

    /// <summary>
    /// Diffuse une alerte accident à tous les agents connectés
    /// </summary>
    public static async Task DiffuserAlerteAccident(IHubContext<AlertesHub> hub, AccidentRoute accident, List<StructureBasicInfo> hopitauxProches)
    {
        await hub.Clients.Group("role:AgentSante").SendAsync("NouvelAccident", new
        {
            AccidentId = accident.Id,
            Latitude = accident.Latitude,
            Longitude = accident.Longitude,
            Adresse = accident.AdresseApproximative,
            NombreVictimes = accident.NombreVictimesEstime,
            Gravite = EvaluerGravite(accident),
            HopitauxRecommandes = hopitauxProches
        });
    }

    /// <summary>
    /// Notifie qu'un accident a été pris en charge
    /// </summary>
    public static async Task NotifierPriseEnCharge(IHubContext<AlertesHub> hub, int accidentId, string hopitalNom)
    {
        await hub.Clients.All.SendAsync("AccidentPrisEnCharge", new
        {
            AccidentId = accidentId,
            Hopital = hopitalNom,
            Heure = DateTime.UtcNow
        });
    }

    /// <summary>
    /// Diffuse une alerte épidémiologique par région
    /// </summary>
    public static async Task DiffuserAlerteEpidemio(IHubContext<AlertesHub> hub, string region, AlerteEpidemiologique alerte)
    {
        await hub.Clients.Group(region).SendAsync("AlerteEpidemiologique", new
        {
            Titre = alerte.Titre,
            Niveau = alerte.NiveauAlerte,
            Maladie = alerte.Maladie,
            Region = alerte.Region,
            Mesures = alerte.MesuresPrevention
        });
    }

    /// <summary>
    /// Mise à jour temps réel des ressources sanitaires (lits, sang)
    /// </summary>
    public static async Task MettreAJourRessources(IHubContext<AlertesHub> hub, int structureId, RessourceSanitaire ressources)
    {
        await hub.Clients.All.SendAsync("RessourcesMiseAJour", new
        {
            StructureId = structureId,
            LitsDisponibles = ressources.LitsDisponibles,
            LitsReanimationDisponibles = ressources.LitsReanimationDisponibles,
            SangOPlus = ressources.StockSangOPlus,
            TempsAttente = ressources.TempsAttenteUrgencesMinutes,
            DerniereMiseAJour = ressources.DerniereMiseAJour
        });
    }

    private static string EvaluerGravite(AccidentRoute accident)
    {
        if (accident.NombreDeces > 0 || accident.RisqueIncendie) return "CRITIQUE";
        if (accident.NombreBlessesGraves > 2) return "GRAVE";
        if (accident.RouteBloquee) return "ELEVEE";
        return "MODEREE";
    }
}

public record StructureBasicInfo(int Id, string Nom, string Adresse, int LitsReaDispo, int DistanceKm);
