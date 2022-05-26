
using BuildingBlocks.CQRS.Events;

namespace AccountSummary.Application.Services;

public interface IMessageBroker
{
    Task PublishAsync(IEvent events);
}