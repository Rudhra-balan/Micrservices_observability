using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;

namespace BuildingBlocks.Swagger;

public static class Extensions
{
    private const string SectionName = "swagger";
   

    public static void AddSwaggerDocs(this IServiceCollection services)
    {
        if (services == null) throw new ArgumentNullException(nameof(services));

        var swaggerSettings = services.GetOptions<SwaggerOptions>(SectionName);
        services.AddSingleton(swaggerSettings);
        services.AddSwaggerGen(s =>
        {
            s.DocumentFilter<HideOcelotControllersFilter>();
            s.SchemaFilter<SwaggerExcludeFilter>();
            s.SwaggerDoc(swaggerSettings.Version, new OpenApiInfo
            {
                Version = swaggerSettings.Version,
                Title = swaggerSettings.Title,
                Description = swaggerSettings.Description
            });
            s.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Description =
                    "JWT Authorization header using the Bearer scheme. \r\n\r\n Enter 'Bearer' [space] and then your token in the text input below.\r\n\r\nExample: \"Bearer <Key>\"",
                Name = "Authorization",
                Scheme = "Bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.ApiKey
            });
            s.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    Array.Empty<string>()
                }
            });
        });
    }





    public static IApplicationBuilder UseSwaggerDocs(this IApplicationBuilder builder)
    {
        var options = builder.ApplicationServices.GetService<SwaggerOptions>();
        

        var routePrefix = string.IsNullOrWhiteSpace(options.RoutePrefix) ? "swagger" : options.RoutePrefix;

        builder.UseSwagger();

        return builder.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint($"/swagger/{options.Version}/swagger.json", options.Title);
                c.RoutePrefix = routePrefix;
                c.DefaultModelsExpandDepth(-1);
            });


       
    }
}