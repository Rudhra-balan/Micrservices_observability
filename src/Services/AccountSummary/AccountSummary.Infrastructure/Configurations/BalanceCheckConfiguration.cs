using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using AccountSummary.Core.Entities;

namespace AccountSummary.Infrastructure.Configurations
{
    public class BalanceCheckConfiguration : IEntityTypeConfiguration<AccountSummaryEntity>
    {
        public void Configure(EntityTypeBuilder<AccountSummaryEntity> builder)
        {
            builder.ToTable("AccountSummary", "dbo");

            builder.HasKey(r => r.AccountNumber);

            builder.Property(r => r.Currency)
                .IsRequired();

            builder.Property(r => r.Balance)
                .IsRequired();
        }
    }
}
