using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SanteSenegal.Domain.Entities;

namespace SanteSenegal.Infrastructure.Persistence.Configurations;

public class TransactionConfiguration : IEntityTypeConfiguration<Transaction>
{
    public void Configure(EntityTypeBuilder<Transaction> builder)
    {
        builder.ToTable("Transactions");

        builder.Property(t => t.Montant);

        builder.Property(t => t.Statut)
            .HasConversion<string>();

        builder.Property(t => t.ModePaiement)
            .HasConversion<string>();

        builder.Property(t => t.Reference)
            .HasMaxLength(255);

        builder.Property(t => t.ProviderReference)
            .HasMaxLength(255);

        builder.Property(t => t.NumeroTelephone)
            .HasMaxLength(20);

        builder.HasOne(t => t.Paiement)
            .WithMany()
            .HasForeignKey(t => t.PaiementId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(t => t.PaiementId);
        builder.HasIndex(t => t.ProviderReference);
        builder.HasIndex(t => t.Statut);
    }
}
