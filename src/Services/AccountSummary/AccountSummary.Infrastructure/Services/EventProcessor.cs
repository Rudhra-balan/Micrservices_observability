
using BuildingBlocks.CQRS.Events;
using BuildingBlocks.Types;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using AccountSummary.Application.Services;

namespace AccountSummary.Infrastructure.Services;

internal sealed class EventProcessor : IEventProcessor
{
    private readonly IEventMapper _eventMapper;
    private readonly ILogger<IEventProcessor> _logger;
    private readonly IMessageBroker _messageBroker;
    private readonly IServiceScopeFactory _serviceScopeFactory;

    public EventProcessor(IServiceScopeFactory serviceScopeFactory, IEventMapper eventMapper,
        IMessageBroker messageBroker, ILogger<IEventProcessor> logger)
    {
        _serviceScopeFactory = serviceScopeFactory;
        _eventMapper = eventMapper;
        _messageBroker = messageBroker;
        _logger = logger;
    }

    public async Task ProcessAsync(IDomainEvent events)
    {
        if (events is null) return;

        _logger.LogTrace("Processing domain events...");
        var integrationEvent = await HandleDomainEvents(events);
        if (integrationEvent is null) return;

        _logger.LogTrace("Processing integration events...");
        await _messageBroker.PublishAsync(integrationEvent);
    }


    private async Task<IEvent> HandleDomainEvents(IDomainEvent domainEvent)
    {
        using var scope = _serviceScopeFactory.CreateScope();
        var eventType = domainEvent.GetType();
        _logger.LogTrace($"Handling domain event: {eventType.Name}");
        var handlerType = typeof(IDomainEventHandler<>).MakeGenericType(eventType);
        dynamic handlers = scope.ServiceProvider.GetServices(handlerType);
        foreach (var handler in handlers) await handler.HandleAsync((dynamic)domainEvent);
        return _eventMapper.Map(domainEvent);
    }
}