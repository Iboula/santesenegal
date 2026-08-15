using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SanteSenegal.Domain.Entities;

namespace SanteSenegal.Infrastructure.Persistence.Configurations;

public class NotificationConfiguration : IEntityTypeConfiguration<Notification>
{
    public void Configure(EntityTypeBuilder<Notification> builder)
    {
        builder.ToTable("Notifications");

        builder.Property(n => n.Type)
            .HasConversion<string>();

        builder.Property(n => n.Canal)
            .HasConversion<string>();

        builder.Property(n => n.Statut)
            .HasConversion<string>();

        builder.Property(n => n.DestinataireTelephone)
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(n => n.DestinataireEmail)
            .HasMaxLength(255);

        builder.Property(n => n.NomDestinataire)
            .HasMaxLength(200);

        builder.Property(n => n.Message)
            .HasMaxLength(800)
            .IsRequired();

        builder.Property(n => n.MessageWolof)
            .HasMaxLength(800);

        builder.Property(n => n.Langue)
            .HasMaxLength(5)
            .HasDefaultValue("fr");

        builder.Property(n => n.ReferenceType)
            .HasMaxLength(50);

        builder.Property(n => n.ProviderReference)
            .HasMaxLength(255);

        builder.Property(n => n.ProviderUtilise)
            .HasMaxLength(50);

        builder.Property(n => n.Erreur)
            .HasMaxLength(500);

        builder.HasIndex(n => n.Statut);
        builder.HasIndex(n => n.DatePlanifie);
        builder.HasIndex(n => n.DestinataireTelephone);
        builder.HasIndex(n => new { n.ReferenceType, n.ReferenceId });
    }
}
