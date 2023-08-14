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
            
            var cardSearchModel = new CardSearchViewModel
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
            var loggedUser = await FindLoggedUserAsync();
            if (!loggedUser.IsSuccessful)
            {
                return this.RedirectToAction("Login", "Account");
            }

            var createCardViewModel = new CreateCardViewModel();

            return this.View(createCardViewModel);
        }
        [HttpPost]
        public async Task<IActionResult> Create(CreateCardViewModel createCardViewModel)
        {
            var loggedUserResult = await FindLoggedUserAsync();
            if (!loggedUserResult.IsSuccessful)
            {
                return RedirectToAction("Login", "Account");
            }

            createCardViewModel.CreateCardDto.AccountUsername = loggedUserResult.Data.Username;
            createCardViewModel.CreateCardDto.CurrencyCode = loggedUserResult.Data.Account.Currency.CurrencyCode;

            var result = await cardService.CreateAsync(loggedUserResult.Data.Id, createCardViewModel.CreateCardDto);
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
