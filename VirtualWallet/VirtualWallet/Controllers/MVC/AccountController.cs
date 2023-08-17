using Business.DTOs.Requests;
using Business.Services.Contracts;
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
        private readonly ISecurityService security;

        public AccountController(
            IUserService userService,
            IAccountService accountService,
            IUserRepository userRepository,
            IEmailService emailService,
            ICurrencyService currencyService,
            ISecurityService security)
        {
            this.userService = userService;
            this.accountService = accountService;
            this.userRepository = userRepository;
            this.emailService = emailService;
            this.currencyService = currencyService;
            this.security = security;
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
                return View(Constant.View.ErrorMessage, loggedUser.Message);
            }

            var result = await this.security.CreateApiTokenAsync(loggedUser.Data);
            if (!result.IsSuccessful)
            {
                return View(Constant.View.ErrorMessage, result.Message);
            }

            Response.Cookies.Append(Constant.View.JWTCookie, result.Data, new CookieOptions()
            {
                HttpOnly = false,
                SameSite = SameSiteMode.Strict
            });

            return RedirectToAction(Constant.Action.Index, Constant.Controller.History);
        }

        [HttpGet("logout")]
        public IActionResult Logout()
        {
            if (Request.Cookies[Constant.View.JWTCookie] != null)
            {
                Response.Cookies.Delete(Constant.View.JWTCookie);
            }

            return RedirectToAction(Constant.Action.Index, Constant.Controller.Home);
        }

        [HttpGet("register")]
        public async Task<IActionResult> Register()
        {
            var createUserModel = new CreateUserModel();
            var currencyResult = await this.currencyService.GetAllAsync();
            if (!currencyResult.IsSuccessful)
            {
                return View(Constant.View.ErrorMessage, currencyResult.Message);
            }
            TempData[Constant.TempData.Currencies] = JsonSerializer.Serialize(currencyResult.Data);

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
                return View(Constant.View.ErrorMessage, result.Message);
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
                return View(Constant.View.ErrorMessage, generatedToken.Message);
            }

            var confirmationLink = Url.Action(Constant.View.ConfirmedRegistration, Constant.Controller.Account, new { userId = user.Id, token = generatedToken }, Request.Scheme);
            var message = await this.emailService.BuildEmailAsync(user, confirmationLink!);

            try
            {
                await emailService.SendEMailAsync(message);
            }
            catch (InvalidOperationException ex)
            {
                return View(Constant.View.ErrorMessage, ex.Message);
            }
                
            return RedirectToAction(Constant.Action.SuccessfulEmailSent, Constant.Controller.Account);
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
                return View(Constant.View.ErrorMessage, result.Message);
            }

          return View(Constant.View.SuccessfulRegistration);
        }
    }
}
