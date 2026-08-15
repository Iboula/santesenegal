using SanteSenegal.Domain.Enums;

namespace SanteSenegal.Application.DTOs.Paiements;

public record PaiementCreateDto(
    int RendezVousId,
    decimal Montant,
    ModePaiement ModePaiement,
    string NumeroTelephone,
    string? Description = null
);
