using SanteSenegal.Domain.Entities;

namespace SanteSenegal.Domain.Events;

public class RendezVousAnnuleEvent : DomainEvent
{
    public int RendezVousId { get; }
    public DateTime Date { get; }

    public RendezVousAnnuleEvent(int rendezVousId, DateTime date)
    {
        RendezVousId = rendezVousId;
        Date = date;
    }
}
