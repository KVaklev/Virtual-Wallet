﻿using Business.DTOs.Requests;
using Business.DTOs.Responses;
using Business.Mappers;
using Business.QueryParameters;
using Business.Services.Contracts;
using Business.Services.Helpers;
using Business.ViewModels.TransactionViewModels;
using DataAccess.Models.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Text.Json;

namespace VirtualWallet.Controllers.MVC
{
    [AllowAnonymous]
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
            var loggedUser = await this.userService.FindLoggedUserAsync(User.FindFirst(ClaimTypes.Name)?.Value!);
            if (!loggedUser.IsSuccessful)
            {
                return this.RedirectToAction(Constant.Action.Login, Constant.Controller.Account);
            }

            var result = await this.transactionService.FilterByAsync(parameters, loggedUser.Data);
            var indexTransactionViewModel = new IndexTransactionViewModel();
            indexTransactionViewModel.TransactionQueryParameters = parameters;
            indexTransactionViewModel.User = loggedUser.Data;

            if (!result.IsSuccessful)
            {
                if (result.Message == Constants.NoRecordsFound)
                { 
                    this.ViewData[Constant.ViewData.ErrorMessage] = result.Message;
                    return View(indexTransactionViewModel);
                }
                else 
                { 
                    return View(Constant.View.ErrorMessage, result.Message);
                }
            }
            indexTransactionViewModel.TransactionDtos = result.Data;
            return View(indexTransactionViewModel);
        }

        [HttpGet]
        public async Task<IActionResult> Details([FromRoute]int id)
        {
            var loggedUser = await userService.FindLoggedUserAsync(User.FindFirst(ClaimTypes.Name)?.Value!);
            if (!loggedUser.IsSuccessful)
            {
                return this.RedirectToAction(Constant.Action.Login, Constant.Controller.Account);
            }

            var result = await this.transactionService.GetByIdAsync(id, loggedUser.Data);
            if (!result.IsSuccessful)
            {
                return View(Constant.View.ErrorMessage, result.Message);
            }

            var detailsTransactionsViewModel = new DetailsTransactionsViewModel();
            detailsTransactionsViewModel.GetTransactionDto = result.Data;

            var senderUserResult = await this.userService.GetLoggedUserByUsernameAsync(result.Data.SenderUsername);
            if (!senderUserResult.IsSuccessful)
            {
                return View(Constant.View.ErrorMessage, result.Message);
            }

            var recipientUserResult = await this.userService.GetLoggedUserByUsernameAsync(result.Data.RecipientUsername);
            if (!recipientUserResult.IsSuccessful)
            {
                return View(Constant.View.ErrorMessage, result.Message);
            }

            detailsTransactionsViewModel.SenderUser = senderUserResult.Data;
            detailsTransactionsViewModel.RecipientUser = recipientUserResult.Data;
            detailsTransactionsViewModel.LoggedUser = loggedUser.Data;

            return this.View(detailsTransactionsViewModel);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var loggedUser = await userService.FindLoggedUserAsync(User.FindFirst(ClaimTypes.Name)?.Value!);
            if (!loggedUser.IsSuccessful)
            {
                return this.RedirectToAction(Constant.Action.Login, Constant.Controller.Account);
            }

            var createTransactionViewModel = new CreateTransactionViewModel();
            var result = await this.currencyService.GetAllAsync();
            if (!result.IsSuccessful)
            {
                return View(Constant.View.ErrorMessage, result.Message);
            }

            TempData[Constant.TempData.Currencies] = JsonSerializer.Serialize(result.Data);
            return this.View(createTransactionViewModel);
        }

        [HttpGet]
        public async Task<IActionResult> CreateWithUsername(string username)
        {
            var loggedUser = await userService.FindLoggedUserAsync(User.FindFirst(ClaimTypes.Name)?.Value!);
            if (!loggedUser.IsSuccessful)
            {
                return this.RedirectToAction(Constant.Action.Login, Constant.Controller.Account);
            }

            var createTransactionViewModel = new CreateTransactionViewModel();
            createTransactionViewModel.CreateTransactionDto = new CreateTransactionDto()
            {
                RecipientUsername = username
            };

            var result = await this.currencyService.GetAllAsync();
            if (!result.IsSuccessful)
            { 
                return View(Constant.View.ErrorMessage, result.Message);
            }

            TempData[Constant.TempData.Currencies] = JsonSerializer.Serialize(result.Data);
            return  View(Constant.View.Create, createTransactionViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateTransactionViewModel transactionDto)
        {
            if (!this.ModelState.IsValid)
            {
                return this.View(transactionDto);
            }
            
            var loggedUserResult = await userService.FindLoggedUserAsync(User.FindFirst(ClaimTypes.Name)?.Value!);
            if (!loggedUserResult.IsSuccessful)
            {
                return this.RedirectToAction(Constant.Action.Login, Constant.Controller.Account);
            }
            
            var result = await this.transactionService
                            .CreateOutTransactionAsync(transactionDto.CreateTransactionDto, loggedUserResult.Data);
            if (!result.IsSuccessful)
            {
                return View(Constant.View.ErrorMessage, result.Message);
            }
            
            return this.RedirectToAction(Constant.Action.Confirm, Constant.Controller.Transaction, new { id = result.Data.Id });
        }

        [HttpGet]
        public async Task<IActionResult> Update([FromRoute] int id)
        {
            var loggedUser = await userService.FindLoggedUserAsync(User.FindFirst(ClaimTypes.Name)?.Value!);
            if (!loggedUser.IsSuccessful)
            {
                return this.RedirectToAction("Login", "Account");
            }

            var transactionResult = await this.transactionService.GetByIdAsync(id, loggedUser.Data);
            if (!transactionResult.IsSuccessful)
            {
                return View(Constant.View.ErrorMessage, transactionResult.Message);
            }

            var createTransactionViewModel = new CreateTransactionViewModel();
            createTransactionViewModel.CreateTransactionDto = await TransactionsMapper
                                      .MapGetDtoToCreateDto(transactionResult.Data);

            var currencyResult = await this.currencyService.GetAllAsync();
            if (!currencyResult.IsSuccessful)
            {
                return View(Constant.View.ErrorMessage, transactionResult.Message);
            }
            TempData[Constant.TempData.Currencies] = JsonSerializer.Serialize(currencyResult.Data);
            
            return this.View(createTransactionViewModel); 
        }

        [HttpPost]
        public async Task<IActionResult> Update(
            [FromRoute] int id, 
            CreateTransactionViewModel createTransactionViewModel)
        { 
                if (!this.ModelState.IsValid)
                {
                    return View(createTransactionViewModel);
                }

                var loggedUserResult = await userService.FindLoggedUserAsync(User.FindFirst(ClaimTypes.Name)?.Value!);
            if (!loggedUserResult.IsSuccessful)
                {
                    return this.RedirectToAction(Constant.Action.Login, Constant.Controller.Account);
                }

                var result = await this.transactionService.UpdateAsync(
                            id, 
                            loggedUserResult.Data, 
                            createTransactionViewModel.CreateTransactionDto);
                if (!result.IsSuccessful)
                {
                    return View(Constant.View.ErrorMessage, result.Message);
                }
               
            return this.RedirectToAction(Constant.Action.Confirm, Constant.Controller.Transaction, new { id =  result.Data.Id});
        }

        [HttpGet]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {  
                var loggedUserResult = await userService.FindLoggedUserAsync(User.FindFirst(ClaimTypes.Name)?.Value!);
                if (!loggedUserResult.IsSuccessful)
                {
                    return this.RedirectToAction(Constant.Action.Login, Constant.Controller.Account);
                }

                var transactionResult = await this.transactionService.GetByIdAsync(id, loggedUserResult.Data);
                if (!transactionResult.IsSuccessful)
                {
                    return View(Constant.View.ErrorMessage, transactionResult.Message);
                }

                var result = await InitializedExecuteTransactionViewModelAsync(transactionResult.Data);
                if (!result.IsSuccessful)
                {
                    return View(Constant.View.ErrorMessage, result.Message);
                }
                ConfirmTransactionViewModel confirmTransactionViewModel=result.Data;
           
            return this.View(confirmTransactionViewModel);
        }

        [HttpPost]
        public async Task<IActionResult>Delete(
            [FromRoute] int id, 
            ConfirmTransactionViewModel executeTransactionViewModel)
        {
             var loggedUserResult = await userService.FindLoggedUserAsync(User.FindFirst(ClaimTypes.Name)?.Value!);
            if (!loggedUserResult.IsSuccessful)
             {
                return this.RedirectToAction(Constant.Action.Login, Constant.Controller.Account);
             }

             var result = await this.transactionService.DeleteAsync(id, loggedUserResult.Data);
             if (!result.IsSuccessful)
             {
                return View(Constant.View.ErrorMessage, result.Message);
             }

            return View(Constant.View.SuccessfulDelete, result.Message); ;
        }
        

        [HttpGet]
        public async Task<IActionResult> Confirm([FromRoute] int id)
        {
            var loggedUserResult = await userService.FindLoggedUserAsync(User.FindFirst(ClaimTypes.Name)?.Value!);
            if (!loggedUserResult.IsSuccessful)
            {
                return this.RedirectToAction(Constant.Action.Login, Constant.Controller.Account);
            }

            var transactionResult = await this.transactionService.GetByIdAsync(id, loggedUserResult.Data);
            if (!transactionResult.IsSuccessful)
            {
                return View(Constant.View.ErrorMessage, transactionResult.Message);
            }

            var result = await InitializedExecuteTransactionViewModelAsync(transactionResult.Data);
            if (!result.IsSuccessful)
            {
                return View(Constant.View.ErrorMessage, result.Message);
            }
            ConfirmTransactionViewModel confirmTransactionViewModel = result.Data;

            return this.View(confirmTransactionViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Confirm(
            [FromRoute] int id,
            ConfirmTransactionViewModel executeTransactionViewModel)
        {
            var loggedUserResult = await userService.FindLoggedUserAsync(User.FindFirst(ClaimTypes.Name)?.Value!);
            if (!loggedUserResult.IsSuccessful)
            {
                return this.RedirectToAction(Constant.Action.Login, Constant.Controller.Account);
            }

            var transactionResult = await this.transactionService.GetByIdAsync(id, loggedUserResult.Data);
            if (!transactionResult.IsSuccessful)
            {
                return View(Constant.View.ErrorMessage, transactionResult.Message);
            }
            executeTransactionViewModel.GetTransactionDto = transactionResult.Data;

            var userResult = await this.userService.GetLoggedUserByUsernameAsync(transactionResult.Data.RecipientUsername);
            if (!userResult.IsSuccessful)
            {
                return View(Constant.View.ErrorMessage, userResult.Message);
            }
            executeTransactionViewModel.Recipient = userResult.Data;

            var result = await this.transactionService.ConfirmAsync(id, loggedUserResult.Data);
            if (!result.IsSuccessful)
            {
                return View(Constant.View.ErrorMessage, result.Message);
            }

            return RedirectToAction(Constant.Action.SuccessfulConfirm, Constant.Controller.Transaction, new { id = transactionResult.Data.Id });
        }

        [HttpGet]
        public async Task<IActionResult> SuccessfulConfirm([FromRoute] int id)
        {
            var loggedUserResult = await userService.FindLoggedUserAsync(User.FindFirst(ClaimTypes.Name)?.Value!);       
            if (!loggedUserResult.IsSuccessful)
            {
                return this.RedirectToAction(Constant.Action.Login, Constant.Controller.Account);
            }

            var transactionResult = await this.transactionService.GetByIdAsync(id, loggedUserResult.Data);
            if (!transactionResult.IsSuccessful)
            {
                return View(Constant.View.ErrorMessage, transactionResult.Message);
            }

            return this.View(transactionResult.Data);
        }

        private async Task<Response<ConfirmTransactionViewModel>> InitializedExecuteTransactionViewModelAsync(GetTransactionDto transaction)
        {
            var result = new Response<ConfirmTransactionViewModel>();
            var executeTransactionViewModel = new ConfirmTransactionViewModel();
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
