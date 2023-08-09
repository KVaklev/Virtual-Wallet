using Business.DTOs.Requests;
using Business.QueryParameters;
using Business.Services.Contracts;
using Business.ViewModels;
using DataAccess.Models.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace VirtualWallet.Controllers.MVC
{
    [Authorize]
	//[Route("users")]
	public class UserController : Controller
    {
        private readonly IUserService userService;
        public UserController(IUserService userService)
        {
            this.userService = userService;
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
                UserQueryParameters = userQueryParameters
            };

            return this.View(newViewModel);
        }


        [HttpGet]
        public async Task<IActionResult> ChangeStatus()
        {
            var loggedUserResponse = await GetLoggedUserAsync();
            if (!loggedUserResponse.IsSuccessful)
            {
                return StatusCode(StatusCodes.Status404NotFound, loggedUserResponse.Message);
            }

            var newViewModel = new UserChangeStatusViewModel();

            return this.View(newViewModel);
        }

        [HttpPost, ActionName("ChangeStatus")]
        [HttpPost]
        public async Task<IActionResult> ChangeStatusConfirmed(int id, UserChangeStatusViewModel userChangeStatusViewModel)
        {
            var loggedUserResponse = await GetLoggedUserAsync();
            if (!loggedUserResponse.IsSuccessful)
            {
                return StatusCode(StatusCodes.Status404NotFound, loggedUserResponse.Message);
            }

            await userService.ChangeStatusAsync(id, loggedUserResponse.Data, userChangeStatusViewModel);

            return this.RedirectToAction("Index", "Users");
        }


        private async Task<Response<User>> GetLoggedUserAsync()
        {
            var loggedUsersUsername = User.FindFirst(ClaimTypes.Name);
            var loggedUserResult = await this.userService.GetLoggedUserByUsernameAsync(loggedUsersUsername.Value);

            return loggedUserResult;
        }
    }
}
