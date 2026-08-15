using SanteSenegal.Application.Common;
using SanteSenegal.Domain.Entities;
using SanteSenegal.Domain.Enums;

namespace SanteSenegal.Application.Abstractions;

public interface INotificationService
{
    // Envoi immédiat
    Task<Result<Notification>> EnvoyerSMSAsync(
        string telephone,
        string message,
        TypeNotification type,
        string? referenceType = null,
        int? referenceId = null,
        string langue = "fr",
        CancellationToken ct = default);

    // Envoi planifié
    Task<Result<Notification>> PlanifierSMSAsync(
        string telephone,
        string message,
        TypeNotification type,
        DateTime dateEnvoi,
        string? referenceType = null,
        int? referenceId = null,
        string langue = "fr",
        CancellationToken ct = default);

    // Templates métier
    Task<Result<Notification>> EnvoyerRappelRendezVousAsync(
        int rendezVousId,
        string langue = "fr",
        CancellationToken ct = default);

    Task<Result<Notification>> EnvoyerConfirmationPaiementAsync(
        int paiementId,
        string langue = "fr",
        CancellationToken ct = default);

    Task<Result<Notification>> EnvoyerAlerteEpidemiologiqueAsync(
        string telephone,
        string titreAlerte,
        string conseils,
        string langue = "fr",
        CancellationToken ct = default);

    Task<Result<Notification>> EnvoyerConfirmationRendezVousAsync(
        int rendezVousId,
        string langue = "fr",
        CancellationToken ct = default);

    // Gestion
    Task<List<Notification>> GetNotificationsEnAttenteAsync(CancellationToken ct = default);
    Task<Result<bool>> RetenterEnvoiAsync(int notificationId, CancellationToken ct = default);
    Task<Result<bool>> AnnulerNotificationAsync(int notificationId, CancellationToken ct = default);
}
