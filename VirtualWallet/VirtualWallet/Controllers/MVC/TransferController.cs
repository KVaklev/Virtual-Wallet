using Business.DTOs.Requests;
using Business.DTOs.Responses;
using Business.Exceptions;
using Business.QueryParameters;
using Business.Services.Contracts;
using DataAccess.Models.Models;
using DataAccess.Repositories.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Org.BouncyCastle.Security;

namespace VirtualWallet.Controllers.MVC
{
    [AllowAnonymous]
    public class TransferController : Controller
    {
        private readonly ITransferService transferService;
        private readonly IUserRepository userRepository;

        public TransferController(ITransferService transferService, IUserRepository userRepository)
        {
            this.transferService = transferService;
            this.userRepository = userRepository;
        }

        [HttpGet]
        public async Task<IActionResult> Index([FromQuery] TransferQueryParameters parameters)
        {
            try
            {
                var loggedUser = await GetLoggedUserAsync();
                //var model = transferService.GetAll(loggedUser).Select(m => new GetTransferDto { Username = m.Account.User.Username, DateCreated = m.DateCreated }).ToList();
                var result = await transferService.FilterByAsync(parameters, loggedUser);
                return View(result);

            }
            catch (EntityNotFoundException ex)
            {
                return await EntityNotFoundErrorViewAsync(ex.Message);
            }

        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            try
            {
                var loggedUser = await GetLoggedUserAsync();
                var transfer = await this.transferService.GetByIdAsync(id, loggedUser);
                return View(transfer);
            }
            catch (EntityNotFoundException ex)
            {
                return await EntityNotFoundErrorViewAsync(ex.Message);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Create([FromQuery] TransferQueryParameters parameters)
        {
            //if (!this.HttpContext.Session.Keys.Contains("LoggedUser)"))
            //{
            //    return RedirectToAction("Login", "Auth");
            //}

            if ((this.HttpContext.Session.GetString("IsBlocked")) == "True")
            {
                return await BlockedUserErrorViewAsync();
            }
            else
            {
                return View();
            }
        }

        [HttpPost]

        public async Task<IActionResult> Create(CreateTransferDto transferDto)
        {
            try 
            {
                if (!this.ModelState.IsValid)
                {
                    return this.View(transferDto);
                }

                var loggedUser = await GetLoggedUserAsync();

                var result = await this.transferService.CreateAsync(transferDto, loggedUser);

                if (!result.IsSuccessful)
                {
                    return await EntityNotFoundErrorViewAsync(result.Message);
                }
                return this.RedirectToAction("Index", "Transfer", new { username = result.Data.Username });
            }
            catch (EntityNotFoundException ex)
            {
                return await EntityNotFoundErrorViewAsync(ex.Message);
            }
        }

        [HttpGet]

        public async Task<IActionResult> Edit([FromRoute] int id)
        {
            try
            {
                var loggedUser = await GetLoggedUserAsync();
                var result = await this.transferService.GetByIdAsync(id, loggedUser);

                if (!result.IsSuccessful)
                {
                    return await EntityNotFoundErrorViewAsync(result.Message);
                }
                return View(result.Data);
            }
            catch (EntityNotFoundException ex)
            {
                return await EntityNotFoundErrorViewAsync(ex.Message);
            }
        }

        [HttpPost]

        public async Task<IActionResult> Edit(int id, UpdateTransferDto transferDto)
        {
            try
            {
                if (!this.ModelState.IsValid)
                {
                    return View(transferDto);
                }

                var loggedUser = await GetLoggedUserAsync();
                var result = await this.transferService.UpdateAsync(id, transferDto, loggedUser);

                if (!result.IsSuccessful)
                {
                    return await EntityNotFoundErrorViewAsync(result.Message);
                }

                return this.RedirectToAction("Index", "Transfer", new { Username = loggedUser.Username });
            }


            catch (EntityNotFoundException ex)
            {
                return await EntityNotFoundErrorViewAsync(ex.Message);
            }
        }

        [HttpGet]

        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            try
            {
                var loggedUser = await GetLoggedUserAsync();
                var result = await this.transferService.GetByIdAsync(id, loggedUser);
                if(!result.IsSuccessful)
                {
                    return await EntityNotFoundErrorViewAsync(result.Message);
                }

                return this.View(result.Data);
            }
            catch (EntityNotFoundException ex)
            {
                return await EntityNotFoundErrorViewAsync(ex.Message);
            }
        }

        [HttpPost, ActionName("Delete")]

        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var loggedUser = await GetLoggedUserAsync();
                var result = await this.transferService.DeleteAsync(id, loggedUser);

                if (!result.IsSuccessful)
                {
                    return await EntityNotFoundErrorViewAsync(result.Message);
                }

                return RedirectToAction("Index", "Transaction", new {Username = loggedUser.Username});

            }
            catch (EntityNotFoundException ex)
            {
                return await EntityNotFoundErrorViewAsync(ex.Message);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Execute(int id)
        {
            try
            {
                var loggedUser = await GetLoggedUserAsync();
                var result = await this.transferService.ExecuteAsync(id, loggedUser);
                if(!result.IsSuccessful)
                {
                    return await EntityNotFoundErrorViewAsync(result.Message);
                }

                return RedirectToAction("Index", "Transfer", new {Username = loggedUser.Username});
            }
            catch (EntityNotFoundException ex)
            {
                return await EntityNotFoundErrorViewAsync(ex.Message);
            }
        }


        private async Task<User> GetLoggedUserAsync()
        {
            var userName = this.HttpContext.Session.GetString("LoggedUser");
            var user = await this.userRepository.GetByUsernameAsync(userName);
            return user;

        }

        private async Task<IActionResult> EntityNotFoundErrorViewAsync(string message)
        {
            this.HttpContext.Response.StatusCode = StatusCodes.Status404NotFound;
            this.ViewData["ErrorMessage"] = message;
            return View("Error404 - Not Found");

        }


        private async Task<IActionResult> BlockedUserErrorViewAsync()
        {
            this.HttpContext.Response.StatusCode = StatusCodes.Status401Unauthorized;
            this.ViewData["BlockedUser"] = "You are a BLOCKED USER!";
            return View("Error401 - Not Authorized");
        }
    }
}
