using SanteSenegal.Domain.Enums;

namespace SanteSenegal.Domain.Entities;

public class Transaction : BaseEntity
{
    public int PaiementId { get; set; }
    public decimal Montant { get; set; }
    public DateTime DateTransaction { get; set; }
    public StatutTransaction Statut { get; set; } = StatutTransaction.EnAttente;
    public string Reference { get; set; } = string.Empty;

    // Paiement mobile
    public ModePaiement ModePaiement { get; set; }
    public string ProviderReference { get; set; } = string.Empty;
    public string? RawResponse { get; set; }
    public string? NumeroTelephone { get; set; }
    public string? MessageErreur { get; set; }

    public Paiement Paiement { get; set; } = null!;
}
