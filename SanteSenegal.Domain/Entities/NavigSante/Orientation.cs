namespace SanteSenegal.Domain.Entities.NavigSante;

/// <summary>
/// Représente le résultat d'un triage symptomatique
/// </summary>
public class Orientation : BaseEntity
{
    public int? UtilisateurId { get; set; } // null = anonyme
    public string? SymptomesDecrits { get; set; }
    public int GraviteCalculee { get; set; } // 1 à 4
    public string NiveauSoins { get; set; } = string.Empty; // Auto-soin, Consultation, Urgence...
    public string? Recommandation { get; set; }
    public string? RecommandationWolof { get; set; }
    public DateTime DateOrientation { get; set; } = DateTime.UtcNow;
    public bool EstUrgence => GraviteCalculee >= 3;
    public ICollection<Pathologie> PathologiesIdentifiees { get; set; } = new List<Pathologie>();
}
