

using BuildingBlocks.CQRS.Events;
using BuildingBlocks.Types;
using AccountSummary.Application.Services;
using AccountSummary.Core.Events;
using AccountSummary.Application.Events;

namespace AccountSummary.Infrastructure.Services;

internal sealed class EventMapper : IEventMapper
{
    public IEvent Map(IDomainEvent @event)
    {
        return @event switch
        {
            BalanceCheckDomainEvent e => new BalanceCheckEvent(e.AccountNumber, true, e.Balance, e.Currency, $"The total amount of money that is currently in your account is {e.Balance}"),
            _ => null
        };
    }
}