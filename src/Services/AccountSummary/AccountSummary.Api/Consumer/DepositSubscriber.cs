using AccountSummary.Application.Events;
using BuildingBlocks.Logging;
using DotNetCore.CAP;
using Newtonsoft.Json;

namespace AccountSummary.Api.Consumer
{
    public class DepositSubscriber : ICapSubscribe
    {
        private readonly ILogger<DepositSubscriber> _logger;
        public DepositSubscriber(ILogger<DepositSubscriber> logger)
        {
            _logger = logger;
        }

        [CapSubscribe("DepositEvent")]
        public void CheckReceivedMessage(DepositEvent deposit)
        {
            _logger.LogFormatInfo(JsonConvert.SerializeObject(deposit));
        }
    }
}
