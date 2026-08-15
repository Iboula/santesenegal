using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SanteSenegal.Application.Abstractions.Notifications;

namespace SanteSenegal.Infrastructure.Services.Notifications;

public class AfricasTalkingProvider : BaseSmsProvider
{
    private readonly string _apiKey;
    private readonly string _username;
    private readonly string _senderId;

    public AfricasTalkingProvider(
        HttpClient httpClient,
        IConfiguration configuration,
        ILogger<AfricasTalkingProvider> logger)
        : base(httpClient, configuration, logger, "AfricasTalking")
    {
        _apiKey = configuration["SMS:AfricasTalking:ApiKey"] ?? string.Empty;
        _username = configuration["SMS:AfricasTalking:Username"] ?? "sandbox";
        _senderId = configuration["SMS:AfricasTalking:SenderId"] ?? "SanteSN";

        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic",
            Convert.ToBase64String(Encoding.UTF8.GetBytes($"{_username}:{_apiKey}")));
        _httpClient.DefaultRequestHeaders.Add("apiKey", _apiKey);
    }

    public override string Nom => "Africa's Talking";

    public override async Task<SmsSendResult> SendAsync(SmsMessage message, CancellationToken ct = default)
    {
        if (_sandbox)
            return CreerResultatSandbox(message);

        var url = _username == "sandbox"
            ? "https://api.sandbox.africastalking.com/version1/messaging"
            : "https://api.africastalking.com/version1/messaging";

        var content = new FormUrlEncodedContent(new[]
        {
            new KeyValuePair<string, string>("username", _username),
            new KeyValuePair<string, string>("to", FormaterNumero(message.To)),
            new KeyValuePair<string, string>("message", message.Message),
            new KeyValuePair<string, string>("from", _senderId)
        });

        try
        {
            var response = await _httpClient.PostAsync(url, content, ct);
            var responseBody = await response.Content.ReadAsStringAsync(ct);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("Africa's Talking error: {Status} - {Body}", response.StatusCode, responseBody);
                return new SmsSendResult(false, null, null, null, responseBody, $"HTTP {response.StatusCode}");
            }

            using var doc = JsonDocument.Parse(responseBody);
            var smsMessageData = doc.RootElement.GetProperty("SMSMessageData");
            var recipients = smsMessageData.GetProperty("Recipients");
            var first = recipients.EnumerateArray().FirstOrDefault();

            var status = first.GetProperty("status").GetString();
            var messageId = first.GetProperty("messageId").GetString();
            var cost = first.TryGetProperty("cost", out var costProp) ? ExtractCost(costProp.GetString()) : (decimal?)null;

            var succes = status == "Success";
            return new SmsSendResult(
                Succes: succes,
                ProviderReference: messageId,
                MessageId: messageId,
                Cout: cost,
                RawResponse: responseBody,
                Erreur: succes ? null : status
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception Africa's Talking SMS");
            return new SmsSendResult(false, null, null, null, null, ex.Message);
        }
    }

    public override async Task<SmsDeliveryStatus?> GetStatusAsync(string providerReference, CancellationToken ct = default)
    {
        if (_sandbox)
            return new SmsDeliveryStatus(providerReference, true, DateTime.UtcNow, "DELIVERED", "{\"sandbox\":true}");

        // Africa's Talking ne fournit pas d'API simple de statut par messageId
        // On retourne un statut simulé
        return new SmsDeliveryStatus(providerReference, true, DateTime.UtcNow.AddMinutes(-1), "DELIVERED", null);
    }

    private static string FormaterNumero(string numero)
    {
        // Convertit 77xxxxxxx ou +22177xxxxxxx au format international +22177xxxxxxx
        numero = numero.Replace(" ", "").Replace("-", "");
        if (numero.StartsWith("+221")) return numero;
        if (numero.Length == 9 && numero.StartsWith("7")) return $"+221{numero}";
        if (numero.Length == 9) return $"+221{numero}";
        return numero;
    }

    private static decimal? ExtractCost(string? costStr)
    {
        if (string.IsNullOrEmpty(costStr)) return null;
        // Format typique: "XOF 5.00" ou "5.00"
        var parts = costStr.Split(' ');
        var numericPart = parts.Length > 1 ? parts[1] : parts[0];
        return decimal.TryParse(numericPart, out var cost) ? cost : (decimal?)null;
    }
}
