namespace SanteSenegal.Application.DTOs.Disponibilites;

public class DisponibiliteUpdateDto
{
    public DateTime? Date { get; set; }
    public TimeSpan? HeureDebut { get; set; }
    public TimeSpan? HeureFin { get; set; }
    public bool? EstDisponible { get; set; }
    public int? MaxRendezVous { get; set; }
    public string? Notes { get; set; }
}
