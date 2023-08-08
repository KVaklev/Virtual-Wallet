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
using System.Security.Claims;
using System.Security.Cryptography.Xml;

namespace VirtualWallet.Controllers.MVC
{
    [Authorize]
    public class TransactionController : Controller
    {
        private readonly ITransactionService transactionService;
        private readonly IUserService userService;
        private readonly ICurrencyService currencyService;
        private readonly IExchangeRateService exchangeRateService;

        public TransactionController(
            ITransactionService transactionService,
            IUserService userService,
            ICurrencyService currencyService,
            IExchangeRateService exchangeRateService)
        {
            this.transactionService = transactionService;
            this.userService = userService;
            this.currencyService = currencyService;
            this.exchangeRateService = exchangeRateService;
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

           var createTransactionViewModel = new CreateTransactionViewModel();
            var result = this.currencyService.GetAll();
            if (!result.IsSuccessful)
            {
                return await EntityErrorViewAsync(result.Message);
            }
            createTransactionViewModel.Currencies = result.Data;
           
            return this.View(createTransactionViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateTransactionViewModel transactionDto)
        {
            var currencyResult = this.currencyService.GetAll();
            transactionDto.Currencies = currencyResult.Data;
            transactionDto.CreateTransactionDto.CurrencyCode = "USD";
                //if (!this.ModelState.IsValid)
                //{
                //    return this.View(transactionDto);
                //}
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
                return this.RedirectToAction("Execute", "Transaction", new { id = result.Data.Id });
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
            var transactionResult = await this.transactionService.GetByIdAsync(id, loggedUserResult.Data);
            if (!transactionResult.IsSuccessful)
            {
                return await EntityErrorViewAsync(transactionResult.Message);
            }
            var result = await InitializedExecuteTransactionViewModelAsync(transactionResult.Data);
            if (!result.IsSuccessful)
            {
                return await EntityErrorViewAsync(transactionResult.Message);
            }
            ExecuteTransactionViewModel executeTransactionViewModel = result.Data;

            return this.View(executeTransactionViewModel);

        }

        [HttpPost]
        public async Task<IActionResult> Execute(
            [FromRoute] int id,
            ExecuteTransactionViewModel executeTransactionViewModel)
        {
            var loggedUserResult = await GetLoggedUserAsync();
            if (!loggedUserResult.IsSuccessful)
            {
                return await EntityErrorViewAsync(loggedUserResult.Message);
            }
            var transactionResult = await this.transactionService.GetByIdAsync(id, loggedUserResult.Data);
            if (!transactionResult.IsSuccessful)
            {
                return await EntityErrorViewAsync(loggedUserResult.Message);
            }
            executeTransactionViewModel.GetTransactionDto = transactionResult.Data;

            var userResult = await this.userService.GetLoggedUserByUsernameAsync(transactionResult.Data.RecipientUsername);
            if (!userResult.IsSuccessful)
            {
                return await EntityErrorViewAsync(userResult.Message);
            }
            executeTransactionViewModel.Recipient = userResult.Data;

            var result = await this.transactionService.ExecuteAsync(id, loggedUserResult.Data);
            if (!result.IsSuccessful)
            {
                return await EntityErrorViewAsync(result.Message);
            }

            return RedirectToAction("Confirm", "Transaction", new { id = transactionResult.Data.Id });
        }

        [HttpGet]
        public async Task<IActionResult> Confirm([FromRoute] int id)
        {
            var loggedUserResult = await GetLoggedUserAsync();
            if (!loggedUserResult.IsSuccessful)
            {
                return await EntityErrorViewAsync(loggedUserResult.Message);
            }
            var transactionResult = await this.transactionService.GetByIdAsync(id, loggedUserResult.Data);
            if (!transactionResult.IsSuccessful)
            {
                return await EntityErrorViewAsync(loggedUserResult.Message);
            }

            return this.View(transactionResult.Data);
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
            var loggedUsersUsername = User.FindFirst(ClaimTypes.Name);
            var loggedUserResult = await this.userService.GetLoggedUserByUsernameAsync(loggedUsersUsername.Value);

            return loggedUserResult;
        }

        private async Task<Response<ExecuteTransactionViewModel>> InitializedExecuteTransactionViewModelAsync(
            GetTransactionDto transaction)
        {
            var result = new Response<ExecuteTransactionViewModel>();
            ExecuteTransactionViewModel executeTransactionViewModel = new ExecuteTransactionViewModel();
            executeTransactionViewModel.GetTransactionDto = transaction;
            var userResult = await this.userService.GetLoggedUserByUsernameAsync(transaction.RecipientUsername);
            if (!userResult.IsSuccessful)
            {
                result.IsSuccessful = false;
                result.Message = userResult.Message;
                return result;
            }
            executeTransactionViewModel.Recipient = userResult.Data;
            var exchngeAmount = await this.exchangeRateService.ExchangeAsync(executeTransactionViewModel.GetTransactionDto.Amount,
                executeTransactionViewModel.GetTransactionDto.CurrencyCode,
                userResult.Data.Account.Currency.CurrencyCode);
            if (!exchngeAmount.IsSuccessful)
            {
                result.IsSuccessful = false;
                result.Message = exchngeAmount.Message;
                return result;
            }
            executeTransactionViewModel.RecipientGetsAmount = exchngeAmount.Data;
            result.Data = executeTransactionViewModel;
            return result;
        }
    }
}
