using System.Net.Http.Headers;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SanteSenegal.Application.Abstractions.Notifications;

namespace SanteSenegal.Infrastructure.Services.Notifications;

public class TwilioProvider : BaseSmsProvider
{
    private readonly string _accountSid;
    private readonly string _authToken;
    private readonly string _fromNumber;

    public TwilioProvider(
        HttpClient httpClient,
        IConfiguration configuration,
        ILogger<TwilioProvider> logger)
        : base(httpClient, configuration, logger, "Twilio")
    {
        _accountSid = configuration["SMS:Twilio:AccountSid"] ?? string.Empty;
        _authToken = configuration["SMS:Twilio:AuthToken"] ?? string.Empty;
        _fromNumber = configuration["SMS:Twilio:FromNumber"] ?? string.Empty;

        var auth = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes($"{_accountSid}:{_authToken}"));
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", auth);
    }

    public override string Nom => "Twilio";

    public override async Task<SmsSendResult> SendAsync(SmsMessage message, CancellationToken ct = default)
    {
        if (_sandbox)
            return CreerResultatSandbox(message);

        var url = $"https://api.twilio.com/2010-04-01/Accounts/{_accountSid}/Messages.json";
        var content = new FormUrlEncodedContent(new[]
        {
            new KeyValuePair<string, string>("To", FormaterNumero(message.To)),
            new KeyValuePair<string, string>("From", _fromNumber),
            new KeyValuePair<string, string>("Body", message.Message)
        });

        try
        {
            var response = await _httpClient.PostAsync(url, content, ct);
            var responseBody = await response.Content.ReadAsStringAsync(ct);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("Twilio error: {Status} - {Body}", response.StatusCode, responseBody);
                return new SmsSendResult(false, null, null, null, responseBody, $"HTTP {response.StatusCode}");
            }

            using var doc = JsonDocument.Parse(responseBody);
            var sid = doc.RootElement.GetProperty("sid").GetString();
            var status = doc.RootElement.GetProperty("status").GetString();
            var price = doc.RootElement.TryGetProperty("price", out var priceProp)
                ? (decimal?)null // Twilio retourne le prix en string négatif
                : (decimal?)null;

            return new SmsSendResult(
                Succes: status == "queued" || status == "sent" || status == "delivered",
                ProviderReference: sid,
                MessageId: sid,
                Cout: price,
                RawResponse: responseBody,
                Erreur: null
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception Twilio SMS");
            return new SmsSendResult(false, null, null, null, null, ex.Message);
        }
    }

    public override async Task<SmsDeliveryStatus?> GetStatusAsync(string providerReference, CancellationToken ct = default)
    {
        if (_sandbox)
            return new SmsDeliveryStatus(providerReference, true, DateTime.UtcNow, "delivered", "{\"sandbox\":true}");

        var url = $"https://api.twilio.com/2010-04-01/Accounts/{_accountSid}/Messages/{providerReference}.json";
        var response = await _httpClient.GetAsync(url, ct);
        var body = await response.Content.ReadAsStringAsync(ct);

        if (!response.IsSuccessStatusCode) return null;

        using var doc = JsonDocument.Parse(body);
        var status = doc.RootElement.GetProperty("status").GetString();
        return new SmsDeliveryStatus(
            providerReference,
            status == "delivered",
            DateTime.UtcNow,
            status,
            body
        );
    }

    private static string FormaterNumero(string numero)
    {
        numero = numero.Replace(" ", "").Replace("-", "");
        if (numero.StartsWith("+")) return numero;
        if (numero.Length == 9) return $"+221{numero}";
        return numero;
    }
}
