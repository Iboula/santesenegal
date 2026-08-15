using SanteSenegal.Domain.Enums;

namespace SanteSenegal.Application.DTOs.Services;

public class ServiceDto
{
    public int Id { get; set; }
    public string Nom { get; set; }
    public string Description { get; set; }
    public TypeService Type { get; set; }
    public bool EstActif { get; set; }
    public ICollection<SousServiceBasicDto> SousServices { get; set; } = new List<SousServiceBasicDto>();
}
