using Business.Services.Contracts;
using Business.Services.Helpers;
using Business.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace VirtualWallet.Controllers.MVC
{
    [AllowAnonymous]
    public class AccountController : Controller
    {
        private readonly IUserService userService;
        private readonly IAccountService accountService;

        public AccountController(
            IUserService userService,
            IAccountService accountService)
        {
            this.userService = userService;
            this.accountService = accountService;
        }

        [HttpGet]
        public async Task<IActionResult> Login()
        {
            var loginViewModel = new LoginViewModel();

            return this.View(loginViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel loginViewModel)
        {
            if (!this.ModelState.IsValid)
            {
                return this.View(loginViewModel);
            }

            var loggedUser = await this.userService.LoginAsync(loginViewModel.Username, loginViewModel.Password);
           
                if (!loggedUser.IsSuccessful)
                {
                    return BadRequest(loggedUser.Message);
                }

            var result = await this.accountService.CreateApiTokenAsync(loggedUser.Data);

                if (!result.IsSuccessful)
                {
                    return BadRequest(result.Message);
                }

            Response.Cookies.Append("Cookie_JWT", result.Data.ToString(), new CookieOptions()
            {
                HttpOnly = false,
                SameSite = SameSiteMode.Strict
            });

            result.Message = Constants.SuccessfullTokenMessage;

            return RedirectToAction("Index", "Home");
        }

        [HttpGet()]
        public IActionResult Logout()
        {

            if (Request.Cookies["Cookie_JWT"] != null)
            {
                Response.Cookies.Delete("Cookie_JWT");
            }

            return RedirectToAction("Index", "Home");
        }
    }
}
