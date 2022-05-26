using BuildingBlocks.Exception;
using BuildingBlocks.Swagger;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Serilog;
using BuildingBlocks.Logging.Enricher;
using Webhook.Application;
using Webhook.Infrastructure;
using Webhook.Helper.Attribute;
using Microsoft.AspNetCore.Http.Connections;
using Webhook.DomainCore.HubService;

namespace Webhook
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

            services.AddControllers(option =>
            {
                option.AllowEmptyInputInBodyModelBinding = true;
                option.Filters.Add(new RequireOptionalBodyAttribute());
            
            });


            services.Configure<ApiBehaviorOptions>(options =>
            {
                options.InvalidModelStateResponseFactory = _ => new ValidationResult();

            });


            services.AddSwaggerDocs();
            services.AddApplication();
            services.AddInfrastructure();
            services.AddSignalR(options => {
                options.ClientTimeoutInterval = TimeSpan.FromMinutes(20);
                options.HandshakeTimeout = TimeSpan.FromMinutes(20);
                options.KeepAliveInterval = TimeSpan.FromMinutes(20);
                options.EnableDetailedErrors = true;
                options.MaximumReceiveMessageSize = null;
                options.StreamBufferCapacity = null;

            });

        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
           

            if (env.IsDevelopment()) app.UseDeveloperExceptionPage();
            app.UseCors("CorsPolicy");
            app.UseSwaggerDocs();
            app.UseRouting();
            app.UseDefaultFiles();
            app.UseStaticFiles();
            //app.UseInfrastructure();

            app.UseSerilogRequestLogging(opts
                => opts.EnrichDiagnosticContext = LogEnricher.EnrichFromRequest);
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHub<EHubMessage>("/eHub/notification", mapper =>
                {
                    mapper.Transports = HttpTransportType.WebSockets | HttpTransportType.LongPolling | HttpTransportType.ServerSentEvents;
                });

            });
            
        }

        #endregion
    }
}
