using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using SanteSenegal.Api.Endpoints;
using SanteSenegal.Api.Endpoints.NavigSante;
using SanteSenegal.Api.Hubs;
using SanteSenegal.Infrastructure;
using SanteSenegal.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(typeof(Program).Assembly);
    cfg.RegisterServicesFromAssembly(typeof(SanteSenegal.Application.Class1).Assembly);
});

// ── SignalR Temps Réel ──
builder.Services.AddSignalR();

// JWT Authentication
var jwtSecret = builder.Configuration["Jwt:Secret"]!;
var jwtIssuer = builder.Configuration["Jwt:Issuer"]!;
var jwtAudience = builder.Configuration["Jwt:Audience"]!;

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret)),
            ValidateIssuer = true,
            ValidIssuer = jwtIssuer,
            ValidateAudience = true,
            ValidAudience = jwtAudience,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };

        // Pour SignalR avec JWT
        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                var accessToken = context.Request.Query["access_token"];
                var path = context.HttpContext.Request.Path;
                if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/hubs"))
                {
                    context.Token = accessToken;
                }
                return Task.CompletedTask;
            }
        };
    });

builder.Services.AddAuthorization();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Sante API", Version = "v1" });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Entrez votre token JWT. Exemple : Bearer eyJhbG..."
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

var app = builder.Build();

// ── Swagger toujours disponible (dev + prod) ──
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Sante API v1");
});

// ── Création auto de la base SQLite + seed si vide ──
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<SanteDbContext>();
    await dbContext.Database.EnsureCreatedAsync();

    // Seed uniquement si la base est vide (aucun utilisateur)
    if (!dbContext.Users.Any())
    {
        await DbSeeder.SeedAsync(dbContext);
    }
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

// ── Hubs SignalR ──
app.MapHub<AlertesHub>("/hubs/alertes");

// ── Endpoints API ──
app.MapAuthEndpoints();
app.MapAccidentsTempsReelEndpoints();
app.MapTriageEndpoints();
app.MapRessourceSanitaireEndpoints();
app.MapNavigSanteEndpoints();
app.MapPatientsEndpoints();
app.MapRendezVousEndpoints();
app.MapPaiementsEndpoints();
app.MapSousServiceEndpoints();
app.MapDisponibiliteEndpoints();
app.MapStructureEndpoints();
app.MapNotificationsEndpoints();

// Endpoint de seed manuel (Admin uniquement)
app.MapPost("/api/seed", [Microsoft.AspNetCore.Authorization.Authorize(Roles = "Admin")] async (SanteDbContext db) =>
{
    await DbSeeder.SeedAsync(db);
    return Results.Ok(new { message = "Base de données seedée avec succès !" });
});

app.Run();
