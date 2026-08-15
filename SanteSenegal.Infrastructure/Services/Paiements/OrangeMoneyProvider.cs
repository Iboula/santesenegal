using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SanteSenegal.Application.Abstractions.Paiements;
using SanteSenegal.Domain.Enums;

namespace SanteSenegal.Infrastructure.Services.Paiements;

public class OrangeMoneyProvider : BasePaiementMobileProvider
{
    public OrangeMoneyProvider(
        HttpClient httpClient,
        IConfiguration configuration,
        ILogger<OrangeMoneyProvider> logger)
        : base(httpClient, configuration, logger, "OrangeMoney")
    {
    }

    public override ModePaiement Mode => ModePaiement.OrangeMoney;

    public override async Task<InitierPaiementResult> InitierPaiementAsync(InitierPaiementRequest request, CancellationToken ct = default)
    {
        if (_sandbox)
            return CreerResultatSandbox(request);

        var reference = GenererReferenceUnique();
        var payload = new
        {
            merchant_key = _apiKey,
            currency = request.Devise,
            order_id = reference,
            amount = request.Montant,
            return_url = $"https://santesenegal.sn/paiement/succes?ref={reference}",
            cancel_url = $"https://santesenegal.sn/paiement/echec?ref={reference}",
            notif_url = $"https://santesenegal.sn/api/paiements/webhook/orange",
            lang = "fr",
            reference = request.ReferenceClient ?? $"RDV-{request.NumeroTelephone}"
        };

        LogRequest("POST", $"{_baseUrl}/payment", payload);

        try
        {
            var response = await _httpClient.PostAsJsonAsync($"{_baseUrl}/payment", payload, ct);
            var content = await response.Content.ReadAsStringAsync(ct);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("Orange Money API error: {Status} - {Content}", response.StatusCode, content);
                return new InitierPaiementResult(false, null, null, null, null, "Erreur API Orange Money", content);
            }

            using var doc = JsonDocument.Parse(content);
            var paymentUrl = doc.RootElement.GetProperty("payment_url").GetString();
            var paymentToken = doc.RootElement.GetProperty("payment_token").GetString();

            return new InitierPaiementResult(
                Succes: true,
                ReferenceExterne: paymentToken,
                UrlPaiement: paymentUrl,
                CodeConfirmation: null,
                DateExpiration: DateTime.UtcNow.AddMinutes(30),
                Message: "Paiement Orange Money initié. Validez sur votre téléphone.",
                RawResponse: content
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception lors de l'initiation Orange Money");
            return new InitierPaiementResult(false, null, null, null, null, $"Exception: {ex.Message}", null);
        }
    }

    public override async Task<VerifierPaiementResult> VerifierPaiementAsync(string referenceExterne, CancellationToken ct = default)
    {
        if (_sandbox)
            return CreerVerificationSandbox(referenceExterne);

        LogRequest("GET", $"{_baseUrl}/payment/status/{referenceExterne}");

        try
        {
            var response = await _httpClient.GetAsync($"{_baseUrl}/payment/status/{referenceExterne}", ct);
            var content = await response.Content.ReadAsStringAsync(ct);

            if (!response.IsSuccessStatusCode)
                return new VerifierPaiementResult(false, false, referenceExterne, null, null, "Erreur vérification OM", content);

            using var doc = JsonDocument.Parse(content);
            var status = doc.RootElement.GetProperty("status").GetString();
            var estPaye = status == "SUCCESS";
            var amount = doc.RootElement.TryGetProperty("amount", out var amountProp) ? amountProp.GetDecimal() : (decimal?)null;

            return new VerifierPaiementResult(
                Succes: true,
                EstPaye: estPaye,
                ReferenceExterne: referenceExterne,
                MontantPaye: amount,
                DatePaiement: estPaye ? DateTime.UtcNow : null,
                Message: estPaye ? "Paiement Orange Money confirmé." : $"Statut OM: {status}",
                RawResponse: content
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception lors de la vérification Orange Money");
            return new VerifierPaiementResult(false, false, referenceExterne, null, null, $"Exception: {ex.Message}", null);
        }
    }
}
