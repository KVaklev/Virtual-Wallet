using Business.QueryParameters;
using Business.Services.Contracts;
using Business.Services.Helpers;
using Business.ViewModels;
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

            var loggedUserResponse = await userService.FindLoggedUserAsync(User.FindFirst(ClaimTypes.Name)?.Value!);
            if (!loggedUserResponse.IsSuccessful)
            {
                return this.RedirectToAction(Constant.Action.Login, Constant.Controller.Account);
            }

            var result = await this.historyService.FilterByAsync(parameters, loggedUserResponse.Data);
            
            var indexHistoryViewModel = new IndexHistoryViewModel();
            indexHistoryViewModel.HistoryQueryParameters = parameters;
            indexHistoryViewModel.LoggedUser = loggedUserResponse.Data;
            if (!result.IsSuccessful)
            {
                if(result.Message == Constants.NoRecordsFound)
                {
                    this.ViewData[Constant.ViewData.ErrorMessage] = result.Message;
                    return View(indexHistoryViewModel);
                }
                else
                {
                    return View(Constant.View.ErrorMessage, result.Message);
                }
            }
            indexHistoryViewModel.GetHistoryDtos = result.Data;

            return this.View(indexHistoryViewModel);
        }
    }
}
