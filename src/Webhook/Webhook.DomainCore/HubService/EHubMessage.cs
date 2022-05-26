
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace Webhook.DomainCore.HubService
{
    [AllowAnonymous]
    public class EHubMessage : Hub
    {
        private readonly ILogger<EHubMessage> _logger;
        
        public EHubMessage(ILogger<EHubMessage> logger)
        {
            _logger = logger;
          
        }
        public override async Task OnConnectedAsync()
        {
          
            await base.OnConnectedAsync();
            _logger.LogInformation($"A client connected to NotificationHub: {Context.ConnectionId}");
        }

     

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            await base.OnDisconnectedAsync(exception);
            _logger.LogError($"A client disconnected from NotificationHub: {Context.ConnectionId}");
        }
    }
}
