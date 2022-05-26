
using BuildingBlocks.CQRS.Commands;
using Microsoft.AspNetCore.Mvc;
using Transaction.Application.Identity.Commands;
using Transaction.Core;

namespace Transaction.Api.Controllers
{
    [ApiController]
    public class WithdrawController : ControllerBase
    {
        private readonly ICommandDispatcher _commandDispatcher;


        public WithdrawController(ICommandDispatcher commandDispatcher)
        {
            _commandDispatcher = commandDispatcher;

        }

        [HttpPost(nameof(Withdraw))]
        public async Task<TransactionResult> Withdraw(WithdrawCommand withdrawCommand)
        {
           return await _commandDispatcher.SendAsync<WithdrawCommand, dynamic>(withdrawCommand);
        }
    }
}
