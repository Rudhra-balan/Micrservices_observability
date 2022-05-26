
using DomainCore.Helper.Constant;
using DomainCore.Models.Response;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Web.Application.Services.Interface;
using Web.DomainCore.Model;

namespace Web.Controllers.Simulater
{
    [Route("Putty")]
    public class PuttyController : Controller
    {
        private readonly ILogger<PuttyController> _logger;
        private readonly IBashService _bashService;
        public PuttyController(ILogger<PuttyController> logger, IBashService bashService)
        {
            _bashService = bashService;
            _logger = logger;
        }

        [HttpGet]
        [Route(nameof(Index))]
        public IActionResult Index()
        {
            return PartialView(UrlConstant.PuttyViewCshtml);
        }

        [HttpPost(nameof(Command))]
        public WebClientResponse Command([FromBody] PuttyRequest puttyRequest)
        {
            var webClientResponse = new WebClientResponse();
            var response = _bashService.Run(puttyRequest.Command);
            if (response.Exception != null)
                throw response.Exception;
            webClientResponse.ErrorId = response.Code;
            webClientResponse.IsOperationSuccess = true;
            webClientResponse.ErrorDescription = string.Empty;
            webClientResponse.SourceObject = response;
            return webClientResponse;
        }

    }
}