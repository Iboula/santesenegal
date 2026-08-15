namespace SanteSenegal.Domain.Entities;

public class SousService : BaseEntity
{
    public string Nom { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Prix { get; set; }
    public bool EstDisponible { get; set; } = true;
    public int DureeMinutes { get; set; } = 30;
    public string? Specialite { get; set; }

    // Relations
    public int ServiceId { get; set; }
    public Service Service { get; set; } = null!;
    public ICollection<Disponibilite> Disponibilites { get; set; } = new List<Disponibilite>();
    public ICollection<RendezVous> RendezVous { get; set; } = new List<RendezVous>();
}
