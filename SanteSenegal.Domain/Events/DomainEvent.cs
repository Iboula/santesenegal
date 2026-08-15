namespace SanteSenegal.Domain.Entities;

public abstract class DomainEvent
{
    public DateTime OccurredOn { get; protected set; } = DateTime.UtcNow;
}
