


using Microsoft.EntityFrameworkCore;
using Transaction.Core.Entities;

namespace Transaction.Application
{
    public interface IAccountTransactionRepository
    {
        DbSet<AccountSummaryEntity> AccountSummary { get; }
        DbSet<AccountTransactionEntity> AccountTransactionEntity { get; }
        Task BeginTransactionAsync(CancellationToken cancellationToken);
        Task CommitTransactionAsync(AccountTransactionEntity accountTransactionEntity, AccountSummaryEntity accountSummaryEntity,CancellationToken cancellationToken);
        Task RollbackTransactionAsync(CancellationToken cancellationToken);
    }
}
