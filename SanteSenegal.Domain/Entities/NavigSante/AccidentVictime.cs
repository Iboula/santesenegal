namespace SanteSenegal.Domain.Entities.NavigSante;

/// <summary>
/// Représente une victime d'un accident de la route
/// </summary>
public class AccidentVictime : BaseEntity
{
    public int AccidentRouteId { get; set; }
    public AccidentRoute AccidentRoute { get; set; } = null!;
    
    public int? AgeApproximatif { get; set; }
    public SexeVictime? Sexe { get; set; }
    public TypeBlessure TypeBlessure { get; set; } = TypeBlessure.Autre;
    public bool EstConscient { get; set; } = true;
    public MoyenTransport? TransportePar { get; set; }
    public StatutVictime Statut { get; set; } = StatutVictime.EnAttente;
    public string? Notes { get; set; }
}

public enum SexeVictime
{
    Masculin,
    Feminin
}

public enum TypeBlessure
{
    Cranien,
    Fracture,
    Brulure,
    Hemorragie,
    Thoracique,
    Abdominal,
    Autre
}

public enum MoyenTransport
{
    Ambulance,
    Particulier,
    Police,
    SapeurPompier,
    NonTransporte
}

public enum StatutVictime
{
    EnAttente,
    EnRoute,
    Admis,
    EnIntervention,
    Stable,
    Decede
}
