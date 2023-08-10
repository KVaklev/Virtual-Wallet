using Business.Exceptions;
using Business.QueryParameters;
using Business.Services.Contracts;
using Business.ViewModels;
using DataAccess.Models.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace VirtualWallet.Controllers.MVC
{
    [Authorize]
    public class UserController : Controller
    {
        private readonly IUserService userService;
        private readonly ICardService cardService;
        public UserController(IUserService userService, ICardService cardService)
        {
            this.userService = userService;
            this.cardService = cardService;
        }

        [HttpGet]
        public async Task<IActionResult> Index(UserQueryParameters userQueryParameters)
        {

            var result = await this.userService.FilterByAsync(userQueryParameters);

            if (!result.IsSuccessful)
            {
                this.ModelState.AddModelError(result.Error.InvalidPropertyName, result.Message);
                return View(result.Data);
            }

            var newViewModel = new UserSearchModel
            {
                Users = result,
                UserQueryParameters = userQueryParameters,
            };

            return this.View(newViewModel);
        }

        [HttpGet]
        public async Task<IActionResult> Details([FromRoute] int id)
        {
            var result = new Response<UserDetailsViewModel>();
            var loggedUserResult = await FindLoggedUserAsync();
			if (!loggedUserResult.IsSuccessful)
			{
				result.IsSuccessful = false;
				result.Message = loggedUserResult.Message;
                return (IActionResult)result.Data;
			}

            var user = await this.userService.GetByIdAsync(id, loggedUserResult.Data);
            var cardsResult = this.cardService.GetByAccountId(id);
            var userDetailsViewModel = new UserDetailsViewModel
            {
                User = user.Data,
                Cards = (!cardsResult.IsSuccessful) ? 0 : cardsResult.Data.Count()
            };

            return this.View(userDetailsViewModel);
        }

        [HttpGet]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            var result = new Response<UserDetailsViewModel>();
            var loggedUserResult = await FindLoggedUserAsync();
            if (!loggedUserResult.IsSuccessful)
            {
                result.IsSuccessful = false;
                result.Message = loggedUserResult.Message;
                return (IActionResult)result.Data;
            }

            var user = await this.userService.GetByIdAsync(id, loggedUserResult.Data);
            var cardsResult = this.cardService.GetByAccountId(id);
            var userDetailsViewModel = new UserDetailsViewModel
            {
                User = user.Data,
                Cards = (!cardsResult.IsSuccessful) ? 0 : cardsResult.Data.Count()
            };

            return this.View(userDetailsViewModel);

        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed([FromRoute] int id)
        {

            var result = new Response<UserDetailsViewModel>();
            var loggedUserResult = await FindLoggedUserAsync();
            if (!loggedUserResult.IsSuccessful)
            {
                result.IsSuccessful = false;
                result.Message = loggedUserResult.Message;
                return (IActionResult)result.Data;
            }

            var isDeleted = await this.userService.DeleteAsync(id, loggedUserResult.Data);
            if (!isDeleted.IsSuccessful)
            {
                result.IsSuccessful = false;
                result.Message = loggedUserResult.Message;
                return (IActionResult)result.Data;
            }
            return this.RedirectToAction("Index", "User");
            
        }
        private async Task<Response<User>> FindLoggedUserAsync()
		{
			var loggedUsersUsername = User.FindFirst(ClaimTypes.Name);
			var loggedUserResult = await this.userService.GetLoggedUserByUsernameAsync(loggedUsersUsername.Value);
			return loggedUserResult;
		}
	}
}