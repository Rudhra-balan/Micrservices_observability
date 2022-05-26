using BuildingBlocks.CQRS.Events;
using BuildingBlocks.Types;

namespace AccountSummary.Application.Services;

public interface IEventMapper
{
    IEvent Map(IDomainEvent @event);
}