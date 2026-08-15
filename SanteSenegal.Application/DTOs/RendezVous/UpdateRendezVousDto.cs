namespace SanteSenegal.Application.DTOs.RendezVous;

public class UpdateRendezVousDto
{
    public DateTime? Date { get; set; }
    public TimeSpan? Heure { get; set; }
    public string? Motif { get; set; }
    public string? Notes { get; set; }
}
