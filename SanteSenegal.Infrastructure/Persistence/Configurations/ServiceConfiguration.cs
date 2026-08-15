using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SanteSenegal.Domain.Entities;

namespace SanteSenegal.Infrastructure.Persistence.Configurations;

public class ServiceConfiguration : IEntityTypeConfiguration<Service>
{
    public void Configure(EntityTypeBuilder<Service> builder)
    {
        builder.HasKey(s => s.Id);

        builder.Property(s => s.Nom)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(s => s.Description)
            .HasMaxLength(500);

        builder.Property(s => s.Type)
            .IsRequired();

        builder.HasOne(s => s.Structure)
            .WithMany(st => st.Services)
            .HasForeignKey(s => s.StructureId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
