using BuildingBlocks.Exception;
using BuildingBlocks.Formatter;
using BuildingBlocks.Swagger;
using BuildingBlocks.TokenHandler;
using Microsoft.AspNetCore.Mvc;
using BuildingBlocks.Repository;

using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using HealthChecks.UI.Client;
using Transaction.Application;
using Transaction.Infrastructure;
using DotNetCore.CAP;
using Serilog;
using BuildingBlocks.Logging.Enricher;
using Microsoft.Extensions.DependencyInjection;
using BuildingBlocks.Correlation;
using Ben.Diagnostics;
using BuildingBlocks.Security;
using BuildingBlocks.Security.Headers;
using BuildingBlocks.Metrics;
using Prometheus;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Transaction.Api
{
    public class Startup
    {
        #region Constructor

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        #endregion

        #region Private Variable
        public IConfiguration Configuration { get; }
        #endregion

        #region public member
        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddApplicationInsightsTelemetry(Configuration);
            services.AddServiceProfiler(Configuration);

            #region Setup CORS

            var corsBuilder = new CorsPolicyBuilder();

            corsBuilder.SetIsOriginAllowed(_ => true);
            corsBuilder.AllowAnyHeader();
            corsBuilder.AllowCredentials();
            corsBuilder.AllowAnyMethod();

            services.AddCors(options =>
            {
                options.AddPolicy("_CorsPolicy", corsBuilder.Build());
            });

            #endregion
          
            services.AddControllers(options =>
            {
                options.InputFormatters.Add(new RawRequestBodyFormatter());
                options.Filters.Add(new AuthorizeFilter());
            });

            services.Configure<ApiBehaviorOptions>(options =>
            {
                options.InvalidModelStateResponseFactory = _ => new ValidationResult();

            });

            services.AddJwtTokenAuthentication(Configuration);
            services.AddDatabaseConnection(Configuration);
            services.AddSwaggerDocs();
            services.AddApplication();
            services.AddInfrastructure();
           
            services.Scan(s =>
                            s.FromAssemblies(AppDomain.CurrentDomain.GetAssemblies())
                                .AddClasses(c => c.AssignableTo(typeof(ICapSubscribe)))
                                .AsImplementedInterfaces()
                                .WithScopedLifetime());

            var connectionString = Configuration.GetSection("DBOption:Connection").Value;
            services.AddHealthChecks();


            services.AddHealthChecksUI()
                .AddSqliteStorage(connectionString);
                
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseBlockingDetection();

            if (env.IsDevelopment()) app.UseDeveloperExceptionPage();

            app.UseAntiXssMiddleware();
            app.UseDosAttackMiddleware();
            app.UseSecurityHeadersMiddleware(
                new SecurityHeadersBuilder()
                    .AddDefaultSecurePolicy());

            app.UseCors("CorsPolicy");
            app.UseSwaggerDocs();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseInfrastructure();
            app.UseAppMetrics();
            app.UseCorrelationId(new CorrelationIdOptions
            {
                IncludeInResponse = true
            });

            app.UseSerilogRequestLogging(opts
                => opts.EnrichDiagnosticContext = LogEnricher.EnrichFromRequest);
            app.UseEndpoints(endpoints =>
            {

                endpoints.MapControllers();
                endpoints.MapHealthChecks("/health", new HealthCheckOptions()
                {
                    Predicate = _ => true,
                    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
                });
                endpoints.MapMetrics();
            });
            app.UseHealthChecksUI();
        }

        #endregion
    }
}
