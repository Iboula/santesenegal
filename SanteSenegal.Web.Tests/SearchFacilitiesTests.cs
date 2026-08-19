using System.Net;
using Bunit;
using Microsoft.Extensions.DependencyInjection;
using SanteSenegal.Domain.Enums;
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

        component.Find("input[name='facility-search']").Input("fievre");
        component.Find("form").Submit();

        Assert.Equal("fievre", query);
        Assert.True(submitted);
        Assert.Contains("radiographie", component.Markup);
        Assert.Contains("pharmacie", component.Markup);
    }

    [Fact]
    public void SearchFilters_StartCollapsedAndExposeSupportedFilterControls()
    {
        SearchFilterState? filters = null;
        var component = RenderComponent<SearchFilters>(parameters => parameters
            .Add(p => p.Filters, SearchFilterState.Default)
            .Add(p => p.FiltersChanged, value => filters = value));

        var toggle = component.Find("button[aria-controls='search-filters-panel']");
        Assert.Equal("false", toggle.GetAttribute("aria-expanded"));

        toggle.Click();
        component.Find("select[aria-label='Filtrer par region']").Change("Dakar");
        component.Find("select[aria-label=\"Filtrer par type d'etablissement\"]").Change("Hopital");

        Assert.Equal("true", component.Find("button[aria-controls='search-filters-panel']").GetAttribute("aria-expanded"));
        Assert.Equal("Dakar", filters?.Region);
        Assert.Equal("Hopital", filters?.FacilityType);
    }

    [Fact]
    public void SearchFilters_DoesNotExposeDistanceAsActiveFilter()
    {
        var component = RenderComponent<SearchFilters>(parameters => parameters
            .Add(p => p.Filters, SearchFilterState.Default));

        component.Find("button[aria-controls='search-filters-panel']").Click();

        Assert.Empty(component.FindAll("input[type='range']"));
        Assert.Contains("Distance - bientot disponible", component.Markup);
        Assert.DoesNotContain("km</strong>", component.Markup);
    }

    [Fact]
    public void SearchFilters_OnlyShowsAvailabilityValuesSupportedByApi()
    {
        var component = RenderComponent<SearchFilters>(parameters => parameters
            .Add(p => p.Filters, SearchFilterState.Default));

        component.Find("button[aria-controls='search-filters-panel']").Click();

        Assert.Contains("Ouvert", component.Markup);
        Assert.Contains("Ferme", component.Markup);
        Assert.DoesNotContain("Urgence", component.Markup);
        Assert.DoesNotContain("Disponible", component.Markup);
    }

    [Fact]
    public void SearchResults_RendersFacilityCardsWhenResultsExist()
    {
        var component = RenderComponent<SearchResults>(parameters => parameters
            .Add(p => p.State, SearchUiState.ResultsFound)
            .Add(p => p.Query, "hopital")
            .Add(p => p.Results, new[]
            {
                CreateFacility("Hopital Principal")
            }));

        Assert.Contains("Hopital Principal", component.Markup);
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

        Assert.Contains("Recherche des", state.TextContent);
        Assert.Equal("polite", state.GetAttribute("aria-live"));
    }

    [Fact]
    public void SearchResults_RendersEmptyState()
    {
        var component = RenderComponent<SearchResults>(parameters => parameters
            .Add(p => p.State, SearchUiState.NoResults));

        Assert.NotNull(component.Find("[role='status']"));
        Assert.Contains("Aucun", component.Markup);
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

        component.Find("input[name='facility-search']").Input("fievre");
        component.Find("form").Submit();

        component.WaitForAssertion(() => Assert.Contains("Structure API", component.Markup));
        Assert.Equal("fievre", service.LastQuery);
    }

    [Fact]
    public void SearchFacilities_RendersNoResultsWhenServiceReturnsEmpty()
    {
        RegisterFacilitySearchService([]);
        var component = RenderComponent<SearchFacilities>();

        component.Find("form").Submit();

        component.WaitForAssertion(() => Assert.Contains("Aucun", component.Markup));
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
        var service = RegisterFacilitySearchService([CreateFacility("Structure filtree")]);
        var component = RenderComponent<SearchFacilities>();

        component.Find("button[aria-controls='search-filters-panel']").Click();
        component.Find("select[aria-label='Filtrer par region']").Change("Thies");
        component.Find("select[aria-label=\"Filtrer par type d'etablissement\"]").Change("Centre de sante");
        component.Find("form").Submit();

        component.WaitForAssertion(() =>
        {
            Assert.Equal("Thies", service.LastFilters?.Region);
            Assert.Equal("Centre de sante", service.LastFilters?.FacilityType);
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

    [Fact]
    public async Task FacilitySearchService_UnknownAvailabilityDoesNotMatchAll()
    {
        var handler = new RecordingHandler(CreateStructureJson("Structure ouverte", "Consultation", TypeService.Consultation));
        var service = CreateSearchService(handler);

        var results = await service.SearchAsync(null, SearchFilterState.Default with { Availability = "Urgence" });

        Assert.Empty(results);
    }

    [Theory]
    [InlineData("Pediatrie")]
    [InlineData("Maternite")]
    [InlineData("Pharmacie")]
    public async Task FacilitySearchService_UnrepresentedSpecialtiesDoNotSendIncorrectServiceType(string specialty)
    {
        var handler = new RecordingHandler(CreateStructureJson("Structure specialisee", specialty, TypeService.Consultation));
        var service = CreateSearchService(handler);

        var results = await service.SearchAsync(null, SearchFilterState.Default with { Specialty = specialty });

        Assert.Single(results);
        Assert.DoesNotContain("serviceType=", handler.LastRequestUri?.Query);
    }

    [Fact]
    public async Task FacilitySearchService_SearchUsesExistingEndpointAndSupportedServiceType()
    {
        var handler = new RecordingHandler(CreateStructureJson("Centre imagerie", "Radiologie", TypeService.Radiographie));
        var service = CreateSearchService(handler);

        var results = await service.SearchAsync("radiographie", SearchFilterState.Default with { Specialty = "Radiologie" });

        Assert.Single(results);
        Assert.Equal("/api/structures/search", handler.LastRequestUri?.AbsolutePath);
        Assert.Contains("term=radiographie", handler.LastRequestUri?.Query);
        Assert.Contains("serviceType=Radiographie", handler.LastRequestUri?.Query);
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
            "Hopital",
            "Dakar",
            StatusBadgeType.Ouvert,
            "Dakar",
            "Urgences",
            "Consultation",
            "15 min",
            2.4,
            new[] { "hopital" });
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

    private static FacilitySearchService CreateSearchService(RecordingHandler handler)
    {
        return new FacilitySearchService(new HttpClient(handler)
        {
            BaseAddress = new Uri("https://api.test/")
        });
    }

    private static string CreateStructureJson(string name, string serviceName, TypeService serviceType)
    {
        return $$"""
[
  {
    "id": 1,
    "nom": "{{name}}",
    "type": {{(int)TypeStructure.CentreDeSante}},
    "adresse": "Mermoz, Dakar",
    "description": "Structure reelle",
    "estActif": true,
    "services": [
      {
        "nom": "{{serviceName}}",
        "type": {{(int)serviceType}}
      }
    ]
  }
]
""";
    }

    private sealed class RecordingHandler : HttpMessageHandler
    {
        private readonly string _json;

        public RecordingHandler(string json)
        {
            _json = json;
        }

        public Uri? LastRequestUri { get; private set; }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            LastRequestUri = request.RequestUri;
            return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(_json, System.Text.Encoding.UTF8, "application/json")
            });
        }
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
