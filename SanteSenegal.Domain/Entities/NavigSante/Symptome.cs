namespace SanteSenegal.Domain.Entities.NavigSante;

/// <summary>
/// Représente un symptôme dans le moteur de triage
/// </summary>
public class Symptome : BaseEntity
{
    public string Nom { get; set; } = string.Empty;
    public string? NomWolof { get; set; }
    public string Description { get; set; } = string.Empty;
    public string? DescriptionWolof { get; set; }
    public int Gravite { get; set; } = 1; // 1=léger, 2=modéré, 3=grave, 4=critique
    public string? ZoneCorporelle { get; set; } // tête, ventre, peau, etc.
    public ICollection<Pathologie> PathologiesAssociees { get; set; } = new List<Pathologie>();
}
