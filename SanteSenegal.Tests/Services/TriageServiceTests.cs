using Microsoft.EntityFrameworkCore;
using SanteSenegal.Domain.Entities;
using SanteSenegal.Domain.Enums;
using SanteSenegal.Infrastructure.Persistence;
using SanteSenegal.Infrastructure.Services.NavigSante;
using Xunit;

namespace SanteSenegal.Tests.Services;

public class TriageServiceTests : IDisposable
{
    private readonly SanteDbContext _context;
    private readonly TriageService _triageService;

    public TriageServiceTests()
    {
        var options = new DbContextOptionsBuilder<SanteDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new SanteDbContext(options);
        _triageService = new TriageService(_context);

        SeedStructures();
    }

    private void SeedStructures()
    {
        _context.Structures.AddRange(
            new Structure
            {
                Id = 1,
                Nom = "Hôpital Principal",
                Type = TypeStructure.Hopital,
                Adresse = "Avenue Nelson Mandela, Dakar",
                Telephone = "338391919",
                EstActif = true
            },
            new Structure
            {
                Id = 2,
                Nom = "Centre de Santé Fann",
                Type = TypeStructure.CentreDeSante,
                Adresse = "Fann, Dakar",
                Telephone = "338214567",
                EstActif = true
            },
            new Structure
            {
                Id = 3,
                Nom = "Clinique du Cap",
                Type = TypeStructure.Clinique,
                Adresse = "Mermoz, Dakar",
                Telephone = "338654321",
                EstActif = true
            }
        );
        _context.SaveChanges();
    }

    [Theory]
    [InlineData(new[] { "difficulte_respirer" }, 4, "URGENCE_IMMEDIATE")]
    [InlineData(new[] { "perte_conscience" }, 4, "URGENCE_IMMEDIATE")]
    [InlineData(new[] { "saignement_abondant" }, 4, "URGENCE_IMMEDIATE")]
    [InlineData(new[] { "fievre_elevee", "vomissements_persistants" }, 3, "URGENCE_RELATIVE")]
    [InlineData(new[] { "toux", "fievre" }, 2, "CONSULTATION_PROGRAMMEE")]
    [InlineData(new[] { "fatigue", "eternuements" }, 1, "AUTO_SOIN")]
    public async Task TrierAsync_Should_Return_Correct_Gravite(string[] symptomes, int expectedGravite, string expectedNiveau)
    {
        // Act
        var result = await _triageService.TrierAsync(symptomes, null, null);

        // Assert
        Assert.Equal(expectedGravite, result.Gravite);
        Assert.Equal(expectedNiveau, result.NiveauSoins);
    }

    [Fact]
    public async Task TrierAsync_Critical_Should_Return_Urgence_Structures()
    {
        // Arrange
        var symptomes = new[] { "douleur_poitrine", "difficulte_respirer" };

        // Act
        var result = await _triageService.TrierAsync(symptomes, null, null);

        // Assert
        Assert.True(result.EstUrgence);
        Assert.NotNull(result.StructuresRecommandees);
        Assert.True(result.StructuresRecommandees.Count <= 3);
    }

    [Fact]
    public async Task TrierAsync_Light_Should_Return_Consultation_Structures()
    {
        // Arrange
        var symptomes = new[] { "toux", "maux_de_tete" };

        // Act
        var result = await _triageService.TrierAsync(symptomes, null, null);

        // Assert
        Assert.False(result.EstUrgence);
        Assert.NotNull(result.StructuresRecommandees);
        Assert.True(result.StructuresRecommandees.Count <= 5);
    }

    [Fact]
    public async Task TrierAsync_Should_Return_Wolof_Message()
    {
        // Arrange
        var symptomes = new[] { "fatigue" };

        // Act
        var result = await _triageService.TrierAsync(symptomes, null, null);

        // Assert
        Assert.NotNull(result.MessageWolof);
        Assert.Contains("fiif", result.MessageWolof.ToLower());
    }

    [Fact]
    public async Task TrierAsync_Should_Identify_Paludisme()
    {
        // Arrange
        var symptomes = new[] { "fievre", "maux_de_tete", "fatigue" };

        // Act
        var result = await _triageService.TrierAsync(symptomes, null, null);

        // Assert
        Assert.Contains(result.PathologiesPossibles, p => p.Contains("Paludisme"));
    }

    [Fact]
    public async Task TrierAsync_Should_Identify_Pneumonie()
    {
        // Arrange
        var symptomes = new[] { "toux", "fievre", "difficulte_respirer" };

        // Act
        var result = await _triageService.TrierAsync(symptomes, null, null);

        // Assert
        Assert.Contains(result.PathologiesPossibles, p => p.Contains("Pneumonie"));
    }

    [Fact]
    public async Task GetSymptomesAsync_Should_Return_Empty_List_Initially()
    {
        // Act
        var result = await _triageService.GetSymptomesAsync();

        // Assert
        Assert.Empty(result);
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}
