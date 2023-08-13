using Business.Exceptions;
using Business.Services.Contracts;
using DataAccess.Models.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Runtime.CompilerServices;
using System.Security.Claims;

namespace VirtualWallet.Controllers.MVC
{
    public class CurrencyController : Controller
    {
        private readonly ICurrencyService currencyService;
        private readonly IUserService userService;

        public CurrencyController(
            ICurrencyService currencyService,
            IUserService userService
            ) 
        {
            this.currencyService = currencyService;
            this.userService = userService;
        }

        [HttpGet,Authorize]
        public async  Task<IActionResult> Index()
        {
            var loggedUser = await FindLoggedUserAsync();
            if (!loggedUser.IsSuccessful)
            {
                return this.RedirectToAction("Login", "Account");
            }
                var currencies = await this.currencyService.GetAllAsync();
                return View(currencies);
        }
            private async Task<Response<User>> FindLoggedUserAsync()
            {
                var loggedUsersUsername = User.FindFirst(ClaimTypes.Name);
                var loggedUserResult = await this.userService.GetLoggedUserByUsernameAsync(loggedUsersUsername.Value);

                return loggedUserResult;
            }
    }
}
