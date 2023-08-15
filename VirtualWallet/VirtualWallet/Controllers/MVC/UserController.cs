using Business.QueryParameters;
using Business.Services.Contracts;
using Business.ViewModels.UserViewModels;
using DataAccess.Models.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace VirtualWallet.Controllers.MVC
{
    [AllowAnonymous]
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
                this.ViewData["Controller"] = "User";
                return View("ErrorMessage", result.Message);
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
            var loggedUserResult = await FindLoggedUserAsync();
			if (!loggedUserResult.IsSuccessful)
			{
                return RedirectToAction("Login", "Account");
			}

            var userResult = await this.userService.GetByIdAsync(id, loggedUserResult.Data);
            if (!userResult.IsSuccessful)
            {
                this.ViewData["Controller"] = "User";
                return View("ErrorMessage", userResult.Message);
            }

            var cardsResult = this.cardService.GetByAccountId(id);
            if (!cardsResult.IsSuccessful)
            {
                this.ViewData["Controller"] = "User";
                return View("ErrorMessage", cardsResult.Message);
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
            var loggedUserResult = await FindLoggedUserAsync();
            if (!loggedUserResult.IsSuccessful)
            {
                return RedirectToAction("Login", "Account");
            }

            var userResult = await this.userService.GetByIdAsync(id, loggedUserResult.Data);
            if (!userResult.IsSuccessful)
            {
                this.ViewData["Controller"] = "User";
                return View("ErrorMessage", userResult.Message);
            }

            var userDetailsViewModel = new UserDetailsViewModel {  User = userResult.Data  };

            return this.View(userDetailsViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Edit([FromRoute] int id, UserDetailsViewModel userDetailsViewModel)
        {
           
            var loggedUserResult = await FindLoggedUserAsync();
            if (!loggedUserResult.IsSuccessful)
            {
                return RedirectToAction("Login", "Account");
            }

            var isChanged = await this.userService.ChangeStatusAsync(id, userDetailsViewModel, loggedUserResult.Data);
            if (!isChanged.IsSuccessful)
            {
                this.ViewData["Controller"] = "User";
                return View("ErrorMessage", isChanged.Message);
            }

            return this.RedirectToAction("Index", "User");  
        }

        [HttpGet]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {    
            var loggedUserResult = await FindLoggedUserAsync();
            if (!loggedUserResult.IsSuccessful)
            {
                return RedirectToAction("Login", "Account");
            }

            var userResult = await this.userService.GetByIdAsync(id, loggedUserResult.Data);
            if (!userResult.IsSuccessful)
            {
                this.ViewData["Controller"] = "User";
                return View("ErrorMessage", userResult.Message);
            }
            var userDetailsViewModel = new UserDetailsViewModel { User = userResult.Data };

            return this.View(userDetailsViewModel);
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed([FromRoute] int id)
        {
            var loggedUserResult = await FindLoggedUserAsync();
            if (!loggedUserResult.IsSuccessful)
            {
                return RedirectToAction("Login", "Account");
            }

            var isDeleted = await this.userService.DeleteAsync(id, loggedUserResult.Data);
            if (!isDeleted.IsSuccessful)
            {
                this.ViewData["Controller"] = "User";
                return View("ErrorMessage", isDeleted.Message);
            }

            return View("SuccessfulDelete");
        }

        [HttpGet]
        public async Task<IActionResult> Profile()
        {
            var loggedUserResult = await FindLoggedUserAsync();
            if (!loggedUserResult.IsSuccessful)
            {
                return RedirectToAction("Login", "Account");
            }

            var userResult = await this.userService.GetByIdAsync(loggedUserResult.Data.Id, loggedUserResult.Data);
            if (!userResult.IsSuccessful)
            {
                this.ViewData["Controller"] = "User";
                return View("ErrorMessage", userResult.Message);
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
          
            var loggedUserResult = await FindLoggedUserAsync();
            if (!loggedUserResult.IsSuccessful)
            {
                return RedirectToAction("Login", "Account");
            }

            var userToUpdate = await this.userService.UpdateAsync(loggedUserResult.Data.Id, userUpdateProfileViewModel.UpdateUserDto, loggedUserResult.Data);
            if (!userToUpdate.IsSuccessful)
            {
                this.ViewData["Controller"] = "User";
                return View("ErrorMessage", userToUpdate.Message);
            }

            return this.RedirectToAction("Profile", "User");   
        }

        [HttpGet]
        public async Task<IActionResult> ChangeProfilePicture()
        {
            var loggedUserResult = await FindLoggedUserAsync();
            if (!loggedUserResult.IsSuccessful)
            {
                return RedirectToAction("Login", "Account");
            }

            var userResult = await this.userService.GetByIdAsync(loggedUserResult.Data.Id, loggedUserResult.Data);
            if (!userResult.IsSuccessful)
            {
                this.ViewData["Controller"] = "User";
                return View("ErrorMessage", userResult.Message);
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
            var loggedUserResult = await FindLoggedUserAsync();
            if (!loggedUserResult.IsSuccessful)
            {
                return RedirectToAction("Login", "Account");
            }

            var userToChangePictureTo = await this.userService.ChangeProfilePictureAsync(loggedUserResult.Data.Id, userUpdateProfileViewModel.DetailsViewModel, loggedUserResult.Data);
            if (!userToChangePictureTo.IsSuccessful)
            {
                this.ViewData["Controller"] = "User";
                return View("ErrorMessage", userToChangePictureTo.Message);
            }

            return this.RedirectToAction("Profile", "User");
        }


        [HttpGet]
        public async Task<IActionResult> ChangePassword()
        {
            var loggedUserResult = await FindLoggedUserAsync();
            if (!loggedUserResult.IsSuccessful)
            {
                return RedirectToAction("Login", "Account");
            }

            var userResult = await this.userService.GetByIdAsync(loggedUserResult.Data.Id, loggedUserResult.Data);
            if (!userResult.IsSuccessful)
            {
                this.ViewData["Controller"] = "User";
                return View("ErrorMessage", userResult.Message);
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
            var loggedUserResult = await FindLoggedUserAsync();
            if (!loggedUserResult.IsSuccessful)
            {
                return RedirectToAction("Login", "Account");
            }

            var userToUpdate = await this.userService.UpdateAsync(loggedUserResult.Data.Id, userUpdateProfileViewModel.UpdateUserDto, loggedUserResult.Data);
            if (!userToUpdate.IsSuccessful)
            {
                this.ViewData["Controller"] = "User";
                return View("ErrorMessage", userToUpdate.Message);
            }

            return this.RedirectToAction("ChangePassword", "User");
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