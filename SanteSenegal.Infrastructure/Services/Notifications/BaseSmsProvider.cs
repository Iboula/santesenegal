using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SanteSenegal.Application.Abstractions.Notifications;

namespace SanteSenegal.Infrastructure.Services.Notifications;

public abstract class BaseSmsProvider : ISmsProvider
{
    protected readonly HttpClient _httpClient;
    protected readonly IConfiguration _configuration;
    protected readonly ILogger _logger;
    protected readonly bool _sandbox;

    protected BaseSmsProvider(
        HttpClient httpClient,
        IConfiguration configuration,
        ILogger logger,
        string configPrefix)
    {
        _httpClient = httpClient;
        _configuration = configuration;
        _logger = logger;
        _sandbox = bool.TryParse(configuration[$"SMS:{configPrefix}:Sandbox"], out var sb) && sb;
    }

    public abstract string Nom { get; }
    public abstract Task<SmsSendResult> SendAsync(SmsMessage message, CancellationToken ct = default);
    public abstract Task<SmsDeliveryStatus?> GetStatusAsync(string providerReference, CancellationToken ct = default);

    protected string GenererReference() => $"SANTE-SMS-{DateTime.UtcNow:yyyyMMddHHmmss}-{Guid.NewGuid().ToString()[..6]}";

    protected SmsSendResult CreerResultatSandbox(SmsMessage message)
    {
        var reference = GenererReference();
        _logger.LogInformation("[SANDBOX] SMS vers {To}: {Message}", message.To, message.Message);
        return new SmsSendResult(
            Succes: true,
            ProviderReference: reference,
            MessageId: reference,
            Cout: 5.0m, // 5 XOF en sandbox
            RawResponse: $"{{\"sandbox\":true,\"to\":\"{message.To}\"}}",
            Erreur: null
        );
    }
}
