using System;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Application;
using Application.Common.Exceptions;
using DomainCore.Helper.Constant;
using Infrastructure;
using Web.Common.Helper.StaticFileService;
using Web.Common.Middleware;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Net.Http.Headers;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Logging;
using BuildingBlocks.Metrics;
using Infrastructure.HubService;
using Microsoft.AspNetCore.Http.Connections;
using BuildingBlocks;
using Prometheus;
using Microsoft.ApplicationInsights.AspNetCore.Extensions;

namespace Web
{
    public class Startup
    {

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
          
        }

        public IConfiguration Configuration { get; }
      

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //SelfLog.Enable(msg =>
            //{
            //    Debug.Print(msg);
            //    Debugger.Break();
            //});
            
            services.AddApplicationInsightsTelemetry(Configuration);
            services.AddServiceProfiler(Configuration);
            var sessionTimeout = AppConstants.SessionIdleSystemTimeout;
            services.Configure<ForwardedHeadersOptions>(options =>
            {
                options.ForwardedHeaders =
                    ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
               
            });
            services.AddSingleton(Configuration);
            services.AddResponseCaching();
          
            services.AddMemoryCache();
            services.AddApplication();
            services.AddInfrastructure(Configuration);
            services.AddSession(options => { options.IdleTimeout = TimeSpan.FromMinutes(sessionTimeout); });

            // to support our authorization, disable automatic challenge

            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.ExpireTimeSpan = TimeSpan.FromMinutes(sessionTimeout);
                    options.LoginPath = "/Authentication/Index";
                    options.Events.OnSignedIn = context => {
                        var httpContext = context.HttpContext;
                        httpContext.Items["Properties"] = context.Properties;
                        httpContext.Features.Set(context.Properties);
                        return Task.CompletedTask;
                    };
                  
                });

           
            services.AddScoped<IStaticFileCacheService, StaticFileCacheService>();

          
            services.AddControllersWithViews(config =>
            {
                config.Filters.Add(typeof(CustomExceptionFilter));
                config.Filters.Add(new AutoValidateAntiforgeryTokenAttribute());
                config.Filters.Add(new AuthorizeFilter());
            
            });
            services.AddDataProtection();
            services.AddMetric();
            services.AddSignalR(options => {
                options.ClientTimeoutInterval = TimeSpan.FromMinutes(20);
                options.HandshakeTimeout = TimeSpan.FromMinutes(20);
                options.KeepAliveInterval = TimeSpan.FromMinutes(20);
                options.EnableDetailedErrors = true;
                options.MaximumReceiveMessageSize = null;
                options.StreamBufferCapacity = null;

            });
          //  services.AddSingleton<MetricReporter>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILogger<Startup> logger, IHostApplicationLifetime lifetime)
        {
           
            app.ApplicationServices.Configure();
            app.UseForwardedHeaders();
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }


            app.UseStaticFiles(new StaticFileOptions
            {
                OnPrepareResponse = ctx =>
                {
                    // Cache static files for 30 days
                    ctx.Context.Response.Headers.Append("Cache-Control", "public,max-age=2592000");

                    ctx.Context.Response.Headers.Append("Expires",
                        DateTime.UtcNow.AddDays(30).ToString("R", CultureInfo.InvariantCulture));
                }
            });
            app.UseCookiePolicy();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseSession();
            app.UseCustomExceptionHandler();
            app.UseResponseCaching();
            app.UseAppMetrics();
           // app.UseMiddleware<MetricMiddleware>();
            app.Use(async (context, nextMiddleware) =>
            {
                context.Response.OnStarting(() =>
                {
                    //// if UAT feedback states that must revalidate should be conditional only for non image responses
                    //// then must-revalidate should conditionally be added if the content type is image/png or image/gif
                    string[] contentTypeBuilder =
                    {
                        "text/css",
                        "application/javascript", "image/png", "font/woff2",
                        "application/x-font-ttf", "image/svg+xml", "image/gif"
                    };
                    if (context.Response.ContentType != null && contentTypeBuilder.Contains(context.Response.ContentType.Trim()))
                    {
                        context.Response.GetTypedHeaders().CacheControl =
                            new CacheControlHeaderValue
                            {
                                Public = true,
                                MaxAge = TimeSpan.FromDays(Configuration.GetSection("StaticContentCacheTimeoutDays").Get<int>()),
                                MustRevalidate = true
                            };
                        context.Response.Headers[HeaderNames.Vary] =
                            new[] { "Accept-Encoding" };

                        if (!context.Response.Headers.ContainsKey("Pragma"))
                            context.Response.Headers.Add("Pragma", "Cache-Control");

                        return Task.FromResult(0);
                    }

                    if (!context.Response.Headers.ContainsKey("Pragma"))
                        context.Response.Headers.Add("Pragma", "no-cache");

                    context.Response.GetTypedHeaders().CacheControl =
                        new CacheControlHeaderValue
                        {
                            MustRevalidate = true,
                            NoCache = true,
                            NoStore = true
                        };
                    context.Response.Headers[HeaderNames.Vary] =
                        new[] { "Accept-Encoding" };


                    return Task.FromResult(0);
                });

                await nextMiddleware();
            });
            app.UseEndpoints(endpoints =>
            {
               
                endpoints.MapControllerRoute(
                    "default",
                    "{controller=Authentication}/{action=Index}/{id?}");
                endpoints.MapHub<EHubMessage>("/eHub/notification", mapper =>
                {
                    mapper.Transports = HttpTransportType.WebSockets | HttpTransportType.LongPolling | HttpTransportType.ServerSentEvents;
                });
                endpoints.MapMetrics();
            });
        }
    }
}