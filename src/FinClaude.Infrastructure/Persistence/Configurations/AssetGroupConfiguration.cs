using FinClaude.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FinClaude.Infrastructure.Persistence.Configurations;

public class AssetGroupConfiguration : IEntityTypeConfiguration<AssetGroup>
{
    public void Configure(EntityTypeBuilder<AssetGroup> builder)
    {
        builder.HasKey(g => g.Id);
        builder.Property(g => g.Name).IsRequired().HasMaxLength(200);
        builder.HasQueryFilter(g => g.DeletedAt == null);
    }
}
