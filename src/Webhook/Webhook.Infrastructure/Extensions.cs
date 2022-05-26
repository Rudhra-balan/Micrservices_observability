using Microsoft.Extensions.DependencyInjection;
using Webhook.Application.Services.Interface;
using Webhook.Infrastructure.ShellExecutor.Service;

namespace Webhook.Infrastructure;

public static class Extensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {

        services.AddSingleton<IBashService, BashService>();
        return services;
            
    }
}