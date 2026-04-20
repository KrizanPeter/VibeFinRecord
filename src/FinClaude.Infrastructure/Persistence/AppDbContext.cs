using FinClaude.Domain.Common;
using FinClaude.Domain.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace FinClaude.Infrastructure.Persistence;

public class AppDbContext : IdentityDbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Account> Accounts => Set<Account>();
    public DbSet<Asset> Assets => Set<Asset>();
    public DbSet<AssetGroup> AssetGroups => Set<AssetGroup>();
    public DbSet<AssetGroupMembership> AssetGroupMemberships => Set<AssetGroupMembership>();
    public DbSet<Snapshot> Snapshots => Set<Snapshot>();
    public DbSet<AssetSnapshot> AssetSnapshots => Set<AssetSnapshot>();
    public DbSet<Goal> Goals => Set<Goal>();
    public DbSet<DashboardChart> DashboardCharts => Set<DashboardChart>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;
        foreach (var entry in ChangeTracker.Entries<BaseEntity>())
        {
            if (entry.State == EntityState.Added)
            {
                entry.Entity.CreatedAt = now;
                entry.Entity.UpdatedAt = now;
            }
            else if (entry.State == EntityState.Modified)
            {
                entry.Entity.UpdatedAt = now;
            }
        }
        return base.SaveChangesAsync(cancellationToken);
    }
}
