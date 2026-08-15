using SanteSenegal.Domain.Enums;

namespace SanteSenegal.Application.DTOs.Services;

public class ServiceBasicDto
{
    public int Id { get; set; }
    public string Nom { get; set; }
    public TypeService Type { get; set; }
}
