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
        private readonly IAccountService accountService;
        public CardController(ICardService cardService,
            IUserService userService,
            IAccountService accountService)
        {
            this.cardService = cardService;
            this.userService = userService;
            this.accountService = accountService;
        }
        [HttpGet]
        public async Task<IActionResult> Index(CardQueryParameters cardQueryParameters)
        {
            var loggedUserResult = await FindLoggedUserAsync();
            if (!loggedUserResult.IsSuccessful)
            {
                return RedirectToAction("Login", "Account");
            }

            //var userResult = await this.userService.GetByIdAsync(loggedUserResult.Data.Id, loggedUserResult.Data);
            //if (!userResult.IsSuccessful)
            //{
            //    return View("HandleErrorNotFound", userResult.Message);
            //}

            //var userAccount = await this.accountService.GetByUsernameAsync(loggedUserResult.Data.Id, loggedUserResult.Data);

            var cardsResult = await this.cardService.FilterByAsync(cardQueryParameters, loggedUserResult.Data);
            
            if (!cardsResult.IsSuccessful)
            {
                return View();
            }
            
            var cardSearchModel = new CardSearchViewModel
            {
                Owner = loggedUserResult.Data,
                Cards = cardsResult,
                CardQueryParameters = cardQueryParameters,
            };

            return this.View(cardSearchModel);
        }
        public IActionResult PaymentMethods()
        {
            return View();
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
