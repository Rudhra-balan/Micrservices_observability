using Application.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Connections.Features;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Infrastructure.HubService
{
    [AllowAnonymous]
    public class EHubMessage : Hub
    {
        private readonly ILogger<EHubMessage> _logger;
        private IHttpContextAccessor _httpContextAccessor;
        public EHubMessage(ILogger<EHubMessage> logger, IHttpContextAccessor httpContextAccessor)
        {
            _logger = logger;
            _httpContextAccessor= httpContextAccessor;
        }
        public override async Task OnConnectedAsync()
        {
            var userId = _httpContextAccessor.HttpContext.User.GetUserId();

            await Groups.AddToGroupAsync(Context.ConnectionId, userId.ToString());

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
