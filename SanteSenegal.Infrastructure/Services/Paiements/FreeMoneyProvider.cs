using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SanteSenegal.Application.Abstractions.Paiements;
using SanteSenegal.Domain.Enums;

namespace SanteSenegal.Infrastructure.Services.Paiements;

public class FreeMoneyProvider : BasePaiementMobileProvider
{
    public FreeMoneyProvider(
        HttpClient httpClient,
        IConfiguration configuration,
        ILogger<FreeMoneyProvider> logger)
        : base(httpClient, configuration, logger, "FreeMoney")
    {
    }

    public override ModePaiement Mode => ModePaiement.FreeMoney;

    public override async Task<InitierPaiementResult> InitierPaiementAsync(InitierPaiementRequest request, CancellationToken ct = default)
    {
        if (_sandbox)
            return CreerResultatSandbox(request);

        var reference = GenererReferenceUnique();
        var payload = new
        {
            api_key = _apiKey,
            service_id = "SANTE_SN",
            order_id = reference,
            amount = request.Montant,
            currency = request.Devise,
            phone_number = request.NumeroTelephone,
            callback_url = $"https://santesenegal.sn/api/paiements/webhook/freemoney",
            return_url = $"https://santesenegal.sn/paiement/succes?ref={reference}"
        };

        LogRequest("POST", $"{_baseUrl}/api/v1/payments/initiate", payload);

        try
        {
            var response = await _httpClient.PostAsJsonAsync($"{_baseUrl}/api/v1/payments/initiate", payload, ct);
            var content = await response.Content.ReadAsStringAsync(ct);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("FreeMoney API error: {Status} - {Content}", response.StatusCode, content);
                return new InitierPaiementResult(false, null, null, null, null, "Erreur API FreeMoney", content);
            }

            using var doc = JsonDocument.Parse(content);
            var transactionId = doc.RootElement.GetProperty("transaction_id").GetString();
            var paymentUrl = doc.RootElement.TryGetProperty("payment_url", out var urlProp) ? urlProp.GetString() : null;
            var otpRequired = doc.RootElement.TryGetProperty("otp_required", out var otpProp) && otpProp.GetBoolean();

            return new InitierPaiementResult(
                Succes: true,
                ReferenceExterne: transactionId,
                UrlPaiement: paymentUrl,
                CodeConfirmation: otpRequired ? new Random().Next(100000, 999999).ToString() : null,
                DateExpiration: DateTime.UtcNow.AddMinutes(30),
                Message: otpRequired
                    ? "Paiement FreeMoney initié. Entrez le OTP reçu par SMS."
                    : "Paiement FreeMoney initié. Confirmez sur votre téléphone.",
                RawResponse: content
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception lors de l'initiation FreeMoney");
            return new InitierPaiementResult(false, null, null, null, null, $"Exception: {ex.Message}", null);
        }
    }

    public override async Task<VerifierPaiementResult> VerifierPaiementAsync(string referenceExterne, CancellationToken ct = default)
    {
        if (_sandbox)
            return CreerVerificationSandbox(referenceExterne);

        LogRequest("GET", $"{_baseUrl}/api/v1/payments/status/{referenceExterne}");

        try
        {
            var response = await _httpClient.GetAsync($"{_baseUrl}/api/v1/payments/status/{referenceExterne}", ct);
            var content = await response.Content.ReadAsStringAsync(ct);

            if (!response.IsSuccessStatusCode)
                return new VerifierPaiementResult(false, false, referenceExterne, null, null, "Erreur vérification FreeMoney", content);

            using var doc = JsonDocument.Parse(content);
            var status = doc.RootElement.GetProperty("status").GetString();
            var estPaye = status == "COMPLETED" || status == "SUCCESS";
            var amount = doc.RootElement.TryGetProperty("amount", out var amountProp) ? amountProp.GetDecimal() : (decimal?)null;

            return new VerifierPaiementResult(
                Succes: true,
                EstPaye: estPaye,
                ReferenceExterne: referenceExterne,
                MontantPaye: amount,
                DatePaiement: estPaye ? DateTime.UtcNow : null,
                Message: estPaye ? "Paiement FreeMoney confirmé." : $"Statut FreeMoney: {status}",
                RawResponse: content
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception lors de la vérification FreeMoney");
            return new VerifierPaiementResult(false, false, referenceExterne, null, null, $"Exception: {ex.Message}", null);
        }
    }
}
