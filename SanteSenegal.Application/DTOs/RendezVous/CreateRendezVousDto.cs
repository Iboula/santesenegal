namespace SanteSenegal.Application.DTOs.RendezVous;

public class CreateRendezVousDto
{
    public DateTime Date { get; set; }
    public TimeSpan Heure { get; set; }
    public string? Motif { get; set; }
    public string? Notes { get; set; }
    public int PatientId { get; set; }
    public int StructureId { get; set; }
    public int ServiceId { get; set; }
    public int SousServiceId { get; set; }
    public int DisponibiliteId { get; set; }
}
