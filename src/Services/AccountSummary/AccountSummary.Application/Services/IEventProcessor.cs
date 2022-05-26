using BuildingBlocks.Types;

namespace AccountSummary.Application.Services;

public interface IEventProcessor
{
    Task ProcessAsync(IDomainEvent events);
}