using AutoMapper;
using Business.Dto;
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
    public class UsersApiController : ControllerBase
    {
        private readonly IUserService userService;
        private readonly IMapper mapper;
        private readonly IAuthManager authManager;

        public UsersApiController(IUserService userService, IMapper mapper, IAuthManager authManager)
        {
            this.userService = userService;
            this.mapper = mapper;
            this.authManager = authManager;
        }
        //[HttpGet(""),Authorize]
        //public IActionResult GetUsers([FromQuery] UserQueryParameters userQueryParameters)
        //{
        //    try
        //    {
        //        List<User> result = userService.FilterBy(userQueryParameters);
        //        List<GetUserDto> userDtos = result
        //            .Select(user => mapper.Map<GetUserDto>(user))
        //            .ToList();

        //        return StatusCode(StatusCodes.Status200OK, userDtos);
        //    }
        //    catch (EntityNotFoundException e)
        //    {
        //        return StatusCode(StatusCodes.Status404NotFound, e.Message);
        //    }
        //}
        [HttpGet(""), Authorize]
        public async Task<IActionResult> GetUsers([FromQuery] UserQueryParameters userQueryParameters)
        {
            try
            {
                List<User> result = await userService.FilterByAsync(userQueryParameters);
                List<GetUserDto> userDtos = result
                    .Select(user => mapper.Map<GetUserDto>(user))
                    .ToList();

                return StatusCode(StatusCodes.Status200OK, userDtos);
            }
            catch (EntityNotFoundException e)
            {
                return StatusCode(StatusCodes.Status404NotFound, e.Message);
            }
        }

        [HttpGet("id"),Authorize]
        public IActionResult GetById(int id)
        {
            try
            {
                User user = userService.GetById(id);
                GetUserDto userDto = mapper.Map<GetUserDto>(user);

                return StatusCode(StatusCodes.Status200OK, userDto);
            }
            catch (EntityNotFoundException ex)
            {
                return StatusCode(StatusCodes.Status404NotFound, ex.Message);
            }
        }

        [HttpPost("")]
        public IActionResult CreateUser([FromBody] CreateUserDto createUserDto)
        {
            try
            {
                GetUserDto createdUser = await userService.CreateAsync(createUserDto);

                return StatusCode(StatusCodes.Status201Created, createdUser);
            }
            catch (DuplicateEntityException e)
            {
                return StatusCode(StatusCodes.Status409Conflict, e.Message);
            }
        }

        [HttpPut("{id}"), Authorize]
        public IActionResult UpdateUser(int id, [FromBody] UpdateUserDto updateUserDto)
        {
            try
            {
                User loggedUser = FindLoggedUser();
                User user = mapper.Map<User>(updateUserDto);
                User updatedUser = userService.Update(id, user, loggedUser);

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
        public IActionResult DeleteUser(int id)
        {
            try
            {
                User loggedUser = FindLoggedUser();
                userService.Delete(id, loggedUser);

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
        public IActionResult Promote(int id)
        {
            try
            {
                User loggedUser = FindLoggedUser();
                var promotedUser = userService.Promote(id, loggedUser);

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
        public IActionResult BlockUser(int id)
        {
            try
            {
                User loggedUser = FindLoggedUser();
                var blockedUser = userService.BlockUser(id, loggedUser);

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
        public IActionResult UnblockUser(int id)
        {
            try
            {
                User loggedUser = FindLoggedUser();
                var unblockedUser = userService.UnblockUser(id, loggedUser);

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
        private User FindLoggedUser()
        {
            var loggedUsersUsername = User.Claims.FirstOrDefault(claim => claim.Type == "Username").Value;
            var loggedUser = authManager.TryGetUserByUsername(loggedUsersUsername);
            return loggedUser;
        }
    }
}
