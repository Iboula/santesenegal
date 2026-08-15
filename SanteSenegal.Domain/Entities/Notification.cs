using SanteSenegal.Domain.Enums;

namespace SanteSenegal.Domain.Entities;

public class Notification : BaseEntity
{
    public TypeNotification Type { get; set; }
    public CanalNotification Canal { get; set; } = CanalNotification.SMS;
    public StatutNotification Statut { get; set; } = StatutNotification.EnAttente;

    public string DestinataireTelephone { get; set; } = string.Empty;
    public string? DestinataireEmail { get; set; }
    public string? NomDestinataire { get; set; }

    public string Message { get; set; } = string.Empty;
    public string? MessageWolof { get; set; }
    public string Langue { get; set; } = "fr"; // fr, wo

    public DateTime? DatePlanifie { get; set; }
    public DateTime? DateEnvoi { get; set; }
    public int Tentatives { get; set; }
    public string? Erreur { get; set; }

    // Référence métier
    public int? ReferenceId { get; set; } // Id du RDV, Paiement, etc.
    public string? ReferenceType { get; set; } // "RendezVous", "Paiement", etc.

    // Provider
    public string? ProviderReference { get; set; }
    public string? ProviderUtilise { get; set; }
    public string? RawResponse { get; set; }
}
