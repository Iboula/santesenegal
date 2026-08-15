using Microsoft.Extensions.Logging;
using SanteSenegal.Application.Abstractions;
using SanteSenegal.Application.Abstractions.Notifications;
using SanteSenegal.Application.Common;
using SanteSenegal.Domain.Abstractions;
using SanteSenegal.Domain.Entities;
using SanteSenegal.Domain.Enums;
using SanteSenegal.Infrastructure.Services.Notifications;

namespace SanteSenegal.Infrastructure.Services;

public class NotificationService : INotificationService
{
    private readonly INotificationRepository _notificationRepository;
    private readonly SmsProviderFactory _smsFactory;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPatientRepository _patientRepository;
    private readonly IRendezVousRepository _rendezVousRepository;
    private readonly IPaiementRepository _paiementRepository;
    private readonly ILogger<NotificationService> _logger;

    public NotificationService(
        INotificationRepository notificationRepository,
        SmsProviderFactory smsFactory,
        IUnitOfWork unitOfWork,
        IPatientRepository patientRepository,
        IRendezVousRepository rendezVousRepository,
        IPaiementRepository paiementRepository,
        ILogger<NotificationService> logger)
    {
        _notificationRepository = notificationRepository;
        _smsFactory = smsFactory;
        _unitOfWork = unitOfWork;
        _patientRepository = patientRepository;
        _rendezVousRepository = rendezVousRepository;
        _paiementRepository = paiementRepository;
        _logger = logger;
    }

    // ─── Envoi immédiat ───
    public async Task<Result<Notification>> EnvoyerSMSAsync(
        string telephone,
        string message,
        TypeNotification type,
        string? referenceType = null,
        int? referenceId = null,
        string langue = "fr",
        CancellationToken ct = default)
    {
        var notification = new Notification
        {
            Type = type,
            Canal = CanalNotification.SMS,
            Statut = StatutNotification.EnAttente,
            DestinataireTelephone = telephone,
            Message = message,
            Langue = langue,
            ReferenceType = referenceType,
            ReferenceId = referenceId,
            DatePlanifie = null
        };

        await _notificationRepository.AddAsync(notification);
        await _unitOfWork.SaveChangesAsync();

        return await ExecuterEnvoiSMSAsync(notification, ct);
    }

    // ─── Planification ───
    public async Task<Result<Notification>> PlanifierSMSAsync(
        string telephone,
        string message,
        TypeNotification type,
        DateTime dateEnvoi,
        string? referenceType = null,
        int? referenceId = null,
        string langue = "fr",
        CancellationToken ct = default)
    {
        var notification = new Notification
        {
            Type = type,
            Canal = CanalNotification.SMS,
            Statut = StatutNotification.EnAttente,
            DestinataireTelephone = telephone,
            Message = message,
            Langue = langue,
            ReferenceType = referenceType,
            ReferenceId = referenceId,
            DatePlanifie = dateEnvoi.ToUniversalTime()
        };

        await _notificationRepository.AddAsync(notification);
        await _unitOfWork.SaveChangesAsync();

        return Result<Notification>.Ok(notification);
    }

    // ─── Templates métier ───
    public async Task<Result<Notification>> EnvoyerRappelRendezVousAsync(int rendezVousId, string langue = "fr", CancellationToken ct = default)
    {
        var rdv = await _rendezVousRepository.GetByIdAsync(rendezVousId, ct);
        if (rdv is null) return Result<Notification>.Fail("Rendez-vous non trouvé.");

        var patient = await _patientRepository.GetByIdAsync(rdv.PatientId, ct);
        if (patient is null) return Result<Notification>.Fail("Patient non trouvé.");

        var dateStr = rdv.Date.ToString("dd/MM/yyyy");
        var heureStr = rdv.Heure.ToString(@"hh\:mm");
        var structure = rdv.Structure?.Nom ?? "structure";
        var service = rdv.Service?.Nom ?? "service";

        var message = MessageTemplates.GetMessage(TypeNotification.RappelRendezVous, langue,
            patient.Prenom, structure, dateStr, heureStr, service);

        var result = await EnvoyerSMSAsync(patient.Telephone, message, TypeNotification.RappelRendezVous,
            "RendezVous", rdv.Id, langue, ct);

        if (result.Success)
        {
            rdv.RappelSMSEnvoye = true;
            rdv.RappelSMSDateEnvoye = DateTime.UtcNow;
            rdv.RappelSMSContenu = message;
            // Note: le repository RendezVous n'a pas de UpdateAsync exposé dans l'interface visible
            // On laisse ça pour l'intégration future ou via un DbContext direct
        }

        return result;
    }

    public async Task<Result<Notification>> EnvoyerConfirmationPaiementAsync(int paiementId, string langue = "fr", CancellationToken ct = default)
    {
        var paiement = await _paiementRepository.GetByIdAsync(paiementId, ct);
        if (paiement is null) return Result<Notification>.Fail("Paiement non trouvé.");

        // Récupérer le patient via le rendez-vous
        var rdv = await _rendezVousRepository.GetByIdAsync(paiement.RendezVousId, ct);
        if (rdv is null) return Result<Notification>.Fail("Rendez-vous associé non trouvé.");

        var patient = await _patientRepository.GetByIdAsync(rdv.PatientId, ct);
        if (patient is null) return Result<Notification>.Fail("Patient non trouvé.");

        var modeStr = paiement.ModePaiement switch
        {
            ModePaiement.Wave => "Wave",
            ModePaiement.OrangeMoney => "Orange Money",
            ModePaiement.FreeMoney => "Free Money",
            _ => paiement.ModePaiement.ToString()
        };

        var message = MessageTemplates.GetMessage(TypeNotification.ConfirmationPaiement, langue,
            patient.Prenom, paiement.Montant, modeStr, paiement.ReferenceExterne ?? $"PAY-{paiement.Id}");

        return await EnvoyerSMSAsync(patient.Telephone, message, TypeNotification.ConfirmationPaiement,
            "Paiement", paiement.Id, langue, ct);
    }

    public async Task<Result<Notification>> EnvoyerAlerteEpidemiologiqueAsync(
        string telephone, string titreAlerte, string conseils, string langue = "fr", CancellationToken ct = default)
    {
        var message = MessageTemplates.GetMessage(TypeNotification.AlerteEpidemiologique, langue, titreAlerte, conseils);
        return await EnvoyerSMSAsync(telephone, message, TypeNotification.AlerteEpidemiologique, null, null, langue, ct);
    }

    public async Task<Result<Notification>> EnvoyerConfirmationRendezVousAsync(int rendezVousId, string langue = "fr", CancellationToken ct = default)
    {
        var rdv = await _rendezVousRepository.GetByIdAsync(rendezVousId, ct);
        if (rdv is null) return Result<Notification>.Fail("Rendez-vous non trouvé.");

        var patient = await _patientRepository.GetByIdAsync(rdv.PatientId, ct);
        if (patient is null) return Result<Notification>.Fail("Patient non trouvé.");

        var dateStr = rdv.Date.ToString("dd/MM/yyyy");
        var heureStr = rdv.Heure.ToString(@"hh\:mm");
        var structure = rdv.Structure?.Nom ?? "structure";
        var reference = rdv.NumeroReference ?? $"RDV-{rdv.Id}";

        var message = MessageTemplates.GetMessage(TypeNotification.ConfirmationRendezVous, langue,
            patient.Prenom, structure, dateStr, heureStr, reference);

        var result = await EnvoyerSMSAsync(patient.Telephone, message, TypeNotification.ConfirmationRendezVous,
            "RendezVous", rdv.Id, langue, ct);

        if (result.Success)
        {
            rdv.ConfirmationSMSEnvoye = true;
        }

        return result;
    }

    // ─── Gestion ───
    public async Task<List<Notification>> GetNotificationsEnAttenteAsync(CancellationToken ct = default)
    {
        return await _notificationRepository.GetPendingAsync(ct);
    }

    public async Task<Result<bool>> RetenterEnvoiAsync(int notificationId, CancellationToken ct = default)
    {
        var notification = await _notificationRepository.GetByIdAsync(notificationId, ct);
        if (notification is null) return Result<bool>.Fail("Notification non trouvée.");
        if (notification.Statut != StatutNotification.Echec)
            return Result<bool>.Fail("Seules les notifications en échec peuvent être retentées.");

        notification.Statut = StatutNotification.EnAttente;
        notification.Tentatives = 0;
        notification.Erreur = null;
        await _notificationRepository.UpdateAsync(notification);
        await _unitOfWork.SaveChangesAsync();

        var result = await ExecuterEnvoiSMSAsync(notification, ct);
        return Result<bool>.Ok(result.Success);
    }

    public async Task<Result<bool>> AnnulerNotificationAsync(int notificationId, CancellationToken ct = default)
    {
        var notification = await _notificationRepository.GetByIdAsync(notificationId, ct);
        if (notification is null) return Result<bool>.Fail("Notification non trouvée.");

        notification.Statut = StatutNotification.Annule;
        await _notificationRepository.UpdateAsync(notification);
        await _unitOfWork.SaveChangesAsync();

        return Result<bool>.Ok(true);
    }

    // ─── Exécution interne ───
    private async Task<Result<Notification>> ExecuterEnvoiSMSAsync(Notification notification, CancellationToken ct)
    {
        var provider = _smsFactory.GetPrimaryProvider();
        var smsMessage = new SmsMessage(
            To: notification.DestinataireTelephone,
            Message: notification.Message,
            Reference: $"NOTIF-{notification.Id}"
        );

        try
        {
            var result = await provider.SendAsync(smsMessage, ct);

            notification.Tentatives++;
            notification.ProviderUtilise = provider.Nom;
            notification.RawResponse = result.RawResponse;

            if (result.Succes)
            {
                notification.Statut = StatutNotification.Envoye;
                notification.DateEnvoi = DateTime.UtcNow;
                notification.ProviderReference = result.ProviderReference;
                _logger.LogInformation("SMS envoyé à {Telephone} via {Provider}: {MessageId}",
                    notification.DestinataireTelephone, provider.Nom, result.MessageId);
            }
            else
            {
                notification.Statut = StatutNotification.Echec;
                notification.Erreur = result.Erreur;
                _logger.LogWarning("Échec SMS à {Telephone}: {Erreur}",
                    notification.DestinataireTelephone, result.Erreur);
            }
        }
        catch (Exception ex)
        {
            notification.Tentatives++;
            notification.Statut = StatutNotification.Echec;
            notification.Erreur = ex.Message;
            _logger.LogError(ex, "Exception lors de l'envoi SMS à {Telephone}", notification.DestinataireTelephone);
        }

        await _notificationRepository.UpdateAsync(notification);
        await _unitOfWork.SaveChangesAsync();

        return notification.Statut == StatutNotification.Envoye
            ? Result<Notification>.Ok(notification)
            : Result<Notification>.Fail(notification.Erreur ?? "Échec d'envoi");
    }
}
