using Microsoft.AspNetCore.Mvc;

namespace VirtualWallet.Controllers.MVC
{
    public class UserController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
