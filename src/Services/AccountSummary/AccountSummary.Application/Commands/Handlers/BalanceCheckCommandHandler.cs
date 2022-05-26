using AccountSummary.Application;
using AccountSummary.Application.Exceptions;
using AccountSummary.Application.IContext;
using AccountSummary.Application.Services;
using AccountSummary.Core;
using AccountSummary.Core.Events;
using BuildingBlocks.CQRS.Commands;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;


namespace Identity.Application.Identity.Commands.Handlers;

public class BalanceCheckCommandHandler :  ICommandHandler<BalanceCheckCommand>
{
    private readonly ILogger<BalanceCheckCommandHandler> _logger;
    private readonly IBalanceDbContext _balanceDbContext;
    private readonly IEventProcessor _eventProcessor;
    private readonly IIdentityContext _identityContext;
    public BalanceCheckCommandHandler(ILogger<BalanceCheckCommandHandler> logger,
                                      IBalanceDbContext balanceDbContext,
                                       IIdentityContext identityContext,
                                      IEventProcessor eventProcessor) 
    {
        _logger = logger;
        _balanceDbContext = balanceDbContext;
        _identityContext = identityContext;
        _eventProcessor = eventProcessor;
        

    }


    public async Task<TResponse> HandleAsync<TResponse>(BalanceCheckCommand command, CancellationToken token)
    {
      
            try
            {
                var accountEntity = await _balanceDbContext.AccountSummary.AsNoTracking()
                    .FirstOrDefaultAsync(e => e.AccountNumber == _identityContext.AccountNumber);

                if (accountEntity == null)
                    throw new InvalidAccountNumberException(_identityContext.AccountNumber);

                var last5Transaction = _balanceDbContext.AccountTransaction.FromSqlRaw($@"SELECT *
                   FROM AccountTransaction
                   where AccountNumber = {_identityContext.AccountNumber}
                  ORDER BY TransactionId desc  LIMIT 5 ").ToList();

                var transactionResult = accountEntity.ToResult();

                if (last5Transaction.Any())
                {
                    foreach (var transaction in last5Transaction)
                    {
                        transactionResult.TransactionHistory.Add(new Transaction()
                        {
                            TransactionType = transaction.TransactionType,
                            Amount = transaction.Amount,
                            ModifiedDate = transaction.ModifiedDate,
                        });
                    }

                }

                var balanceCheckDomainEvent = new BalanceCheckDomainEvent($"{_identityContext.FirstName} {_identityContext.LastName}", transactionResult.AccountNumber, transactionResult.Balance.Amount, transactionResult.Balance.Currency);
                await _eventProcessor.ProcessAsync(balanceCheckDomainEvent);

                return (TResponse)Convert.ChangeType(transactionResult, transactionResult.GetType());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.GetBaseException().Message);
                throw;
            }
        
    }

  
}