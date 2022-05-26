using AccountSummary.Application.Events;
using BuildingBlocks.Logging;
using DotNetCore.CAP;
using Newtonsoft.Json;

namespace AccountSummary.Api.Consumer
{
    public class WithdrawSubscriber : ICapSubscribe
    {
        private readonly ILogger<WithdrawSubscriber> _logger;
        public WithdrawSubscriber(ILogger<WithdrawSubscriber> logger)
        {
            _logger = logger;
        }

        [CapSubscribe("WithdrawEvent")]
        public void CheckReceivedMessage(WithdrawEvent withdrawEvent)
        {
            _logger.LogFormatInfo(JsonConvert.SerializeObject(withdrawEvent));
        }
    }
}
