using Microsoft.AspNetCore.Mvc;

namespace VirtualWallet.Controllers.MVC
{
    public class TransferController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
