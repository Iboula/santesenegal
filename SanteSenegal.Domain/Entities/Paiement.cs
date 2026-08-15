using SanteSenegal.Domain.Enums;

namespace SanteSenegal.Domain.Entities;

public class Paiement : BaseEntity
{
    public int RendezVousId { get; set; }
    public RendezVous RendezVous { get; set; } = null!;
    public decimal Montant { get; set; }
    public StatutPaiement Statut { get; set; } = StatutPaiement.EnAttente;
    public DateTime DatePaiement { get; set; }

    // Paiement mobile sénégalais
    public ModePaiement ModePaiement { get; set; }
    public string NumeroTelephone { get; set; } = string.Empty;
    public string? ReferenceExterne { get; set; }
    public string? UrlPaiement { get; set; }
    public string? CodeConfirmation { get; set; }
    public DateTime? DateExpiration { get; set; }
}
