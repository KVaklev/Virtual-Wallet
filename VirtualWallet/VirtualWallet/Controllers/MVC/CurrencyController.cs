using Business.DTOs.Requests;
using Business.Services.Contracts;
using Business.ViewModels;
using DataAccess.Models.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace VirtualWallet.Controllers.MVC
{
    [AllowAnonymous]
    public class CurrencyController : Controller
    {
        private readonly ICurrencyService currencyService;
        private readonly IUserService userService;

        public CurrencyController(
            ICurrencyService currencyService,
            IUserService userService) 
        {
            this.currencyService = currencyService;
            this.userService = userService;
        }

        [HttpGet]
        public async  Task<IActionResult> Index()
        {
            var loggedUser = await userService.FindLoggedUserAsync(User.FindFirst(ClaimTypes.Name)?.Value!);
            if (!loggedUser.IsSuccessful)
            {
                return this.RedirectToAction(Constant.Action.Login, Constant.Controller.Account);
            }
            var result = await this.currencyService.GetAllAndDeletedAsync(loggedUser.Data);
            if (!result.IsSuccessful)
            {
                return View(Constant.View.ErrorMessage, result.Message);
            }
            var currencyViwModel = new CurrencyViewModel();
            currencyViwModel.Currencies = result.Data;
            currencyViwModel.User = loggedUser.Data;

            return View(currencyViwModel);
        }

        [HttpGet]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            var loggedUser = await userService.FindLoggedUserAsync(User.FindFirst(ClaimTypes.Name)?.Value!);
            if (!loggedUser.IsSuccessful)
            {
                return this.RedirectToAction(Constant.Action.Login, Constant.Controller.Account);
            }

            var result = await this.currencyService.GetCurrencyByIdAsync(id);
            if (!result.IsSuccessful)
            {
                return View(Constant.View.ErrorMessage, result.Message);
            }            
            return View(result.Data);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(Currency currency)
        {
            var loggedUser = await userService.FindLoggedUserAsync(User.FindFirst(ClaimTypes.Name)?.Value!);
            if (!loggedUser.IsSuccessful)
            {
                return this.RedirectToAction(Constant.Action.Login, Constant.Controller.Account);
            }

            var result = await this.currencyService.DeleteAsync(currency.Id, loggedUser.Data);
            if (!result.IsSuccessful)
            {
                return View(Constant.View.ErrorMessage, result.Message);
            }
            return this.RedirectToAction(Constant.Action.Index, Constant.Controller.Currency);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var loggedUser = await userService.FindLoggedUserAsync(User.FindFirst(ClaimTypes.Name)?.Value!);
            if (!loggedUser.IsSuccessful)
            {
                return this.RedirectToAction(Constant.Action.Login, Constant.Controller.Account);
            }
            var currency = new CreateCurrencyDto();

            return View(currency);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateCurrencyDto currencyDto)
        {
            var loggedUser = await userService.FindLoggedUserAsync(User.FindFirst(ClaimTypes.Name)?.Value!);
            if (!loggedUser.IsSuccessful)
            {
                return this.RedirectToAction(Constant.Action.Login, Constant.Controller.Account);
            }
            var result = await this.currencyService.CreateAsync(currencyDto, loggedUser.Data);
            if (!result.IsSuccessful)
            {
                return View(Constant.View.ErrorMessage, result.Message);
            }
            return this.RedirectToAction(Constant.Action.Index, Constant.Controller.Currency);
        }

        [HttpGet]
        public async Task<IActionResult> Update([FromRoute] int id)
        {
            var loggedUser = await userService.FindLoggedUserAsync(User.FindFirst(ClaimTypes.Name)?.Value!);
            if (!loggedUser.IsSuccessful)
            {
                return this.RedirectToAction(Constant.Action.Login, Constant.Controller.Account);
            }

            var result = await this.currencyService.UpdateAsync(id, loggedUser.Data);
            if (!result.IsSuccessful)
            {        
                return View(Constant.View.ErrorMessage, result.Message);
            }
            
            return View(Constant.View.Update,result.Message);
        }
    }
}
