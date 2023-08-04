using Business.Exceptions;
using Business.QueryParameters;
using Microsoft.AspNetCore.Mvc;
using Presentation.Helpers;

namespace VirtualWallet.Controllers.MVC
{
    public class TransactionController : Controller
    {
        private readonly ITransactionService transactionService; 

        public TransactionController(ITransactionService transactionService)
        {
            this.transactionService = transactionService;
        }

        
        [HttpGet]
        public async Task<IActionResult> Index([FromQuery] TransactionQueryParameters parameters)
        {
            try
            {
                var result = await this.transactionService.FilterByAsync(parameters);

                return View(result.Data);
            }
            catch (EntityNotFoundException ex)
            {
                return EntityErrorView(ex.Message);
            }
        }

        private IActionResult EntityErrorViewAsync(string message)
        {
            this.HttpContext.Response.StatusCode = StatusCodes.Status404NotFound;
            this.ViewData["ErrorMessage"] = message;

            return this.View("Error404");
        }
        [HttpGet]
        public async Task<IActionResult> Create([FromQuery] TransactionQueryParameters parameters)
        {
            try
            {
                var result = await this.transactionService.Create(parameters);

                return View(result.Data);
            }
            catch (EntityNotFoundException ex)
            {
                return await EntityErrorView(ex.Message);
            }
        }

        private async Task<IActionResult> EntityErrorView(string message)
        {
            this.HttpContext.Response.StatusCode = StatusCodes.Status404NotFound;
            this.ViewData["ErrorMessage"] = message;

            return this.View("Error404");
        }
    }
}
