﻿using AutoMapper;
using Business.DTOs.Responses;
using Business.Mappers;
using Business.QueryParameters;
using Business.Services.Contracts;
using Business.ViewModels;
using DataAccess.Models.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Text.Json;

namespace VirtualWallet.Controllers.MVC
{
    [Authorize]
    public class TransactionController : Controller
    {
        private readonly ITransactionService transactionService;
        private readonly IUserService userService;
        private readonly ICurrencyService currencyService;
        private readonly IExchangeRateService exchangeRateService;
        private readonly IMapper mapper;

        public TransactionController(
            ITransactionService transactionService,
            IUserService userService,
            ICurrencyService currencyService,
            IExchangeRateService exchangeRateService,
            IMapper mapper)
        {
            this.transactionService = transactionService;
            this.userService = userService;
            this.currencyService = currencyService;
            this.exchangeRateService = exchangeRateService;
            this.mapper = mapper;
        }


        [HttpGet]

        public async Task<IActionResult> Index([FromQuery] TransactionQueryParameters parameters)
        {
            
                var loggedUser = await FindLoggedUserAsync();
                if (!loggedUser.IsSuccessful)
                {
                return this.RedirectToAction("Login", "Account");
            }


            var result = await this.transactionService.FilterByAsync(parameters, loggedUser.Data);
            if (!result.IsSuccessful)
            {
                return await EntityErrorViewAsync(result.Message);
            }
            var indexTransactionViewModel = new IndexTransactionViewModel();
            indexTransactionViewModel.TransactionDtos = result.Data;
            indexTransactionViewModel.TransactionQueryParameters = parameters;
            indexTransactionViewModel.User = loggedUser.Data;
            return View(indexTransactionViewModel);
        }

        [HttpGet]
        public async Task<IActionResult> Details([FromRoute]int id)
        {
            var loggedUser = await FindLoggedUserAsync();
            if (!loggedUser.IsSuccessful)
            {
                return this.RedirectToAction("Login", "Account");
            }

            
            var result = await this.transactionService.GetByIdAsync(id, loggedUser.Data);
            if (!result.IsSuccessful)
            {
                return await EntityErrorViewAsync(result.Message);
            }

            var detailsTransactionsViewModel = new DetailsTransactionsViewModel();
            detailsTransactionsViewModel.GetTransactionDto = result.Data;

            var senderUserResult = await this.userService.GetLoggedUserByUsernameAsync(result.Data.SenderUsername);
            if (!senderUserResult.IsSuccessful)
            {
                return await EntityErrorViewAsync(result.Message);
            }

            var recipientUserResult = await this.userService.GetLoggedUserByUsernameAsync(result.Data.RecipientUsername);
            if (!recipientUserResult.IsSuccessful)
            {
                return await EntityErrorViewAsync(result.Message);
            }

            detailsTransactionsViewModel.SenderUser = senderUserResult.Data;
            detailsTransactionsViewModel.RecipientUser = recipientUserResult.Data;
            detailsTransactionsViewModel.LoggedUser = loggedUser.Data;

            return this.View(detailsTransactionsViewModel);
        }


        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var loggedUser = await FindLoggedUserAsync();
            if (!loggedUser.IsSuccessful)
            {
                return this.RedirectToAction("Login", "Account");
            }

            var createTransactionViewModel = new CreateTransactionViewModel();
            var result = this.currencyService.GetAll();
            if (!result.IsSuccessful)
            {
                return await EntityErrorViewAsync(result.Message);
            }

            TempData["Currencies"] = JsonSerializer.Serialize(result.Data);
            return this.View(createTransactionViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateTransactionViewModel transactionDto)
        {
            if (!this.ModelState.IsValid)
            {
                return this.View(transactionDto);
            }
            var loggedUserResult = await FindLoggedUserAsync();
                if (!loggedUserResult.IsSuccessful)
                {
                     return this.RedirectToAction("Login", "Account");
                }
                var result = await this.transactionService.CreateOutTransactionAsync(transactionDto.CreateTransactionDto, loggedUserResult.Data);
                if (!result.IsSuccessful)
                {
                    return await EntityErrorViewAsync(result.Message);
                }
                return this.RedirectToAction("Confirm", "Transaction", new { id = result.Data.Id });
        }

        [HttpGet]
        public async Task<IActionResult> Update([FromRoute] int id)
        {
            var loggedUserResult = await FindLoggedUserAsync();
            if (!loggedUserResult.IsSuccessful)
            {
                return this.RedirectToAction("Login", "Account");
            }

            var transactionResult = await this.transactionService.GetByIdAsync(id, loggedUserResult.Data);

            if (!transactionResult.IsSuccessful)
            {
                return await EntityErrorViewAsync(transactionResult.Message);
            }

            var createTransactionViewModel = new CreateTransactionViewModel();
            createTransactionViewModel.CreateTransactionDto = await TransactionsMapper
                            .MapGetDtoToCreateDto(transactionResult.Data);

            var currencyResult = this.currencyService.GetAll();
            if (!currencyResult.IsSuccessful)
            {
                return await EntityErrorViewAsync(currencyResult.Message);
            }
            TempData["Currencies"] = JsonSerializer.Serialize(currencyResult.Data);
            return this.View(createTransactionViewModel); 
        }

        [HttpPost]
        public async Task<IActionResult> Update([FromRoute] int id, CreateTransactionViewModel createTransactionViewModel)
        {
            
                if (!this.ModelState.IsValid)
                {
                    return View(createTransactionViewModel);
                }

                var loggedUserResult = await FindLoggedUserAsync();
                if (!loggedUserResult.IsSuccessful)
                {
                return this.RedirectToAction("Login", "Account");
            }

                var result = await this.transactionService.UpdateAsync(
                            id, 
                            loggedUserResult.Data, 
                            createTransactionViewModel.CreateTransactionDto);
                if (!result.IsSuccessful)
                {
                    return await EntityErrorViewAsync(result.Message);
                }

                return this.RedirectToAction(
                            "Confirm", 
                            "Transaction", 
                            new { id =  result.Data.Id});
            
        }

        [HttpGet]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            
                var loggedUserResult = await FindLoggedUserAsync();
                if (!loggedUserResult.IsSuccessful)
                {
                return this.RedirectToAction("Login", "Account");
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
            ConfirmTransactionViewModel confirmTransactionViewModel=result.Data;
            return this.View(confirmTransactionViewModel);
        }

        [HttpPost]
        public async Task<IActionResult>Delete(
            [FromRoute] int id, 
            ConfirmTransactionViewModel executeTransactionViewModel)
        {
            
                var loggedUserResult = await FindLoggedUserAsync();
            if (!loggedUserResult.IsSuccessful)
            {
                return this.RedirectToAction("Login", "Account");
            }
                var result = await this.transactionService.DeleteAsync(id, loggedUserResult.Data);
                if (!result.IsSuccessful)
                {
                    return await EntityErrorViewAsync(result.Message);
                }

            return this.RedirectToAction("SuccessfulDelete", "Transaction");

        }
        [HttpGet]
        public async Task<IActionResult> SuccessfulDelete()
        {
            return this.View();
        }

        [HttpGet]
        public async Task<IActionResult> Confirm([FromRoute] int id)
        {
            var loggedUserResult = await FindLoggedUserAsync();
            if (!loggedUserResult.IsSuccessful)
            {
                return this.RedirectToAction("Login", "Account");
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
            ConfirmTransactionViewModel confirmTransactionViewModel = result.Data;

            return this.View(confirmTransactionViewModel);

        }

        [HttpPost]
        public async Task<IActionResult> Confirm(
            [FromRoute] int id,
            ConfirmTransactionViewModel executeTransactionViewModel)
        {
            var loggedUserResult = await FindLoggedUserAsync();
            if (!loggedUserResult.IsSuccessful)
            {
                return this.RedirectToAction("Login", "Account");
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

            var result = await this.transactionService.ConfirmAsync(id, loggedUserResult.Data);
            if (!result.IsSuccessful)
            {
                return await EntityErrorViewAsync(result.Message);
            }

            return RedirectToAction("SuccessfulConfirm", "Transaction", new { id = transactionResult.Data.Id });
        }

        [HttpGet]
        public async Task<IActionResult> SuccessfulConfirm([FromRoute] int id)
        {
            var loggedUserResult = await FindLoggedUserAsync();
            if (!loggedUserResult.IsSuccessful)
            {
                return this.RedirectToAction("Login", "Account");
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

            return this.View("Error");
        }

        private IActionResult BlockedErrorView()
        {
            this.HttpContext.Response.StatusCode = StatusCodes.Status401Unauthorized;
            this.ViewData["ErrorMessage"] = "You are a \"BLOCKED USER\"!";
            return this.View("UnauthorizedError");
        }


        private async Task<Response<User>> FindLoggedUserAsync()
        {
            var loggedUsersUsername = User.FindFirst(ClaimTypes.Name);
            var loggedUserResult = await this.userService.GetLoggedUserByUsernameAsync(loggedUsersUsername.Value);

            return loggedUserResult;
        }

        private async Task<Response<ConfirmTransactionViewModel>> InitializedExecuteTransactionViewModelAsync(
            GetTransactionDto transaction)
        {
            var result = new Response<ConfirmTransactionViewModel>();

            ConfirmTransactionViewModel executeTransactionViewModel = new ConfirmTransactionViewModel();

            executeTransactionViewModel.GetTransactionDto = transaction;

            var userResult = await this.userService.GetLoggedUserByUsernameAsync(transaction.RecipientUsername);
            if (!userResult.IsSuccessful)
            {
                result.IsSuccessful = false;
                result.Message = userResult.Message;
                return result;
            }
            executeTransactionViewModel.Recipient = userResult.Data;

            var exchngeAmount = await this.exchangeRateService.ExchangeAsync(
                transaction.Amount,
                transaction.CurrencyCode,
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
