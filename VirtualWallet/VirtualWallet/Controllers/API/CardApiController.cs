
using Business.DTOs.Requests;
using Business.QueryParameters;
using Business.Services.Contracts;
using DataAccess.Models.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace VirtualWallet.Controllers.API
{
    [ApiController]
    [Route("api/cards")]
    public class CardApiController : ControllerBase
    {
        private readonly ICardService cardService;
        private readonly IUserService userService;

        public CardApiController(
            ICardService cardService, 
            IUserService userService)
        {
            this.cardService = cardService;
            this.userService = userService;
        }

        [HttpGet, Authorize]
        public async Task<IActionResult> GetCardsAsync([FromQuery] CardQueryParameters cardQueryParameters)
        {
            var loggedUserResponse = await userService.FindLoggedUserAsync(User.FindFirst(ClaimTypes.Name)?.Value!);
            if (!loggedUserResponse.IsSuccessful)
            {
                return StatusCode(StatusCodes.Status404NotFound, loggedUserResponse.Message);
            }

            var result = await cardService.FilterByAsync(cardQueryParameters, loggedUserResponse.Data);
            if (!result.IsSuccessful)
            {
                return BadRequest(result.Message);
            }
            
            return StatusCode(StatusCodes.Status200OK, result.Data);
        }

        [HttpGet("id"), Authorize]
        public async Task<IActionResult> GetByIdAsync(int id)
        {
            var loggedUserResponse = await userService.FindLoggedUserAsync(User.FindFirst(ClaimTypes.Name)?.Value!);
            if (!loggedUserResponse.IsSuccessful)
            {
                return StatusCode(StatusCodes.Status404NotFound, loggedUserResponse.Message);
            }

            var result = await cardService.GetByIdAsync(id, loggedUserResponse.Data);
            if (!result.IsSuccessful)
            {
                return BadRequest(result.Message);
            }

            return StatusCode(StatusCodes.Status200OK, result.Data);         
        }

        [HttpPost, Authorize]
        public async Task<IActionResult> CreateCardAsync([FromBody] CreateCardDto createCardDto)
        {
            var loggedUsersAccountId = await FindLoggedUsersAccountAsync();
            if (!loggedUsersAccountId.IsSuccessful)
            {
                return StatusCode(StatusCodes.Status404NotFound, loggedUsersAccountId.Message);
            }

            var result = await cardService.CreateAsync(loggedUsersAccountId.Data, createCardDto);
            if (!result.IsSuccessful) 
            {
                return BadRequest(result.Message);
            }

            return StatusCode(StatusCodes.Status201Created, result);          
        }

        [HttpPut("{id}"), Authorize]
        public async Task<IActionResult> UpdateCard(int id, [FromBody] UpdateCardDto updateCardDto)
        {

            var loggedUserResponse = await userService.FindLoggedUserAsync(User.FindFirst(ClaimTypes.Name)?.Value!);
            if (!loggedUserResponse.IsSuccessful)
            {
                return StatusCode(StatusCodes.Status404NotFound, loggedUserResponse.Message);
            }

            var result = await this.cardService.UpdateAsync(id, loggedUserResponse.Data, updateCardDto);
            if (!result.IsSuccessful)
            {
                return BadRequest(result.Message);
            }

            return StatusCode(StatusCodes.Status200OK, result);
        }

        [HttpDelete("{id}"), Authorize]
        public async Task<IActionResult> DeleteCardAsync(int id)
        {
            var loggedUserResponse = await userService.FindLoggedUserAsync(User.FindFirst(ClaimTypes.Name)?.Value!);
            if (!loggedUserResponse.IsSuccessful)
            {
                return StatusCode(StatusCodes.Status404NotFound, loggedUserResponse.Message);
            }

            var result = await this.cardService.DeleteAsync(id, loggedUserResponse.Data);
            if (!result.IsSuccessful)
            {
                return BadRequest(result.Message);
            }

            return StatusCode(StatusCodes.Status200OK, result.Message);
        }

        private async Task<Response<int>> FindLoggedUsersAccountAsync()
        {
            var result = new Response<int>();
            var loggedUsersAccountIdAsString = User.Claims?.FirstOrDefault(claim => claim.Type == "UsersAccountId")?.Value!;
            var accountId = int.Parse(loggedUsersAccountIdAsString);
            result.Data = accountId;

            return await Task.FromResult(result);
        }

    }
}
