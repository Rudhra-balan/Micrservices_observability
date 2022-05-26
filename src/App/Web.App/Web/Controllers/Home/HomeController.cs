
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers.Home
{

    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}