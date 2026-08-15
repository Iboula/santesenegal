namespace SanteSenegal.Domain.Entities;

public class Patient : BaseEntity
{
    public string Nom { get; set; } = string.Empty;
    public string Prenom { get; set; } = string.Empty;
    public DateTime DateNaissance { get; set; }
    public string Telephone { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? Adresse { get; set; }
    public string? NumeroIdentification { get; set; }
    public string? GroupeSanguin { get; set; }
    public string? AntecedentsMedicaux { get; set; }
    public string? ContactUrgence { get; set; }
    public string? TelephoneContactUrgence { get; set; }

    // Relations
    public ICollection<RendezVous> RendezVous { get; set; } = new List<RendezVous>();
    public ICollection<Structure> StructuresFavorites { get; set; } = new List<Structure>();
}
