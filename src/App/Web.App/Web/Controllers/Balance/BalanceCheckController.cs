

using Application.Common;
using Application.Services.Interface;
using BuildingBlocks;
using DomainCore.Helper.Constant;
using DomainCore.Models.Balance;
using Infrastructure.HubService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Web.Controllers.Export
{

   
    [Route("Balance")]
    public class BalanceCheckController : Controller
    {
        #region Consturctor
      
        public BalanceCheckController(ILogger<BalanceCheckController> logger, 
             IBalance balance)
        {
            _logger = logger;
            _balance = balance;
            Interlocked.Increment(ref DiagnosticsConstant.Requests);

        }

        #endregion

        #region Private Variable
        private readonly ILogger<BalanceCheckController> _logger;
        private readonly IBalance _balance;


        #endregion

        [HttpGet]
        [Route(UrlConstant.BalanceView)]
        public async Task<IActionResult> BalanceView()
        {
         
            using (_logger.BeginScope("Getting the balance inforamtion"))
            {
                try
                {
                    var balance = await _balance.BalanceAsync();
                    return View(UrlConstant.BalanceViewCshtml, balance);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "An Error Occured During the Getting the Account Balance.");
                    throw;
                }
              
            }
        }

        [HttpGet]
        [Route(nameof(BalanceSimulation))]
        public async Task<BalanceModel> BalanceSimulation()
        {

            using (_logger.BeginScope("Getting the balance inforamtion"))
            {
                try
                {
                   return await _balance.BalanceAsync();
                    
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "An Error Occured During the Getting the Account Balance.");
                    throw;
                }

            }
        }
    }
}