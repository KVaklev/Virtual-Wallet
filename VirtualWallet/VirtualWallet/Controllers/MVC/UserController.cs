using Business.QueryParameters;
using Business.Services.Contracts;
using Business.ViewModels.UserViewModels;
using DataAccess.Models.Models;
using Microsoft.AspNetCore.Authorization;
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
                //this.ModelState.AddModelError(result.Error.InvalidPropertyName, result.Message);
                return View("HandleErrorNotFound");
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
           // var result = new Response<UserDetailsViewModel>();
           
            var loggedUserResult = await FindLoggedUserAsync();
			if (!loggedUserResult.IsSuccessful)
			{
                return View("Error", loggedUserResult.Message);
			}

            var userResult = await this.userService.GetByIdAsync(id, loggedUserResult.Data);
            if (!userResult.IsSuccessful)
            {
                return View("HandleErrorNotFound", userResult.Message);
            }

            var cardsResult = this.cardService.GetByAccountId(id);
            if (!cardsResult.IsSuccessful)
            {
                return View("HandleErrorNotFound", cardsResult.Message);
            }

            var userDetailsViewModel = new UserDetailsViewModel
            {
                User = userResult.Data,
                Cards = (!cardsResult.IsSuccessful) ? 0 : cardsResult.Data.Count()
            };

            return this.View(userDetailsViewModel);
        }

        [HttpGet]
        public async Task<IActionResult> Edit([FromRoute] int id)
        {
            //var result = new Response<UserDetailsViewModel>();

            var loggedUserResult = await FindLoggedUserAsync();
            if (!loggedUserResult.IsSuccessful)
            {
                return View("HandleErrorNotLoggedIn", loggedUserResult.Message);
            }

            var userResult = await this.userService.GetByIdAsync(id, loggedUserResult.Data);
            if (!userResult.IsSuccessful)
            {
                return View("HandleErrorNotFound", userResult.Message);
            }

            var userDetailsViewModel = new UserDetailsViewModel {  User = userResult.Data  };

            return this.View(userDetailsViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Edit([FromRoute] int id, UserDetailsViewModel userDetailsViewModel)
        {
           // var result = new Response<UserDetailsViewModel>();
           
            var loggedUserResult = await FindLoggedUserAsync();
            if (!loggedUserResult.IsSuccessful)
            {
                return View("HandleErrorNotLoggedIn", loggedUserResult.Message);
            }

            var isChanged = await this.userService.ChangeStatusAsync(id, userDetailsViewModel, loggedUserResult.Data);
            if (!isChanged.IsSuccessful)
            {
                return View("HandleErrorInvalidOperation", isChanged.Message);
            }

            return this.RedirectToAction("Index", "User");  
        }

        [HttpGet]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            //var result = new Response<UserDetailsViewModel>();      
            var loggedUserResult = await FindLoggedUserAsync();
            if (!loggedUserResult.IsSuccessful)
            {
                return View("HandleErrorNotLoggedIn", loggedUserResult.Message);
            }

            var userResult = await this.userService.GetByIdAsync(id, loggedUserResult.Data);
            if (!userResult.IsSuccessful)
            {
                return View("HandleErrorNotFound", userResult.Message);
            }
            var userDetailsViewModel = new UserDetailsViewModel { User = userResult.Data };

            return this.View(userDetailsViewModel);
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed([FromRoute] int id)
        {
            //var result = new Response<UserDetailsViewModel>();

            var loggedUserResult = await FindLoggedUserAsync();
            if (!loggedUserResult.IsSuccessful)
            {
                return View("HandleErrorNotLoggedIn", loggedUserResult.Message);
            }

            var isDeleted = await this.userService.DeleteAsync(id, loggedUserResult.Data);
            if (!isDeleted.IsSuccessful)
            {
                return View("HandleErrorInvalidOperation", isDeleted.Message);
            }
            return this.RedirectToAction("Index", "User");
        }

        [HttpGet]
        public async Task<IActionResult> Profile()
        {
           // var result = new Response<UserDetailsViewModel>();
            var loggedUserResult = await FindLoggedUserAsync();
            if (!loggedUserResult.IsSuccessful)
            {
                return View("HandleErrorNotLoggedIn", loggedUserResult.Message);
            }

            var userResult = await this.userService.GetByIdAsync(loggedUserResult.Data.Id, loggedUserResult.Data);
            if (!userResult.IsSuccessful)
            {
                return View("HandleErrorNotFound", userResult.Message);
            }
            var userUpdatePersonalProfileViewModel = new UserUpdateProfileViewModel()
            {
                DetailsViewModel = new UserDetailsViewModel()
            };
            userUpdatePersonalProfileViewModel.DetailsViewModel.User = userResult.Data;

            return this.View(userUpdatePersonalProfileViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Profile(UserUpdateProfileViewModel userUpdateProfileViewModel)
        {
           // var result = new Response<UserUpdateProfileViewModel>();
            var loggedUserResult = await FindLoggedUserAsync();
            if (!loggedUserResult.IsSuccessful)
            {
                return View("HandleErrorNotLoggedIn", loggedUserResult.Message);
            }

            var userToUpdate = await this.userService.UpdateAsync(loggedUserResult.Data.Id, userUpdateProfileViewModel.UpdateUserDto, loggedUserResult.Data);
            if (!userToUpdate.IsSuccessful)
            {
                return View("HandleErrorInvalidOperation", userToUpdate.Message);
            }
            return this.RedirectToAction("Profile", "User");   
        }

        [HttpGet]
        public async Task<IActionResult> ChangeProfilePicture()
        {
           //var result = new Response<UserDetailsViewModel>();

            var loggedUserResult = await FindLoggedUserAsync();
            if (!loggedUserResult.IsSuccessful)
            {
                return View("HandleErrorNotLoggedIn", loggedUserResult.Message);
            }

            var userResult = await this.userService.GetByIdAsync(loggedUserResult.Data.Id, loggedUserResult.Data);
            if (!userResult.IsSuccessful)
            {
                return View("HandleErrorNotFound", userResult.Message);
            }

            var userUpdatePersonalProfileViewModel = new UserUpdateProfileViewModel()
            {
                DetailsViewModel = new UserDetailsViewModel()
            };
            userUpdatePersonalProfileViewModel.DetailsViewModel.User = userResult.Data;

            return this.View(userUpdatePersonalProfileViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> ChangeProfilePicture(UserUpdateProfileViewModel userUpdateProfileViewModel)
        {
           // var result = new Response<UserDetailsViewModel>();
            var loggedUserResult = await FindLoggedUserAsync();
            if (!loggedUserResult.IsSuccessful)
            {
                return View("HandleErrorNotLoggedIn", loggedUserResult.Message);
            }

            var userToChangePictureTo = await this.userService.ChangeProfilePictureAsync(loggedUserResult.Data.Id, userUpdateProfileViewModel.DetailsViewModel, loggedUserResult.Data);
            if (!userToChangePictureTo.IsSuccessful)
            {
                return View("HandleErrorInvalidOperation", userToChangePictureTo.Message);
            }

            return this.RedirectToAction("Profile", "User");
        }


        [HttpGet]
        public async Task<IActionResult> ChangePassword()
        {
           // var result = new Response<UserDetailsViewModel>();
            var loggedUserResult = await FindLoggedUserAsync();
            if (!loggedUserResult.IsSuccessful)
            {
                return View("HandleErrorNotLoggedIn", loggedUserResult.Message);
            }

            var userResult = await this.userService.GetByIdAsync(loggedUserResult.Data.Id, loggedUserResult.Data);
            if (!userResult.IsSuccessful)
            {
                return View("HandleErrorNotFound", userResult.Message);
            }

            var userUpdatePersonalProfileViewModel = new UserUpdateProfileViewModel()
            {
                DetailsViewModel = new UserDetailsViewModel()
            };
            userUpdatePersonalProfileViewModel.DetailsViewModel.User = userResult.Data;

            return this.View(userUpdatePersonalProfileViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> ChangePassword(UserUpdateProfileViewModel userUpdateProfileViewModel)
        {
           // var result = new Response<UserUpdateProfileViewModel>();
            var loggedUserResult = await FindLoggedUserAsync();
            if (!loggedUserResult.IsSuccessful)
            {
                return View("HandleErrorNotLoggedIn", loggedUserResult.Message);
            }

            var userToUpdate = await this.userService.UpdateAsync(loggedUserResult.Data.Id, userUpdateProfileViewModel.UpdateUserDto, loggedUserResult.Data);
            if (!userToUpdate.IsSuccessful)
            {
                return View("HandleErrorInvalidOperation", userToUpdate.Message);
            }

            return this.RedirectToAction("ChangePassword", "User");

        }

        private async Task<Response<User>> FindLoggedUserAsync()
		{
			var loggedUsersUsername = User.FindFirst(ClaimTypes.Name);
			var loggedUserResult = await this.userService.GetLoggedUserByUsernameAsync(loggedUsersUsername.Value);
			return loggedUserResult;
		}
	}
}