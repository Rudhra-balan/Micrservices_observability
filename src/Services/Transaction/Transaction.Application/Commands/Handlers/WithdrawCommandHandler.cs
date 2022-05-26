
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

public class WithdrawCommandHandler : SqlServerConnectionFactory, ICommandHandler<WithdrawCommand>
{
    private readonly ILogger<DepositCommandHandler> _logger;
    private readonly IEventProcessor _eventProcessor;
    private readonly IIdentityContext _identityContext;
    
    public WithdrawCommandHandler(ILogger<DepositCommandHandler> logger,
                                    IOptions<DatabaseConnection> databaseOptions,
                                       IIdentityContext identityContext,
                                      IEventProcessor eventProcessor) : base(databaseOptions)
    {
        _logger = logger;
        _identityContext = identityContext;
        _eventProcessor = eventProcessor;


    }


    public async Task<TResponse> HandleAsync<TResponse>(WithdrawCommand command, CancellationToken token)
    {
      
            try
            {
                var accountNumber = _identityContext.AccountNumber;

                _logger.LogInformation(LoggingEvents.Withdrawal, "account transaction:{0}", JsonConvert.SerializeObject(new { AccountNumber = accountNumber, TransactionType = "Withdraw", Amount = command.Amount }));



                AccountSummaryEntity accountSummaryEntity = null;

                var accountEntity = await Connection(async conn =>
                {

                    if (((Currency)Enum.Parse(typeof(Currency), command.Currency, true)) == Currency.Unknown)
                        throw new InvalidCurrencyException(command.Currency.ToString());


                    if (command.Amount <= 0)
                        throw new InvalidAmountException(command.Amount);

                    using var transaction = conn.BeginTransaction();

                    var balance = await conn.QueryFirstOrDefaultAsync<int>("Select Balance from AccountSummary  where AccountNumber = @iAccountNumber;", new
                    {
                        iAccountNumber = accountNumber
                    });

                    var affectedRows = await conn.ExecuteAsync("update AccountSummary set Balance = @iBalance where AccountNumber = @iAccountNumber; ", new
                    {
                        iBalance = balance - command.Amount,
                        iAccountNumber = accountNumber

                    }, transaction);

                    if (affectedRows == 0)
                        throw new InvalidAccountNumberException(accountNumber);

                    affectedRows = await conn.ExecuteAsync(@"INSERT into AccountTransaction (AccountNumber,ModifiedDate,Description,TransactionType,Amount)
                                                   VALUES (@iAccountNumber,@iModifiedDate,@iDescription,@iTransactionType,@iAmount)", new
                    {
                        iAccountNumber = _identityContext.AccountNumber,
                        iModifiedDate = DateTime.Now,
                        iDescription = "Debit",
                        iTransactionType = "Withdraw",
                        iAmount = command.Amount

                    }, transaction);

                    if (affectedRows == 0)
                        throw new InvalidAccountNumberException(accountNumber);

                    var summary = await conn.QueryAsync<AccountSummaryEntity, AccountTransactionEntity, AccountSummaryEntity>(@$"SELECT summary.AccountNumber,summary.Balance,summary.Currency, trans.TransactionType,
                                trans.Amount,trans.ModifiedDate from AccountSummary summary 
                                left JOIN AccountTransaction trans on trans.AccountNumber = summary.AccountNumber
                                where summary.AccountNumber = {accountNumber};", (summary, transaction) =>
                    {
                        if (accountSummaryEntity == null)
                            accountSummaryEntity = summary;
                        accountSummaryEntity.AccountTransactions.Add(transaction);
                        return accountSummaryEntity;

                    }, splitOn: "TransactionType");

                    transaction.Commit();

                    return accountSummaryEntity;
                });

                if (accountSummaryEntity == null)
                    throw new InvalidAccountNumberException(accountNumber);

                var balance = accountSummaryEntity.Balance;
                var amount = command.Amount;


                var WithdrawDomainEvent = new WithdrawDomainEvent($"{_identityContext.FirstName} {_identityContext.LastName}", _identityContext.AccountNumber, accountSummaryEntity.Balance, command.Currency);

                await _eventProcessor.ProcessAsync(WithdrawDomainEvent);

                _logger.LogInformation(LoggingEvents.Withdrawal, "transaction result:{0}", JsonConvert.SerializeObject(accountSummaryEntity.ToResult()));

                return (TResponse)Convert.ChangeType(accountSummaryEntity.ToResult(), accountSummaryEntity.ToResult().GetType());

            }
            catch (Exception ex)
            {
                _logger.LogFormatError(ex);
                throw;
            }
        }
    }
  
