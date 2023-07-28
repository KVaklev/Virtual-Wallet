﻿using AutoMapper;
using Business.Dto;
using Business.Exceptions;
using Business.Services.Contracts;
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
    [Route("api/auth")]
    public class AuthApiController : ControllerBase
    {
        private readonly IAuthManager authManager;
        private readonly IMapper mapper;
        private readonly IUserService userService;

        public AuthApiController(
            IAuthManager authManager,
            IMapper mapper, 
            IUserService userService)
        {
            this.authManager = authManager;
            this.mapper = mapper;
            this.userService = userService;
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> LoginAsync(string username, string password)
        {
            try
            {
                var loggedUser = await authManager.TryGetUserByUsernameAsync(username);

                if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
                {
                    return BadRequest("Username and/or Password not specified");
                }

                string token = await CreateApiTokenAsync(loggedUser);

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

        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<IActionResult> RegisterAsync([FromBody] CreateUserDto createUserDto)
        {
            try
            {
                var user = this.mapper.Map<User>(createUserDto);
                var createdUser = await this.userService.CreateAsync(user);
                return StatusCode(StatusCodes.Status201Created, createdUser);
            }
            catch (DuplicateEntityException ex)
            {
                return StatusCode(StatusCodes.Status409Conflict, ex.Message);
            }
        }

        private async Task<string> CreateApiTokenAsync(User loggedUser)
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
                new Claim("UsersAccountId", loggedUser.Account.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, loggedUser.Email),
                new Claim(JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString())
        },
            expires: DateTime.Now.AddMinutes(30),
                    signingCredentials: signinCredentials
                );
            string token = new JwtSecurityTokenHandler()

            .WriteToken(jwtSecurityToken);
            Response.Cookies.Append("Cookie_JWT", token);
            return await Task.FromResult(token);
        }
    }
}
