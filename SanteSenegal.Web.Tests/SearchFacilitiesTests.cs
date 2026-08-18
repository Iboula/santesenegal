using Bunit;
using Microsoft.Extensions.DependencyInjection;
using SanteSenegal.Web.Components.Badges;
using SanteSenegal.Web.Components.Search;
using SanteSenegal.Web.Pages;
using SanteSenegal.Web.Services;
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
                CreateFacility("Hôpital Principal")
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

    [Fact]
    public void SearchFacilities_UsesInjectedServiceForSuccessfulSearch()
    {
        var service = RegisterFacilitySearchService([CreateFacility("Structure API")]);

        var component = RenderComponent<SearchFacilities>();

        component.Find("input[aria-label='Rechercher un établissement, un symptôme, une spécialité ou un service']").Input("fièvre");
        component.Find("form").Submit();

        component.WaitForAssertion(() => Assert.Contains("Structure API", component.Markup));
        Assert.Equal("fièvre", service.LastQuery);
    }

    [Fact]
    public void SearchFacilities_RendersNoResultsWhenServiceReturnsEmpty()
    {
        RegisterFacilitySearchService([]);

        var component = RenderComponent<SearchFacilities>();

        component.Find("form").Submit();

        component.WaitForAssertion(() => Assert.Contains("Aucun établissement trouvé", component.Markup));
    }

    [Fact]
    public void SearchFacilities_RendersErrorWhenServiceFails()
    {
        RegisterFacilitySearchService([], throwOnSearch: true);

        var component = RenderComponent<SearchFacilities>();

        component.Find("form").Submit();

        component.WaitForAssertion(() => Assert.Contains("Recherche indisponible", component.Markup));
    }

    [Fact]
    public void SearchFacilities_PassesFiltersToInjectedService()
    {
        var service = RegisterFacilitySearchService([CreateFacility("Structure filtrée")]);
        var component = RenderComponent<SearchFacilities>();

        component.Find("button[aria-controls='search-filters-panel']").Click();
        component.Find("select[aria-label='Filtrer par région']").Change("Thiès");
        component.Find("select[aria-label=\"Filtrer par type d'établissement\"]").Change("Centre de santé");
        component.Find("form").Submit();

        component.WaitForAssertion(() =>
        {
            Assert.Equal("Thiès", service.LastFilters?.Region);
            Assert.Equal("Centre de santé", service.LastFilters?.FacilityType);
        });
    }

    [Fact]
    public void SearchFacilities_DoesNotContainHardcodedFacilityData()
    {
        var source = ReadSourceFile("SanteSenegal.Web", "Pages", "SearchFacilities.razor");

        Assert.DoesNotContain("static readonly", source, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("Facilities =", source, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("Task.Delay", source, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("Normalize(", source, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("erreur", source, StringComparison.OrdinalIgnoreCase);
    }

    private FakeFacilitySearchService RegisterFacilitySearchService(
        IReadOnlyList<FacilitySearchItem> results,
        bool throwOnSearch = false)
    {
        var service = new FakeFacilitySearchService(results, throwOnSearch);
        Services.AddSingleton<IFacilitySearchService>(service);
        return service;
    }

    private static FacilitySearchItem CreateFacility(string name)
    {
        return new FacilitySearchItem(
            name,
            "Hôpital",
            "Dakar",
            StatusBadgeType.Ouvert,
            "Dakar",
            "Urgences",
            "Consultation",
            "15 min",
            2.4,
            new[] { "hôpital" });
    }

    private static string ReadSourceFile(params string[] pathParts)
    {
        var current = new DirectoryInfo(AppContext.BaseDirectory);
        while (current is not null)
        {
            var candidate = Path.Combine(new[] { current.FullName }.Concat(pathParts).ToArray());
            if (File.Exists(candidate))
            {
                return File.ReadAllText(candidate);
            }

            current = current.Parent;
        }

        throw new FileNotFoundException("Unable to locate source file.", Path.Combine(pathParts));
    }

    private sealed class FakeFacilitySearchService : IFacilitySearchService
    {
        private readonly IReadOnlyList<FacilitySearchItem> _results;
        private readonly bool _throwOnSearch;

        public FakeFacilitySearchService(IReadOnlyList<FacilitySearchItem> results, bool throwOnSearch)
        {
            _results = results;
            _throwOnSearch = throwOnSearch;
        }

        public string? LastQuery { get; private set; }
        public SearchFilterState? LastFilters { get; private set; }

        public Task<IReadOnlyList<FacilitySearchItem>> SearchAsync(
            string? query,
            SearchFilterState filters,
            CancellationToken cancellationToken = default)
        {
            LastQuery = query;
            LastFilters = filters;

            if (_throwOnSearch)
            {
                throw new InvalidOperationException("Service unavailable.");
            }

            return Task.FromResult(_results);
        }
    }
}
