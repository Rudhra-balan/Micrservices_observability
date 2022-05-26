
using DomainCore.Helper.Constant;
using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers.Simulater
{
    [Route("Simulator")]
    public class SimulatorController : Controller
    {
        [HttpGet]
        [Route(nameof(Index))]
        public IActionResult Index()
        {
            return PartialView(UrlConstant.SimulatorViewCshtml);
        }
    }
}