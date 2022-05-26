
using BuildingBlocks.Logging;
using DotNetCore.CAP;
using Newtonsoft.Json;
using Transaction.Application.Events;

namespace AccountSummary.Api.Consumer
{
    public class BalanceCheckSubscriber : ICapSubscribe
    {
        private readonly ILogger<BalanceCheckSubscriber> _logger;
        public BalanceCheckSubscriber(ILogger<BalanceCheckSubscriber> logger)
        {
            _logger = logger;
        }

        [CapSubscribe("BalanceCheckEvent")]
        public void CheckReceivedMessage(BalanceCheckEvent accountSummary)
        {
            _logger.LogFormatInfo(JsonConvert.SerializeObject(accountSummary));
        }
    }
}
