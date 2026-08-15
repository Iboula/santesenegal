using SanteSenegal.Mobile.Services;

namespace SanteSenegal.Mobile.Pages;

public partial class SignalerAccidentPage : ContentPage
{
    private readonly IGeolocationService _geolocationService;
    private readonly IOfflineAccidentService _offlineService;
    private Location? _currentLocation;

    public SignalerAccidentPage()
    {
        InitializeComponent();
        _geolocationService = new GeolocationService();
        _offlineService = new OfflineAccidentService(new HttpClient
        {
            BaseAddress = new Uri("https://localhost:7001/")
        });
        _offlineService.OnSyncStatusChanged += msg => MainThread.BeginInvokeOnMainThread(() =>
        {
            LblNetworkStatus.Text = msg;
        });
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await GetLocationAsync();
        UpdateNetworkStatus();
    }

    private async Task GetLocationAsync()
    {
        LblGpsStatus.Text = "📡 Acquisition GPS...";
        _currentLocation = await _geolocationService.GetCurrentLocationAsync();

        if (_currentLocation != null)
        {
            LblGpsStatus.Text = "✅ Localisation obtenue";
            LblCoordinates.Text = $"Lat: {_currentLocation.Latitude:F5}, Lng: {_currentLocation.Longitude:F5}";
        }
        else
        {
            LblGpsStatus.Text = "❌ GPS indisponible — saisie manuelle nécessaire";
            LblCoordinates.Text = "Lat: ---, Lng: ---";
        }
    }

    private void UpdateNetworkStatus()
    {
        var isOnline = Connectivity.NetworkAccess == NetworkAccess.Internet;
        LblNetworkStatus.Text = isOnline
            ? "🌐 En ligne — l'alerte sera envoyée immédiatement"
            : "📴 Hors ligne — l'alerte sera stockée et envoyée plus tard";
        LblNetworkStatus.TextColor = isOnline ? Colors.LightGreen : Colors.Gold;
    }

    private async void OnSignalerClicked(object? sender, EventArgs e)
    {
        BtnSignaler.IsEnabled = false;
        BtnSignaler.Text = "⏳ Envoi en cours...";

        try
        {
            if (_currentLocation == null)
            {
                await DisplayAlertAsync("GPS", "Localisation non disponible. Veuillez l'activer.", "OK");
                return;
            }

            int victimes = int.TryParse(EntryVictimes.Text, out var v) ? v : 1;

            await _offlineService.SignalerAccidentAsync(
                _currentLocation.Latitude,
                _currentLocation.Longitude,
                null,
                victimes,
                EntryDescription.Text,
                EntryTelephone.Text
            );

            // Vibration + son
            Vibration.Default.Vibrate(TimeSpan.FromSeconds(1));

            await DisplayAlertAsync("✅ Signalé", "Votre signalement a été enregistré. Les secours seront alertés.", "OK");

            // Reset
            EntryDescription.Text = "";
            EntryVictimes.Text = "";
        }
        catch (Exception ex)
        {
            await DisplayAlertAsync("Erreur", $"Impossible de signaler : {ex.Message}", "OK");
        }
        finally
        {
            BtnSignaler.IsEnabled = true;
            BtnSignaler.Text = "🚨 SIGNALER UN ACCIDENT";
        }
    }
}
