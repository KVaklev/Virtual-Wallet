using Business.DTOs.Requests;
using Business.QueryParameters;
using Business.Services.Contracts;
using Business.Services.Helpers;
using Business.ViewModels;
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

            var cardSearchModel = new CardViewModel
            {
                Owner = loggedUserResult.Data,
                CardQueryParameters = cardQueryParameters,
            };

            var cardsResult = await this.cardService.FilterByAsync(cardQueryParameters, loggedUserResult.Data);   
            if (!cardsResult.IsSuccessful)
            {
                if (cardsResult.Message==Constants.NoRecordsFoundByFilter)
                {
                    this.ViewData["ErrorMessage"] = cardsResult.Message;
                    return View(cardSearchModel);
                }
                else
                {
                    this.ViewData["Controller"] = "Card";
                    return View("ErrorMessage", cardsResult.Message);
                }  
            }
            cardSearchModel.Cards = cardsResult;
           
            return this.View(cardSearchModel);
        }

        [HttpGet]
        public async Task<IActionResult> Details(CardQueryParameters cardQueryParameters)
        {
            var loggedUserResult = await FindLoggedUserAsync();
            if (!loggedUserResult.IsSuccessful)
            {
                return RedirectToAction("Login", "Account");
            }

            var cardsResult = await this.cardService.FilterByAsync(cardQueryParameters, loggedUserResult.Data);
            if (!cardsResult.IsSuccessful)
            {
                this.ViewData["Controller"] = "Card";
                return View("ErrorMessage", cardsResult.Message);
            }

            var cardsAllUsersViewModel = new CardsAllUsersViewModel
            {
                Cards = cardsResult,
                CardQueryParameters = cardQueryParameters,
            };

            return this.View(cardsAllUsersViewModel);
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
                this.ViewData["Controller"] = "Card";
                return View("ErrorMessage", result.Message);
            }

            return RedirectToAction("Index", "Card");
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var loggedUserResult = await FindLoggedUserAsync();
            if (!loggedUserResult.IsSuccessful)
            {
                return RedirectToAction("Login", "Account");
            }

            var getCardDtoResult = await this.cardService.GetByIdAsync(id, loggedUserResult.Data);
            if (!getCardDtoResult.IsSuccessful)
            {
                this.ViewData["Controller"] = "Card";
                return View("ErrorMessage", getCardDtoResult.Message);
            }
            var cardViewModel = new CardViewModel()
            {
                GetCardDto=getCardDtoResult.Data,
                UpdateCardDto = new UpdateCardDto(),
            };
            
            return this.View(cardViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, CardViewModel cardViewModel)
        {
            var loggedUserResult = await FindLoggedUserAsync();
            if (!loggedUserResult.IsSuccessful)
            {
                return RedirectToAction("Login", "Account");
            }          

            var result = await cardService.UpdateAsync(id, loggedUserResult.Data, cardViewModel.UpdateCardDto);
            if (!result.IsSuccessful)
            {
                this.ViewData["Controller"] = "Card";
                return View("ErrorMessage", result.Message);
            }

            return RedirectToAction("Index", "Card");
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var loggedUserResult = await FindLoggedUserAsync();
            if (!loggedUserResult.IsSuccessful)
            {
                return RedirectToAction("Login", "Account");
            }

            var cardResult = await this.cardService.GetByIdAsync(id, loggedUserResult.Data);
            if (!cardResult.IsSuccessful)
            {
                this.ViewData["Controller"] = "Card";
                return View("ErrorMessage", cardResult.Message);
            }

            var cardViewModel = new CardViewModel();

            return this.View(cardViewModel);
        }

        [HttpPost,ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var loggedUserResult = await FindLoggedUserAsync();
            if (!loggedUserResult.IsSuccessful)
            {
                return RedirectToAction("Login", "Account");
            }

            var cardResult = await this.cardService.GetByIdAsync(id, loggedUserResult.Data);
            if (!cardResult.IsSuccessful)
            {
                this.ViewData["Controller"] = "Card";
                return View("ErrorMessage", cardResult.Message);
            }

            var result = await this.cardService.DeleteAsync(id, loggedUserResult.Data);

            if (!result.IsSuccessful)
            {
                this.ViewData["Controller"] = "Card";
                return View("ErrorMessage", result.Message);
            }

            return View("SuccessfulDelete");
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
