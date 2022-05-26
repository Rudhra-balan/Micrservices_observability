

using Application.Services.Interface;
using DomainCore.Helper.Constant;
using DomainCore.Models;
using Infrastructure.Extensions;
using Microsoft.Extensions.Logging;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Infrastructure.Services
{
  
    public class Authentication : IAuthentication
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<Authentication> _logger;

        public Authentication(IHttpClientFactory httpClientFactory, ILogger<Authentication> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(httpClientFactory));
            _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
        }

        public async Task<TokenModel> Login(string userName,string password)
        {
            using (_logger.BeginScope("Sending authentication request"))
            {
                var client = _httpClientFactory.CreateClient(AppConstants.AuthenticationClient);
                var response = await client.PostAsJson($"/Identity/Authenticate", new { username = userName, password = password });
                return await response.ReadContentAs<TokenModel>();
            }
        }
    }
}
