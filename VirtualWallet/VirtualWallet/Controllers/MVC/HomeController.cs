using Microsoft.AspNetCore.Mvc;

namespace VirtualWallet.Controllers.MVC
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
