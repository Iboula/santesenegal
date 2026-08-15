namespace SanteSenegal.Domain.Entities.NavigSante;

/// <summary>
/// Représente les ressources disponibles dans une structure de santé (lits, sang, médicaments...)
/// </summary>
public class RessourceSanitaire : BaseEntity
{
    public int StructureId { get; set; }
    public Structure Structure { get; set; } = null!;
    
    // Lits
    public int LitsTotal { get; set; }
    public int LitsDisponibles { get; set; }
    public int LitsReanimationTotal { get; set; }
    public int LitsReanimationDisponibles { get; set; }
    public int LitsMaterniteTotal { get; set; }
    public int LitsMaterniteDisponibles { get; set; }
    
    // Sang
    public int StockSangAPlus { get; set; }
    public int StockSangAMoins { get; set; }
    public int StockSangBPlus { get; set; }
    public int StockSangBMoins { get; set; }
    public int StockSangABPlus { get; set; }
    public int StockSangABMoins { get; set; }
    public int StockSangOPlus { get; set; }
    public int StockSangOMoins { get; set; }
    
    // Équipements
    public bool ScannerFonctionnel { get; set; }
    public bool RadioFonctionnel { get; set; }
    public bool LaboFonctionnel { get; set; }
    public bool BlocOperatoireFonctionnel { get; set; }
    
    // Temps d'attente (minutes)
    public int? TempsAttenteUrgencesMinutes { get; set; }
    
    // Mise à jour
    public DateTime DerniereMiseAJour { get; set; } = DateTime.UtcNow;
}
