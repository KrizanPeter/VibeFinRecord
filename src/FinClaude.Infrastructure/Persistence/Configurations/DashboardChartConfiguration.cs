using FinClaude.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FinClaude.Infrastructure.Persistence.Configurations;

public class DashboardChartConfiguration : IEntityTypeConfiguration<DashboardChart>
{
    public void Configure(EntityTypeBuilder<DashboardChart> builder)
    {
        builder.HasKey(c => c.Id);
        builder.Property(c => c.Name).IsRequired().HasMaxLength(200);
        builder.HasQueryFilter(c => c.DeletedAt == null);
    }
}
