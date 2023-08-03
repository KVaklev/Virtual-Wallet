using Business.Exceptions;
using Business.Services.Contracts;
using Business.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace VirtualWallet.Controllers.MVC
{
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

            try
            {
                var loggedUser = await this.userService.LoginAsync(loginViewModel.Username, loginViewModel.Password);
                var token = await this.accountService.CreateApiTokenAsync(loggedUser);

                Response.Cookies.Append("Cookie_JWT", token.ToString(), new CookieOptions()
                {
                    HttpOnly = false,
                    SameSite = SameSiteMode.Strict
                });

                return Ok("Logged in successfully. Token: " + token);
            }
            catch (ArgumentException e)
            {
                return StatusCode(StatusCodes.Status400BadRequest, e.Message);
            }
            catch (UnauthorizedOperationException e)
            {
                return StatusCode(StatusCodes.Status401Unauthorized, e.Message);
            }
            catch
            {
                return BadRequest("An error occurred in generating the token");
            }
        }
    }
}
