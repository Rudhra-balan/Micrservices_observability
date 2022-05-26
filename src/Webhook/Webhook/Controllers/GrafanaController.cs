using BuildingBlocks.CQRS.Commands;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using Webhook.Application.Commands;
using Webhook.DomainCore;
using Webhook.DomainCore.Model;
using Webhook.DomainCore.HubService;

namespace Webhook.Controllers
{

    [ApiController, Route("webhooks/[controller]")]
    public class GrafanaController : ControllerBase
    {
        private readonly ICommandDispatcher _commandDispatcher;
        private readonly ILogger<GrafanaController> _logger;
        private readonly IHubContext<EHubMessage> _hubContext;

        public GrafanaController(ICommandDispatcher commandDispatcher, ILogger<GrafanaController> logger, IHubContext<EHubMessage> hubContext)
        {
            _commandDispatcher = commandDispatcher;
            _logger = logger;
            _hubContext = hubContext;

        }

        [HttpPost(nameof(Subscriber))]
        public async Task Subscriber(GrafanaRequestCommand grafanaRequestCommand)
        {
            _logger.LogInformation("Request {@grafanaRequestCommand}", grafanaRequestCommand);
            await _commandDispatcher.SendAsync<GrafanaRequestCommand, dynamic>(grafanaRequestCommand);
            await _hubContext.Clients.All.SendAsync($"{SignalRMethod.BalanceNotification}", JsonConvert.SerializeObject(grafanaRequestCommand, Formatting.Indented));
        }

    }
}
