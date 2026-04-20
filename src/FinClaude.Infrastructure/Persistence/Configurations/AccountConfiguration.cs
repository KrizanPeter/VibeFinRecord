using FinClaude.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FinClaude.Infrastructure.Persistence.Configurations;

public class AccountConfiguration : IEntityTypeConfiguration<Account>
{
    public void Configure(EntityTypeBuilder<Account> builder)
    {
        builder.HasKey(a => a.Id);
        builder.Property(a => a.IdentityUserId).IsRequired();
        builder.Property(a => a.Currency).HasMaxLength(3);
        builder.HasIndex(a => a.IdentityUserId).IsUnique();
    }
}
