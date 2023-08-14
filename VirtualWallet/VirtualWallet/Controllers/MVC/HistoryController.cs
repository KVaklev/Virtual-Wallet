
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

            var loggedUserResponse = await GetLoggedUserAsync();
            if (!loggedUserResponse.IsSuccessful)
            {
                return await EntityErrorViewAsync(loggedUserResponse.Message);
            }

            var result = await this.historyService.FilterByAsync(parameters, loggedUserResponse.Data);
            if (!result.IsSuccessful)
            {
                return await EntityErrorViewAsync(result.Message);
            }
            var indexHistoryViewModel = new IndexHistoryViewModel();
            indexHistoryViewModel.GetHistoryDtos = result.Data;
            indexHistoryViewModel.HistoryQueryParameters = parameters;
            indexHistoryViewModel.LoggedUser = loggedUserResponse.Data;


                return this.View(indexHistoryViewModel);
        }

        private async Task<Response<User>> GetLoggedUserAsync()
        {
            var loggedUsersUsername = User.FindFirst(ClaimTypes.Name);
            var loggedUserResult = await this.userService.GetLoggedUserByUsernameAsync(loggedUsersUsername.Value);

            return loggedUserResult;
        }

        private async Task<IActionResult> EntityErrorViewAsync(string message)
        {
           this.HttpContext.Response.StatusCode = StatusCodes.Status404NotFound;
            this.ViewData["ErrorMessage"] = message;

            return this.View("Error");
        }

    }
}
