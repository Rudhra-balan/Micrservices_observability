

using AccountSummary.Core.Entities;
using Microsoft.EntityFrameworkCore;


namespace AccountSummary.Application
{
    public interface IBalanceDbContext
    {
        DbSet<AccountSummaryEntity> AccountSummary { get; }
        Task BeginTransactionAsync(CancellationToken cancellationToken);
        Task CommitTransactionAsync(CancellationToken cancellationToken);
        Task RollbackTransactionAsync(CancellationToken cancellationToken);
        DbSet<AccountTransaction> AccountTransaction { get;  }
    }
}
