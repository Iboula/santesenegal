using SanteSenegal.Application.Common;
using SanteSenegal.Domain.Entities;
using SanteSenegal.Domain.Enums;

namespace SanteSenegal.Application.Abstractions;

public interface IPaiementService
{
    Task<bool> ConfirmerPaiementAsync(int rendezVousId, decimal montant);
    Task<bool> RembourserPaiementAsync(int rendezVousId, decimal montant);
    Task<IEnumerable<Paiement>> GetAllPaiementsAsync();
    Task<Paiement?> GetPaiementByIdAsync(int id);
    Task<Result<Paiement>> ProcessPaiementAsync(Paiement paiement);
    Task<IEnumerable<Paiement>> GetPaiementsByRendezVousAsync(int rendezVousId);
    Task<IEnumerable<Paiement>> GetPaiementsByPatientAsync(int patientId);

    // ── Paiement Mobile Sénégalais ──
    Task<Result<Paiement>> InitierPaiementMobileAsync(
        int rendezVousId,
        decimal montant,
        ModePaiement mode,
        string numeroTelephone,
        string? description = null);

    Task<Result<(Paiement Paiement, bool EstPaye)>> VerifierPaiementMobileAsync(int paiementId);
}
