using Business.DTOs.Requests;
using Business.QueryParameters;
using Business.Services.Contracts;
using Business.Services.Helpers;
using DataAccess.Models.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace VirtualWallet.Controllers.API
{
    [ApiController]
    [Route("api/users")]
    public class UserApiController : ControllerBase
    {
        private readonly IUserService userService;

        public UserApiController(IUserService userService)
        {
            this.userService = userService;
        }
       
        [HttpGet, Authorize]
        public async Task<IActionResult> GetUsersAsync([FromQuery] UserQueryParameters userQueryParameters)
        {
            var result = await userService.FilterByAsync(userQueryParameters);
            if (!result.IsSuccessful)
            {
                  return StatusCode(StatusCodes.Status404NotFound);
            }

            return StatusCode(StatusCodes.Status200OK, result);
        }

        [HttpGet("id"),Authorize]
        public async Task<IActionResult> GetByIdAsync(int id)
        {
            var loggedUserResponse = await FindLoggedUserAsync();
            if (!loggedUserResponse.IsSuccessful)
            {
                return StatusCode(StatusCodes.Status404NotFound, loggedUserResponse.Message);
            }

            var result = await this.userService.GetByIdAsync(id, loggedUserResponse.Data);

            if (!result.IsSuccessful)
            {
                return BadRequest(result.Message);
            }

            return StatusCode(StatusCodes.Status200OK, result.Data);
        }

        [HttpPost]
        public async Task<IActionResult> CreateUserAsync([FromBody] CreateUserModel createUserDto)
        {
            var result = await userService.CreateAsync(createUserDto);
            if (!result.IsSuccessful)
            {
                return BadRequest(result.Message);
            }
          
            return StatusCode(StatusCodes.Status201Created, result.Data);
        }

        [HttpPut("{id}"), Authorize]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] UpdateUserDto updateUserDto)
        {
            var loggedUserResponse = await FindLoggedUserAsync();
            if (!loggedUserResponse.IsSuccessful)
            {
                return StatusCode(StatusCodes.Status404NotFound, loggedUserResponse.Message);
            }
            var result = await userService.UpdateAsync(id, updateUserDto, loggedUserResponse.Data);

            if (!result.IsSuccessful)
            {
                return BadRequest(result.Message);
            }

            return StatusCode(StatusCodes.Status200OK, result.Data);
        }

        [HttpDelete("{id}"), Authorize]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var loggedUserResponse = await FindLoggedUserAsync();
            if (!loggedUserResponse.IsSuccessful)
            {
                return StatusCode(StatusCodes.Status404NotFound, loggedUserResponse.Message);
            }
            var result = await userService.DeleteAsync(id, loggedUserResponse.Data);

            if (!result.IsSuccessful)
            {
                return BadRequest(result.Message);
            }

            result.Message = Constants.SuccessfullDeletedUserMessage;

            return StatusCode(StatusCodes.Status200OK, result.Message);
        }

        [HttpPut("{id}/promote"), Authorize]
        public async Task<IActionResult> Promote(int id)
        {
            var loggedUserResponse = await FindLoggedUserAsync();
            if (!loggedUserResponse.IsSuccessful)
            {
                return StatusCode(StatusCodes.Status404NotFound, loggedUserResponse.Message);
            }
            var result = await userService.PromoteAsync(id, loggedUserResponse.Data);

            if (!result.IsSuccessful)
            {
                return BadRequest(result.Message);
            }

            result.Message = Constants.SuccessfullPromoteddUserMessage;

            return StatusCode(StatusCodes.Status200OK, result.Message);
        }

        [HttpPut("{id}/block"), Authorize]
        public async Task<IActionResult> BlockUser(int id)
        {
            var loggedUserResponse = await FindLoggedUserAsync();
            if (!loggedUserResponse.IsSuccessful)
            {
                return StatusCode(StatusCodes.Status404NotFound, loggedUserResponse.Message);
            }
            var result = await userService.BlockUserAsync(id, loggedUserResponse.Data);

            if (!result.IsSuccessful)
            {
                return BadRequest(result.Message);
            }

            result.Message = Constants.SuccessfullBlockedUserMessage;

            return StatusCode(StatusCodes.Status200OK, result.Message);
        }

        [HttpPut("{id}/unblock"), Authorize]
        public async Task<IActionResult> UnblockUser(int id)
        {
           
            var loggedUserResponse = await FindLoggedUserAsync();
            if (!loggedUserResponse.IsSuccessful)
            {
                return StatusCode(StatusCodes.Status404NotFound, loggedUserResponse.Message);
            }
            var result = await userService.UnblockUserAsync(id, loggedUserResponse.Data);

            if (!result.IsSuccessful)
            {
                return BadRequest(result.Message);
            }

            result.Message = Constants.SuccessfullUnblockedUserMessage;

            return StatusCode(StatusCodes.Status200OK, result.Message);
        }
        private async Task<Response<User>> FindLoggedUserAsync()
        {
            var loggedUsersUsername = User.FindFirst(ClaimTypes.Name);
            var loggedUserResult = await this.userService.GetLoggedUserByUsernameAsync(loggedUsersUsername.Value);
            return loggedUserResult;
        }
    }
}
