namespace SanteSenegal.Application.DTOs;

using SanteSenegal.Application.DTOs.Disponibilites;

public record SousServiceDto
{
    public int Id { get; init; }
    public string Nom { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public decimal Prix { get; init; }
    public bool EstDisponible { get; init; }
    public int DureeMinutes { get; init; }
    public string? Specialite { get; init; }
    public ServiceBasicDto Service { get; init; } = null!;
}

public record SousServiceBasicDto
{
    public int Id { get; init; }
    public string Nom { get; init; } = string.Empty;
    public decimal Prix { get; init; }
    public string? Specialite { get; init; }
}

public record SousServiceCreateDto
{
    public string Nom { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public decimal Prix { get; init; }
    public int DureeMinutes { get; init; }
    public string? Specialite { get; init; }
    public int ServiceId { get; init; }
}

public record SousServiceUpdateDto
{
    public string? Nom { get; init; }
    public string? Description { get; init; }
    public decimal? Prix { get; init; }
    public int? DureeMinutes { get; init; }
    public string? Specialite { get; init; }
    public bool? EstDisponible { get; init; }
}

public record SousServiceDisponibiliteDto
{
    public SousServiceBasicDto SousService { get; init; } = null!;
    public List<DisponibiliteDto> Disponibilites { get; init; } = new();
}
