using Bunit;
using Microsoft.AspNetCore.Components;
using SanteSenegal.Web.Components.Badges;
using SanteSenegal.Web.Components.Search;
using Xunit;

namespace SanteSenegal.Web.Tests;

public sealed class SearchFacilitiesTests : TestContext
{
    [Fact]
    public void SearchBar_RendersFreeSearchAndGuidedSuggestions()
    {
        string? query = null;
        var submitted = false;

        var component = RenderComponent<SearchBar>(parameters => parameters
            .Add(p => p.Query, query)
            .Add(p => p.QueryChanged, value => query = value)
            .Add(p => p.OnSearch, () => submitted = true));

        component.Find("input[aria-label='Rechercher un établissement, un symptôme, une spécialité ou un service']").Input("fièvre");
        component.Find("form").Submit();

        Assert.Equal("fièvre", query);
        Assert.True(submitted);
        Assert.Contains("radiographie", component.Markup);
        Assert.Contains("pédiatre", component.Markup);
        Assert.Contains("pharmacie", component.Markup);
        Assert.Contains("maternité", component.Markup);
        Assert.Contains("hôpital", component.Markup);
    }

    [Fact]
    public void SearchFilters_StartCollapsedAndExposeFilterControls()
    {
        SearchFilterState? filters = null;
        var component = RenderComponent<SearchFilters>(parameters => parameters
            .Add(p => p.Filters, SearchFilterState.Default)
            .Add(p => p.FiltersChanged, value => filters = value));

        var toggle = component.Find("button[aria-controls='search-filters-panel']");
        Assert.Equal("false", toggle.GetAttribute("aria-expanded"));

        toggle.Click();
        component.Find("select[aria-label='Filtrer par région']").Change("Dakar");
        component.Find("select[aria-label=\"Filtrer par type d'établissement\"]").Change("Hôpital");

        Assert.Equal("true", component.Find("button[aria-controls='search-filters-panel']").GetAttribute("aria-expanded"));
        Assert.Equal("Dakar", filters?.Region);
        Assert.Equal("Hôpital", filters?.FacilityType);
    }

    [Fact]
    public void SearchResults_RendersFacilityCardsWhenResultsExist()
    {
        var component = RenderComponent<SearchResults>(parameters => parameters
            .Add(p => p.State, SearchUiState.ResultsFound)
            .Add(p => p.Query, "hôpital")
            .Add(p => p.Results, new[]
            {
                new FacilitySearchItem(
                    "Hôpital Principal",
                    "Hôpital",
                    "Dakar",
                    StatusBadgeType.Ouvert,
                    "Dakar",
                    "Urgences",
                    "hôpital",
                    "15 min",
                    2.4,
                    new[] { "hôpital" })
            }));

        Assert.Contains("Hôpital Principal", component.Markup);
        Assert.Contains("Ouvert", component.Markup);
        Assert.Contains("km", component.Markup);
        Assert.Contains("15 min", component.Markup);
        Assert.Contains("Consulter", component.Markup);
    }

    [Fact]
    public void SearchResults_RendersLoadingState()
    {
        var component = RenderComponent<SearchResults>(parameters => parameters
            .Add(p => p.State, SearchUiState.Searching));

        var state = component.Find("[role='status']");

        Assert.Contains("Recherche des établissements en cours", state.TextContent);
        Assert.Equal("polite", state.GetAttribute("aria-live"));
    }

    [Fact]
    public void SearchResults_RendersEmptyState()
    {
        var component = RenderComponent<SearchResults>(parameters => parameters
            .Add(p => p.State, SearchUiState.NoResults));

        Assert.NotNull(component.Find("[role='status']"));
        Assert.Contains("Aucun établissement trouvé", component.Markup);
    }

    [Fact]
    public void SearchResults_RendersErrorState()
    {
        var component = RenderComponent<SearchResults>(parameters => parameters
            .Add(p => p.State, SearchUiState.Error));

        Assert.NotNull(component.Find("[role='alert']"));
        Assert.Contains("Recherche indisponible", component.Markup);
    }
}
