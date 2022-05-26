using AccountSummary.Core;
using BuildingBlocks.CQRS.Commands;
using Identity.Application.Identity.Commands;
using Microsoft.AspNetCore.Mvc;

namespace AccountSummary.Api.Controllers
{
    [ApiController]
    public class BalanceCheckController : ControllerBase
    {
        private readonly ICommandDispatcher _commandDispatcher;
        private readonly ILogger<BalanceCheckController> _logger;


        public BalanceCheckController(ICommandDispatcher commandDispatcher, ILogger<BalanceCheckController> logger)
        {
            _logger = logger;
            _commandDispatcher = commandDispatcher;

        }

        [HttpGet(nameof(Balance))]
        public async Task<TransactionResult> Balance()
        {
            using (_logger.BeginScope("API: Getting the balance information"))
                return await _commandDispatcher.SendAsync<BalanceCheckCommand, dynamic>(new BalanceCheckCommand());
        }
    }
}
