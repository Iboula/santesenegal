using SanteSenegal.Domain.Enums;

namespace SanteSenegal.Domain.Entities;

public class RendezVous : BaseEntity
{
    public DateTime Date { get; set; }
    public TimeSpan Heure { get; set; }
    public StatutRendezVous Statut { get; set; } = StatutRendezVous.EnAttente;
    public string? Motif { get; set; }
    public string? Notes { get; set; }
    public string? NumeroReference { get; set; }

    // ── Notifications ──
    public bool RappelSMSEnvoye { get; set; }
    public DateTime? RappelSMSDateEnvoye { get; set; }
    public string? RappelSMSContenu { get; set; }
    public bool ConfirmationSMSEnvoye { get; set; }
    public string LanguePreference { get; set; } = "fr"; // fr, wo

    // Relations
    public int PatientId { get; set; }
    public Patient Patient { get; set; } = null!;

    public int StructureId { get; set; }
    public Structure Structure { get; set; } = null!;

    public int ServiceId { get; set; }
    public Service Service { get; set; } = null!;

    public int SousServiceId { get; set; }
    public SousService SousService { get; set; } = null!;

    public int DisponibiliteId { get; set; }
    public Disponibilite Disponibilite { get; set; } = null!;

    public Paiement? Paiement { get; set; }
}
