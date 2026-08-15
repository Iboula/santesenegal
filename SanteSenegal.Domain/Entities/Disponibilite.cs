namespace SanteSenegal.Domain.Entities;

public class Disponibilite : BaseEntity
{
    public DateTime Date { get; set; }
    public TimeSpan HeureDebut { get; set; }
    public TimeSpan HeureFin { get; set; }
    public bool EstDisponible { get; set; } = true;
    public string? Notes { get; set; }
    public int? MaxRendezVous { get; set; }

    // Relations
    public int StructureId { get; set; }
    public Structure Structure { get; set; } = null!;
    
    public int SousServiceId { get; set; }
    public SousService SousService { get; set; } = null!;

    public ICollection<RendezVous> RendezVous { get; set; } = new List<RendezVous>();
}
