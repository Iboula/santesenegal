using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SanteSenegal.Domain.Entities;

namespace SanteSenegal.Infrastructure.Persistence.Configurations;

public class PaiementConfiguration : IEntityTypeConfiguration<Paiement>
{
    public void Configure(EntityTypeBuilder<Paiement> builder)
    {
        builder.ToTable("Paiements");

        builder.Property(p => p.Montant);

        builder.Property(p => p.Statut)
            .HasConversion<string>();

        builder.Property(p => p.ModePaiement)
            .HasConversion<string>();

        builder.Property(p => p.NumeroTelephone)
            .HasMaxLength(20);

        builder.Property(p => p.ReferenceExterne)
            .HasMaxLength(255);

        builder.Property(p => p.UrlPaiement)
            .HasMaxLength(512);

        builder.Property(p => p.CodeConfirmation)
            .HasMaxLength(20);

        builder.HasOne(p => p.RendezVous)
            .WithMany()
            .HasForeignKey(p => p.RendezVousId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(p => p.ReferenceExterne);
        builder.HasIndex(p => p.RendezVousId);
        builder.HasIndex(p => p.Statut);
    }
}
