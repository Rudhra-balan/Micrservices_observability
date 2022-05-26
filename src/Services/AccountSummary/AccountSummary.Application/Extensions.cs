
using BuildingBlocks.CQRS.Commands;
using BuildingBlocks.CQRS.Events;
using BuildingBlocks.CQRS.Queries;
using BuildingBlocks.Logging;
using Microsoft.Extensions.DependencyInjection;

namespace AccountSummary.Application;

public static class Extensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        // configure ip rate limiting middleware
        services.AddTransient<CorrelationContextLoggingMiddleware>();
        return services
            .AddCommandHandlers()
            .AddEventHandlers()
            .AddInMemoryCommandDispatcher()
            .AddInMemoryEventDispatcher()
            .AddQueryHandlers()
            .AddInMemoryQueryDispatcher();
    }
}