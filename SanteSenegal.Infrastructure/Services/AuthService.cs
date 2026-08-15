using SanteSenegal.Application.Abstractions;
using SanteSenegal.Domain.Abstractions;
using SanteSenegal.Domain.Entities;
using SanteSenegal.Domain.Enums;

namespace SanteSenegal.Infrastructure.Services;

public interface IAuthService
{
    Task<AuthResult> RegisterAsync(string email, string password, string nom, string prenom, string? telephone, UserRole role);
    Task<AuthResult> LoginAsync(string email, string password);
    Task<AuthResult?> RefreshTokenAsync(string refreshToken);
    Task<bool> LogoutAsync(int userId, string refreshToken);
}

public record AuthResult(
    bool Success,
    string? AccessToken = null,
    string? RefreshToken = null,
    int? ExpiresIn = null,
    UserDto? User = null,
    string? Error = null
);

public record UserDto(int Id, string Email, string Nom, string Prenom, string? Telephone, UserRole Role);

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IJwtService _jwtService;
    private readonly IUnitOfWork _unitOfWork;

    public AuthService(IUserRepository userRepository, IJwtService jwtService, IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _jwtService = jwtService;
        _unitOfWork = unitOfWork;
    }

    public async Task<AuthResult> RegisterAsync(string email, string password, string nom, string prenom, string? telephone, UserRole role)
    {
        if (await _userRepository.EmailExistsAsync(email))
            return new AuthResult(false, Error: "Cet email est déjà utilisé.");

        var user = new User
        {
            Email = email.ToLower().Trim(),
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(password, workFactor: 12),
            Nom = nom.Trim(),
            Prenom = prenom.Trim(),
            Telephone = telephone?.Trim(),
            Role = role
        };

        await _userRepository.AddAsync(user);
        await _unitOfWork.SaveChangesAsync();

        return await GenerateTokensAsync(user);
    }

    public async Task<AuthResult> LoginAsync(string email, string password)
    {
        var user = await _userRepository.GetByEmailAsync(email.ToLower().Trim());
        if (user == null || !user.EstActif)
            return new AuthResult(false, Error: "Email ou mot de passe incorrect.");

        if (!BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
            return new AuthResult(false, Error: "Email ou mot de passe incorrect.");

        return await GenerateTokensAsync(user);
    }

    public async Task<AuthResult?> RefreshTokenAsync(string refreshToken)
    {
        // Simplifié - en production avec un RefreshTokenRepository dédié
        return null;
    }

    public async Task<bool> LogoutAsync(int userId, string refreshToken)
    {
        await _unitOfWork.SaveChangesAsync();
        return true;
    }

    private async Task<AuthResult> GenerateTokensAsync(User user)
    {
        var accessToken = _jwtService.GenerateAccessToken(user);
        var refreshTokenString = _jwtService.GenerateRefreshToken();

        var refreshToken = new RefreshToken
        {
            Token = refreshTokenString,
            ExpiresAt = DateTime.UtcNow.AddDays(7),
            UserId = user.Id
        };

        user.RefreshTokens.Add(refreshToken);
        await _unitOfWork.SaveChangesAsync();

        return new AuthResult(
            true,
            accessToken,
            refreshTokenString,
            3600,
            new UserDto(user.Id, user.Email, user.Nom, user.Prenom, user.Telephone, user.Role)
        );
    }
}
