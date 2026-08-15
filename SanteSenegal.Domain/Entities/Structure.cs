using SanteSenegal.Domain.Enums;

namespace SanteSenegal.Domain.Entities;

public class Structure : BaseEntity
{
    public string Nom { get; set; } = string.Empty;
    public TypeStructure Type { get; set; }
    public string Adresse { get; set; } = string.Empty;
    public string Telephone { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? ImageUrl { get; set; }
    public bool EstActif { get; set; } = true;
    public string? HorairesOuverture { get; set; }

    // Relations
    public ICollection<Service> Services { get; set; } = new List<Service>();
    public ICollection<Disponibilite> Disponibilites { get; set; } = new List<Disponibilite>();
    public ICollection<RendezVous> RendezVous { get; set; } = new List<RendezVous>();

}
