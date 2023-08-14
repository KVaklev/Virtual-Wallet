﻿using AutoMapper;
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
using System.Text.Json;
using Org.BouncyCastle.Security;
using System.Security.Claims;
using Business.Mappers;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace VirtualWallet.Controllers.MVC
{
    [Authorize]
    public class TransferController : Controller
    {
        private readonly ITransferService transferService;
        private readonly IUserService userService;
        private readonly ICurrencyService currencyService;
        private readonly ICardService cardService;
        private readonly IAccountService accountService;

        private readonly IExchangeRateService exchangeRateService;
        private readonly IMapper mapper;

        public TransferController(ITransferService transferService, IUserService userService,
            ICurrencyService currencyService,
            IExchangeRateService exchangeRateService,
            ICardService cardService, IAccountService accountService,
        IMapper mapper)
        {
            this.transferService = transferService;
            this.userService = userService;
            this.currencyService = currencyService;
            this.exchangeRateService = exchangeRateService;
            this.cardService = cardService;
            this.mapper = mapper;
            this.accountService = accountService;
        }

        [HttpGet]

        public async Task<IActionResult> Index([FromQuery] TransferQueryParameters parameters)
        {

            var loggedUser = await FindLoggedUserAsync();

            if (!loggedUser.IsSuccessful)
            {
                return this.RedirectToAction("Login", "Account");
            }

            var result = await transferService.FilterByAsync(parameters, loggedUser.Data);


            if (!result.IsSuccessful)
            {
                return await EntityNotFoundErrorViewAsync(result.Message);
            }

            var indexTransferViewModel = new IndexTransferViewModel();

            indexTransferViewModel.TransferDtos = result.Data;
            indexTransferViewModel.TransferQueryParameters= parameters;
            indexTransferViewModel.User = loggedUser.Data;

            return View(indexTransferViewModel);
        }
                

        [HttpGet]
        public async Task<IActionResult> Details([FromRoute] int id)
        {
            var loggedUser = await FindLoggedUserAsync();
            if (!loggedUser.IsSuccessful)
            {
                return this.RedirectToAction("Login", "Account");
            }

            var result = await this.transferService.GetByIdAsync(id, loggedUser.Data);

            if (!result.IsSuccessful)
            {
                return await EntityNotFoundErrorViewAsync(result.Message);
            }

            var detailsTransferViewModel = new DetailsTransferViewModel();
            detailsTransferViewModel.GetTransferDto = result.Data;
            detailsTransferViewModel.LoggedUser = loggedUser.Data;

            return this.View(detailsTransferViewModel);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var query = new CardQueryParameters();

            var loggedUser = await FindLoggedUserAsync();

            if (!loggedUser.IsSuccessful)
            {
                return this.RedirectToAction("Login", "Account");
            }
            var createTransferViewModel = new CreateTransferViewModel();

            var resultAccount = await this.accountService.GetByIdAsync((int)loggedUser.Data.AccountId, loggedUser.Data);

            if (!resultAccount.IsSuccessful)
            {
                return await EntityNotFoundErrorViewAsync(resultAccount.Message);
            }

            //createTransferViewModel.CurrencyCode = resultAccount.Data.CurrencyCode;

            //var currencies = this.currencyService.GetAll();

            //if (!currencies.IsSuccessful)
            //{
            //    return await EntityNotFoundErrorViewAsync(currencies.Message);
            //}

            var cards = await this.cardService.FilterByAsync(query, loggedUser.Data);

            if (!cards.IsSuccessful)
            {
                return await EntityNotFoundErrorViewAsync(cards.Message);
            }

            createTransferViewModel.Cards = cards.Data;

            //TempData["Currencies"] = JsonSerializer.Serialize(currencies.Data);
            TempData["Cards"] = JsonSerializer.Serialize(cards.Data);

            return View(createTransferViewModel);

        }

        [HttpPost]

        public async Task<IActionResult> Create(CreateTransferViewModel transferDto)
        {
            //if (!this.ModelState.IsValid)
            //{
            //    return this.View(transferDto);
            //}
            var loggedUser = await FindLoggedUserAsync();

            if (!loggedUser.IsSuccessful)
            {
                return this.RedirectToAction("Login", "Account");
            }

            //transferDto.CreateTransferDto.CurrencyCode = "USD";
            //transferDto.CreateTransferDto.CardNumber = "5554567891011121";

            var result = await this.transferService.CreateAsync(transferDto.CreateTransferDto, loggedUser.Data);
            if (!result.IsSuccessful)
            {
                return await EntityNotFoundErrorViewAsync(result.Message);
            }

            return this.RedirectToAction("Confirm", "Transfer", new { id = result.Data.Id });

        }

        [HttpGet]
        public async Task<IActionResult> Update([FromRoute] int id)
        {
            var query = new CardQueryParameters();

            var loggedUser = await FindLoggedUserAsync();

            if (!loggedUser.IsSuccessful)
            {
                return this.RedirectToAction("Login", "Account");
            }
            var result = await this.transferService.GetByIdAsync(id, loggedUser.Data);

            if (!result.IsSuccessful)
            {
                return await EntityNotFoundErrorViewAsync(result.Message);
            }

            var createTransferViewModel = new CreateTransferViewModel();


            createTransferViewModel.CreateTransferDto = await TransfersMapper.MapGetDtoToCreateDto(result.Data);


            var cards = await this.cardService.FilterByAsync(query, loggedUser.Data);

            createTransferViewModel.Cards = cards.Data;

            //var currencies = this.currencyService.GetAll();

            //if (!currencies.IsSuccessful)
            //{
            //    return await EntityNotFoundErrorViewAsync(currencies.Message);
            //}

            //TempData["Currencies"] = JsonSerializer.Serialize(currencies.Data);
            //TempData["Cards"] = JsonSerializer.Serialize(cards.Data);

            return View(createTransferViewModel);

        }

        [HttpPost]
        public async Task<IActionResult> Update(int id, UpdateTransferDto transferDto)
        {
            //if (!this.ModelState.IsValid)
            //{
            //    return View(transferDto);
            //}

            var loggedUser = await FindLoggedUserAsync();
            if (!loggedUser.IsSuccessful)
            {
                return this.RedirectToAction("Login", "Account");
            }
            var result = await this.transferService.UpdateAsync(id, transferDto, loggedUser.Data);
            if (!result.IsSuccessful)
            {
                return await EntityNotFoundErrorViewAsync(result.Message);
            }

            return this.RedirectToAction("Confirm", "Transfer", new { Username = loggedUser.Data.Username });

        }

        [HttpGet]

        public async Task<IActionResult> Delete([FromRoute] int id)
        {

            var loggedUserresult = await FindLoggedUserAsync();

            if (!loggedUserresult.IsSuccessful)
            {
                return this.RedirectToAction("Login", "Account");
            }

            var transferResult = await this.transferService.GetByIdAsync(id, loggedUserresult.Data);

            if (!transferResult.IsSuccessful)
            {
                return await EntityNotFoundErrorViewAsync(transferResult.Message);
            }

            var result = await ExecuteTransferAsync(transferResult.Data);
            if (!result.IsSuccessful)
            {
                return await EntityNotFoundErrorViewAsync(result.Message);
            }

            ConfirmTransferViewModel confirmTransferViewModel = result.Data;

            return this.View(confirmTransferViewModel);
        }

        [HttpPost]

        public async Task<IActionResult> Delete([FromRoute] int id,
            ConfirmTransferViewModel confirmTransferViewModel)
        {

            var loggedUser = await FindLoggedUserAsync();

            if (!loggedUser.IsSuccessful)
            {
                return this.RedirectToAction("Login", "Account");
            }

            var result = await this.transferService.DeleteAsync(id, loggedUser.Data);

            if (!result.IsSuccessful)
            {
                return await EntityNotFoundErrorViewAsync(result.Message);
            }

            return RedirectToAction("SuccessfulCancellation", "Transfer");
        }

        [HttpGet]
        public async Task<IActionResult> SuccessfulCancellation()
        {
            return this.View();
        }

        [HttpGet]
        public async Task<IActionResult> Confirm([FromRoute] int id)
        {
            var loggedUser = await FindLoggedUserAsync();

            if (!loggedUser.IsSuccessful)
            {
                return this.RedirectToAction("Login", "Account");
            }

            var transferResult = await this.transferService.GetByIdAsync(id, loggedUser.Data);

            if (!transferResult.IsSuccessful)
            {
                return await EntityNotFoundErrorViewAsync(transferResult.Message);
            }

            var result = await ExecuteTransferAsync(transferResult.Data);

            if (!result.IsSuccessful)
            {
                return await EntityNotFoundErrorViewAsync(result.Message);
            }

            ConfirmTransferViewModel confirmTransferViewModel = result.Data;

            return this.View(confirmTransferViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Confirm([FromRoute] int id, ConfirmTransferViewModel confirmViewModel)
        {

            var loggedUser = await FindLoggedUserAsync();

            if (!loggedUser.IsSuccessful)
            {
                return this.RedirectToAction("Login", "Account");
            }

            var transferResult = await this.transferService.GetByIdAsync(id, loggedUser.Data);

            if (!transferResult.IsSuccessful)
            {
                return await EntityNotFoundErrorViewAsync(transferResult.Message);
            }

            confirmViewModel.GetTransferDto = transferResult.Data;

            var userResult = await this.userService.GetLoggedUserByUsernameAsync(transferResult.Data.Username);

            if (!userResult.IsSuccessful)
            {
                return await EntityNotFoundErrorViewAsync(userResult.Message);
            }

            confirmViewModel.UserDetails = userResult.Data;

            var result = await this.transferService.ConfirmAsync(id, loggedUser.Data);

            if (!result.IsSuccessful)
            {
                return await EntityNotFoundErrorViewAsync(result.Message);
            }

            return RedirectToAction("SuccessfulConfirmation", "Transfer", new { id = transferResult.Data.Id });

        }

        [HttpGet]
        public async Task<IActionResult> SuccessfulConfirmation([FromRoute] int id)
        {
            var loggedUser = await FindLoggedUserAsync();

            if (!loggedUser.IsSuccessful)
            {
                return this.RedirectToAction("Login", "Account");
            }
            var transferResult = await this.transferService.GetByIdAsync(id, loggedUser.Data);

            if (!transferResult.IsSuccessful)
            {
                return await EntityNotFoundErrorViewAsync(transferResult.Message);
            }
            return View(transferResult.Data);

        }

        private async Task<Response<ConfirmTransferViewModel>> ExecuteTransferAsync(GetTransferDto transferDto)
        {
            var result = new Response<ConfirmTransferViewModel>();

            ConfirmTransferViewModel transferViewModel = new ConfirmTransferViewModel();

            transferViewModel.GetTransferDto = transferDto;

            var user = await this.userService.GetLoggedUserByUsernameAsync(transferDto.Username);

            if (!user.IsSuccessful)
            {
                result.IsSuccessful = false;
                result.Message = user.Message;
                return result;
            }

            transferViewModel.UserDetails = user.Data;

            var exchangeAmount = await this.exchangeRateService.ExchangeAsync(
                transferDto.Amount,
                transferDto.Card.Currency.CurrencyCode,
                user.Data.Account.Currency.CurrencyCode);

            if (!exchangeAmount.IsSuccessful)
            {
                result.IsSuccessful = false;
                result.Message = exchangeAmount.Message;
                return result;
            }

            transferViewModel.GetTransferDto.Amount = exchangeAmount.Data;

            result.Data = transferViewModel;

            return result;
        }

        private async Task<Response<User>> FindLoggedUserAsync()
        {
            var loggedUsersUsername = User.FindFirst(ClaimTypes.Name);
            var loggedUserResult = await this.userService.GetLoggedUserByUsernameAsync(loggedUsersUsername.Value);

            return loggedUserResult;

        }

        private async Task<IActionResult> EntityNotFoundErrorViewAsync(string message)
        {
            this.HttpContext.Response.StatusCode = StatusCodes.Status404NotFound;
            this.ViewData["ErrorMessage"] = message;
            return View("Error");

        }
    }
}
