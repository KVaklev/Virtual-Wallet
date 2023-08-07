using Business.DTOs.Requests;
using Business.DTOs.Responses;
using Business.Exceptions;
using Business.QueryParameters;
using Business.Services.Contracts;
using Business.ViewModels;
using DataAccess.Models.Models;
using DataAccess.Repositories.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Security.Cryptography.Xml;

namespace VirtualWallet.Controllers.MVC
{
    [AllowAnonymous]
    public class TransactionController : Controller
    {
        private readonly ITransactionService transactionService;
        private readonly IUserService userService;
        private readonly ICurrencyService currencyService;

        public TransactionController(
            ITransactionService transactionService,
            IUserService userService,
			ICurrencyService currencyService)
        {
            this.transactionService = transactionService;
            this.userService = userService;
            this.currencyService = currencyService;
        }


        [HttpGet]
        public async Task<IActionResult> Index([FromQuery] TransactionQueryParameters parameters)
        {
			//try
			//{
			//    var loggedUser = await GetLoggedUserAsync();
			//    var result = await this.transactionService.FilterByAsync(parameters, loggedUser);

			//    return View(result);
			//}
			//catch (EntityNotFoundException ex)
			//{
			//    return await EntityErrorViewAsync(ex.Message);
			//}

			return View();
		}

        [HttpGet]
        public async Task<IActionResult> Create()
        {

            //if ((this.HttpContext.Session.GetString("IsBlocked")) == "True")
            //{
            //    return BlockedErrorView();
            //}
            //else
            //{
            //    return this.View();
            //}
            var createTransactionViewModel = new CreateTransactionViewModel();
            createTransactionViewModel.Currencies = this.currencyService.GetAll();

            return this.View(createTransactionViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateTransactionViewModel transactionDto)
        {
            try
            {
                if (!this.ModelState.IsValid)
                {
                    return this.View(transactionDto);
                }
                var loggedUserResult = await GetLoggedUserAsync();
                if (!loggedUserResult.IsSuccessful)
                {
                    return await EntityErrorViewAsync(loggedUserResult.Message);
                }
                var result = await this.transactionService.CreateOutTransactionAsync(transactionDto.CreateTransactionDto, loggedUserResult.Data);
                if (!result.IsSuccessful)
                {
                    return await EntityErrorViewAsync(result.Message);
                }
                return this.RedirectToAction("Execute", "Transacion", new { username = result.Data.SenderUsername });
            }
            catch (EntityNotFoundException ex)
            {
                return await EntityErrorViewAsync(ex.Message);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Edit([FromRoute] int id)
        {
            try
            {
                var loggedUserResult = await GetLoggedUserAsync();
                if (!loggedUserResult.IsSuccessful)
                {
                    return await EntityErrorViewAsync(loggedUserResult.Message);
                }
                var result = await this.transactionService.GetByIdAsync(id, loggedUserResult.Data);
                if (!result.IsSuccessful)
                {
                    return await EntityErrorViewAsync(result.Message);
                }
                return this.View(result.Data);
            }
            catch (EntityNotFoundException ex)
            {
                return await EntityErrorViewAsync(ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> EditAsync([FromRoute] int id, CreateTransactionDto transactionDto)
        {
            try
            {
                if (!this.ModelState.IsValid)
                {
                    return View(transactionDto);
                }
                var loggedUserResult = await GetLoggedUserAsync();
                if (!loggedUserResult.IsSuccessful)
                {
                    return await EntityErrorViewAsync(loggedUserResult.Message);
                }
                var result = await this.transactionService.UpdateAsync(id, loggedUserResult.Data, transactionDto);
                if (!result.IsSuccessful)
                {
                    return await EntityErrorViewAsync(result.Message);
                }
                return this.RedirectToAction("Index", "Transaction", new { Username = loggedUserResult.Data.Username });
            }
            catch (EntityNotFoundException ex)
            {
                return await EntityErrorViewAsync(ex.Message);
            }
        }

        [HttpGet]
        public async Task<IActionResult> DeleteAsync([FromRoute] int id)
        {
            try
            {
                var loggedUserResult = await GetLoggedUserAsync();
                if (!loggedUserResult.IsSuccessful)
                {
                    return await EntityErrorViewAsync(loggedUserResult.Message);
                }
                var result = await this.transactionService.GetByIdAsync(id, loggedUserResult.Data);
                if (!result.IsSuccessful)
                {
                    return await EntityErrorViewAsync(result.Message);
                }
                return this.View(result.Data);
            }
            catch (EntityNotFoundException ex)
            {
                return await EntityErrorViewAsync(ex.Message);
            }
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed([FromRoute] int id)
        {
            try
            {
                var loggedUserResult = await GetLoggedUserAsync();
                if (!loggedUserResult.IsSuccessful)
                {
                    return await EntityErrorViewAsync(loggedUserResult.Message);
                }
                var result = await this.transactionService.DeleteAsync(id, loggedUserResult.Data);
                if (!result.IsSuccessful)
                {
                    return await EntityErrorViewAsync(result.Message);
                }

                return this.RedirectToAction("Index", "Transaction", new { Username = loggedUserResult.Data.Username });
            }
            catch (EntityNotFoundException ex)
            {
                return await EntityErrorViewAsync(ex.Message);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Execute([FromRoute] int id)
        {
                var loggedUserResult = await GetLoggedUserAsync();
                if (!loggedUserResult.IsSuccessful)
                {
                    return await EntityErrorViewAsync(loggedUserResult.Message);
                }
                var result = await this.transactionService.GetByIdAsync(id, loggedUserResult.Data);
                if (!result.IsSuccessful)
                {
                    return await EntityErrorViewAsync(result.Message);
                }
                return this.View(result.Data);
            
        }

        [HttpPost]
        public async Task<IActionResult> Execute(int id, GetTransactionDto getTransactionDto)
        {
            try
            {
                var loggedUserResult = await GetLoggedUserAsync();
                if (!loggedUserResult.IsSuccessful)
                {
                    return await EntityErrorViewAsync(loggedUserResult.Message);
                }
                var result = await this.transactionService.ExecuteAsync(id, loggedUserResult.Data);
                if (!result.IsSuccessful)
                {
                    return await EntityErrorViewAsync(result.Message);
                }

                return this.RedirectToAction("Index", "Transaction", new { Username = loggedUserResult.Data.Username });
            }
            catch (EntityNotFoundException ex)
            {
                return await EntityErrorViewAsync(ex.Message);
            }
        }

        private async Task<IActionResult> EntityErrorViewAsync(string message)
        {
            this.HttpContext.Response.StatusCode = StatusCodes.Status404NotFound;
            this.ViewData["ErrorMessage"] = message;

            return this.View("Error404");
        }

        private IActionResult BlockedErrorView()
        {
            this.HttpContext.Response.StatusCode = StatusCodes.Status401Unauthorized;
            this.ViewData["ErrorMessage"] = "You are a \"BLOCKED USER\"!";
            return this.View("UnauthorizedError");
        }


        private async Task<Response<User>> GetLoggedUserAsync()
        {
            //var username = this.HttpContext.Session.GetString("LoggedUser");
            var userResult = await this.userService.GetLoggedUserByUsernameAsync("ivanGorev");
            return userResult;
        }

        
    }
}
