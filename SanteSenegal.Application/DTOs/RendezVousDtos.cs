using SanteSenegal.Domain.Enums;

namespace SanteSenegal.Application.DTOs;

public class RendezVousDto
{
    public int Id { get; set; }
    public DateTime Date { get; set; }
    public TimeSpan HeureDebut { get; set; }
    public TimeSpan HeureFin { get; set; }
    public string? Notes { get; set; }
    public StatutRendezVous Statut { get; set; }
    public int PatientId { get; set; }
    public PatientBasicDto Patient { get; set; } = null!;
    public int StructureId { get; set; }
    public StructureBasicDto Structure { get; set; } = null!;
    public int SousServiceId { get; set; }
    public SousServiceBasicDto SousService { get; set; } = null!;
}

public class RendezVousCreateDto
{
    public DateTime Date { get; set; }
    public TimeSpan HeureDebut { get; set; }
    public TimeSpan HeureFin { get; set; }
    public string? Notes { get; set; }
    public int PatientId { get; set; }
    public int StructureId { get; set; }
    public int SousServiceId { get; set; }
}

public class RendezVousUpdateDto
{
    public DateTime? Date { get; set; }
    public TimeSpan? HeureDebut { get; set; }
    public TimeSpan? HeureFin { get; set; }
    public string? Notes { get; set; }
    public StatutRendezVous? Statut { get; set; }
}

public class PatientBasicDto
{
    public int Id { get; set; }
    public string Nom { get; set; } = string.Empty;
    public string Prenom { get; set; } = string.Empty;
    public string Telephone { get; set; } = string.Empty;
}
