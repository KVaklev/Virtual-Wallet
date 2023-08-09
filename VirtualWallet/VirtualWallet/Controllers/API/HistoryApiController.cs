﻿using Business.QueryParameters;
using Business.Services.Contracts;
using Business.Services.Helpers;
using DataAccess.Models.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace VirtualWallet.Controllers.API
{
    [ApiController]
    [Route("api/histories")]
    public class HistoryApiController : ControllerBase
    {
        private readonly IHistoryService historyService;
        private readonly IUserService userService;

        public HistoryApiController(
            IHistoryService historyService,
            IUserService userService)
        {
            this.historyService = historyService;
            this.userService = userService;
        }

        [HttpGet("{id}"), Authorize]
        public async Task<IActionResult> GetbyIdAsync(int id)
        {
            var loggedUserResult = await FindLoggedUserAsync();
            if (!loggedUserResult.IsSuccessful)
            {
                return StatusCode(StatusCodes.Status404NotFound, loggedUserResult.Message);

            }

            var result = await this.historyService.GetByIdAsync(id, loggedUserResult.Data);
            if (!result.IsSuccessful)
            {
                if (result.Message == Constants.NoFoundResulte)
                {
                    return StatusCode(StatusCodes.Status404NotFound, result.Message);
                }
                else
                {
                    return StatusCode(StatusCodes.Status401Unauthorized, result.Message);
                }
            }
          return StatusCode(StatusCodes.Status200OK, result.Data);
        }

        [HttpGet, Authorize]
        public async Task<IActionResult> GetHistoryAsync([FromQuery] HistoryQueryParameters historyQueryParameters) 
        {
           
           var loggedUserResult = await FindLoggedUserAsync();
           if (!loggedUserResult.IsSuccessful)
           {
               return StatusCode(StatusCodes.Status404NotFound, loggedUserResult.Message);
           }

           var result = await this.historyService.FilterByAsync(historyQueryParameters, loggedUserResult.Data);

           if (!result.IsSuccessful)
           {
               if (result.Message == Constants.NoFoundResulte)
               {
                   return StatusCode(StatusCodes.Status404NotFound, loggedUserResult.Message);
               }
               else
               {
                   return StatusCode(StatusCodes.Status401Unauthorized, result.Message);

               }
           }
          return StatusCode(StatusCodes.Status200OK, result.Data);   
        }

        private async Task<Response<User>> FindLoggedUserAsync()
        {
            var loggedUsersUsername = User.FindFirst(ClaimTypes.Name);
            var loggedUserResult = await this.userService.GetLoggedUserByUsernameAsync(loggedUsersUsername.Value);
            return loggedUserResult;
        }
    }
}
