using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SanteSenegal.Domain.Entities;

namespace SanteSenegal.Infrastructure.Persistence.Configurations;

public class PatientStructureFavoriteConfiguration : IEntityTypeConfiguration<PatientStructureFavorite>
{
    public void Configure(EntityTypeBuilder<PatientStructureFavorite> builder)
    {
        builder.HasKey(p => p.Id);

        builder.Property(p => p.DateAjout)
            .IsRequired();

        builder.Property(p => p.Notes)
            .HasMaxLength(500);

        builder.HasOne(p => p.Patient)
            .WithMany()
            .HasForeignKey(p => p.PatientId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(p => p.Structure)
            .WithMany()
            .HasForeignKey(p => p.StructureId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
