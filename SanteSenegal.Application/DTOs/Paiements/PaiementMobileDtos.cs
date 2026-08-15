using SanteSenegal.Domain.Enums;

namespace SanteSenegal.Application.DTOs.Paiements;

public record PaiementInitierRequestDto(
    int RendezVousId,
    decimal Montant,
    ModePaiement ModePaiement,
    string NumeroTelephone,
    string? Description = null
);

public record PaiementInitierResponseDto(
    int PaiementId,
    string Statut,
    string? ReferenceExterne,
    string? UrlPaiement,
    string? CodeConfirmation,
    DateTime? DateExpiration,
    string? Message
);

public record PaiementVerifierRequestDto(
    int PaiementId,
    string? ReferenceExterne = null
);

public record PaiementVerifierResponseDto(
    int PaiementId,
    bool EstPaye,
    string Statut,
    decimal? MontantPaye,
    DateTime? DateVerification,
    string? Message
);

public record PaiementMobileConfigDto(
    string Operateur,
    string Nom,
    bool Disponible,
    string? LogoUrl,
    decimal? FraisFixe,
    decimal? FraisPourcentage
);

public record PaiementRemboursementRequestDto(
    int PaiementId,
    string? Motif = null
);

public record PaiementHistoriqueDto(
    int Id,
    int RendezVousId,
    decimal Montant,
    string Statut,
    string ModePaiement,
    string NumeroTelephone,
    string? ReferenceExterne,
    DateTime DateCreation,
    DateTime? DatePaiementEffectif
);
