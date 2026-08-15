using System.Net.Http.Json;
using System.Text.Json;

namespace SanteSenegal.Mobile.Services;

public interface IOfflineAccidentService
{
    Task SignalerAccidentAsync(double latitude, double longitude, string? adresse, int nombreVictimes, string? description, string? telephone);
    Task<List<PendingAccident>> GetPendingAccidentsAsync();
    Task SyncPendingAccidentsAsync();
    event Action<string>? OnSyncStatusChanged;
}

public class PendingAccident
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public string? Adresse { get; set; }
    public int NombreVictimes { get; set; }
    public string? Description { get; set; }
    public string? Telephone { get; set; }
    public DateTime DateSignalement { get; set; } = DateTime.UtcNow;
}

public class OfflineAccidentService : IOfflineAccidentService
{
    private readonly HttpClient _httpClient;
    private const string PendingKey = "pending_accidents";

    public event Action<string>? OnSyncStatusChanged;

    public OfflineAccidentService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task SignalerAccidentAsync(double latitude, double longitude, string? adresse, int nombreVictimes, string? description, string? telephone)
    {
        var accident = new PendingAccident
        {
            Latitude = latitude,
            Longitude = longitude,
            Adresse = adresse,
            NombreVictimes = nombreVictimes,
            Description = description,
            Telephone = telephone
        };

        var pending = GetPendingAccidents();
        pending.Add(accident);
        SavePendingAccidents(pending);

        // Si online, essayer d'envoyer immédiatement
        if (Connectivity.NetworkAccess == NetworkAccess.Internet)
        {
            await TrySendAccidentAsync(accident);
        }
    }

    public Task<List<PendingAccident>> GetPendingAccidentsAsync()
    {
        return Task.FromResult(GetPendingAccidents());
    }

    public async Task SyncPendingAccidentsAsync()
    {
        if (Connectivity.NetworkAccess != NetworkAccess.Internet)
        {
            OnSyncStatusChanged?.Invoke("❌ Pas de connexion internet");
            return;
        }

        var pending = GetPendingAccidents();
        if (pending.Count == 0)
        {
            OnSyncStatusChanged?.Invoke("✅ Aucun accident en attente");
            return;
        }

        var sent = new List<PendingAccident>();
        foreach (var accident in pending)
        {
            if (await TrySendAccidentAsync(accident))
                sent.Add(accident);
        }

        // Retirer les accidents envoyés
        var remaining = pending.Except(sent).ToList();
        SavePendingAccidents(remaining);

        OnSyncStatusChanged?.Invoke($"🚀 {sent.Count} accident(s) envoyé(s), {remaining.Count} en attente");
    }

    private async Task<bool> TrySendAccidentAsync(PendingAccident accident)
    {
        try
        {
            var request = new
            {
                Latitude = accident.Latitude,
                Longitude = accident.Longitude,
                Adresse = accident.Adresse,
                NombreVictimes = accident.NombreVictimes,
                Description = accident.Description,
                TelephoneSignaleur = accident.Telephone
            };

            var response = await _httpClient.PostAsJsonAsync("api/accidents/signaler", request);
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Échec envoi accident: {ex.Message}");
            return false;
        }
    }

    private List<PendingAccident> GetPendingAccidents()
    {
        var json = Preferences.Get(PendingKey, "[]");
        return JsonSerializer.Deserialize<List<PendingAccident>>(json) ?? new List<PendingAccident>();
    }

    private void SavePendingAccidents(List<PendingAccident> accidents)
    {
        var json = JsonSerializer.Serialize(accidents);
        Preferences.Set(PendingKey, json);
    }
}
