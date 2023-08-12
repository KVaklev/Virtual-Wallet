using Business.DTOs.Requests;
using Business.Services.Contracts;
using Business.Services.Helpers;
using Business.Services.Models;
using DataAccess.Repositories.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace VirtualWallet.Controllers.MVC
{
    [AllowAnonymous]
    [Route("Account")]
    public class AccountController : Controller
    {
        private readonly IUserService userService;
        private readonly IAccountService accountService;
        private readonly IUserRepository userRepository;
        private readonly IEmailService emailService;
        private readonly ICurrencyService currencyService;

        public AccountController(
            IUserService userService,
            IAccountService accountService,
            IUserRepository userRepository,
            IEmailService emailService,
            ICurrencyService currencyService)
        {
            this.userService = userService;
            this.accountService = accountService;
            this.userRepository = userRepository;
            this.emailService = emailService;
            this.currencyService = currencyService;
        }

        [HttpGet("login")]
        public IActionResult Login()
        {
            var loginViewModel = new LoginUserModel();

            return this.View(loginViewModel);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginUserModel loginUserModel)
        {   
            if (!this.ModelState.IsValid)
            {
                return this.View(loginUserModel);
            }

            var loggedUser = await this.userService.LoginAsync(loginUserModel.Username, loginUserModel.Password); 
            if (!loggedUser.IsSuccessful)
            {
                this.ModelState.AddModelError(loggedUser.Error.InvalidPropertyName, loggedUser.Message);
                return this.View(loginUserModel);
            }

            var result = await Security.CreateApiTokenAsync(loggedUser.Data);
            if (!result.IsSuccessful)
            {
                return BadRequest(result.Message);
            }

            Response.Cookies.Append("Cookie_JWT", result.Data, new CookieOptions()
            {
                HttpOnly = false,
                SameSite = SameSiteMode.Strict
            });

            return RedirectToAction("Index", "Home");
        }

        [HttpGet("logout")]
        public IActionResult Logout()
        {
            if (Request.Cookies["Cookie_JWT"] != null)
            {
                Response.Cookies.Delete("Cookie_JWT");
            }

            return RedirectToAction("Index", "Home");
        }

        [HttpGet("register")]
        public IActionResult Register()
        {
            var createUserModel = new CreateUserModel();
            var currencyResult = this.currencyService.GetAll();
            if (!currencyResult.IsSuccessful)
            {
                return this.View("Error");
            }
            TempData["Currencies"] = JsonSerializer.Serialize(currencyResult.Data);
            return this.View(createUserModel);
        }

 
        [HttpPost("register")]
        public async Task<IActionResult> Register(CreateUserModel createUserModel)
        {
            if (!this.ModelState.IsValid)
            {
                return this.View(createUserModel);
            }

            var result = await this.userService.CreateAsync(createUserModel);
            if (!result.IsSuccessful)
            {
                this.ModelState.AddModelError(result.Error.InvalidPropertyName, result.Message);
                return View(createUserModel);
            }

            return await SendConfirmationEmailAsync(result.Data.Username);
        }

        [HttpGet("confirm-email")]
        public async Task<IActionResult> SendConfirmationEmailAsync(string username)
        {
            var user = await this.userRepository.GetByUsernameAsync(username);
            var generatedToken = await this.accountService.GenerateTokenAsync(user.Id);

            if (!generatedToken.IsSuccessful)
            {
                return RedirectToAction("Register", "Account");
            }

            var confirmationLink = Url.Action("confirm-registration", "Account", new { userId = user.Id, token = generatedToken }, Request.Scheme);
            var message = await this.emailService.BuildEmailAsync(user, confirmationLink);

            await emailService.SendEMailAsync(message);
                
            return RedirectToAction("SuccessfulRegistration", "Account");
        }

        [HttpGet("successful-registration")]
        public IActionResult SuccessfulRegistration()
        {
			return this.View();
		}

        //ToDo - fix messages if not confirmed
        [HttpGet("confirm-registration")]
        public async Task<IActionResult> ConfirmRegistrationAsync([FromQuery] int userId, [FromQuery] string token)
        {
			if (!ModelState.IsValid)
			{
				return RedirectToAction("Index", "Home");
			}

			var result = await this.accountService.ConfirmRegistrationAsync(userId, token);
            if (!result.IsSuccessful)
            {
				return RedirectToAction("Index", "Home");

			}
          return RedirectToAction("Login", "Account");
        }
    }
}
