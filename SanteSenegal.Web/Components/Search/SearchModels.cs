using SanteSenegal.Web.Components.Badges;

namespace SanteSenegal.Web.Components.Search;

public enum SearchUiState
{
    Idle,
    Searching,
    ResultsFound,
    NoResults,
    Error
}

public sealed record FacilitySearchItem(
    string Name,
    string Type,
    string Address,
    StatusBadgeType Availability,
    string Region,
    string Specialty,
    string Service,
    string? WaitingTime,
    double? DistanceKm,
    IReadOnlyList<string> Keywords);

public sealed record SearchFilterState(
    int DistanceKm,
    string Region,
    string Availability,
    string FacilityType,
    string Specialty)
{
    public static SearchFilterState Default => new(10, string.Empty, string.Empty, string.Empty, string.Empty);
}
