using SanteSenegal.Domain.Entities.NavigSante;

namespace SanteSenegal.Application.DTOs.NavigSante;

// Signalement Accident
public record SignalerAccidentRequest(
    double Latitude,
    double Longitude,
    string? AdresseApproximative,
    string? Route,
    string? PointKilometrique,
    string? Description,
    int NombreVictimesEstime,
    string? TypeAccident,
    string? TypeVehicule,
    bool RisqueIncendie,
    bool FuiteCarburant,
    bool RouteBloquee,
    string? ConditionsMeteo,
    string? NomSignaleur,
    string? TelephoneSignaleur,
    string? PhotoUrl
);

public record AccidentResponse(
    int Id,
    double Latitude,
    double Longitude,
    string? AdresseApproximative,
    string? Route,
    DateTime DateAccident,
    int NombreVictimesEstime,
    int NombreBlessesGraves,
    int NombreDeces,
    string? TypeAccident,
    string? TypeVehicule,
    bool RisqueIncendie,
    bool FuiteCarburant,
    bool RouteBloquee,
    string? ConditionsMeteo,
    string Statut,
    DateTime? DatePriseEnCharge,
    DateTime? DateCloture
);

// Zones à risque
public record ZoneNoireDto(
    string Route,
    string? PointKilometrique,
    double Latitude,
    double Longitude,
    int NombreAccidents,
    int NombreDeces,
    int NombreBlesses,
    string NiveauRisque // Rouge, Orange, Jaune
);

// Statistiques
public record StatistiquesAccidentDto(
    int TotalAccidents,
    int TotalDeces,
    int TotalBlesses,
    double TauxMortalite, // %
    int AccidentsAujourdhui,
    int AccidentsCetteSemaine,
    int AccidentsCeMois,
    string? RouteLaPlusDangereuse,
    string? HeureLaPlusDangereuse,
    List<AccidentParRegionDto> AccidentsParRegion
);

public record AccidentParRegionDto(
    string Region,
    int NombreAccidents,
    int NombreDeces
);

// Mise à jour statut (pour SAMU/Admin)
public record MettreAJourStatutAccidentRequest(
    StatutAccident NouveauStatut,
    int? StructureIdIntervention,
    string? NotesIntervention
);
