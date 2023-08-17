using Business.DTOs.Requests;
using Business.QueryParameters;
using Business.Services.Contracts;
using Business.Services.Helpers;
using Business.ViewModels.CardViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Text.Json;

namespace VirtualWallet.Controllers.MVC
{
    [AllowAnonymous]
    public class CardController : Controller
    {
        private readonly ICardService cardService;
        private readonly IUserService userService;
        private readonly ICurrencyService currencyService;
        
        public CardController(
            ICardService cardService,
            IUserService userService,
            ICurrencyService currencyService)
            
        {
            this.cardService = cardService;
            this.userService = userService;
            this.currencyService = currencyService;
        }

        [HttpGet]
        public async Task<IActionResult> Index(CardQueryParameters cardQueryParameters)
        {
            var loggedUserResult = await userService.FindLoggedUserAsync(User.FindFirst(ClaimTypes.Name)?.Value!);
            if (!loggedUserResult.IsSuccessful)
            {
                return RedirectToAction(Constant.Action.Login, Constant.Controller.Account);
            }

            var currencyResult = await this.currencyService.GetAllAsync();
            if (!currencyResult.IsSuccessful)
            {
                return View(Constant.View.ErrorMessage, currencyResult.Message);
            }
            TempData[Constant.TempData.Currencies] = JsonSerializer.Serialize(currencyResult.Data);

            var cardSearchModel = new CardViewModel
            {
                Owner = loggedUserResult.Data,
                CardQueryParameters = cardQueryParameters,
            };

            var cardsResult = await this.cardService.FilterByAsync(cardQueryParameters, loggedUserResult.Data);   
            if (!cardsResult.IsSuccessful)
            {
                if (cardsResult.Message==Constants.NoRecordsFound)
                {
                    this.ViewData[Constant.View.ErrorMessage] = cardsResult.Message;
                    return View(cardSearchModel);
                }
                else
                {
                    return View(Constant.View.ErrorMessage, cardsResult.Message);
                }  
            }
            cardSearchModel.Cards = cardsResult;
           
            return this.View(cardSearchModel);
        }

        [HttpGet]
        public async Task<IActionResult> Details(CardQueryParameters cardQueryParameters)
        {
            var loggedUserResult = await userService.FindLoggedUserAsync(User.FindFirst(ClaimTypes.Name)?.Value!);
            if (!loggedUserResult.IsSuccessful)
            {
                return RedirectToAction(Constant.Action.Login, Constant.Controller.Account);
            }

            var cardsResult = await this.cardService.FilterByAsync(cardQueryParameters, loggedUserResult.Data);
            if (!cardsResult.IsSuccessful)
            {
                return View(Constant.View.ErrorMessage, cardsResult.Message);
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
            var loggedUserResult = await userService.FindLoggedUserAsync(User.FindFirst(ClaimTypes.Name)?.Value!);
            if (!loggedUserResult.IsSuccessful)
            {
                return RedirectToAction(Constant.Action.Login, Constant.Controller.Account);
            }
            var cardViewModel = new CardViewModel();

            return this.View(cardViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CardViewModel cardSearchViewModel)
        {
            var loggedUserResult = await userService.FindLoggedUserAsync(User.FindFirst(ClaimTypes.Name)?.Value!);
            if (!loggedUserResult.IsSuccessful)
            {
                return RedirectToAction(Constant.Action.Login, Constant.Controller.Account);
            }

            cardSearchViewModel.CreateCardDto.AccountUsername = loggedUserResult.Data.Username;

            var result = await cardService.CreateAsync(loggedUserResult.Data.Id, cardSearchViewModel.CreateCardDto);
            if (!result.IsSuccessful)
            {        
                return View(Constant.View.ErrorMessage, result.Message);
            }

            return RedirectToAction(Constant.Action.Index, Constant.Controller.Card);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var loggedUserResult = await userService.FindLoggedUserAsync(User.FindFirst(ClaimTypes.Name)?.Value!);
            if (!loggedUserResult.IsSuccessful)
            {
                return RedirectToAction(Constant.Action.Login, Constant.Controller.Account);
            }

            var getCardDtoResult = await this.cardService.GetByIdAsync(id, loggedUserResult.Data);
            if (!getCardDtoResult.IsSuccessful)
            {
                return View(Constant.View.ErrorMessage, getCardDtoResult.Message);
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
            var loggedUserResult = await userService.FindLoggedUserAsync(User.FindFirst(ClaimTypes.Name)?.Value!);
            if (!loggedUserResult.IsSuccessful)
            {
                return RedirectToAction(Constant.Action.Login, Constant.Controller.Account);
            }          

            var result = await cardService.UpdateAsync(id, loggedUserResult.Data, cardViewModel.UpdateCardDto);
            if (!result.IsSuccessful)
            {
                return View(Constant.View.ErrorMessage, result.Message);
            }

            return RedirectToAction(Constant.Action.Index, Constant.Controller.Card);
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var loggedUserResult = await userService.FindLoggedUserAsync(User.FindFirst(ClaimTypes.Name)?.Value!);
            if (!loggedUserResult.IsSuccessful)
            {
                return RedirectToAction(Constant.Action.Login, Constant.Controller.Account);
            }

            var cardResult = await this.cardService.GetByIdAsync(id, loggedUserResult.Data);
            if (!cardResult.IsSuccessful)
            {
                return View(Constant.View.ErrorMessage, cardResult.Message);
            }

            var cardViewModel = new CardViewModel();

            return this.View(cardViewModel);
        }

        [HttpPost,ActionName(Constant.Action.Delete)]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var loggedUserResult = await userService.FindLoggedUserAsync(User.FindFirst(ClaimTypes.Name)?.Value!);
            if (!loggedUserResult.IsSuccessful)
            {
                return RedirectToAction(Constant.Action.Login, Constant.Controller.Account);
            }

            var cardResult = await this.cardService.GetByIdAsync(id, loggedUserResult.Data);
            if (!cardResult.IsSuccessful)
            {
                return View(Constant.View.ErrorMessage, cardResult.Message);
            }

            var result = await this.cardService.DeleteAsync(id, loggedUserResult.Data);

            if (!result.IsSuccessful)
            {
                return View(Constant.View.ErrorMessage, result.Message);
            }

            return View(Constant.View.SuccessfulDelete);
        }
    }
}
