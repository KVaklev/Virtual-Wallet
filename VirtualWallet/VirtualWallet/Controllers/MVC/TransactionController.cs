using Business.DTOs.Requests;
using Business.Exceptions;
using Business.QueryParameters;
using Business.Services.Contracts;
using DataAccess.Models.Models;
using DataAccess.Repositories.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace VirtualWallet.Controllers.MVC
{
    [Authorize]
    public class TransactionController : Controller
    {
        private readonly ITransactionService transactionService;
        private readonly IUserRepository userRepository;

        public TransactionController(
            ITransactionService transactionService,
            IUserRepository userRepository)
        {
            this.transactionService = transactionService;
            this.userRepository = userRepository;
        }


        [HttpGet]
        public async Task<IActionResult> Index([FromQuery] TransactionQueryParameters parameters)
        {
            try
            {
                var loggedUser = await GetLoggedUserAsync();
                var result = await this.transactionService.FilterByAsync(parameters, loggedUser);

                return View(result);
            }
            catch (EntityNotFoundException ex)
            {
                return await EntityErrorViewAsync(ex.Message);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Create([FromQuery] TransactionQueryParameters parameters)
        {

            if ((this.HttpContext.Session.GetString("IsBlocked")) == "True")
            {
                return BlockedErrorView();
            }
            else
            {
                return this.View();
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateAsync([FromRoute] CreateTransactionDto transactionDto)
        {
            try
            {
                if (!this.ModelState.IsValid)
                {
                    return this.View(transactionDto);
                }
                var loggedUser = await GetLoggedUserAsync();
                var result = await this.transactionService.CreateOutTransactionAsync(transactionDto, loggedUser);
                if (!result.IsSuccessful)
                {
                    return await EntityErrorViewAsync(result.Message);
                }
                return this.RedirectToAction("Index", "Transacion", new { username = result.Data.SenderUsername });
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
                var loggedUser = await GetLoggedUserAsync();
                var result = await this.transactionService.GetByIdAsync(id, loggedUser);
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
                var loggedUser = await GetLoggedUserAsync();
                var result = await this.transactionService.UpdateAsync(id, loggedUser, transactionDto);
                if (!result.IsSuccessful)
                {
                    return await EntityErrorViewAsync(result.Message);
                }
                return this.RedirectToAction("Index", "Transaction", new { Username = loggedUser.Username });
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
                var loggedUser = await GetLoggedUserAsync();
                var result = await this.transactionService.GetByIdAsync(id, loggedUser);
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
                var loggedUser = await GetLoggedUserAsync();
                var result = await this.transactionService.DeleteAsync(id, loggedUser);
                if (!result.IsSuccessful)
                {
                    return await EntityErrorViewAsync(result.Message);
                }

                return this.RedirectToAction("Index", "Transaction", new { Username = loggedUser.Username });
            }
            catch (EntityNotFoundException ex)
            {
                return await EntityErrorViewAsync(ex.Message);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Execute([FromRoute] int id)
        {
            try
            {
                var loggedUser = await GetLoggedUserAsync();
                var result = await this.transactionService.ExecuteAsync(id, loggedUser);
                if (!result.IsSuccessful)
                {
                    return await EntityErrorViewAsync(result.Message);
                }

                return this.RedirectToAction("Index", "Transaction", new { Username = loggedUser.Username });
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


        private async Task<User> GetLoggedUserAsync()
        {
            var username = this.HttpContext.Session.GetString("LoggedUser");
            var user = await this.userRepository.GetByUsernameAsync(username);
            return user;
        }
    }
}
