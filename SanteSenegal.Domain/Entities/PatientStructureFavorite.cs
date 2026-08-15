namespace SanteSenegal.Domain.Entities;

public class PatientStructureFavorite : BaseEntity
{
    public int PatientId { get; set; }
    public Patient Patient { get; set; } = null!;

    public int StructureId { get; set; }
    public Structure Structure { get; set; } = null!;

    public DateTime DateAjout { get; set; } = DateTime.UtcNow;
    public string? Notes { get; set; }
}
