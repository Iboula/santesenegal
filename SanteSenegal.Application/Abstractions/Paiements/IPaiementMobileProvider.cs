using SanteSenegal.Domain.Enums;

namespace SanteSenegal.Application.Abstractions.Paiements;

public interface IPaiementMobileProvider
{
    ModePaiement Mode { get; }
    Task<InitierPaiementResult> InitierPaiementAsync(InitierPaiementRequest request, CancellationToken ct = default);
    Task<VerifierPaiementResult> VerifierPaiementAsync(string referenceExterne, CancellationToken ct = default);
}

public record InitierPaiementRequest(
    decimal Montant,
    string NumeroTelephone,
    string Devise = "XOF",
    string? Description = null,
    string? ReferenceClient = null
);

public record InitierPaiementResult(
    bool Succes,
    string? ReferenceExterne,
    string? UrlPaiement,
    string? CodeConfirmation,
    DateTime? DateExpiration,
    string? Message,
    string? RawResponse
);

public record VerifierPaiementResult(
    bool Succes,
    bool EstPaye,
    string? ReferenceExterne,
    decimal? MontantPaye,
    DateTime? DatePaiement,
    string? Message,
    string? RawResponse
);
