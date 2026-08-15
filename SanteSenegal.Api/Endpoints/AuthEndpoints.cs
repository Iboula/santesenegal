using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using SanteSenegal.Application.DTOs.Auth;
using SanteSenegal.Domain.Enums;
using SanteSenegal.Infrastructure.Services;

namespace SanteSenegal.Api.Endpoints;

public static class AuthEndpoints
{
    public static void MapAuthEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/auth");

        // POST /api/auth/register
        group.MapPost("/register", async (RegisterRequest request, IAuthService authService) =>
        {
            if (!Enum.TryParse<UserRole>(request.Role, true, out var role))
                return Results.BadRequest(new { Error = "Rôle invalide. Valeurs acceptées : Patient, Medecin, Admin" });

            var result = await authService.RegisterAsync(
                request.Email, request.Password, request.Nom, request.Prenom, request.Telephone, role);

            if (!result.Success)
                return Results.BadRequest(new { Error = result.Error });

            return Results.Ok(new AuthResponse(
                result.AccessToken!,
                result.RefreshToken!,
                result.ExpiresIn!.Value,
                new UserInfo(result.User!.Id, result.User.Email, result.User.Nom, result.User.Prenom, result.User.Telephone, result.User.Role.ToString())
            ));
        })
        .AllowAnonymous();

        // POST /api/auth/login
        group.MapPost("/login", async (LoginRequest request, IAuthService authService) =>
        {
            var result = await authService.LoginAsync(request.Email, request.Password);

            if (!result.Success)
                return Results.BadRequest(new { Error = result.Error });

            return Results.Ok(new AuthResponse(
                result.AccessToken!,
                result.RefreshToken!,
                result.ExpiresIn!.Value,
                new UserInfo(result.User!.Id, result.User.Email, result.User.Nom, result.User.Prenom, result.User.Telephone, result.User.Role.ToString())
            ));
        })
        .AllowAnonymous();

        // GET /api/auth/me
        group.MapGet("/me", [Authorize] (ClaimsPrincipal user) =>
        {
            var userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var email = user.FindFirst(ClaimTypes.Email)?.Value;
            var name = user.FindFirst(ClaimTypes.Name)?.Value;
            var role = user.FindFirst(ClaimTypes.Role)?.Value;

            return Results.Ok(new { UserId = userId, Email = email, Name = name, Role = role });
        });

        // POST /api/auth/logout
        group.MapPost("/logout", [Authorize] async (ClaimsPrincipal user, IAuthService authService) =>
        {
            var userId = int.Parse(user.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            // En production, passer le refresh token depuis le body/cookie
            await authService.LogoutAsync(userId, string.Empty);
            return Results.Ok(new { Message = "Déconnecté avec succès." });
        });
    }
}
