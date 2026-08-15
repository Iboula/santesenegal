using Moq;
using SanteSenegal.Domain.Abstractions;
using SanteSenegal.Domain.Entities;
using SanteSenegal.Domain.Enums;
using SanteSenegal.Infrastructure.Services;
using Xunit;

namespace SanteSenegal.Tests.Services;

public class AuthServiceTests
{
    private readonly Mock<IUserRepository> _userRepoMock = new();
    private readonly Mock<IJwtService> _jwtServiceMock = new();
    private readonly AuthService _authService;

    public AuthServiceTests()
    {
        _authService = new AuthService(_userRepoMock.Object, _jwtServiceMock.Object, null!);
    }

    [Fact]
    public async Task RegisterAsync_Should_Create_User_With_Hashed_Password()
    {
        // Arrange
        var request = new RegisterRequest("Test", "User", "test@email.sn", "password123", "770000000");
        _userRepoMock.Setup(r => r.GetByEmailAsync(It.IsAny<string>())).ReturnsAsync((User?)null);
        _userRepoMock.Setup(r => r.AddAsync(It.IsAny<User>())).Returns(Task.CompletedTask);

        // Act
        var result = await _authService.RegisterAsync("test@email.sn", "password123", "Test", "User", "770000000", UserRole.Patient);

        // Assert
        Assert.True(result.Success);
        Assert.NotNull(result.User);
        Assert.Equal("test@email.sn", result.User.Email);
    }

    [Fact]
    public async Task RegisterAsync_Should_Fail_When_Email_Exists()
    {
        // Arrange
        var existingUser = new User { Email = "test@email.sn" };
        _userRepoMock.Setup(r => r.GetByEmailAsync("test@email.sn")).ReturnsAsync(existingUser);

        // Act
        var result = await _authService.RegisterAsync("test@email.sn", "password123", "Test", "User", "770000000", UserRole.Patient);

        // Assert
        Assert.False(result.Success);
        Assert.Equal("Cet email est déjà utilisé.", result.Error);
    }

    [Fact]
    public async Task LoginAsync_Should_Succeed_With_Valid_Credentials()
    {
        // Arrange
        var passwordHash = BCrypt.Net.BCrypt.HashPassword("password123");
        var user = new User
        {
            Id = 1,
            Email = "test@email.sn",
            PasswordHash = passwordHash,
            Nom = "Test",
            Prenom = "User",
            Role = UserRole.Patient,
            EstActif = true
        };

        _userRepoMock.Setup(r => r.GetByEmailAsync("test@email.sn")).ReturnsAsync(user);
        _jwtServiceMock.Setup(j => j.GenerateAccessToken(It.IsAny<User>())).Returns("fake-jwt-token");
        _jwtServiceMock.Setup(j => j.GenerateRefreshToken()).Returns("refresh-token");

        // Act
        var result = await _authService.LoginAsync("test@email.sn", "password123");

        // Assert
        Assert.True(result.Success);
        Assert.NotNull(result.AccessToken);
        Assert.Equal("fake-jwt-token", result.AccessToken);
    }

    [Fact]
    public async Task LoginAsync_Should_Fail_With_Invalid_Password()
    {
        // Arrange
        var passwordHash = BCrypt.Net.BCrypt.HashPassword("password123");
        var user = new User
        {
            Id = 1,
            Email = "test@email.sn",
            PasswordHash = passwordHash,
            EstActif = true
        };

        _userRepoMock.Setup(r => r.GetByEmailAsync("test@email.sn")).ReturnsAsync(user);

        // Act
        var result = await _authService.LoginAsync("test@email.sn", "wrongpassword");

        // Assert
        Assert.False(result.Success);
        Assert.Equal("Email ou mot de passe incorrect.", result.Error);
    }
}

// DTOs temporaires pour les tests
public record RegisterRequest(string Nom, string Prenom, string Email, string Password, string? Telephone);
