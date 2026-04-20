using FinClaude.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FinClaude.Infrastructure.Persistence.Configurations;

public class AssetGroupMembershipConfiguration : IEntityTypeConfiguration<AssetGroupMembership>
{
    public void Configure(EntityTypeBuilder<AssetGroupMembership> builder)
    {
        builder.HasKey(m => new { m.AssetId, m.GroupId });
    }
}
