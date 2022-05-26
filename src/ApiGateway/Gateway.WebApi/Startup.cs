using BuildingBlocks.TokenHandler;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MMLib.Ocelot.Provider.AppConfiguration;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using Ocelot.Provider.Polly;
using Ocelot.Cache.CacheManager;
using BuildingBlocks.Correlation;
using BuildingBlocks.Metrics;

namespace Gateway.WebApi
{
    public class Startup
    {
        public Startup(IConfiguration config)
        {
            Configuration = config;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddApplicationInsightsTelemetry(Configuration);
            services.AddServiceProfiler(Configuration);
            services.AddMvc(options => options.EnableEndpointRouting = false);
            services.AddOcelot().AddAppConfiguration().AddCacheManager(option => option.WithDictionaryHandle())
                .AddPolly(); ;
            services.AddSwaggerForOcelot(Configuration);
            services.AddJwtTokenAuthentication(Configuration);

           
            services.AddMetric();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseDeveloperExceptionPage();
            app.UseMiddleware<ResponseTimeMiddleware>();
            app.UsePathBase("/gateway");
            app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseStaticFiles();
            app.UseMvc();
            app.UseAppMetrics();


            app.UseCorrelationId(new CorrelationIdOptions
            {
                IncludeInResponse = true
            });
            app.UseSwaggerForOcelotUI(
                    opt =>
                    {
                        opt.DownstreamSwaggerEndPointBasePath = "/gateway/swagger/docs";
                        opt.PathToSwaggerGenerator = "/swagger/docs";
                        opt.DefaultModelsExpandDepth(-1);
                    })
                .UseOcelot()
                .Wait();
           
        }
    }
}
