using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SanteSenegal.Application.Abstractions.Paiements;
using SanteSenegal.Domain.Enums;

namespace SanteSenegal.Infrastructure.Services.Paiements;

public abstract class BasePaiementMobileProvider : IPaiementMobileProvider
{
    protected readonly HttpClient _httpClient;
    protected readonly IConfiguration _configuration;
    protected readonly ILogger _logger;
    protected readonly string _apiKey;
    protected readonly string _baseUrl;
    protected readonly bool _sandbox;

    protected BasePaiementMobileProvider(
        HttpClient httpClient,
        IConfiguration configuration,
        ILogger logger,
        string configPrefix)
    {
        _httpClient = httpClient;
        _configuration = configuration;
        _logger = logger;
        _apiKey = configuration[$"PaiementMobile:{configPrefix}:ApiKey"] ?? string.Empty;
        _baseUrl = configuration[$"PaiementMobile:{configPrefix}:BaseUrl"] ?? string.Empty;
        _sandbox = bool.TryParse(configuration[$"PaiementMobile:{configPrefix}:Sandbox"], out var sb) && sb;
    }

    public abstract ModePaiement Mode { get; }
    public abstract Task<InitierPaiementResult> InitierPaiementAsync(InitierPaiementRequest request, CancellationToken ct = default);
    public abstract Task<VerifierPaiementResult> VerifierPaiementAsync(string referenceExterne, CancellationToken ct = default);

    protected string GenererReferenceUnique() => $"SANTE-{Mode}-{DateTime.UtcNow:yyyyMMddHHmmss}-{Guid.NewGuid().ToString()[..8]}";

    protected void LogRequest(string method, string url, object? payload = null)
    {
        _logger.LogInformation("[{Mode}] {Method} {Url} | Sandbox={Sandbox}", Mode, method, url, _sandbox);
        if (payload != null)
            _logger.LogDebug("Payload: {Payload}", JsonSerializer.Serialize(payload));
    }

    protected InitierPaiementResult CreerResultatSandbox(InitierPaiementRequest request)
    {
        var reference = GenererReferenceUnique();
        return new InitierPaiementResult(
            Succes: true,
            ReferenceExterne: reference,
            UrlPaiement: $"https://sandbox.santesenegal.sn/payer/{reference}",
            CodeConfirmation: new Random().Next(100000, 999999).ToString(),
            DateExpiration: DateTime.UtcNow.AddMinutes(30),
            Message: $"[SANDBOX] Paiement {Mode} initié. Utilisez le code {reference} pour simuler.",
            RawResponse: $"{{\"sandbox\":true,\"ref\":\"{reference}\"}}"
        );
    }

    protected VerifierPaiementResult CreerVerificationSandbox(string reference)
    {
        // En sandbox, on simule un paiement réussi 70% du temps
        var estPaye = new Random().NextDouble() > 0.3;
        return new VerifierPaiementResult(
            Succes: true,
            EstPaye: estPaye,
            ReferenceExterne: reference,
            MontantPaye: estPaye ? 5000 : null,
            DatePaiement: estPaye ? DateTime.UtcNow.AddMinutes(-5) : null,
            Message: estPaye ? "[SANDBOX] Paiement confirmé." : "[SANDBOX] Paiement en attente.",
            RawResponse: $"{{\"sandbox\":true,\"status\":\"{(estPaye ? "SUCCESS" : "PENDING")}\"}}"
        );
    }
}
