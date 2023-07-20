using Microsoft.AspNetCore.Mvc;

namespace VirtualWallet.Controllers.API
{
    [ApiController]
    [Route("api/users")]
    public class UsersApiController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
