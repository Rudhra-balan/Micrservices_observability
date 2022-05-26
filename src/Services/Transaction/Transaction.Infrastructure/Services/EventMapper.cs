

using AccountSummary.Core.Events;
using BuildingBlocks.CQRS.Events;
using BuildingBlocks.Types;
using Transaction.Application.Events;
using Transaction.Application.Services;

namespace Transaction.Infrastructure.Services;

internal sealed class EventMapper : IEventMapper
{
    public IEvent Map(IDomainEvent @event)
    {
        return @event switch
        {
            DepositDomainEvent e => new DepositEvent(e.AccountNumber, true, e.Balance, e.Currency, $"Credit Sucess"),

            WithdrawDomainEvent e => new WithdrawEvent(e.AccountNumber, true, e.Balance, e.Currency, $"Debit Success "),

            _ => null
        };
    }
}