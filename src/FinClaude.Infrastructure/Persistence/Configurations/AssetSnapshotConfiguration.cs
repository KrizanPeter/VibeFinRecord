using FinClaude.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FinClaude.Infrastructure.Persistence.Configurations;

public class AssetSnapshotConfiguration : IEntityTypeConfiguration<AssetSnapshot>
{
    public void Configure(EntityTypeBuilder<AssetSnapshot> builder)
    {
        builder.HasKey(a => a.Id);
        builder.Property(a => a.Value).HasPrecision(18, 4);
    }
}
