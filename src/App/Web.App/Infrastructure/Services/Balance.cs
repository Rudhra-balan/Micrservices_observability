


using Application.Services.Interface;
using DomainCore.Helper.Constant;
using DomainCore.Models;
using DomainCore.Models.Balance;
using Infrastructure.Extensions;
using Microsoft.Extensions.Logging;
using Prometheus;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Infrastructure.Services
{
    public class Balance : IBalance
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IIdentityContext _identityContext;
        private readonly ILogger<Balance> _logger;

       

        private  readonly Gauge _serviceUpGauge = Metrics
    .CreateGauge("balance_service_up_gauge", "counter will increase if not avaiable reset when its avaiable.");
        public Balance(IHttpClientFactory httpClientFactory, IIdentityContext identityContext, ILogger<Balance> logger)
        {
            _logger = logger;
            _httpClientFactory = httpClientFactory  ?? throw new ArgumentNullException(nameof(httpClientFactory));
            _identityContext = identityContext ?? throw new ArgumentNullException(nameof(identityContext));
        }

        public async Task<BalanceModel> BalanceAsync()
        {
            try
            {
                var client = _httpClientFactory.CreateClient(AppConstants.BalanceClient);
                client.DefaultRequestHeaders.Add("Authorization", "Bearer " + await _identityContext.FetchTokenAsync());
                var response = await client.GetAsync($"/AccountSummary/Balance");
                _logger.LogInformation($"balance service status code : {response.StatusCode}");
                if (response.StatusCode == System.Net.HttpStatusCode.ServiceUnavailable 
                    || response.StatusCode == System.Net.HttpStatusCode.BadGateway)
                    _serviceUpGauge.Inc();
                else
                    _serviceUpGauge.Set(0);
                
                return await response.ReadContentAs<BalanceModel>();
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.GetExceptionMessage());
                throw;
            }
           
        }
       
    }
}
