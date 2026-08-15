namespace SanteSenegal.Domain.Entities.NavigSante;

/// <summary>
/// Représente une alerte épidémiologique
/// </summary>
public class AlerteEpidemiologique : BaseEntity
{
    public string Titre { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Maladie { get; set; } = string.Empty; // Paludisme, Choléra, Fièvre jaune...
    public string NiveauAlerte { get; set; } = string.Empty; // Vert, Jaune, Orange, Rouge
    public string? Region { get; set; } // Dakar, Thiès, Saint-Louis...
    public string? Departement { get; set; }
    public string? Commune { get; set; }
    public int? CasConfirmes { get; set; }
    public int? CasSuspects { get; set; }
    public int? Deces { get; set; }
    public DateTime DateDebut { get; set; }
    public DateTime? DateFin { get; set; }
    public bool EstActive { get; set; } = true;
    public string? MesuresPrevention { get; set; }
    public string? Source { get; set; } // Ministère, OMS, Surveillance...
}
