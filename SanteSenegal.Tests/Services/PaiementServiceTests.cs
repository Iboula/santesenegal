using Moq;
using SanteSenegal.Application.Abstractions;
using SanteSenegal.Application.Abstractions.Paiements;
using SanteSenegal.Domain.Abstractions;
using SanteSenegal.Domain.Entities;
using SanteSenegal.Domain.Enums;
using SanteSenegal.Infrastructure.Services;
using SanteSenegal.Infrastructure.Services.Paiements;
using Xunit;

namespace SanteSenegal.Tests.Services;

public class PaiementServiceTests
{
    private readonly Mock<IPaiementRepository> _paiementRepoMock = new();
    private readonly Mock<IBaseRepository<Transaction>> _transactionRepoMock = new();
    private readonly Mock<PaiementMobileFactory> _factoryMock;
    private readonly Mock<IPaiementMobileProvider> _providerMock = new();
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();
    private readonly PaiementService _service;

    public PaiementServiceTests()
    {
        _providerMock.Setup(p => p.Mode).Returns(ModePaiement.Wave);
        _providerMock.Setup(p => p.InitierPaiementAsync(It.IsAny<InitierPaiementRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new InitierPaiementResult(
                Succes: true,
                ReferenceExterne: "WAVE-TEST-123",
                UrlPaiement: "https://pay.wave.com/test",
                CodeConfirmation: null,
                DateExpiration: DateTime.UtcNow.AddMinutes(30),
                Message: "Paiement initié",
                RawResponse: "{}"));

        _factoryMock = new Mock<PaiementMobileFactory>(new[] { _providerMock.Object });
        _factoryMock.Setup(f => f.GetProvider(It.IsAny<ModePaiement>())).Returns(_providerMock.Object);

        _service = new PaiementService(
            _paiementRepoMock.Object,
            _transactionRepoMock.Object,
            _factoryMock.Object,
            _unitOfWorkMock.Object);

        _paiementRepoMock.Setup(r => r.AddAsync(It.IsAny<Paiement>(), It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
        _transactionRepoMock.Setup(r => r.AddAsync(It.IsAny<Transaction>(), It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
        _paiementRepoMock.Setup(r => r.UpdateAsync(It.IsAny<Paiement>(), It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
        _transactionRepoMock.Setup(r => r.UpdateAsync(It.IsAny<Transaction>(), It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);
    }

    [Fact]
    public async Task InitierPaiementMobileAsync_Should_Create_Paiement_And_Transaction()
    {
        // Act
        var result = await _service.InitierPaiementMobileAsync(
            1, 5000, ModePaiement.Wave, "770123456", "Consultation");

        // Assert
        Assert.True(result.Success);
        Assert.NotNull(result.Value);
        Assert.Equal(5000, result.Value.Montant);
        Assert.Equal(ModePaiement.Wave, result.Value.ModePaiement);
        Assert.Equal("WAVE-TEST-123", result.Value.ReferenceExterne);
        Assert.Equal(StatutPaiement.EnAttente, result.Value.Statut);

        _paiementRepoMock.Verify(r => r.AddAsync(It.IsAny<Paiement>(), It.IsAny<CancellationToken>()), Times.Once);
        _transactionRepoMock.Verify(r => r.AddAsync(It.IsAny<Transaction>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task InitierPaiementMobileAsync_Should_Fail_When_Provider_Fails()
    {
        // Arrange
        _providerMock.Setup(p => p.InitierPaiementAsync(It.IsAny<InitierPaiementRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new InitierPaiementResult(
                Succes: false, null, null, null, null, "Erreur API", null));

        // Act
        var result = await _service.InitierPaiementMobileAsync(
            1, 5000, ModePaiement.Wave, "770123456");

        // Assert
        Assert.False(result.Success);
        Assert.Equal("Erreur API", result.Error);
    }

    [Fact]
    public async Task VerifierPaiementMobileAsync_Should_Update_Status_When_Paid()
    {
        // Arrange
        var paiement = new Paiement
        {
            Id = 1,
            RendezVousId = 1,
            Montant = 5000,
            ModePaiement = ModePaiement.Wave,
            ReferenceExterne = "WAVE-TEST-123",
            Statut = StatutPaiement.EnAttente
        };

        _paiementRepoMock.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(paiement);
        _providerMock.Setup(p => p.VerifierPaiementAsync("WAVE-TEST-123", It.IsAny<CancellationToken>()))
            .ReturnsAsync(new VerifierPaiementResult(
                Succes: true,
                EstPaye: true,
                ReferenceExterne: "WAVE-TEST-123",
                MontantPaye: 5000,
                DatePaiement: DateTime.UtcNow,
                Message: "Payé",
                RawResponse: "{}"));

        // Act
        var result = await _service.VerifierPaiementMobileAsync(1);

        // Assert
        Assert.True(result.Success);
        Assert.True(result.Data.EstPaye);
        Assert.Equal(StatutPaiement.Recu, result.Data.Paiement.Statut);
        _paiementRepoMock.Verify(r => r.UpdateAsync(It.Is<Paiement>(p => p.Statut == StatutPaiement.Recu), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task VerifierPaiementMobileAsync_Should_Fail_When_Paiement_Not_Found()
    {
        // Arrange
        _paiementRepoMock.Setup(r => r.GetByIdAsync(999, It.IsAny<CancellationToken>())).ReturnsAsync((Paiement?)null);

        // Act
        var result = await _service.VerifierPaiementMobileAsync(999);

        // Assert
        Assert.False(result.Success);
        Assert.Equal("Paiement non trouvé.", result.Error);
    }

    [Fact]
    public async Task ConfirmerPaiementAsync_Should_Update_Status()
    {
        // Arrange
        var paiement = new Paiement
        {
            Id = 1,
            RendezVousId = 1,
            Montant = 5000,
            Statut = StatutPaiement.EnAttente
        };
        _paiementRepoMock.Setup(r => r.GetByRendezVousIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(new List<Paiement> { paiement });

        // Act
        var result = await _service.ConfirmerPaiementAsync(1, 5000);

        // Assert
        Assert.True(result);
        Assert.Equal(StatutPaiement.Recu, paiement.Statut);
    }

    [Fact]
    public async Task RembourserPaiementAsync_Should_Update_Status()
    {
        // Arrange
        var paiement = new Paiement
        {
            Id = 1,
            RendezVousId = 1,
            Montant = 5000,
            Statut = StatutPaiement.Recu
        };
        _paiementRepoMock.Setup(r => r.GetByRendezVousIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(new List<Paiement> { paiement });

        // Act
        var result = await _service.RembourserPaiementAsync(1, 5000);

        // Assert
        Assert.True(result);
        Assert.Equal(StatutPaiement.Rembourse, paiement.Statut);
    }
}
