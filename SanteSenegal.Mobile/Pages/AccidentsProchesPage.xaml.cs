using SanteSenegal.Mobile.Services;
using System.Collections.ObjectModel;

namespace SanteSenegal.Mobile.Pages;

public partial class AccidentsProchesPage : ContentPage
{
    private readonly IOfflineAccidentService _offlineService;
    private ObservableCollection<PendingAccident> _accidents = new();

    public AccidentsProchesPage()
    {
        InitializeComponent();
        _offlineService = new OfflineAccidentService(new HttpClient
        {
            BaseAddress = new Uri("https://localhost:7001/")
        });
        _offlineService.OnSyncStatusChanged += msg => MainThread.BeginInvokeOnMainThread(() =>
        {
            LblSyncStatus.Text = msg;
        });
        AccidentsList.ItemsSource = _accidents;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await LoadAccidentsAsync();
    }

    private async Task LoadAccidentsAsync()
    {
        var pending = await _offlineService.GetPendingAccidentsAsync();
        _accidents.Clear();
        foreach (var a in pending)
            _accidents.Add(a);
    }

    private async void OnSyncClicked(object? sender, EventArgs e)
    {
        BtnSync.IsEnabled = false;
        BtnSync.Text = "⏳ Synchronisation...";

        await _offlineService.SyncPendingAccidentsAsync();
        await LoadAccidentsAsync();

        BtnSync.IsEnabled = true;
        BtnSync.Text = "🔄 Synchroniser maintenant";
    }
}
