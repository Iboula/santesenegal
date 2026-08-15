using SanteSenegal.Domain.Entities.NavigSante;

namespace SanteSenegal.Application.Abstractions.NavigSante;

public interface ITriageService
{
    Task<OrientationResult> TrierAsync(string[] symptomes, int? age, string? sexe);
    Task<List<Symptome>> GetSymptomesAsync();
    Task<List<Pathologie>> GetPathologiesAsync();
}

public record OrientationResult(
    int Gravite,
    string NiveauSoins,
    string Message,
    string? MessageWolof,
    List<string> PathologiesPossibles,
    bool EstUrgence,
    List<StructureProcheDto>? StructuresRecommandees
);

public record StructureProcheDto(
    int Id,
    string Nom,
    string Type,
    string Adresse,
    string? Telephone,
    double? DistanceKm,
    int? TempsAttenteMinutes
);
