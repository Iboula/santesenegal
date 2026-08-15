namespace SanteSenegal.Domain.Entities.NavigSante;

/// <summary>
/// Représente un accident de la route signalé via NavigSante
/// </summary>
public class AccidentRoute : BaseEntity
{
    // Localisation
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public string? AdresseApproximative { get; set; }
    public string? Route { get; set; } // N1, N2, Autoroute...
    public string? PointKilometrique { get; set; } // PK 45
    
    // Détails de l'accident
    public DateTime DateAccident { get; set; } = DateTime.UtcNow;
    public string? Description { get; set; }
    public int NombreVictimesEstime { get; set; } = 1;
    public int NombreBlessesGraves { get; set; } = 0;
    public int NombreDeces { get; set; } = 0;
    
    // Type
    public string? TypeAccident { get; set; } // Collision, Roulage, Piéton, Moto...
    public string? TypeVehicule { get; set; } // Voiture, Camion, Moto, Bus...
    public bool RisqueIncendie { get; set; } = false;
    public bool FuiteCarburant { get; set; } = false;
    public bool RouteBloquee { get; set; } = false;
    
    // Météo et conditions
    public string? ConditionsMeteo { get; set; } // Pluie, Brouillard, Nuit...
    public string? EtatRoute { get; set; } // Bon, Mauvais, Très mauvais...
    
    // Signalement
    public string? NomSignaleur { get; set; }
    public string? TelephoneSignaleur { get; set; }
    public string? PhotoUrl { get; set; }
    
    // Statut
    public StatutAccident Statut { get; set; } = StatutAccident.Signale;
    
    // Réponse
    public DateTime? DatePriseEnCharge { get; set; }
    public int? StructureIdIntervention { get; set; }
    public string? NotesIntervention { get; set; }
    
    // Historique
    public DateTime? DateCloture { get; set; }
    public string? RapportIntervention { get; set; }
}

public enum StatutAccident
{
    Signale,           // Signalé par un témoin
    Confirme,          // Confirmé par les autorités
    PriseEnCharge,     // Équipe sur place
    VictimesEvacuees,  // Transport vers structure
    Cloture            // Accident clôturé
}
