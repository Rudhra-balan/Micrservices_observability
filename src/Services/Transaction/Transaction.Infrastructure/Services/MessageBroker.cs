using BuildingBlocks.CQRS.Events;
using DotNetCore.CAP;
using Microsoft.Extensions.Logging;
using Transaction.Application.Services;


namespace Transaction.Infrastructure.Services;

public class MessageBroker : IMessageBroker
{
    private readonly ICapPublisher _capPublisher;
    private readonly ILogger<MessageBroker> _logger;



    
    public MessageBroker(ICapPublisher capPublisher, ILogger<MessageBroker> logger)
    {
        _capPublisher = capPublisher;
        _logger = logger;
       
    }

    public async Task PublishAsync(IEvent domainEvent)
    {
        if (domainEvent is null) return;

        await _capPublisher.PublishAsync(domainEvent.GetType().Name, domainEvent);
        _logger.LogInformation($"Published event: {domainEvent}");

    }
}