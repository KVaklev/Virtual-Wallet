using Business.QueryParameters;
using Business.Services.Contracts;
using DataAccess.Models.Models;
using Microsoft.AspNetCore.Mvc;

namespace VirtualWallet.Controllers.MVC
{
    public class TransferController : Controller
    {
        private readonly ITransferService transferService;

        public TransferController(ITransferService transferService)
        {
            this.transferService = transferService;
        }

        [HttpGet]
        public async Task<IActionResult> Index([FromQuery] TransferQueryParameters transferQueryParameters)
        {
            if (!this.HttpContext.Session.Keys.Contains("LoggedUser"))
            {
                return RedirectToAction("Login", "Auth");
            }

            return View();
        }
    }
}
