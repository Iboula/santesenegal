using Moq;
using SanteSenegal.Application.Abstractions;
using SanteSenegal.Application.Abstractions.Notifications;
using SanteSenegal.Domain.Abstractions;
using SanteSenegal.Domain.Entities;
using SanteSenegal.Domain.Enums;
using SanteSenegal.Infrastructure.Services;
using SanteSenegal.Infrastructure.Services.Notifications;
using Xunit;

namespace SanteSenegal.Tests.Services;

public class NotificationServiceTests
{
    private readonly Mock<INotificationRepository> _notifRepoMock = new();
    private readonly Mock<SmsProviderFactory> _smsFactoryMock;
    private readonly Mock<ISmsProvider> _smsProviderMock = new();
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();
    private readonly Mock<IPatientRepository> _patientRepoMock = new();
    private readonly Mock<IRendezVousRepository> _rdvRepoMock = new();
    private readonly Mock<IPaiementRepository> _paiementRepoMock = new();
    private readonly NotificationService _service;

    public NotificationServiceTests()
    {
        _smsProviderMock.Setup(s => s.Nom).Returns("TestProvider");
        _smsProviderMock.Setup(s => s.SendAsync(It.IsAny<SmsMessage>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new SmsSendResult(true, "ref-123", "msg-123", 5.0m, "{}", null));

        _smsFactoryMock = new Mock<SmsProviderFactory>(new[] { _smsProviderMock.Object });
        _smsFactoryMock.Setup(f => f.GetPrimaryProvider()).Returns(_smsProviderMock.Object);

        _service = new NotificationService(
            _notifRepoMock.Object,
            _smsFactoryMock.Object,
            _unitOfWorkMock.Object,
            _patientRepoMock.Object,
            _rdvRepoMock.Object,
            _paiementRepoMock.Object,
            Mock.Of<Microsoft.Extensions.Logging.ILogger<NotificationService>>());
    }

    [Fact]
    public async Task EnvoyerSMSAsync_Should_Create_Notification_And_Send_SMS()
    {
        // Arrange
        _notifRepoMock.Setup(r => r.AddAsync(It.IsAny<Notification>(), It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
        _notifRepoMock.Setup(r => r.UpdateAsync(It.IsAny<Notification>(), It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        // Act
        var result = await _service.EnvoyerSMSAsync(
            "770123456", "Test message", TypeNotification.RappelRendezVous, null, null, "fr");

        // Assert
        Assert.True(result.Success);
        Assert.Equal(StatutNotification.Envoye, result.Value!.Statut);
        _smsProviderMock.Verify(s => s.SendAsync(It.Is<SmsMessage>(m => m.To == "770123456"), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task EnvoyerConfirmationPaiementAsync_Should_Send_SMS_When_Payment_Exists()
    {
        // Arrange
        var paiement = new Paiement
        {
            Id = 1,
            RendezVousId = 1,
            Montant = 5000,
            ModePaiement = ModePaiement.Wave,
            ReferenceExterne = "WAVE-123"
        };
        var rdv = new RendezVous { Id = 1, PatientId = 1 };
        var patient = new Patient { Id = 1, Prenom = "Amadou", Telephone = "770123456" };

        _paiementRepoMock.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(paiement);
        _rdvRepoMock.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(rdv);
        _patientRepoMock.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(patient);

        _notifRepoMock.Setup(r => r.AddAsync(It.IsAny<Notification>(), It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
        _notifRepoMock.Setup(r => r.UpdateAsync(It.IsAny<Notification>(), It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        // Act
        var result = await _service.EnvoyerConfirmationPaiementAsync(1, "fr");

        // Assert
        Assert.True(result.Success);
        Assert.Equal(TypeNotification.ConfirmationPaiement, result.Value!.Type);
    }

    [Fact]
    public async Task EnvoyerConfirmationPaiementAsync_Should_Fail_When_Payment_Not_Found()
    {
        // Arrange
        _paiementRepoMock.Setup(r => r.GetByIdAsync(999, It.IsAny<CancellationToken>())).ReturnsAsync((Paiement?)null);

        // Act
        var result = await _service.EnvoyerConfirmationPaiementAsync(999, "fr");

        // Assert
        Assert.False(result.Success);
        Assert.Equal("Paiement non trouvé.", result.Error);
    }

    [Fact]
    public async Task PlanifierSMSAsync_Should_Create_Notification_With_DatePlanifie()
    {
        // Arrange
        var dateEnvoi = DateTime.UtcNow.AddHours(2);
        _notifRepoMock.Setup(r => r.AddAsync(It.IsAny<Notification>(), It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        // Act
        var result = await _service.PlanifierSMSAsync(
            "770123456", "Rappel demain", TypeNotification.RappelRendezVous, dateEnvoi);

        // Assert
        Assert.True(result.Success);
        Assert.Equal(StatutNotification.EnAttente, result.Value!.Statut);
        Assert.Equal(dateEnvoi, result.Value.DatePlanifie);
    }

    [Fact]
    public async Task AnnulerNotificationAsync_Should_Set_Status_To_Annule()
    {
        // Arrange
        var notification = new Notification { Id = 1, Statut = StatutNotification.EnAttente };
        _notifRepoMock.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(notification);
        _notifRepoMock.Setup(r => r.UpdateAsync(It.IsAny<Notification>(), It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        // Act
        var result = await _service.AnnulerNotificationAsync(1);

        // Assert
        Assert.True(result.Success);
        Assert.True(result.Value);
        Assert.Equal(StatutNotification.Annule, notification.Statut);
    }
}
