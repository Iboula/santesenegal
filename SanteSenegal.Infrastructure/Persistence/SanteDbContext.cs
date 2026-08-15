using Microsoft.EntityFrameworkCore;
using SanteSenegal.Domain.Entities;
using SanteSenegal.Domain.Entities.NavigSante;

namespace SanteSenegal.Infrastructure.Persistence;

public class SanteDbContext : DbContext
{
    public SanteDbContext(DbContextOptions<SanteDbContext> options) : base(options) { }

    // Core
    public DbSet<Patient> Patients => Set<Patient>();
    public DbSet<RendezVous> RendezVous => Set<RendezVous>();
    public DbSet<Paiement> Paiements => Set<Paiement>();
    public DbSet<Disponibilite> Disponibilites => Set<Disponibilite>();
    public DbSet<Service> Services => Set<Service>();
    public DbSet<SousService> SousServices => Set<SousService>();
    public DbSet<Structure> Structures => Set<Structure>();
    public DbSet<Transaction> Transactions => Set<Transaction>();

    // Auth
    public DbSet<User> Users => Set<User>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();

    // Notifications
    public DbSet<Notification> Notifications => Set<Notification>();

    // NavigSante
    public DbSet<Symptome> Symptomes => Set<Symptome>();
    public DbSet<Pathologie> Pathologies => Set<Pathologie>();
    public DbSet<Orientation> Orientations => Set<Orientation>();
    public DbSet<RessourceSanitaire> RessourcesSanitaires => Set<RessourceSanitaire>();
    public DbSet<AlerteEpidemiologique> AlertesEpidemiologiques => Set<AlerteEpidemiologique>();
    public DbSet<ArticleInfoSante> ArticlesInfoSante => Set<ArticleInfoSante>();
    
    // Accidents de la route
    public DbSet<AccidentRoute> AccidentsRoute => Set<AccidentRoute>();
    public DbSet<AccidentVictime> AccidentVictimes => Set<AccidentVictime>();
    public DbSet<AlerteAccidentNotification> AlerteAccidentNotifications => Set<AlerteAccidentNotification>();

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var entries = ChangeTracker.Entries<BaseEntity>();

        foreach (var entry in entries)
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.CreatedAt = DateTime.UtcNow;
                    entry.Entity.UpdatedAt = DateTime.UtcNow;
                    break;

                case EntityState.Modified:
                    entry.Entity.UpdatedAt = DateTime.UtcNow;
                    entry.Property(nameof(BaseEntity.CreatedAt)).IsModified = false;
                    break;
            }
        }

        return await base.SaveChangesAsync(cancellationToken);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(SanteDbContext).Assembly);
    }
}
