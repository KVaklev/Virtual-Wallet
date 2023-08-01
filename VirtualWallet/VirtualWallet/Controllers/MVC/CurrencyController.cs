using Business.Exceptions;
using Business.Services.Contracts;
using DataAccess.Models.Models;
using Microsoft.AspNetCore.Mvc;

namespace VirtualWallet.Controllers.MVC
{
    public class CurrencyController : Controller
    {
        private readonly ICurrencyService currencyService;

        public CurrencyController(ICurrencyService currencyService) 
        {
            this.currencyService = currencyService;
        }

        [HttpGet]
        public IActionResult Index()
        {
            try
            {
                var currencies = this.currencyService.GetAll;
                return View(currencies);
            }
            catch (EntityNotFoundException ex)
            {
                return View(ex.Message);
            }
        }
    }
}
