using Business.DTOs.Requests;
using Business.QueryParameters;
using Business.Services.Contracts;
using Business.ViewModels.CardViewModels;
using Business.ViewModels.UserViewModels;
using DataAccess.Models.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace VirtualWallet.Controllers.MVC
{
    [AllowAnonymous]
    public class CardController : Controller
    {
        private readonly ICardService cardService;
        private readonly IUserService userService;
        public CardController(ICardService cardService,
            IUserService userService)
            
        {
            this.cardService = cardService;
            this.userService = userService;
        }
        [HttpGet]
        public async Task<IActionResult> Index(CardQueryParameters cardQueryParameters)
        {
            var loggedUserResult = await FindLoggedUserAsync();
            if (!loggedUserResult.IsSuccessful)
            {
                return RedirectToAction("Login", "Account");
            }

            var cardsResult = await this.cardService.FilterByAsync(cardQueryParameters, loggedUserResult.Data);
            if (!cardsResult.IsSuccessful)
            {
                return View("HandleErrorNotFound");
            }
            
            var cardSearchModel = new CardViewModel
            {
                Owner = loggedUserResult.Data,
                Cards = cardsResult,
                CardQueryParameters = cardQueryParameters,
            };

            return this.View(cardSearchModel);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var loggedUserResult = await FindLoggedUserAsync();
            if (!loggedUserResult.IsSuccessful)
            {
                return RedirectToAction("Login", "Account");
            }

            var cardViewModel = new CardViewModel();

            return this.View(cardViewModel);
        }
        [HttpPost]
        public async Task<IActionResult> Create(CardViewModel cardSearchViewModel)
        {
            var loggedUserResult = await FindLoggedUserAsync();
            if (!loggedUserResult.IsSuccessful)
            {
                return RedirectToAction("Login", "Account");
            }

            cardSearchViewModel.CreateCardDto.AccountUsername = loggedUserResult.Data.Username;
            cardSearchViewModel.CreateCardDto.CurrencyCode = loggedUserResult.Data.Account.Currency.CurrencyCode;

            var result = await cardService.CreateAsync(loggedUserResult.Data.Id, cardSearchViewModel.CreateCardDto);
            if (!result.IsSuccessful)
            {
                return View("HandleErrorNotFound", result.Message);
            }

            return RedirectToAction("Index", "Card");
        }

        [HttpGet]
        public async Task<IActionResult> Edit()
        {
            var loggedUserResult = await FindLoggedUserAsync();
            if (!loggedUserResult.IsSuccessful)
            {
                return RedirectToAction("Login", "Account");
            }

            var cardViewModel = new CardViewModel();

            return this.View(cardViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(CardViewModel cardViewModel)
        {
            var loggedUserResult = await FindLoggedUserAsync();
            if (!loggedUserResult.IsSuccessful)
            {
                return RedirectToAction("Login", "Account");
            }

            cardViewModel.CreateCardDto.AccountUsername = loggedUserResult.Data.Username;
            cardViewModel.CreateCardDto.CurrencyCode = loggedUserResult.Data.Account.Currency.CurrencyCode;

            var result = await cardService.UpdateAsync(loggedUserResult.Data.Id, loggedUserResult.Data, cardViewModel.UpdateCardDto);
            if (!result.IsSuccessful)
            {
                return View("HandleErrorNotFound", result.Message);
            }

            return RedirectToAction("Index", "Card");
        }
        private async Task<Response<User>> FindLoggedUserAsync()
        {
            var result = new Response<User>();
            var loggedUsersUsername = User.FindFirst(ClaimTypes.Name);
            if (loggedUsersUsername == null)
            {
                result.IsSuccessful = false;
                return result;
            }
            var loggedUserResult = await this.userService.GetLoggedUserByUsernameAsync(loggedUsersUsername.Value);
            if (loggedUserResult == null)
            {
                result.IsSuccessful = false;
                return result;
            }
            return loggedUserResult;
        }
    }
}
