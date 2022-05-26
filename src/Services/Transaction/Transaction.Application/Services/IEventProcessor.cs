using BuildingBlocks.Types;

namespace Transaction.Application.Services;

public interface IEventProcessor
{
    Task ProcessAsync(IDomainEvent events);
}