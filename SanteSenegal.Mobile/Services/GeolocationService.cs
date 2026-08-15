namespace SanteSenegal.Mobile.Services;

public interface IGeolocationService
{
    Task<Location?> GetCurrentLocationAsync();
}

public class GeolocationService : IGeolocationService
{
    public async Task<Location?> GetCurrentLocationAsync()
    {
        try
        {
            var status = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();
            if (status != PermissionStatus.Granted)
                return null;

            var location = await Geolocation.Default.GetLocationAsync(new GeolocationRequest
            {
                DesiredAccuracy = GeolocationAccuracy.Best,
                Timeout = TimeSpan.FromSeconds(10)
            });

            return location;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Erreur GPS: {ex.Message}");
            return null;
        }
    }
}
