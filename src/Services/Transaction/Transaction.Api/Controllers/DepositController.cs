
using BuildingBlocks.CQRS.Commands;
using Microsoft.AspNetCore.Mvc;
using Transaction.Application.Identity.Commands;
using Transaction.Core;

namespace Transaction.Api.Controllers
{
    [ApiController]
    public class DepositController : ControllerBase
    {
        private readonly ICommandDispatcher _commandDispatcher;


        public DepositController(ICommandDispatcher commandDispatcher)
        {
            _commandDispatcher = commandDispatcher;

        }

        [HttpPost(nameof(Deposit))]
        public async Task<TransactionResult> Deposit(DepositCommand depositCommand)
        {
           var result = await _commandDispatcher.SendAsync<DepositCommand, dynamic>(depositCommand);
            return result;
        }
    }
}
