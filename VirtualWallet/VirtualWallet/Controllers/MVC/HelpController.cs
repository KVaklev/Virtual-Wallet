using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace VirtualWallet.Controllers.MVC
{
    [AllowAnonymous]
    public class HelpController : Controller
    {
        
        [HttpGet]
        public IActionResult AboutUs()
        {
            return View();
        }

        [HttpGet]
        public IActionResult FAQ()
        {
            return View();
        }

        [HttpGet]
        public IActionResult PrivacyPolicyAndTermsOfUse()
        {
            return View();
        }

        [HttpGet]
        public IActionResult ContactUs()
        {
            return View();
        }
    }
}
