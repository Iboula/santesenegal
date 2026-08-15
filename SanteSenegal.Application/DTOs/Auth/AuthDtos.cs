namespace SanteSenegal.Application.DTOs.Auth;

public record RegisterRequest(
    string Email,
    string Password,
    string Nom,
    string Prenom,
    string? Telephone,
    string Role
);

public record LoginRequest(
    string Email,
    string Password
);

public record AuthResponse(
    string AccessToken,
    string RefreshToken,
    int ExpiresIn,
    UserInfo User
);

public record UserInfo(
    int Id,
    string Email,
    string Nom,
    string Prenom,
    string? Telephone,
    string Role
);
