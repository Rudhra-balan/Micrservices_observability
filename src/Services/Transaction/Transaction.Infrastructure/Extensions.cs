using BuildingBlocks;
using BuildingBlocks.CAP;
using BuildingBlocks.Exception;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Transaction.Application.IContext;
using Transaction.Application.Services;
using Transaction.Infrastructure.Context;
using Transaction.Infrastructure.Services;

namespace Transaction.Infrastructure
{
    public static class Extensions
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services)
        {
          
            services.AddErrorHandler<ExceptionToResponseMapper>();
            services.AddTransient<IExceptionToMessageMapper, ExceptionToMessageMapper>();
            services.AddTransient<IMessageBroker, MessageBroker>();
            services.AddTransient<IEventMapper, EventMapper>();
            services.AddTransient<IEventProcessor, EventProcessor>();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddTransient<IIdentityContext, IdentityContext>();
            services.AddCustomCap();
          

            return services;
        }

        public static IApplicationBuilder UseInfrastructure(this IApplicationBuilder app)
        {
            app.UseErrorHandler();

            return app;
        }
    }
}