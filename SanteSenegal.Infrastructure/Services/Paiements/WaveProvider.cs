using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SanteSenegal.Application.Abstractions.Paiements;
using SanteSenegal.Domain.Enums;

namespace SanteSenegal.Infrastructure.Services.Paiements;

public class WaveProvider : BasePaiementMobileProvider
{
    public WaveProvider(
        HttpClient httpClient,
        IConfiguration configuration,
        ILogger<WaveProvider> logger)
        : base(httpClient, configuration, logger, "Wave")
    {
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _apiKey);
    }

    public override ModePaiement Mode => ModePaiement.Wave;

    public override async Task<InitierPaiementResult> InitierPaiementAsync(InitierPaiementRequest request, CancellationToken ct = default)
    {
        if (_sandbox)
            return CreerResultatSandbox(request);

        var reference = GenererReferenceUnique();
        var payload = new
        {
            amount = request.Montant,
            currency = request.Devise,
            error_url = $"https://santesenegal.sn/paiement/echec?ref={reference}",
            success_url = $"https://santesenegal.sn/paiement/succes?ref={reference}",
            client_reference = reference
        };

        LogRequest("POST", $"{_baseUrl}/v1/checkout/sessions", payload);

        try
        {
            var response = await _httpClient.PostAsJsonAsync($"{_baseUrl}/v1/checkout/sessions", payload, ct);
            var content = await response.Content.ReadAsStringAsync(ct);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("Wave API error: {Status} - {Content}", response.StatusCode, content);
                return new InitierPaiementResult(false, null, null, null, null, "Erreur API Wave", content);
            }

            using var doc = JsonDocument.Parse(content);
            var waveUrl = doc.RootElement.GetProperty("wave_launch_url").GetString();
            var waveId = doc.RootElement.GetProperty("id").GetString();

            return new InitierPaiementResult(
                Succes: true,
                ReferenceExterne: waveId,
                UrlPaiement: waveUrl,
                CodeConfirmation: null,
                DateExpiration: DateTime.UtcNow.AddMinutes(30),
                Message: "Paiement Wave initié. Scannez le QR code ou utilisez le lien.",
                RawResponse: content
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception lors de l'initiation Wave");
            return new InitierPaiementResult(false, null, null, null, null, $"Exception: {ex.Message}", null);
        }
    }

    public override async Task<VerifierPaiementResult> VerifierPaiementAsync(string referenceExterne, CancellationToken ct = default)
    {
        if (_sandbox)
            return CreerVerificationSandbox(referenceExterne);

        LogRequest("GET", $"{_baseUrl}/v1/checkout/sessions/{referenceExterne}");

        try
        {
            var response = await _httpClient.GetAsync($"{_baseUrl}/v1/checkout/sessions/{referenceExterne}", ct);
            var content = await response.Content.ReadAsStringAsync(ct);

            if (!response.IsSuccessStatusCode)
                return new VerifierPaiementResult(false, false, referenceExterne, null, null, "Erreur vérification Wave", content);

            using var doc = JsonDocument.Parse(content);
            var status = doc.RootElement.GetProperty("payment_status").GetString();
            var estPaye = status == "succeeded";
            var amount = doc.RootElement.TryGetProperty("amount", out var amountProp) ? amountProp.GetDecimal() : (decimal?)null;

            return new VerifierPaiementResult(
                Succes: true,
                EstPaye: estPaye,
                ReferenceExterne: referenceExterne,
                MontantPaye: amount,
                DatePaiement: estPaye ? DateTime.UtcNow : null,
                Message: estPaye ? "Paiement Wave confirmé." : $"Statut Wave: {status}",
                RawResponse: content
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception lors de la vérification Wave");
            return new VerifierPaiementResult(false, false, referenceExterne, null, null, $"Exception: {ex.Message}", null);
        }
    }
}
