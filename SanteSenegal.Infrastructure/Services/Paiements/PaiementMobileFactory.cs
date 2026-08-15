using SanteSenegal.Application.Abstractions.Paiements;
using SanteSenegal.Application.DTOs.Paiements;
using SanteSenegal.Domain.Enums;

namespace SanteSenegal.Infrastructure.Services.Paiements;

public class PaiementMobileFactory
{
    private readonly IEnumerable<IPaiementMobileProvider> _providers;

    public PaiementMobileFactory(IEnumerable<IPaiementMobileProvider> providers)
    {
        _providers = providers;
    }

    public IPaiementMobileProvider GetProvider(ModePaiement mode)
    {
        var provider = _providers.FirstOrDefault(p => p.Mode == mode)
            ?? throw new NotSupportedException($"Mode de paiement {mode} non supporté.");
        return provider;
    }

    public IReadOnlyList<PaiementMobileConfigDto> GetConfigurationsDisponibles()
    {
        return _providers.Select(p => new PaiementMobileConfigDto(
            Operateur: p.Mode.ToString().ToLowerInvariant(),
            Nom: p.Mode switch
            {
                ModePaiement.Wave => "Wave",
                ModePaiement.OrangeMoney => "Orange Money",
                ModePaiement.FreeMoney => "Free Money",
                _ => p.Mode.ToString()
            },
            Disponible: true,
            LogoUrl: $"/assets/logos/{p.Mode.ToString().ToLowerInvariant()}.svg",
            FraisFixe: p.Mode == ModePaiement.Wave ? 0 : (decimal?)null,
            FraisPourcentage: p.Mode == ModePaiement.Wave ? 1 : 1.5m
        )).ToList();
    }
}
