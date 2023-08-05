using Business.Exceptions;
using Business.QueryParameters;
using Business.Services.Contracts;
using Business.Services.Models;
using Business.ViewModels;
using DataAccess.Models.Models;
using DataAccess.Repositories.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace VirtualWallet.Controllers.MVC
{
    [AllowAnonymous]
    public class HistoryController : Controller

    {
        private readonly IHistoryService historyService;
        private readonly IUserRepository userRepository;

        public HistoryController(
            IHistoryService historyService,
            IUserRepository userRepository)
        {
            this.historyService = historyService;
            this.userRepository = userRepository;

        }


        [HttpGet]
        public async Task<IActionResult> IndexAsync([FromQuery] HistoryQueryParameters parameters)
        {
			//try
			//{
			//    var loggedUser = await GetLoggedUserAsync();
			//    var result = await this.historyService.FilterByAsync(parameters, loggedUser);

			//    return this.View(result);
			//}
			//catch (EntityNotFoundException ex)
			//{
			//    return await EntityErrorViewAsync(ex.Message);
			//}

			return this.View();
		}

        private async Task<User> GetLoggedUserAsync()
        {
            var username = this.HttpContext.Session.GetString("LoggedUser");
            var user = await this.userRepository.GetByUsernameAsync(username);
            return user;
        }

        private async Task<IActionResult> EntityErrorViewAsync(string message)
        {
            this.HttpContext.Response.StatusCode = StatusCodes.Status404NotFound;
            this.ViewData["ErrorMessage"] = message;

            return this.View("Error404");
        }

    }
}
