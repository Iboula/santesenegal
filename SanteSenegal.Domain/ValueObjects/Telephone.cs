namespace SanteSenegal.Domain.ValueObjects;

public record Telephone(string Numero)
{
    public override string ToString() => Numero;
}
