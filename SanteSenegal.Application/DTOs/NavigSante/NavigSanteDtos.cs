namespace SanteSenegal.Application.DTOs.NavigSante;

// Triage DTOs
public record TriageRequest(
    string[] Symptomes,
    int? Age,
    string? Sexe // M/F
);

public record TriageResponse(
    int Gravite,
    string NiveauSoins,
    string Message,
    string? MessageWolof,
    List<string> PathologiesPossibles,
    bool EstUrgence,
    List<StructureRecommandeeDto>? StructuresRecommandees
);

public record StructureRecommandeeDto(
    int Id,
    string Nom,
    string Type,
    string Adresse,
    string? Telephone,
    double? DistanceKm,
    int? TempsAttenteMinutes
);

// Alerte DTOs
public record AlerteResponse(
    int Id,
    string Titre,
    string Description,
    string Maladie,
    string NiveauAlerte,
    string? Region,
    int? CasConfirmes,
    int? CasSuspects,
    bool EstActive,
    DateTime DateDebut
);

// Article Info Santé DTOs
public record ArticleResponse(
    int Id,
    string Titre,
    string Resume,
    string Categorie,
    string? ImageUrl,
    DateTime? DatePublication,
    int NombreVues,
    string? Tags
);

// Ressource Sanitaire DTOs
public record RessourceSanitaireResponse(
    int StructureId,
    string NomStructure,
    int LitsDisponibles,
    int LitsReanimationDisponibles,
    int LitsMaterniteDisponibles,
    int StockSangOPlus,
    bool ScannerFonctionnel,
    bool RadioFonctionnel,
    bool LaboFonctionnel,
    int? TempsAttenteUrgencesMinutes,
    DateTime DerniereMiseAJour
);
