using Microsoft.AspNetCore.SignalR.Client;
using SanteSenegal.Domain.Entities.NavigSante;
using SanteSenegal.Web.Auth;

namespace SanteSenegal.Web.Services;

public interface IAlertesHubClient
{
    event Action<AccidentRoute>? NouvelAccident;
    event Action<int, string>? PriseEnCharge;
    event Action<int, RessourceSanitaire>? MiseAJourRessources;
    Task StartAsync();
    Task StopAsync();
    bool IsConnected { get; }
}

public class AlertesHubClient : IAlertesHubClient, IAsyncDisposable
{
    private readonly IJwtAuthenticationStateProvider _authState;
    private readonly string _hubUrl;
    private HubConnection? _hubConnection;

    public event Action<AccidentRoute>? NouvelAccident;
    public event Action<int, string>? PriseEnCharge;
    public event Action<int, RessourceSanitaire>? MiseAJourRessources;

    public bool IsConnected => _hubConnection?.State == HubConnectionState.Connected;

    public AlertesHubClient(IJwtAuthenticationStateProvider authState, IConfiguration configuration)
    {
        _authState = authState;
        var apiBase = configuration["ApiBaseUrl"] ?? "https://localhost:7001/";
        _hubUrl = apiBase.TrimEnd('/') + "/hubs/alertes";
    }

    public async Task StartAsync()
    {
        if (_hubConnection != null) return;

        var token = await _authState.GetTokenAsync();

        _hubConnection = new HubConnectionBuilder()
            .WithUrl(_hubUrl, options =>
            {
                if (!string.IsNullOrEmpty(token))
                {
                    options.AccessTokenProvider = () => Task.FromResult<string?>(token);
                }
            })
            .WithAutomaticReconnect(new[] { TimeSpan.Zero, TimeSpan.FromSeconds(2), TimeSpan.FromSeconds(10), TimeSpan.FromSeconds(30) })
            .Build();

        _hubConnection.On<AccidentRoute>("NouvelAccident", accident =>
        {
            NouvelAccident?.Invoke(accident);
        });

        _hubConnection.On<int, string>("PriseEnCharge", (accidentId, nomHopital) =>
        {
            PriseEnCharge?.Invoke(accidentId, nomHopital);
        });

        _hubConnection.On<int, RessourceSanitaire>("MiseAJourRessources", (structureId, ressources) =>
        {
            MiseAJourRessources?.Invoke(structureId, ressources);
        });

        await _hubConnection.StartAsync();
    }

    public async Task StopAsync()
    {
        if (_hubConnection != null)
        {
            await _hubConnection.StopAsync();
            await _hubConnection.DisposeAsync();
            _hubConnection = null;
        }
    }

    public async ValueTask DisposeAsync()
    {
        await StopAsync();
    }
}
