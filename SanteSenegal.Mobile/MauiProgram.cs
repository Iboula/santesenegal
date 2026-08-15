using Microsoft.Extensions.Logging;
using SanteSenegal.Mobile.Services;

namespace SanteSenegal.Mobile;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
			});

		// ── Services HTTP & Métier ──
		builder.Services.AddSingleton<IGeolocationService, GeolocationService>();
		builder.Services.AddSingleton<HttpClient>(sp => new HttpClient
		{
			BaseAddress = new Uri("https://localhost:7001/")
		});
		builder.Services.AddSingleton<IOfflineAccidentService, OfflineAccidentService>();

#if DEBUG
		builder.Logging.AddDebug();
#endif

		return builder.Build();
	}
}
