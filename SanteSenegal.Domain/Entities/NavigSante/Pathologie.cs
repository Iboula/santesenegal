namespace SanteSenegal.Domain.Entities.NavigSante;

/// <summary>
/// Représente une pathologie identifiée par le moteur de triage
/// </summary>
public class Pathologie : BaseEntity
{
    public string Nom { get; set; } = string.Empty;
    public string? NomWolof { get; set; }
    public string Description { get; set; } = string.Empty;
    public int NiveauGravite { get; set; } = 1; // 1 à 4
    public string? Conseil { get; set; }
    public string? ConseilWolof { get; set; }
    
    /// <summary>
    /// Niveau de soins recommandé : 1=Auto-soin, 2=Poste santé, 3=Centre santé, 4=Hôpital, 5=Urgence
    /// </summary>
    public int NiveauSoinsRecommande { get; set; } = 1;
    
    public ICollection<Symptome> Symptomes { get; set; } = new List<Symptome>();
}
