using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SanteSenegal.Domain.Entities;

namespace SanteSenegal.Infrastructure.Persistence.Configurations;

public class DisponibiliteConfiguration : IEntityTypeConfiguration<Disponibilite>
{
    public void Configure(EntityTypeBuilder<Disponibilite> builder)
    {
        builder.HasKey(d => d.Id);

        builder.Property(d => d.Date)
            .IsRequired();

        builder.Property(d => d.HeureDebut)
            .IsRequired();

        builder.Property(d => d.HeureFin)
            .IsRequired();

        builder.Property(d => d.Notes)
            .HasMaxLength(500);

        builder.HasOne(d => d.Structure)
            .WithMany(s => s.Disponibilites)
            .HasForeignKey(d => d.StructureId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(d => d.SousService)
            .WithMany(s => s.Disponibilites)
            .HasForeignKey(d => d.SousServiceId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
