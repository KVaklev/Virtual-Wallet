using AutoMapper;
using Business.DTOs.Requests;
using Business.Exceptions;
using Business.QueryParameters;
using Business.Services.Contracts;
using Business.ViewModels;
using DataAccess.Models.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Text;

namespace VirtualWallet.Controllers.MVC
{
    [Authorize]
	[Route("users")]
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
                UserQueryParameters = userQueryParameters,
                UserChangeStatusViewModel = new UserChangeStatusViewModel()
            };

            return this.View(newViewModel);
        }


        [HttpGet("ChangeStatus")]
        public async Task<IActionResult> ChangeStatus([FromRoute] int id)
        {
            var loggedUserResponse = await GetLoggedUserAsync();
            if (!loggedUserResponse.IsSuccessful)
            {
                return StatusCode(StatusCodes.Status404NotFound, loggedUserResponse.Message);
            }

            var users = userService.GetAll();
            var newViewModel = new UserSearchModel
            {
                Users = null,
                UserQueryParameters = null,
                UserChangeStatusViewModel = new UserChangeStatusViewModel
                {
                    IsAdmin = default,
                    IsBlocked = default
                }
            };

            return this.View(newViewModel);
        }

        
        [HttpPost]
        public async Task<IActionResult> ChangeStatusConfirmed([FromRoute] int id, UserSearchModel userSearchModel)
        {
            var loggedUserResponse = await GetLoggedUserAsync();
            if (!loggedUserResponse.IsSuccessful)
            {
                return StatusCode(StatusCodes.Status404NotFound, loggedUserResponse.Message);
            }

            await userService.ChangeStatusAsync(id, loggedUserResponse.Data, userSearchModel.UserChangeStatusViewModel);

            return this.RedirectToAction("Index", "Users");
        }


        //[HttpGet]
        //public IActionResult Profile([FromRoute] int id)
        //{
        //    try
        //    {
        //        var user = userService.GetById(id);

        //        var userUpdateProfileViewModel = this.mapper.Map<UserUpdateProfileViewModel>(user);

        //        return this.View(userUpdateProfileViewModel);
        //    }
        //    catch (EntityNotFoundException ex)
        //    {
        //        this.HttpContext.Response.StatusCode = StatusCodes.Status404NotFound;
        //        this.ViewData["ErrorMessage"] = ex.Message;

        //        return this.View("Error");
        //    }
        //}
        //[HttpPost, ActionName("Profile")]
        //[HttpPost]
        //public IActionResult ProfileConfirmed([FromRoute] int id, UserUpdateProfileViewModel userUpdateProfileViewModel)
        //{

        //    if (!this.ModelState.IsValid)
        //    {
        //        return View(userUpdateProfileViewModel);
        //    }
        //    try
        //    {
        //        var userToUpdate = userService.GetById(id);

        //        if (userUpdateProfileViewModel.NewPassword != null)
        //        {
        //            var codedPassword = Convert.ToBase64String(Encoding.UTF8.GetBytes(userUpdateProfileViewModel.NewPassword));

        //            userUpdateProfileViewModel.NewPassword = codedPassword.ToString();
        //        }

        //        var loggedUser = userToUpdate;

        //        var user = mapper.Map<User>(userUpdateProfileViewModel);

        //        userToUpdate = this.userService.Update(id, user, loggedUser);

        //        if (userUpdateProfileViewModel.ImageFile != null)
        //        {
        //            string imageUploadedFolder = Path.Combine(webHostEnvironment.WebRootPath, "UploadedImages");
        //            string uniqueFileName = userToUpdate.FirstName + " " + userToUpdate.LastName + ".png";
        //            string filePath = Path.Combine(imageUploadedFolder, uniqueFileName);

        //            using (var fileStream = new FileStream(filePath, FileMode.Create))
        //            {
        //                userUpdateProfileViewModel.ImageFile.CopyTo(fileStream);
        //            }
        //            userToUpdate.ProfilePhotoPath = "~/UploadedImages";
        //            userToUpdate.ProfilePhotoFileName = uniqueFileName;
        //        }

        //        return this.RedirectToAction("Details", "Users", new { id = userToUpdate.Id });
        //    }
        //    catch (EntityNotFoundException ex)
        //    {
        //        this.HttpContext.Response.StatusCode = StatusCodes.Status404NotFound;
        //        this.ViewData["ErrorMessage"] = ex.Message;

        //        return this.View("Error");
        //    }
        //    catch (DuplicateEntityException ex)
        //    {
        //        this.HttpContext.Response.StatusCode = StatusCodes.Status404NotFound;
        //        this.ViewData["ErrorMessage"] = ex.Message;

        //        return this.View(userUpdateProfileViewModel);
        //    }
        //    catch (UnauthorizedOperationException ex)
        //    {
        //        this.HttpContext.Response.StatusCode = StatusCodes.Status401Unauthorized;
        //        this.ViewData["ErrorMessage"] = ex.Message;

        //        return this.View(userUpdateProfileViewModel);
        //    }
        //}
        private async Task<Response<User>> GetLoggedUserAsync()
        {
            var loggedUsersUsername = User.FindFirst(ClaimTypes.Name);
            var loggedUserResult = await this.userService.GetLoggedUserByUsernameAsync(loggedUsersUsername.Value);

            return loggedUserResult;
        }
    }
}
