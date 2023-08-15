
using Business.QueryParameters;
using Business.Services.Contracts;
using Business.ViewModels;
using DataAccess.Models.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace VirtualWallet.Controllers.MVC
{
    [AllowAnonymous]
    public class HistoryController : Controller

    {
        private readonly IHistoryService historyService;
        private readonly IUserService userService;


        public HistoryController(
            IHistoryService historyService,
            IUserService userService)
        {
            this.historyService = historyService;
            this.userService = userService;
        }


        [HttpGet]
        public async Task<IActionResult> IndexAsync([FromQuery] HistoryQueryParameters parameters)
        {

            var loggedUserResponse = await FindLoggedUserAsync();
            if (!loggedUserResponse.IsSuccessful)
            {
                return this.RedirectToAction("Login", "Account");
            }

            var result = await this.historyService.FilterByAsync(parameters, loggedUserResponse.Data);
            if (!result.IsSuccessful)
            {
                this.ViewData["Controller"] = "History";
                return View("ErrorMessage", result.Message);
            }
            var indexHistoryViewModel = new IndexHistoryViewModel();
            indexHistoryViewModel.GetHistoryDtos = result.Data;
            indexHistoryViewModel.HistoryQueryParameters = parameters;
            indexHistoryViewModel.LoggedUser = loggedUserResponse.Data;


                return this.View(indexHistoryViewModel);
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
