


using Application.Services.Interface;
using DomainCore.Helper.Constant;
using DomainCore.Models;
using DomainCore.Models.Response;
using Infrastructure.Extensions;
using Microsoft.Extensions.Logging;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Infrastructure.Services
{
    public class Transaction : ITransaction
    {
        private readonly IHttpClientFactory _httpClientFactory;

        private readonly IIdentityContext _identityContext;

        private readonly ILogger<Transaction> _logger;
        public Transaction(IHttpClientFactory httpClientFactory, IIdentityContext identityContext, ILogger<Transaction> logger)
        {
            _logger = logger;
            _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));

            _identityContext = identityContext ?? throw new ArgumentNullException(nameof(identityContext));
        }


        public async Task<WebClientResponse> DepositAsync(TransactionInput transactionDetails)
        {
            try
            {
                var client = _httpClientFactory.CreateClient(AppConstants.TransactionClienct);
                client.DefaultRequestHeaders.Add("Authorization", "Bearer " + await _identityContext.FetchTokenAsync());
                return await client.PostAsJson<TransactionResult>("/Transaction/Deposit", transactionDetails);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.GetExceptionMessage());
                throw new Exception(ex.GetExceptionMessage());
            }
        }

        public async Task<WebClientResponse> WithdrawAsync(TransactionInput transactionDetails)
        {
            try
            {
                var client = _httpClientFactory.CreateClient(AppConstants.TransactionClienct);

                client.DefaultRequestHeaders.Add("Authorization", "Bearer " + await _identityContext.FetchTokenAsync());
                return await client.PostAsJson<TransactionResult>("/Transaction/Withdraw", transactionDetails);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.GetExceptionMessage());
                throw new Exception(ex.GetExceptionMessage());
            }

        }
    }
}
