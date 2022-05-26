
using AccountSummary.Core.Events;

using BuildingBlocks;
using BuildingBlocks.CQRS.Commands;
using BuildingBlocks.Logging;
using BuildingBlocks.Metrics;
using BuildingBlocks.Repository;
using Dapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Transaction.Application.Exceptions;
using Transaction.Application.IContext;
using Transaction.Application.Services;
using Transaction.Core;
using Transaction.Core.Entities;

namespace Transaction.Application.Identity.Commands.Handlers;

public class DepositCommandHandler : SqlServerConnectionFactory, ICommandHandler<DepositCommand>
{
    private readonly ILogger<DepositCommandHandler> _logger;

    private readonly IEventProcessor _eventProcessor;
    private readonly IIdentityContext _identityContext;
   
    public DepositCommandHandler(ILogger<DepositCommandHandler> logger,
                                        IOptions<DatabaseConnection> databaseOptions,
                                       IIdentityContext identityContext, 
                                      IEventProcessor eventProcessor) : base(databaseOptions)
    {
        _logger = logger;
     
        _identityContext = identityContext;
        _eventProcessor = eventProcessor;
        
    }

   
    public async Task<TResponse> HandleAsync<TResponse>(DepositCommand command, CancellationToken token)
    {
      
            try
            {
                var accountNumber = _identityContext.AccountNumber;
                _logger.LogInformation(LoggingEvents.Deposit, "account transaction:{0}", JsonConvert.SerializeObject(new { AccountNumber = accountNumber, TransactionType = "Deposit", Amount = command.Amount }));

                AccountSummaryEntity summaryEntity = null;

                var accountEntity = await Connection(async conn =>
                {

                    using var transaction = conn.BeginTransaction();

                    var balance = await conn.QueryFirstOrDefaultAsync<int>("Select Balance from AccountSummary  where AccountNumber = @iAccountNumber;", new
                    {
                        iAccountNumber = accountNumber
                    });

                    var affectedRows = await conn.ExecuteAsync("update AccountSummary set Balance = @iBalance where AccountNumber = @iAccountNumber; ", new
                    {
                        iBalance = balance + command.Amount,
                        iAccountNumber = accountNumber

                    }, transaction);

                    if (affectedRows == 0)
                        throw new InvalidAccountNumberException(accountNumber);

                    affectedRows = await conn.ExecuteAsync(@"INSERT into AccountTransaction (AccountNumber,ModifiedDate,Description,TransactionType,Amount)
                                                   VALUES (@iAccountNumber,@iModifiedDate,@iDescription,@iTransactionType,@iAmount)", new
                    {
                        iAccountNumber = _identityContext.AccountNumber,
                        iModifiedDate = DateTime.Now,
                        iDescription = "Credit",
                        iTransactionType = "Deposit",
                        iAmount = command.Amount

                    }, transaction);

                    if (affectedRows == 0)
                        throw new InvalidAccountNumberException(accountNumber);

                    var summary = await conn.QueryAsync<AccountSummaryEntity, AccountTransactionEntity, AccountSummaryEntity>(@$"SELECT summary.AccountNumber,summary.Balance,summary.Currency, trans.TransactionType,
                                trans.Amount,trans.ModifiedDate from AccountSummary summary 
                                left JOIN AccountTransaction trans on trans.AccountNumber = summary.AccountNumber
                                where summary.AccountNumber = {accountNumber};", (summary, transaction) =>
                    {
                        if (summaryEntity == null)
                            summaryEntity = summary;
                        summaryEntity.AccountTransactions.Add(transaction);
                        return summaryEntity;

                    },
                      splitOn: "TransactionType");

                    transaction.Commit();

                    return summaryEntity;
                });

                if (summaryEntity == null)
                    throw new InvalidAccountNumberException(accountNumber);


                if (((Currency)Enum.Parse(typeof(Currency), command.Currency, true)) == Currency.Unknown)
                    throw new InvalidCurrencyException(command.Currency.ToString());


                if (command.Amount <= 0)
                    throw new InvalidAmountException(command.Amount);

                var depositDomainEvent = new DepositDomainEvent($"{_identityContext.FirstName} {_identityContext.LastName}", _identityContext.AccountNumber, summaryEntity.Balance, command.Currency);

                await _eventProcessor.ProcessAsync(depositDomainEvent);

                _logger.LogInformation(LoggingEvents.Deposit, "transaction result:{0}", JsonConvert.SerializeObject(summaryEntity.ToResult()));

                return (TResponse)Convert.ChangeType(summaryEntity.ToResult(), summaryEntity.ToResult().GetType());
            }
            catch (Exception ex)
            {
                _logger.LogFormatError(ex);
                throw;
            }
        }
    }

  
