using System;
using System.Net;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Threading.Tasks;
using BuildingBlocks.Exception;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace BuildingBlocks.TokenHandler
{
    public static class RegisterToken
    {
        
        public static IServiceCollection AddJwtTokenAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            var tokenSettings = configuration.GetSection(nameof(TokenSettings)).Get<TokenSettings>();
            services.Configure<TokenSettings>(configuration.GetSection(nameof(TokenSettings)));
            var secretKey = Convert.FromBase64String($"{tokenSettings.PrivateKey}");

            var privateKey = RSA.Create();
            privateKey.ImportRSAPrivateKey(secretKey, out _);

            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;

            })
            .AddJwtBearer(x =>
            {
                x.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new RsaSecurityKey(privateKey),
                    ValidateIssuer = true,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ValidIssuer = tokenSettings.Issuer,
                    ValidAudience = tokenSettings.Audience, 
                    ClockSkew = TimeSpan.Zero,
                };
                x.RequireHttpsMetadata = false;
                x.SaveToken = false; 
                x.Events = new JwtBearerEvents
                {
                    OnChallenge = OnJwtChallengeEvent,
                    OnForbidden = OnJwtForbiddenEvent,
                    OnTokenValidated = OnJwtTokenValidatedEvent,
                    OnAuthenticationFailed = OnJwtAuthenticationFailedEvent
                };
            });
            return services;
        }

      

        #region Private Member

        private static Task OnJwtChallengeEvent(JwtBearerChallengeContext context)
        {
            var apiResponse = new List<ExceptionResponse>();
            if (context.Response.HasStarted == false)
            {
                var error = new ExceptionResponse
                {
                    StatusCode =HttpStatusCode.Unauthorized,
                    Response = ResponseMessage.UnAuthorized
                };
                apiResponse.Add(error);
                var response = apiResponse.ToJson();
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                context.Response.ContentType = "application/json";
                context.HandleResponse();
                return context.Response.WriteAsync(response);
            }
            return Task.CompletedTask;
        }

        private static Task OnJwtTokenValidatedEvent(TokenValidatedContext context)
        {
            //TODO:Scope authorizationHandler
            return Task.CompletedTask;
        }

        private static Task OnJwtAuthenticationFailedEvent(AuthenticationFailedContext context)
        {
            var apiResponse = new List<ExceptionResponse>();

           
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            context.Response.ContentType = "application/json";

            var error = new ExceptionResponse
            {
                StatusCode = HttpStatusCode.Unauthorized,
                Response = ResponseMessage.UnAuthorized
            };


            if (context.Exception.GetType() ==
                typeof(SecurityTokenExpiredException))
            {
                context.Response.Headers.Add("Token-Expired", "true");
                error.StatusCode = HttpStatusCode.Unauthorized;
                error.Response = ResponseMessage.TokenExpired;
                
            }

            apiResponse.Add(error);
            var response = apiResponse.ToJson();
            context.Response.WriteAsync(response);
            return Task.CompletedTask;
        }

        private static Task OnJwtForbiddenEvent(ForbiddenContext context)
        {
            var apiResponse = new List<ExceptionResponse>();
            var error = new ExceptionResponse
            {
                StatusCode = HttpStatusCode.Unauthorized,
                Response = ResponseMessage.UnAuthorized
            };
            apiResponse.Add(error);
            var response = apiResponse.ToJson();
            context.Response.StatusCode = StatusCodes.Status403Forbidden;
            context.Response.ContentType = "application/json";
            return context.Response.WriteAsync(response);
        }


        #endregion


    }
}
