
using Application.Services.Interface;
using BuildingBlocks;
using DomainCore.Helper.Constant;
using DomainCore.Models;
using DomainCore.Models.Response;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;

namespace Web.Controllers.TimeSheet
{


    [Route("Transaction")]
    public class TransactionController : Controller
    {
        #region Consturctor

        public TransactionController(ILogger<TransactionController> logger, ITransaction transaction)
        {
           _logger = logger;
            _transaction = transaction;
            Interlocked.Increment(ref DiagnosticsConstant.Requests);
        }

        #endregion

        #region Private Variable
        private readonly ILogger<TransactionController> _logger;
        private readonly ITransaction _transaction;


        #endregion

        #region Public Member

        [HttpGet]
        [Route(nameof(Index))]
        public IActionResult Index()
        {
            return PartialView(UrlConstant.TransactionViewCshtml);
        }

        [HttpPost]
        [Route(UrlConstant.Deposit)]
        public async Task<WebClientResponse> Deposit([FromBody]TransactionInput transactionInput)
        {
            using (_logger.BeginScope("Deposit {@transactionInput}", transactionInput))
            {
                var response = await _transaction.DepositAsync(transactionInput);
                return response;
            }
            
        }

        [HttpPost]
        [Route(UrlConstant.Withdraw)]
        public async Task<WebClientResponse> Withdraw([FromBody] TransactionInput transactionInput)
        {
            using (_logger.BeginScope("Withdraw {@transactionInput}", transactionInput))
            {
                return await _transaction.WithdrawAsync(transactionInput);
            }
           
        }


        #endregion


    }
}