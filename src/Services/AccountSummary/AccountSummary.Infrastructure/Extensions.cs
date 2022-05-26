using BuildingBlocks;
using BuildingBlocks.CAP;
using BuildingBlocks.Exception;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using AccountSummary.Application;
using AccountSummary.Application.Services;
using AccountSummary.Infrastructure.EfCore;
using AccountSummary.Infrastructure.Services;
using Microsoft.AspNetCore.Http;
using AccountSummary.Application.IContext;
using AccountSummary.Infrastructure.Context;
using BuildingBlocks.Repository;
using BuildingBlocks.Metrics;

namespace AccountSummary.Infrastructure
{
    public static class Extensions
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services)
        {
            var configuration = services.BuildServiceProvider().GetService<IConfiguration>();
            var connectionString = configuration!.GetSection("DBOption:Connection").Value;

            services.AddErrorHandler<ExceptionToResponseMapper>();
            services.AddTransient<IExceptionToMessageMapper, ExceptionToMessageMapper>();

            services.AddDbContext<BalanceDbContext>(options =>
                options.UseSqlite(connectionString));


            services.AddTransient<IBalanceDbContext>(provider => provider.GetService<BalanceDbContext>());

            services.AddTransient<IMessageBroker, MessageBroker>();
            services.AddTransient<IEventMapper, EventMapper>();
            services.AddTransient<IEventProcessor, EventProcessor>();

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddTransient<IIdentityContext, IdentityContext>();
            services.AddCustomCap<BalanceDbContext>();
            services.AddMetric();
            return services;
        }

        public static IApplicationBuilder UseInfrastructure(this IApplicationBuilder app)
        {
            app.UseErrorHandler();

            return app;
        }
    }
}