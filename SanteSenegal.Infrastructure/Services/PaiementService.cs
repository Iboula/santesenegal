using SanteSenegal.Application.Abstractions;
using SanteSenegal.Application.Abstractions.Paiements;
using SanteSenegal.Application.Common;
using SanteSenegal.Domain.Abstractions;
using SanteSenegal.Domain.Entities;
using SanteSenegal.Domain.Enums;
using SanteSenegal.Infrastructure.Services.Paiements;

namespace SanteSenegal.Infrastructure.Services;

public class PaiementService : IPaiementService
{
    private readonly IPaiementRepository _paiementRepository;
    private readonly IBaseRepository<Transaction> _transactionRepository;
    private readonly PaiementMobileFactory _factory;
    private readonly IUnitOfWork _unitOfWork;

    public PaiementService(
        IPaiementRepository paiementRepository,
        IBaseRepository<Transaction> transactionRepository,
        PaiementMobileFactory factory,
        IUnitOfWork unitOfWork)
    {
        _paiementRepository = paiementRepository;
        _transactionRepository = transactionRepository;
        _factory = factory;
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> ConfirmerPaiementAsync(int rendezVousId, decimal montant)
    {
        var paiements = await _paiementRepository.GetByRendezVousIdAsync(rendezVousId);
        var paiement = paiements.FirstOrDefault(p => p.Statut == StatutPaiement.EnAttente && p.Montant == montant);
        if (paiement is null) return false;

        paiement.Statut = StatutPaiement.Recu;
        paiement.DatePaiement = DateTime.UtcNow;
        await _paiementRepository.UpdateAsync(paiement);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }

    public async Task<bool> RembourserPaiementAsync(int rendezVousId, decimal montant)
    {
        var paiements = await _paiementRepository.GetByRendezVousIdAsync(rendezVousId);
        var paiement = paiements.FirstOrDefault(p => p.Statut == StatutPaiement.Recu && p.Montant == montant);
        if (paiement is null) return false;

        paiement.Statut = StatutPaiement.Rembourse;
        await _paiementRepository.UpdateAsync(paiement);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }

    public async Task<IEnumerable<Paiement>> GetAllPaiementsAsync()
        => await _paiementRepository.GetAllAsync();

    public Task<Paiement?> GetPaiementByIdAsync(int id)
        => _paiementRepository.GetByIdAsync(id);

    public async Task<IEnumerable<Paiement>> GetPaiementsByRendezVousAsync(int rendezVousId)
        => await _paiementRepository.GetByRendezVousIdAsync(rendezVousId);

    public async Task<IEnumerable<Paiement>> GetPaiementsByPatientAsync(int patientId)
        => await _paiementRepository.GetByPatientIdAsync(patientId);

    public async Task<Result<Paiement>> ProcessPaiementAsync(Paiement paiement)
    {
        await _paiementRepository.AddAsync(paiement);
        await _unitOfWork.SaveChangesAsync();
        return Result<Paiement>.Ok(paiement);
    }

    // ── Paiement Mobile Sénégalais ──

    public async Task<Result<Paiement>> InitierPaiementMobileAsync(
        int rendezVousId,
        decimal montant,
        ModePaiement mode,
        string numeroTelephone,
        string? description = null)
    {
        var provider = _factory.GetProvider(mode);

        var request = new InitierPaiementRequest(
            Montant: montant,
            NumeroTelephone: numeroTelephone,
            Devise: "XOF",
            Description: description,
            ReferenceClient: $"RDV-{rendezVousId}"
        );

        var result = await provider.InitierPaiementAsync(request);

        if (!result.Succes)
            return Result<Paiement>.Fail(result.Message ?? "Échec de l'initiation du paiement");

        var paiement = new Paiement
        {
            RendezVousId = rendezVousId,
            Montant = montant,
            ModePaiement = mode,
            NumeroTelephone = numeroTelephone,
            ReferenceExterne = result.ReferenceExterne,
            UrlPaiement = result.UrlPaiement,
            CodeConfirmation = result.CodeConfirmation,
            DateExpiration = result.DateExpiration,
            Statut = StatutPaiement.EnAttente,
            DatePaiement = DateTime.UtcNow
        };

        await _paiementRepository.AddAsync(paiement);

        var transaction = new Transaction
        {
            PaiementId = paiement.Id,
            Montant = montant,
            DateTransaction = DateTime.UtcNow,
            Statut = StatutTransaction.EnAttente,
            Reference = result.ReferenceExterne ?? Guid.NewGuid().ToString(),
            ModePaiement = mode,
            ProviderReference = result.ReferenceExterne ?? string.Empty,
            RawResponse = result.RawResponse,
            NumeroTelephone = numeroTelephone
        };

        await _transactionRepository.AddAsync(transaction);
        await _unitOfWork.SaveChangesAsync();

        return Result<Paiement>.Ok(paiement);
    }

    public async Task<Result<(Paiement Paiement, bool EstPaye)>> VerifierPaiementMobileAsync(int paiementId)
    {
        var paiement = await _paiementRepository.GetByIdAsync(paiementId);
        if (paiement is null)
            return Result<(Paiement, bool)>.Fail("Paiement non trouvé.");

        if (string.IsNullOrEmpty(paiement.ReferenceExterne))
            return Result<(Paiement, bool)>.Fail("Référence externe manquante.");

        var provider = _factory.GetProvider(paiement.ModePaiement);
        var result = await provider.VerifierPaiementAsync(paiement.ReferenceExterne);

        if (!result.Succes)
            return Result<(Paiement, bool)>.Fail(result.Message ?? "Échec de la vérification.");

        if (result.EstPaye && paiement.Statut != StatutPaiement.Recu)
        {
            paiement.Statut = StatutPaiement.Recu;
            paiement.DatePaiement = result.DatePaiement ?? DateTime.UtcNow;
            await _paiementRepository.UpdateAsync(paiement);
        }

        // Mettre à jour la transaction
        var transactions = await _transactionRepository.GetAllAsync();
        var transaction = transactions.FirstOrDefault(t => t.PaiementId == paiementId && t.ProviderReference == paiement.ReferenceExterne);
        if (transaction is not null)
        {
            transaction.Statut = result.EstPaye ? StatutTransaction.Succes : StatutTransaction.EnAttente;
            transaction.RawResponse = result.RawResponse;
            if (result.EstPaye)
                transaction.DateTransaction = result.DatePaiement ?? DateTime.UtcNow;
            await _transactionRepository.UpdateAsync(transaction);
        }

        await _unitOfWork.SaveChangesAsync();

        return Result<(Paiement, bool)>.Ok((paiement, result.EstPaye));
    }
}
