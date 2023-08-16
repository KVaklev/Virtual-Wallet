using Business.DTOs.Requests;
using Business.Services.Contracts;
using Business.Services.Helpers;
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
                this.ViewData["Controller"] = "Account";
                return View("ErrorMessage", loggedUser.Message);
            }

            var result = await Security.CreateApiTokenAsync(loggedUser.Data);
            if (!result.IsSuccessful)
            {
                this.ViewData["Controller"] = "Account";
                return View("ErrorMessage", result.Message);
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
        public async Task<IActionResult> Register()
        {
            var createUserModel = new CreateUserModel();
            var currencyResult = await this.currencyService.GetAllAsync();
            if (!currencyResult.IsSuccessful)
            {
                this.ViewData["Controller"] = "Account";
                return View("ErrorMessage", currencyResult.Message);
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
                this.ViewData["Controller"] = "Account";
                return View("ErrorMessage", result.Message);
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
                this.ViewData["Controller"] = "Account";
                return View("ErrorMessage", generatedToken.Message);
            }

            var confirmationLink = Url.Action("confirmed-registration", "Account", new { userId = user.Id, token = generatedToken }, Request.Scheme);
            var message = await this.emailService.BuildEmailAsync(user, confirmationLink);

            try
            {
                await emailService.SendEMailAsync(message);
            }
            catch (InvalidOperationException ex)
            {
                this.ViewData["Controller"] = "Account";
                return View("ErrorMessage", ex.Message);
            }
                
            return RedirectToAction("SuccessfulEmailSent", "Account");
        }

        [HttpGet("successful-email-sent")]
        public IActionResult SuccessfulEmailSent()
        {
			return this.View();
		}

        [HttpGet("confirmed-registration")]
        public async Task<IActionResult> ConfirmRegistrationAsync([FromQuery] int userId, [FromQuery] string token)
        {
			var result = await this.accountService.ConfirmRegistrationAsync(userId, token);
            if (!result.IsSuccessful)
            {
                this.ViewData["Controller"] = "Account";
                return View("ErrorMessage", result.Message);
            }

          return View("SuccessfulRegistration");
        }
    }
}
