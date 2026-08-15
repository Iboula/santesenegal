using SanteSenegal.Application.Abstractions.Notifications;

namespace SanteSenegal.Infrastructure.Services.Notifications;

public class SmsProviderFactory
{
    private readonly IEnumerable<ISmsProvider> _providers;

    public SmsProviderFactory(IEnumerable<ISmsProvider> providers)
    {
        _providers = providers;
    }

    public ISmsProvider GetPrimaryProvider()
    {
        // Priorité: Africa's Talking > Twilio
        return _providers.FirstOrDefault(p => p.Nom == "Africa's Talking")
            ?? _providers.FirstOrDefault(p => p.Nom == "Twilio")
            ?? _providers.First()
            ?? throw new InvalidOperationException("Aucun provider SMS configuré.");
    }

    public ISmsProvider? GetProviderByName(string nom)
    {
        return _providers.FirstOrDefault(p => p.Nom.Equals(nom, StringComparison.OrdinalIgnoreCase));
    }
}
