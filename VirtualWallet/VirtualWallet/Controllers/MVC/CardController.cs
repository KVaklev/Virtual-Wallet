using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace VirtualWallet.Controllers.MVC
{
    [Authorize]
    public class CardController : Controller
    {
        public IActionResult PaymentMethods()
        {
            return View();
        }
    }
}
