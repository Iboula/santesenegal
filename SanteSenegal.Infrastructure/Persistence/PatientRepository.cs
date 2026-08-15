using SanteSenegal.Application.Abstractions;
using SanteSenegal.Domain.Entities;

namespace SanteSenegal.Infrastructure.Persistence;

public class PatientRepository : BaseRepository<Patient>, IPatientRepository
{
    public PatientRepository(SanteDbContext context) : base(context)
    {
    }
}
