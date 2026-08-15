using SanteSenegal.Domain.Enums;

namespace SanteSenegal.Application.DTOs.Notifications;

public record EnvoyerSMSRequestDto(
    string Telephone,
    string Message,
    TypeNotification Type,
    string? ReferenceType = null,
    int? ReferenceId = null,
    string Langue = "fr"
);

public record NotificationResponseDto(
    int Id,
    string Type,
    string Statut,
    string Canal,
    string DestinataireTelephone,
    string Message,
    string? MessageWolof,
    string Langue,
    DateTime? DatePlanifie,
    DateTime? DateEnvoi,
    int Tentatives,
    string? Erreur,
    string? ProviderUtilise
);

public record NotificationPlanifierRequestDto(
    string Telephone,
    string Message,
    TypeNotification Type,
    DateTime DateEnvoi,
    string? ReferenceType = null,
    int? ReferenceId = null,
    string Langue = "fr"
);

public record RappelRendezVousRequestDto(
    int RendezVousId,
    string Langue = "fr"
);

public record AlerteEpidemiologiqueRequestDto(
    List<string> Telephones,
    string TitreAlerte,
    string Conseils,
    string Langue = "fr"
);

public record NotificationStatsDto(
    int TotalEnvoyes,
    int TotalEnAttente,
    int TotalEchecs,
    decimal? CoutTotal,
    List<NotificationParTypeDto> ParType
);

public record NotificationParTypeDto(
    string Type,
    int Count
);
