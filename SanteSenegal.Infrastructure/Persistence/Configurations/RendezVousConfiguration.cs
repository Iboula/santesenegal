using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SanteSenegal.Domain.Entities;

namespace SanteSenegal.Infrastructure.Persistence.Configurations;

public class RendezVousConfiguration : IEntityTypeConfiguration<RendezVous>
{
    public void Configure(EntityTypeBuilder<RendezVous> builder)
    {
        builder.HasKey(r => r.Id);

        builder.Property(r => r.Date)
            .IsRequired();

        builder.Property(r => r.Heure)
            .IsRequired();

        builder.Property(r => r.Statut)
            .IsRequired();

        builder.Property(r => r.NumeroReference)
            .HasMaxLength(50);

        builder.Property(r => r.Motif)
            .HasMaxLength(500);

        builder.Property(r => r.Notes)
            .HasMaxLength(1000);

        builder.Property(r => r.LanguePreference)
            .HasMaxLength(5)
            .HasDefaultValue("fr");

        builder.HasOne(r => r.Patient)
            .WithMany(p => p.RendezVous)
            .HasForeignKey(r => r.PatientId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(r => r.Structure)
            .WithMany(s => s.RendezVous)
            .HasForeignKey(r => r.StructureId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(r => r.Service)
            .WithMany(s => s.RendezVous)
            .HasForeignKey(r => r.ServiceId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(r => r.SousService)
            .WithMany(s => s.RendezVous)
            .HasForeignKey(r => r.SousServiceId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(r => r.Disponibilite)
            .WithMany(d => d.RendezVous)
            .HasForeignKey(r => r.DisponibiliteId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
