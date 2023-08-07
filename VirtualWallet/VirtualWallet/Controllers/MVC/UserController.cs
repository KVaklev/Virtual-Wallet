﻿using Business.DTOs.Responses;
using Business.QueryParameters;
using Business.Services.Contracts;
using Business.ViewModels;
using DataAccess.Models.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace VirtualWallet.Controllers.MVC
{
    [Authorize]
	[Route("User")]
	public class UserController : Controller
    {
        private readonly IUserService userService;
        public UserController(IUserService userService)
        {
            this.userService = userService;
        }

        [HttpGet]
		public async Task<IActionResult> Index(UserQueryParameters userQueryParameters)
        {

           var result = await this.userService.FilterByAsync(userQueryParameters);

            if (!result.IsSuccessful)
            {
                this.ModelState.AddModelError(result.Error.InvalidPropertyName, result.Message);
                return View(result.Data);
            }

            var newViewModel = new UserSearchModel
            {
                Users = result,
                UserQueryParameters = userQueryParameters
            };

            return this.View(newViewModel);
        }

    }
}
