using System.Globalization;
using System.Net.Http.Json;
using System.Text;
using SanteSenegal.Domain.Enums;
using SanteSenegal.Web.Components.Badges;
using SanteSenegal.Web.Components.Search;

namespace SanteSenegal.Web.Services;

public interface IFacilitySearchService
{
    Task<IReadOnlyList<FacilitySearchItem>> SearchAsync(
        string? query,
        SearchFilterState filters,
        CancellationToken cancellationToken = default);
}

public sealed class FacilitySearchService : IFacilitySearchService
{
    private readonly HttpClient _httpClient;

    public FacilitySearchService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<IReadOnlyList<FacilitySearchItem>> SearchAsync(
        string? query,
        SearchFilterState filters,
        CancellationToken cancellationToken = default)
    {
        var structures = await _httpClient.GetFromJsonAsync<List<StructureSearchResponse>>(
            BuildSearchUri(query, filters),
            cancellationToken) ?? [];

        return structures
            .Where(structure => MatchesPresentationFilters(structure, filters))
            .Select(MapToSearchItem)
            .ToArray();
    }

    private static string BuildSearchUri(string? query, SearchFilterState filters)
    {
        var queryParameters = new List<string>();

        if (!string.IsNullOrWhiteSpace(query))
        {
            queryParameters.Add($"term={Uri.EscapeDataString(query.Trim())}");
        }

        if (TryMapStructureType(filters.FacilityType, out var type))
        {
            queryParameters.Add($"type={Uri.EscapeDataString(type.ToString())}");
        }

        if (TryMapServiceType(filters.Specialty, out var serviceType))
        {
            queryParameters.Add($"serviceType={Uri.EscapeDataString(serviceType.ToString())}");
        }

        return queryParameters.Count == 0
            ? "api/structures/search"
            : $"api/structures/search?{string.Join("&", queryParameters)}";
    }

    private static FacilitySearchItem MapToSearchItem(StructureSearchResponse structure)
    {
        var services = structure.Services ?? [];
        var specialty = services.FirstOrDefault()?.Nom ?? "Services de santé";
        var service = services.FirstOrDefault()?.Type.ToString() ?? specialty;

        return new FacilitySearchItem(
            structure.Nom,
            FormatStructureType(structure.Type),
            structure.Adresse,
            structure.EstActif ? StatusBadgeType.Ouvert : StatusBadgeType.Ferme,
            ExtractRegion(structure.Adresse),
            specialty,
            service,
            null,
            null,
            BuildKeywords(structure, services));
    }

    private static bool MatchesPresentationFilters(StructureSearchResponse structure, SearchFilterState filters)
    {
        // L'endpoint actuel ne fournit ni géodistance ni temps d'attente : aucun filtre distance n'est simulé côté client.
        return MatchesRegion(structure, filters.Region)
            && MatchesAvailability(structure, filters.Availability)
            && MatchesFacilityType(structure, filters.FacilityType)
            && MatchesSpecialty(structure, filters.Specialty);
    }

    private static bool MatchesRegion(StructureSearchResponse structure, string region)
    {
        return string.IsNullOrWhiteSpace(region)
            || Normalize(structure.Adresse).Contains(Normalize(region), StringComparison.Ordinal);
    }

    private static bool MatchesAvailability(StructureSearchResponse structure, string availability)
    {
        if (string.IsNullOrWhiteSpace(availability))
        {
            return true;
        }

        var current = structure.EstActif ? "ouvert" : "ferme";
        return Normalize(availability) switch
        {
            "ouvert" => current == "ouvert",
            "ferme" => current == "ferme",
            _ => false
        };
    }

    private static bool MatchesFacilityType(StructureSearchResponse structure, string facilityType)
    {
        return string.IsNullOrWhiteSpace(facilityType)
            || string.Equals(Normalize(FormatStructureType(structure.Type)), Normalize(facilityType), StringComparison.Ordinal)
            || string.Equals(Normalize(structure.Type.ToString()), Normalize(facilityType), StringComparison.Ordinal);
    }

    private static bool MatchesSpecialty(StructureSearchResponse structure, string specialty)
    {
        if (string.IsNullOrWhiteSpace(specialty))
        {
            return true;
        }

        var normalizedSpecialty = Normalize(specialty);
        return structure.Services?.Any(service =>
            Normalize(service.Nom).Contains(normalizedSpecialty, StringComparison.Ordinal)
            || Normalize(service.Type.ToString()).Contains(normalizedSpecialty, StringComparison.Ordinal)) == true;
    }

    private static bool TryMapStructureType(string value, out TypeStructure type)
    {
        type = default;
        return Normalize(value) switch
        {
            "hopital" => Set(TypeStructure.Hopital, out type),
            "clinique" => Set(TypeStructure.Clinique, out type),
            "centre de sante" => Set(TypeStructure.CentreDeSante, out type),
            _ => false
        };
    }

    private static bool TryMapServiceType(string value, out TypeService type)
    {
        type = default;
        return Normalize(value) switch
        {
            "radiologie" or "radiographie" => Set(TypeService.Radiographie, out type),
            "analyse" => Set(TypeService.Analyse, out type),
            "consultation" => Set(TypeService.Consultation, out type),
            "echographie" => Set(TypeService.Echographie, out type),
            "autre" => Set(TypeService.Autre, out type),
            _ => false
        };
    }

    private static bool Set<T>(T value, out T target)
    {
        target = value;
        return true;
    }

    private static string FormatStructureType(TypeStructure type)
    {
        return type switch
        {
            TypeStructure.PosteDeSante => "Poste de santé",
            TypeStructure.CentreDeSante => "Centre de santé",
            TypeStructure.Hopital => "Hôpital",
            TypeStructure.Clinique => "Clinique",
            TypeStructure.Cabinet => "Cabinet",
            _ => type.ToString()
        };
    }

    private static string ExtractRegion(string address)
    {
        if (string.IsNullOrWhiteSpace(address))
        {
            return string.Empty;
        }

        var parts = address.Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
        return parts.Length > 0 ? parts[^1] : address;
    }

    private static IReadOnlyList<string> BuildKeywords(StructureSearchResponse structure, IReadOnlyList<ServiceSearchResponse> services)
    {
        return new[]
            {
                structure.Nom,
                structure.Adresse,
                FormatStructureType(structure.Type),
                structure.Description ?? string.Empty
            }
            .Concat(services.SelectMany(service => new[] { service.Nom, service.Type.ToString() }))
            .Where(value => !string.IsNullOrWhiteSpace(value))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray();
    }

    private static string Normalize(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return string.Empty;
        }

        var normalized = value.Trim().ToLowerInvariant().Normalize(NormalizationForm.FormD);
        var builder = new StringBuilder(normalized.Length);

        foreach (var character in normalized)
        {
            if (CharUnicodeInfo.GetUnicodeCategory(character) != UnicodeCategory.NonSpacingMark)
            {
                builder.Append(character);
            }
        }

        return builder.ToString().Normalize(NormalizationForm.FormC);
    }

    private sealed record StructureSearchResponse
    {
        public int Id { get; init; }
        public string Nom { get; init; } = string.Empty;
        public TypeStructure Type { get; init; }
        public string Adresse { get; init; } = string.Empty;
        public string? Description { get; init; }
        public bool EstActif { get; init; }
        public List<ServiceSearchResponse>? Services { get; init; }
    }

    private sealed record ServiceSearchResponse
    {
        public string Nom { get; init; } = string.Empty;
        public TypeService Type { get; init; }
    }
}
