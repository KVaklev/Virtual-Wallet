using AutoMapper;
using Business.DTOs.Requests;
using Business.DTOs.Responses;
using Business.Exceptions;
using Business.QueryParameters;
using Business.Services.Contracts;
using DataAccess.Models.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Presentation.Helpers;

namespace VirtualWallet.Controllers.API
{

    [ApiController]
    [Route("api/users")]
    public class UserApiController : ControllerBase
    {
        private readonly IUserService userService;
        private readonly IMapper mapper;
        private readonly IAuthManager authManager;

        public UserApiController(
            IUserService userService,
            IMapper mapper,
            IAuthManager authManager)
        {
            this.userService = userService;
            this.mapper = mapper;
            this.authManager = authManager;
        }
       
        [HttpGet, Authorize]
        public async Task<IActionResult> GetUsersAsync([FromQuery] UserQueryParameters userQueryParameters)
        {
            try
            {
                List<User> result = await userService.FilterByAsync(userQueryParameters);
                List<GetCreatedUserDto> userDtos = result
                    .Select(user => mapper.Map<GetCreatedUserDto>(user))
                    .ToList();

                return StatusCode(StatusCodes.Status200OK, userDtos);
            }
            catch (EntityNotFoundException e)
            {
                return StatusCode(StatusCodes.Status404NotFound, e.Message);
            }
        }

        [HttpGet("id"),Authorize]
        public async Task<IActionResult> GetByIdAsync(int id)
        {
            try
            {
                User user = await userService.GetByIdAsync(id);
                GetCreatedUserDto userDto = mapper.Map<GetCreatedUserDto>(user);

                return StatusCode(StatusCodes.Status200OK, userDto);
            }
            catch (EntityNotFoundException e)
            {
                return StatusCode(StatusCodes.Status404NotFound, e.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateUserAsync([FromBody] CreateUserDto createUserDto)
        {
            try
            {
                GetCreatedUserDto createdUser = await userService.CreateAsync(createUserDto);

                return StatusCode(StatusCodes.Status201Created, createdUser);
            }
            catch (DuplicateEntityException e)
            {
                return StatusCode(StatusCodes.Status409Conflict, e.Message);
            }
        }

        [HttpPut("{id}"), Authorize]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] UpdateUserDto updateUserDto)
        {
            try
            {
                User loggedUser = await FindLoggedUserAsync();

                GetUpdatedUserDto updatedUser = await userService.UpdateAsync(id, updateUserDto, loggedUser);

                return StatusCode(StatusCodes.Status200OK, updatedUser);
            }
            catch (UnauthorizedOperationException e)
            {
                return StatusCode(StatusCodes.Status401Unauthorized, e.Message);
            }
            catch (DuplicateEntityException e)
            {
                return StatusCode(StatusCodes.Status409Conflict, e.Message);
            }
        }


        [HttpDelete("{id}"), Authorize]
        public async Task<IActionResult> DeleteUser(int id)
        {
            try
            {
                User loggedUser = await FindLoggedUserAsync();
                await userService.DeleteAsync(id, loggedUser);

                return StatusCode(StatusCodes.Status200OK, "User was successfully deleted.");
            }
            catch (EntityNotFoundException e)
            {
                return StatusCode(StatusCodes.Status404NotFound, e.Message);
            }
            catch (UnauthorizedOperationException e)
            {
                return StatusCode(StatusCodes.Status401Unauthorized, e.Message);
            }
        }

        [HttpPut("{id}/promote"), Authorize]
        public async Task<IActionResult> Promote(int id)
        {
            try
            {
                User loggedUser = await FindLoggedUserAsync();
                var promotedUser = await userService.PromoteAsync(id, loggedUser);

                return StatusCode(StatusCodes.Status200OK, "User was successfully promoted with admin rights.");
            }
            catch (EntityNotFoundException e)
            {
                return StatusCode(StatusCodes.Status404NotFound, e.Message);
            }
            catch (UnauthorizedOperationException e)
            {
                return StatusCode(StatusCodes.Status401Unauthorized, e.Message);
            }
        }

        [HttpPut("{id}/block"), Authorize]
        public async Task<IActionResult> BlockUser(int id)
        {
            try
            {
                User loggedUser = await FindLoggedUserAsync();
                var blockedUser = await userService.BlockUserAsync(id, loggedUser);

                return StatusCode(StatusCodes.Status200OK, "User was successfully blocked.");
            }
            catch (EntityNotFoundException e)
            {
                return StatusCode(StatusCodes.Status404NotFound, e.Message);
            }
            catch (UnauthorizedOperationException e)
            {
                return StatusCode(StatusCodes.Status401Unauthorized, e.Message);
            }
        }

        [HttpPut("{id}/unblock"), Authorize]
        public async Task<IActionResult> UnblockUser(int id)
        {
            try
            {
                User loggedUser = await FindLoggedUserAsync();
                var unblockedUser = await userService.UnblockUserAsync(id, loggedUser);

                return StatusCode(StatusCodes.Status200OK, "User was successfully unblocked.");
            }
            catch (EntityNotFoundException e)
            {
                return StatusCode(StatusCodes.Status404NotFound, e.Message);
            }
            catch (UnauthorizedOperationException e)
            {
                return StatusCode(StatusCodes.Status401Unauthorized, e.Message);
            }
        }
        private async Task<User> FindLoggedUserAsync()
        {
            var loggedUsersUsername = User.Claims.FirstOrDefault(claim => claim.Type == "Username").Value;
            var loggedUser = await this.userService.GetByUsernameAsync(loggedUsersUsername);
            return loggedUser;
        }
    }
}
