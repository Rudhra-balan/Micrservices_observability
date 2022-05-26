using Microsoft.AspNetCore.Mvc;
using Webhook.Application.Services.Interface;
using Webhook.DomainCore.Model;

namespace Webhook.Controllers
{

    [ApiController, Route("[controller]")]
    public class PuttyController : ControllerBase
    {
        
        private readonly ILogger<GrafanaController> _logger;
        private readonly IBashService _bashService;

        public PuttyController( ILogger<GrafanaController> logger, IBashService bashService)
        {
            
            _logger = logger;
            _bashService = bashService;
          

        }

        [HttpPost(nameof(Command))]
        public Response Command(PuttyRequest puttyRequest)
        { 
           var response = _bashService.Run(puttyRequest.Command);
           if( response.Exception != null )
                throw response.Exception;
            return response;
        }

    }
}
