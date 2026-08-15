using SanteSenegal.Domain.Enums;

namespace SanteSenegal.Application.DTOs;

public record StructureDto
{
    public int Id { get; init; }
    public string Nom { get; init; } = string.Empty;
    public TypeStructure Type { get; init; }
    public string Adresse { get; init; } = string.Empty;
    public string Telephone { get; init; } = string.Empty;
    public string? Email { get; init; }
    public string? Description { get; init; }
    public string? ImageUrl { get; init; }
    public string? HorairesOuverture { get; init; }
    public bool EstActif { get; init; }
    public List<ServiceBasicDto> Services { get; init; } = new();
}

public record StructureBasicDto
{
    public int Id { get; init; }
    public string Nom { get; init; } = string.Empty;
    public TypeStructure Type { get; init; }
    public string Adresse { get; init; } = string.Empty;
}

public record StructureCreateDto
{
    public string Nom { get; init; } = string.Empty;
    public TypeStructure Type { get; init; }
    public string Adresse { get; init; } = string.Empty;
    public string Telephone { get; init; } = string.Empty;
    public string? Email { get; init; }
    public string? Description { get; init; }
    public string? ImageUrl { get; init; }
    public string? HorairesOuverture { get; init; }
}

public record StructureUpdateDto
{
    public string? Nom { get; init; }
    public string? Adresse { get; init; }
    public string? Telephone { get; init; }
    public string? Email { get; init; }
    public string? Description { get; init; }
    public string? ImageUrl { get; init; }
    public string? HorairesOuverture { get; init; }
    public bool? EstActif { get; init; }
}
