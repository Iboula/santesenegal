using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using SanteSenegal.Web;
using SanteSenegal.Web.Auth;
using SanteSenegal.Web.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// ── Configuration API ──
builder.Services.AddScoped(sp => new HttpClient
{
    BaseAddress = new Uri(builder.Configuration["ApiBaseUrl"] ?? "https://localhost:7001/")
});

// ── Local Storage ──
builder.Services.AddScoped<ILocalStorageService, LocalStorageService>();

// ── Auth JWT ──
builder.Services.AddAuthorizationCore();
builder.Services.AddScoped<AuthenticationStateProvider, JwtAuthenticationStateProvider>();
builder.Services.AddScoped<IJwtAuthenticationStateProvider>(sp =>
    (JwtAuthenticationStateProvider)sp.GetRequiredService<AuthenticationStateProvider>());

// ── Services métier ──
builder.Services.AddScoped<IAuthService, AuthServiceClient>();
builder.Services.AddScoped<IStructureService, StructureServiceClient>();
builder.Services.AddScoped<IRendezVousService, RendezVousServiceClient>();
builder.Services.AddScoped<INavigSanteService, NavigSanteServiceClient>();
builder.Services.AddScoped<IPaiementService, PaiementServiceClient>();
builder.Services.AddScoped<INotificationService, NotificationServiceClient>();
builder.Services.AddScoped<IAlertesHubClient, AlertesHubClient>();

await builder.Build().RunAsync();
