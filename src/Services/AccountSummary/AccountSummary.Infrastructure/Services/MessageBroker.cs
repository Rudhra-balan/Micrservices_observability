using BuildingBlocks.CQRS.Events;
using DotNetCore.CAP;
using Microsoft.Extensions.Logging;
using AccountSummary.Application.Services;

using AccountSummary.Infrastructure.EfCore;

namespace AccountSummary.Infrastructure.Services;

public class MessageBroker : IMessageBroker
{
    private readonly ICapPublisher _capPublisher;
    private readonly ILogger<MessageBroker> _logger;
    private readonly BalanceDbContext _balanceDbContext;


    
    public MessageBroker(ICapPublisher capPublisher, ILogger<MessageBroker> logger, BalanceDbContext balanceDbContext)
    {
        _capPublisher = capPublisher;
        _logger = logger;
        _balanceDbContext = balanceDbContext;
    }

    public async Task PublishAsync(IEvent domainEvent)
    {
        if (domainEvent is null) return;

        await _capPublisher.PublishAsync(domainEvent.GetType().Name, domainEvent);
        _logger.LogInformation($"Published event: {domainEvent}");

    }
}