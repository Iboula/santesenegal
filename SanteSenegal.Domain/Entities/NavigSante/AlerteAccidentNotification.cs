namespace SanteSenegal.Domain.Entities.NavigSante;

/// <summary>
/// Notification envoyée lors d'un accident (SMS, Push, Email)
/// </summary>
public class AlerteAccidentNotification : BaseEntity
{
    public int AccidentRouteId { get; set; }
    public AccidentRoute AccidentRoute { get; set; } = null!;
    
    public CanalNotification Canal { get; set; }
    public string Destinataire { get; set; } = string.Empty; // Téléphone, email, token push
    public string Contenu { get; set; } = string.Empty;
    public bool EstEnvoye { get; set; }
    public DateTime? DateEnvoi { get; set; }
    public string? Erreur { get; set; }
}

public enum CanalNotification
{
    SMS,
    Push,
    Email,
    AppelVocal
}
