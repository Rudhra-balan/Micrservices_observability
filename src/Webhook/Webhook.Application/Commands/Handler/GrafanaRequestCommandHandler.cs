using BuildingBlocks.CQRS.Commands;
using BuildingBlocks.Logging;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Webhook.Application.Services.Interface;
using Webhook.DomainCore;
using Webhook.DomainCore.HubService;

namespace Webhook.Application.Commands.Handlers;

public class GrafanaRequestCommandHandler : ICommandHandler<GrafanaRequestCommand>
{
    private readonly ILogger<GrafanaRequestCommandHandler> _logger;
    private readonly IBashService _bashService;
    private readonly IHubContext<EHubMessage> _hubContext;

    public GrafanaRequestCommandHandler(ILogger<GrafanaRequestCommandHandler> logger, IBashService bashService, IHubContext<EHubMessage> hubContext)
    {
        _logger = logger;
        _bashService = bashService;
        _hubContext = hubContext;
    }


    public async Task<TResponse> HandleAsync<TResponse>(GrafanaRequestCommand command, CancellationToken token)
    {
        try
        {
            if (0 != string.Compare(command.state, "ok", StringComparison.OrdinalIgnoreCase))
            {
                // systemctl is -active--quiet service && echo Service is running
                // systemctl show -p ActiveState --value x11-common
                var response =  _bashService.Run("systemctl restart microservice-balance.service");
                response = _bashService.Run("systemctl status microservice-balance.service");
               if(string.IsNullOrEmpty( response.Stderr) == false)
                    await _hubContext.Clients.All.SendAsync($"{SignalRMethod.BalanceNotification}", response.Stderr);
                if (string.IsNullOrEmpty(response.Stdout) == false)
                    await _hubContext.Clients.All.SendAsync($"{SignalRMethod.BalanceNotification}", response.Stdout);

            }
            
        }
        catch (Exception ex)
        {
            _logger.LogFormatError(ex);

        }
        return default;
       
    }
}

  
