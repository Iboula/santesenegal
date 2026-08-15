using SanteSenegal.Domain.Enums;

namespace SanteSenegal.Application.DTOs;

public record ServiceDto
{
    public int Id { get; init; }
    public string Nom { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public TypeService Type { get; init; }
    public bool EstActif { get; init; }
    public List<SousServiceBasicDto> SousServices { get; init; } = new();
}

public record ServiceBasicDto
{
    public int Id { get; init; }
    public string Nom { get; init; } = string.Empty;
    public TypeService Type { get; init; }
}

public record ServiceCreateDto
{
    public string Nom { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public TypeService Type { get; init; }
    public int StructureId { get; init; }
}

public record ServiceUpdateDto
{
    public string? Nom { get; init; }
    public string? Description { get; init; }
    public bool? EstActif { get; init; }
}
