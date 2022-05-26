
using Application.Services.Interface;
using BuildingBlocks.Metrics;
using BuildingBlocks.Metrics.Helper;
using DomainCore.Helper.Constant;
using Infrastructure.Extensions;
using Infrastructure.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Prometheus;
using System;
using Web.Application.Services.Interface;
using Web.Infrastructure.ShellExecutor.Service;

namespace Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddTransient<LoggingDelegatingHandler>();
            services.AddTransient<DiagnosticsDelegatingHandler>();

            services.AddHttpClient<IBalance, Balance>(AppConstants.BalanceClient, c =>
            {
                c.BaseAddress = new Uri(configuration["ApiSettings:GatewayAddress"]);
                c.DefaultRequestHeaders.Clear();
                c.DefaultRequestHeaders.TryAddWithoutValidation("Accept", "application/json");
                c.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/json");
                c.DefaultRequestHeaders.TryAddWithoutValidation("Accept-Encoding", "gzip, deflate, br");

            }).UseHttpClientMetrics()
            .AddHttpMessageHandler<DiagnosticsDelegatingHandler>()
            .AddHttpMessageHandler<LoggingDelegatingHandler>()
            .AddPolicyHandler(Resilience.HttpResilienceWraper);


            services.AddHttpClient<ITransaction, Transaction>(AppConstants.TransactionClienct, c =>
            {
                c.BaseAddress = new Uri(configuration["ApiSettings:GatewayAddress"]);
                c.DefaultRequestHeaders.Clear();
                c.DefaultRequestHeaders.TryAddWithoutValidation("Accept", "application/json");
                c.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/json");
                c.DefaultRequestHeaders.TryAddWithoutValidation("Accept-Encoding", "gzip, deflate, br");
              
            }).UseHttpClientMetrics()
              .AddHttpMessageHandler<DiagnosticsDelegatingHandler>()
              .AddHttpMessageHandler<LoggingDelegatingHandler>()
              .AddPolicyHandler(Resilience.HttpResilienceWraper);

            services.AddHttpClient<IAuthentication, Authentication>(AppConstants.AuthenticationClient, c =>
            {
                c.BaseAddress = new Uri(configuration["ApiSettings:GatewayAddress"]);
                c.DefaultRequestHeaders.Clear();
                c.DefaultRequestHeaders.TryAddWithoutValidation("Accept", "application/json");
                c.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/json");
                c.DefaultRequestHeaders.TryAddWithoutValidation("Accept-Encoding", "gzip, deflate, br");

            }).UseHttpClientMetrics()
               .AddHttpMessageHandler<DiagnosticsDelegatingHandler>()
               .AddHttpMessageHandler<LoggingDelegatingHandler>()
               .AddPolicyHandler(Resilience.HttpResilienceWraper);
            
            services.AddSingleton<IBashService, BashService>();
            services.AddTransient<IAuthentication, Authentication>();
            services.AddTransient<ITransaction, Transaction>();
            services.AddTransient<IBalance, Balance>();
            services.AddTransient<IGenericMemoryCache, GenericMemoryCache>();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddTransient<IIdentityContext, IdentityContext>();

            return services;
        }
    }
}