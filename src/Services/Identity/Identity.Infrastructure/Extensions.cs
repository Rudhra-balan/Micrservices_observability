
using BuildingBlocks;
using BuildingBlocks.CAP;
using BuildingBlocks.Exception;
using BuildingBlocks.Logging;
using BuildingBlocks.Security;
using BuildingBlocks.Security.Headers;
using Identity.Application.Services;
using Identity.Infrastructure.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Identity.Infrastructure
{
    public static class Extensions
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services)
        {
           
            services.AddErrorHandler<ExceptionToResponseMapper>();
            services.AddTransient<IExceptionToMessageMapper, ExceptionToMessageMapper>();
            services.AddCustomCap();
            services.AddTransient<ITokenService, TokenService>();
            services.AddTransient<CorrelationContextLoggingMiddleware>();


            return services;
        }

        public static IApplicationBuilder UseInfrastructure(this IApplicationBuilder app)
        {
        
            app.UseAntiXssMiddleware();
            app.UseSecurityHeadersMiddleware(
                new SecurityHeadersBuilder()
                    .AddDefaultSecurePolicy());

            app.UseCors(x => x
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader()
                .SetPreflightMaxAge(TimeSpan.FromMinutes(20)));
            app.UserCorrelationContextLogging();
            app.UseErrorHandler();
          
            return app;
        }
    }
}
