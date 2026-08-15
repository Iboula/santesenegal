using SanteSenegal.Domain.Enums;

namespace SanteSenegal.Application.Abstractions.Notifications;

public interface ISmsProvider
{
    string Nom { get; }
    Task<SmsSendResult> SendAsync(SmsMessage message, CancellationToken ct = default);
    Task<SmsDeliveryStatus?> GetStatusAsync(string providerReference, CancellationToken ct = default);
}

public record SmsMessage(
    string To,
    string Message,
    string? From = null,
    string? Reference = null
);

public record SmsSendResult(
    bool Succes,
    string? ProviderReference,
    string? MessageId,
    decimal? Cout,
    string? RawResponse,
    string? Erreur
);

public record SmsDeliveryStatus(
    string ProviderReference,
    bool EstLivre,
    DateTime? DateLivraison,
    string? Statut,
    string? RawResponse
);
