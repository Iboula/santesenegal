using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SanteSenegal.Application.Abstractions;
using SanteSenegal.Application.Abstractions.NavigSante;
using SanteSenegal.Application.Abstractions.Notifications;
using SanteSenegal.Application.Abstractions.Paiements;
using SanteSenegal.Domain.Abstractions;
using SanteSenegal.Infrastructure.Persistence;
using SanteSenegal.Infrastructure.Services;
using SanteSenegal.Infrastructure.Services.NavigSante;
using SanteSenegal.Infrastructure.Services.Notifications;
using SanteSenegal.Infrastructure.Services.Paiements;

namespace SanteSenegal.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? "Data Source=/app/data/sante_senegal.db";

        services.AddDbContext<SanteDbContext>(options =>
            options.UseSqlite(connectionString));

        // Repositories
        services.AddScoped<IPatientRepository, PatientRepository>();
        services.AddScoped<IStructureRepository, StructureRepository>();
        services.AddScoped<IServiceRepository, ServiceRepository>();
        services.AddScoped<ISousServiceRepository, SousServiceRepository>();
        services.AddScoped<IDisponibiliteRepository, DisponibiliteRepository>();
        services.AddScoped<IRendezVousRepository, RendezVousRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IPaiementRepository, PaiementRepository>();
        services.AddScoped<INotificationRepository, NotificationRepository>();
        services.AddScoped(typeof(IBaseRepository<>), typeof(BaseRepository<>));
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        // Services
        services.AddScoped<IPaiementService, PaiementService>();
        services.AddScoped<IJwtService, JwtService>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<ITriageService, TriageService>();
        services.AddScoped<IAccidentRouteService, AccidentRouteService>();
        services.AddScoped<INotificationService, NotificationService>();

        // ── Paiement Mobile Sénégalais ──
        services.AddHttpClient<WaveProvider>();
        services.AddHttpClient<OrangeMoneyProvider>();
        services.AddHttpClient<FreeMoneyProvider>();

        services.AddScoped<IPaiementMobileProvider, WaveProvider>();
        services.AddScoped<IPaiementMobileProvider, OrangeMoneyProvider>();
        services.AddScoped<IPaiementMobileProvider, FreeMoneyProvider>();
        services.AddScoped<PaiementMobileFactory>();

        // ── Notifications SMS ──
        services.AddHttpClient<AfricasTalkingProvider>();
        services.AddHttpClient<TwilioProvider>();

        services.AddScoped<ISmsProvider, AfricasTalkingProvider>();
        services.AddScoped<ISmsProvider, TwilioProvider>();
        services.AddScoped<SmsProviderFactory>();

        return services;
    }
}
