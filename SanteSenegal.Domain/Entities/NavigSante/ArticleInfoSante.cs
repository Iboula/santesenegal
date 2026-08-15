namespace SanteSenegal.Domain.Entities.NavigSante;

/// <summary>
/// Représente un article d'information santé publique
/// </summary>
public class ArticleInfoSante : BaseEntity
{
    public string Titre { get; set; } = string.Empty;
    public string Contenu { get; set; } = string.Empty;
    public string? Resume { get; set; }
    public string Categorie { get; set; } = string.Empty; // Prévention, Vaccination, Nutrition, Hygiène...
    public string? ImageUrl { get; set; }
    public bool EstPublie { get; set; } = false;
    public DateTime? DatePublication { get; set; }
    public int NombreVues { get; set; } = 0;
    public string? Tags { get; set; } // séparés par virgule
}
