using System.Net.Http.Json;
using SanteSenegal.Web.Auth;

namespace SanteSenegal.Web.Services;

public interface IAuthService
{
    Task<AuthResult?> LoginAsync(string email, string password);
    Task<AuthResult?> RegisterAsync(RegisterRequest request);
    Task LogoutAsync();
    Task<bool> IsAuthenticatedAsync();
}

public class AuthServiceClient : IAuthService
{
    private readonly HttpClient _httpClient;
    private readonly IJwtAuthenticationStateProvider _authState;

    public AuthServiceClient(HttpClient httpClient, IJwtAuthenticationStateProvider authState)
    {
        _httpClient = httpClient;
        _authState = authState;
    }

    public async Task<AuthResult?> LoginAsync(string email, string password)
    {
        var response = await _httpClient.PostAsJsonAsync("api/auth/login", new { email, password });
        if (!response.IsSuccessStatusCode) return null;

        var result = await response.Content.ReadFromJsonAsync<AuthResult>();
        if (result?.AccessToken != null)
        {
            await _authState.LoginAsync(result.AccessToken, result.RefreshToken ?? "");
        }
        return result;
    }

    public async Task<AuthResult?> RegisterAsync(RegisterRequest request)
    {
        var response = await _httpClient.PostAsJsonAsync("api/auth/register", request);
        if (!response.IsSuccessStatusCode) return null;
        return await response.Content.ReadFromJsonAsync<AuthResult>();
    }

    public async Task LogoutAsync()
    {
        await _httpClient.PostAsync("api/auth/logout", null);
        await _authState.LogoutAsync();
    }

    public async Task<bool> IsAuthenticatedAsync()
    {
        var token = await _authState.GetTokenAsync();
        return !string.IsNullOrEmpty(token);
    }
}

public record AuthResult(bool Success, string? AccessToken = null, string? RefreshToken = null, int? ExpiresIn = null, UserDto? User = null, string? Error = null);
public record UserDto(int Id, string Email, string Nom, string Prenom, string? Telephone, string Role);
public record RegisterRequest(string Email, string Password, string Nom, string Prenom, string? Telephone, string Role = "Patient");

// Stubs pour les autres services
public interface IStructureService { }
public class StructureServiceClient : IStructureService { public StructureServiceClient(HttpClient http) { } }

public interface IRendezVousService { }
public class RendezVousServiceClient : IRendezVousService { public RendezVousServiceClient(HttpClient http) { } }

public interface INavigSanteService { }
public class NavigSanteServiceClient : INavigSanteService { public NavigSanteServiceClient(HttpClient http) { } }

public interface IPaiementService { }
public class PaiementServiceClient : IPaiementService { public PaiementServiceClient(HttpClient http) { } }

public interface INotificationService { }
public class NotificationServiceClient : INotificationService { public NotificationServiceClient(HttpClient http) { } }
