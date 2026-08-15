using SanteSenegal.Domain.Enums;

namespace SanteSenegal.Domain.Entities;

public class Service : BaseEntity
{
    public string Nom { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public TypeService Type { get; set; }
    public bool EstActif { get; set; } = true;

    // Relations
    public int StructureId { get; set; }
    public Structure Structure { get; set; } = null!;
    public ICollection<SousService> SousServices { get; set; } = new List<SousService>();
    public ICollection<RendezVous> RendezVous { get; set; } = new List<RendezVous>();
}
