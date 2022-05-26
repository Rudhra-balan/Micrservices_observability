using BuildingBlocks.CQRS.Events;
using BuildingBlocks.Types;

namespace Transaction.Application.Services;

public interface IEventMapper
{
    IEvent Map(IDomainEvent @event);
}