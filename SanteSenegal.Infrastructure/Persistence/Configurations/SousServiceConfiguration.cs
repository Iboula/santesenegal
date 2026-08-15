using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SanteSenegal.Domain.Entities;

namespace SanteSenegal.Infrastructure.Persistence.Configurations;

public class SousServiceConfiguration : IEntityTypeConfiguration<SousService>
{
    public void Configure(EntityTypeBuilder<SousService> builder)
    {
        builder.HasKey(s => s.Id);

        builder.Property(s => s.Nom)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(s => s.Description)
            .HasMaxLength(500);

        builder.Property(s => s.Prix)
            .IsRequired();

        builder.Property(s => s.DureeMinutes)
            .IsRequired();

        builder.Property(s => s.Specialite)
            .HasMaxLength(100);

        builder.HasOne(s => s.Service)
            .WithMany(ser => ser.SousServices)
            .HasForeignKey(s => s.ServiceId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
