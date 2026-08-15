using SanteSenegal.Domain.Enums;

namespace SanteSenegal.Application.DTOs.Services;

public class ServiceCreateDto
{
    public string Nom { get; set; }
    public string Description { get; set; }
    public TypeService Type { get; set; }
}
