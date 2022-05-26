
using BuildingBlocks.CQRS.Events;

namespace Transaction.Application.Services;

public interface IMessageBroker
{
    Task PublishAsync(IEvent events);
}