using BuildingBlocks.Exception;
using BuildingBlocks.Formatter;
using BuildingBlocks.Security;
using BuildingBlocks.Security.Headers;
using BuildingBlocks.Swagger;
using BuildingBlocks.TokenHandler;
using Microsoft.AspNetCore.Mvc;
using Identity.Application;
using Identity.Infrastructure;
using BuildingBlocks.Repository;
using Serilog;
using BuildingBlocks.Logging.Enricher;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using HealthChecks.UI.Client;
using BuildingBlocks.Correlation;
using Ben.Diagnostics;
using BuildingBlocks.Metrics;
using Prometheus;

namespace Identity.Api
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
            services.AddCors();
            services.AddControllers(options =>
            {
                options.InputFormatters.Add(new RawRequestBodyFormatter());
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

            var connectionString = Configuration.GetSection("DBOption:Connection").Value;
            services.AddHealthChecks();

            services.AddHealthChecksUI().AddSqliteStorage(connectionString);
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

            app.UseSwaggerDocs();
            app.UseRouting();
            app.UseAuthentication();
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
