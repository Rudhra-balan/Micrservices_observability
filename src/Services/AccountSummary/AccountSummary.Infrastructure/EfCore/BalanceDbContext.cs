

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using AccountSummary.Application;
using AccountSummary.Core.Entities;
using AccountSummary.Infrastructure.Configurations;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AccountSummary.Infrastructure.EfCore
{
    public class BalanceDbContext : DbContext, IBalanceDbContext
    {
        private IDbContextTransaction _currentTransaction;

        public BalanceDbContext(DbContextOptions<BalanceDbContext> options) : base(options)
        {
        }

        public DbSet<AccountSummaryEntity> AccountSummary { get; set; }

        public DbSet<AccountTransaction> AccountTransaction { get; set; }

       
        public async Task BeginTransactionAsync(CancellationToken cancellationToken)
        {
            if (_currentTransaction != null) return;

            _currentTransaction = await Database.BeginTransactionAsync(cancellationToken);

           
        }

        public async Task CommitTransactionAsync(CancellationToken cancellationToken)
        {
            try
            {
                await SaveChangesAsync(cancellationToken);

                await (_currentTransaction?.CommitAsync(cancellationToken) ?? Task.CompletedTask);
            }
            catch
            {
                await RollbackTransactionAsync(cancellationToken);
                throw;
            }
            finally
            {
                if (_currentTransaction != null)
                {
                    _currentTransaction.Dispose();
                    _currentTransaction = null;
                }
            }
        }

        public async Task RollbackTransactionAsync(CancellationToken cancellationToken)
        {
            try
            {
                await _currentTransaction!.RollbackAsync(cancellationToken);
            }
            finally
            {
                if (_currentTransaction != null)
                {
                    _currentTransaction.Dispose();
                    _currentTransaction = null;
                }
            }
        }


        protected override void OnModelCreating(ModelBuilder builder)
        {
            TranasctionConfiguration
             .Configure(builder.Entity<AccountSummaryEntity>());

            TranasctionConfiguration
                .Configure(builder.Entity<AccountTransaction>());

            base.OnModelCreating(builder);
        }
    }
    public class TranasctionConfiguration
    {
        public static void Configure(EntityTypeBuilder<AccountSummaryEntity> entityBuilder)
        {
            entityBuilder.HasKey(t => t.AccountNumber);
            entityBuilder.Property(t => t.Balance).IsConcurrencyToken().IsRequired();
            entityBuilder.Property(t => t.Currency).IsRequired();
        }

        public static void Configure(EntityTypeBuilder<AccountTransaction> entityBuilder)
        {
            entityBuilder.HasKey(t => t.TransactionId);
            entityBuilder.HasOne(u => u.AccountSummary).WithMany(e => e.AccountTransactions).HasForeignKey(u => u.AccountNumber);
            entityBuilder.Property(t => t.ModifiedDate).IsRequired();
            entityBuilder.Property(t => t.Description).IsRequired();
            entityBuilder.Property(t => t.TransactionType).IsRequired();
            entityBuilder.Property(t => t.Amount).IsRequired();
        }
    }
}