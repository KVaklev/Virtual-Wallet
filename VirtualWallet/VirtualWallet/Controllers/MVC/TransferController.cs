using AutoMapper;
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
using Newtonsoft.Json;
using Org.BouncyCastle.Security;
using System.Security.Claims;

namespace VirtualWallet.Controllers.MVC
{
    [Authorize]
    public class TransferController : Controller
    {
        private readonly ITransferService transferService;
        private readonly IUserService userService;
        private readonly ICurrencyService currencyService;
        private readonly IExchangeRateService exchangeRateService;
        private readonly IMapper mapper;

        public TransferController(ITransferService transferService, IUserService userService,
            ICurrencyService currencyService,
            IExchangeRateService exchangeRateService,
            IMapper mapper)
        {
            this.transferService = transferService;
            this.userService = userService;
            this.currencyService = currencyService;
            this.exchangeRateService = exchangeRateService;
            this.mapper = mapper;
        }

        [HttpGet]

        public async Task<IActionResult> Index([FromQuery] TransferQueryParameters parameters)
        {
            try
            {
                var loggedUser = await GetLoggedUserAsync();
                //var model = transferService.GetAll(loggedUser).Select(m => new GetTransferDto { Username = m.Account.User.Username, DateCreated = m.DateCreated }).ToList();
                var result = await transferService.FilterByAsync(parameters, loggedUser.Data);
                return View(result.Data);

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
                var transfer = await this.transferService.GetByIdAsync(id, loggedUser.Data);
                return View(transfer);
            }
            catch (EntityNotFoundException ex)
            {
                return await EntityNotFoundErrorViewAsync(ex.Message);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var createTransferViewModel = new CreateTransferViewModel();

            var result = this.currencyService.GetAll();

            if (!result.IsSuccessful)
            {
                return await EntityNotFoundErrorViewAsync(result.Message);
            }

            //TempData["Currencies"] = JsonSerializer.Serialize(result.Data);

            return View(createTransferViewModel);

        }

        [HttpPost]

        public async Task<IActionResult> Create(CreateTransferViewModel transferDto)
        {
            if (!this.ModelState.IsValid)
            {
                return this.View(transferDto);
            }
            var loggedUser = await GetLoggedUserAsync();

            if (!loggedUser.IsSuccessful)
            {
                return await EntityNotFoundErrorViewAsync(loggedUser.Message);
            }

            var result = await this.transferService.CreateAsync(transferDto.CreateTransferDto, loggedUser.Data);
            if (!result.IsSuccessful)
            {
                return await EntityNotFoundErrorViewAsync(result.Message);
            }

            return this.RedirectToAction("Execute", "Transfer", new { id = result.Data.Id });

        }

        [HttpGet]

        public async Task<IActionResult> Edit([FromRoute] int id)
        {
            try
            {
                var loggedUser = await GetLoggedUserAsync();
                var result = await this.transferService.GetByIdAsync(id, loggedUser.Data);

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
                var result = await this.transferService.UpdateAsync(id, transferDto, loggedUser.Data);

                if (!result.IsSuccessful)
                {
                    return await EntityNotFoundErrorViewAsync(result.Message);
                }

                return this.RedirectToAction("Index", "Transfer", new { Username = loggedUser.Data.Username });
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
                var result = await this.transferService.GetByIdAsync(id, loggedUser.Data);
                if (!result.IsSuccessful)
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
                var result = await this.transferService.DeleteAsync(id, loggedUser.Data);

                if (!result.IsSuccessful)
                {
                    return await EntityNotFoundErrorViewAsync(result.Message);
                }

                return RedirectToAction("Index", "Transaction", new { Username = loggedUser.Data.Username });

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
                var result = await this.transferService.ExecuteAsync(id, loggedUser.Data);
                if (!result.IsSuccessful)
                {
                    return await EntityNotFoundErrorViewAsync(result.Message);
                }

                return RedirectToAction("Index", "Transfer", new { Username = loggedUser.Data.Username });
            }
            catch (EntityNotFoundException ex)
            {
                return await EntityNotFoundErrorViewAsync(ex.Message);
            }
        }


        private async Task<Response<User>> GetLoggedUserAsync()
        {
            var loggedUsersUsername = User.FindFirst(ClaimTypes.Name);
            var loggedUserResult = await this.userService.GetLoggedUserByUsernameAsync(loggedUsersUsername.Value);

            return loggedUserResult;

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
