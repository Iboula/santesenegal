using Microsoft.EntityFrameworkCore;
using SanteSenegal.Application.DTOs.NavigSante;
using SanteSenegal.Domain.Entities.NavigSante;
using SanteSenegal.Domain.Enums;
using SanteSenegal.Infrastructure.Persistence;
using SanteSenegal.Infrastructure.Services.NavigSante;
using Xunit;

namespace SanteSenegal.Tests.Services;

public class AccidentRouteServiceTests : IDisposable
{
    private readonly SanteDbContext _context;
    private readonly AccidentRouteService _service;

    public AccidentRouteServiceTests()
    {
        var options = new DbContextOptionsBuilder<SanteDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new SanteDbContext(options);
        _service = new AccidentRouteService(_context);
    }

    [Fact]
    public async Task SignalerAccidentAsync_Should_Create_Accident()
    {
        // Arrange
        var request = new SignalerAccidentRequest(
            Latitude: 14.7167,
            Longitude: -17.4677,
            AdresseApproximative: "Autoroute à péage, PK 15",
            Route: "N1",
            PointKilometrique: 15,
            Description: "Collision entre 2 véhicules",
            NombreVictimesEstime: 3,
            TypeAccident: "Collision",
            TypeVehicule: "Voiture",
            RisqueIncendie: false,
            FuiteCarburant: true,
            RouteBloquee: true,
            ConditionsMeteo: "Pluie",
            NomSignaleur: "Amadou Diop",
            TelephoneSignaleur: "770123456",
            PhotoUrl: null
        );

        // Act
        var result = await _service.SignalerAccidentAsync(request);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(StatutAccident.Signale, result.Statut);
        Assert.Equal(3, result.NombreVictimesEstime);
        Assert.True(result.RouteBloquee);
    }

    [Fact]
    public async Task GetAccidentsActifsAsync_Should_Exclude_Closed_Accidents()
    {
        // Arrange
        await SeedAccidents();

        // Act
        var result = await _service.GetAccidentsActifsAsync();

        // Assert
        Assert.All(result, a => Assert.NotEqual("Cloture", a.Statut));
    }

    [Fact]
    public async Task MettreAJourStatutAsync_Should_Update_Status()
    {
        // Arrange
        var accident = new AccidentRoute
        {
            Latitude = 14.7,
            Longitude = -17.4,
            Statut = StatutAccident.Signale
        };
        _context.AccidentsRoute.Add(accident);
        await _context.SaveChangesAsync();

        var request = new MettreAJourStatutAccidentRequest(
            NouveauStatut: StatutAccident.PriseEnCharge,
            StructureIdIntervention: 1,
            NotesIntervention: "SMUR en route"
        );

        // Act
        var result = await _service.MettreAJourStatutAsync(accident.Id, request);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(StatutAccident.PriseEnCharge, result.Statut);
        Assert.NotNull(result.DatePriseEnCharge);
    }

    [Fact]
    public async Task GetAccidentsParZoneAsync_Should_Filter_By_Radius()
    {
        // Arrange
        _context.AccidentsRoute.AddRange(
            new AccidentRoute { Latitude = 14.7167, Longitude = -17.4677, Statut = StatutAccident.Signale },
            new AccidentRoute { Latitude = 14.8000, Longitude = -17.5000, Statut = StatutAccident.Signale }
        );
        await _context.SaveChangesAsync();

        // Act
        var result = await _service.GetAccidentsParZoneAsync(14.7167, -17.4677, 5);

        // Assert
        Assert.Single(result);
    }

    [Fact]
    public async Task GetZonesNoiresAsync_Should_Return_High_Risk_Zones()
    {
        // Arrange
        for (int i = 0; i < 5; i++)
        {
            _context.AccidentsRoute.Add(new AccidentRoute
            {
                Latitude = 14.7,
                Longitude = -17.4,
                Route = "N1",
                PointKilometrique = 10,
                DateAccident = DateTime.UtcNow.AddDays(-i * 10),
                NombreDeces = 1,
                NombreBlessesGraves = 2,
                Statut = StatutAccident.Cloture
            });
        }
        await _context.SaveChangesAsync();

        // Act
        var result = await _service.GetZonesNoiresAsync();

        // Assert
        Assert.NotEmpty(result);
        Assert.All(result, z => Assert.True(z.NombreAccidents >= 3));
    }

    [Fact]
    public async Task GetStatistiquesAsync_Should_Return_Stats()
    {
        // Arrange
        await SeedAccidents();

        // Act
        var result = await _service.GetStatistiquesAsync(null, null);

        // Assert
        Assert.True(result.TotalAccidents > 0);
        Assert.NotNull(result.RouteLaPlusDangereuse);
    }

    private async Task SeedAccidents()
    {
        _context.AccidentsRoute.AddRange(
            new AccidentRoute
            {
                Latitude = 14.7, Longitude = -17.4,
                Statut = StatutAccident.Signale,
                DateAccident = DateTime.UtcNow.AddHours(-2)
            },
            new AccidentRoute
            {
                Latitude = 14.8, Longitude = -17.5,
                Statut = StatutAccident.PriseEnCharge,
                DateAccident = DateTime.UtcNow.AddHours(-5)
            },
            new AccidentRoute
            {
                Latitude = 14.9, Longitude = -17.6,
                Statut = StatutAccident.Cloture,
                DateAccident = DateTime.UtcNow.AddDays(-2)
            }
        );
        await _context.SaveChangesAsync();
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}
