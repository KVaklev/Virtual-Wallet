using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace VirtualWallet.Controllers.MVC
{
    [AllowAnonymous]
    public class HelpController : Controller
    {
        [Route("/AboutUs")]
        [HttpGet]
        public IActionResult AboutUs()
        {
            return View();
        }

        [Route("/FAQ")]
        [HttpGet]
        public IActionResult FAQ()
        {
            return View();
        }
    }
}
