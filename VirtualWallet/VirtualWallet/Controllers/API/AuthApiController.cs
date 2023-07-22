using Business.Exceptions;
using DataAccess.Models.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Presentation.Helpers;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace VirtualWallet.Controllers.API
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthApiController : Controller
    {
        private readonly IAuthManager authManager;

        public AuthApiController(IAuthManager authManager)
        {
            this.authManager = authManager;
        }

        [AllowAnonymous]
        [HttpPost]
        public IActionResult Login(string username, string password)
        {
            try
            {
                var loggedUser = authManager.TryGetUserByUsername(username);

                if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
                {
                    return BadRequest("Username and/or Password not specified");
                }

                string token = CreateApiToken(loggedUser);
                return Ok("Logged in successfully. Token: " + token);

            }
            catch (UnauthorizedOperationException ex)
            {
                return StatusCode(StatusCodes.Status401Unauthorized, ex.Message);
            }
            catch
            {
                return BadRequest("An error occurred in generating the token");
            }
        }

        private string CreateApiToken(User loggedUser)
        {
            var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("This is my secret testing key"));
            var signinCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);
            var jwtSecurityToken = new JwtSecurityToken(
                    issuer: "VirtualWallet",
                    audience: "Where is that audience",
                    claims: new[] {
                new Claim("LoggedUserId", loggedUser.Id.ToString()),
                new Claim("Username", loggedUser.Username),
                new Claim("IsAdmin", loggedUser.IsAdmin.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, loggedUser.Email),
                new Claim(JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString())
        },
            expires: DateTime.Now.AddMinutes(10),
                    signingCredentials: signinCredentials
                );
            string token = new JwtSecurityTokenHandler()

            .WriteToken(jwtSecurityToken);
            Response.Cookies.Append("Cookie_JWT", token);
            return token;
        }
    }
}
