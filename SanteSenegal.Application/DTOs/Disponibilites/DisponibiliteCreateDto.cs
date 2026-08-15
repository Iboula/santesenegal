namespace SanteSenegal.Application.DTOs.Disponibilites;

public class DisponibiliteCreateDto
{
    public DateTime Date { get; set; }
    public TimeSpan HeureDebut { get; set; }
    public TimeSpan HeureFin { get; set; }
    public int? MaxRendezVous { get; set; }
    public string? Notes { get; set; }
    public int StructureId { get; set; }
    public int SousServiceId { get; set; }
}
