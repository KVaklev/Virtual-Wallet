﻿using AutoMapper;
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
    [Route("api/cards")]
    public class CardApiController : ControllerBase
    {
        private readonly IMapper mapper;
        private readonly IAuthManager authManager;
        private readonly ICardService cardService;

        public CardApiController(
            IMapper mapper,
            IAuthManager authManager,
            ICardService cardService)
        {
            this.mapper = mapper;
            this.authManager = authManager;
            this.cardService = cardService;

        }

        [HttpGet, Authorize]
        public async Task<IActionResult> GetCardsAsync([FromQuery] CardQueryParameters cardQueryParameters)
        {
            try
            {
                List<Card> result = await cardService.FilterByAsync(cardQueryParameters);

                List<GetCardDto> cardDtos = result
                    .Select(card => mapper.Map<GetCardDto>(card))
                    .ToList();

                return StatusCode(StatusCodes.Status200OK, cardDtos);
            }
            catch (EntityNotFoundException e)
            {
                return StatusCode(StatusCodes.Status404NotFound, e.Message);
            }
        }

        [HttpGet("id"), Authorize]
        public async Task<IActionResult> GetByIdAsync(int id)
        {
            try
            {
                Card card = await cardService.GetByIdAsync(id);
                GetCardDto cardDto = mapper.Map<GetCardDto>(card);

                return StatusCode(StatusCodes.Status200OK, cardDto);
            }
            catch (EntityNotFoundException e)
            {
                return StatusCode(StatusCodes.Status404NotFound, e.Message);
            }
        }

        [HttpPost, Authorize]
        public async Task<IActionResult> CreateCardAsync([FromBody] CreateCardDto createCardDto)
        {
            try
            {
                var loggedUsersAccountId = await FindLoggedUsersAccountAsync();
                Card createCard = mapper.Map<Card>(createCardDto);
                Card createdCard = await cardService.CreateAsync(loggedUsersAccountId, createCard);

                return StatusCode(StatusCodes.Status201Created, createdCard);
            }
            catch (DuplicateEntityException e)
            {
                return StatusCode(StatusCodes.Status409Conflict, e.Message);
            }
        }

        [HttpPut, Authorize]
        public async Task<IActionResult> UpdateCardAsync(int id, [FromBody] UpdateCardDto updateCardDto)
        {
            try
            {
                User loggedUser = await FindLoggedUserAsync();
                var loggedUsersAccountId = await FindLoggedUsersAccountAsync();
                Card updateCard = mapper.Map<Card>(updateCardDto);
                Card updatedCard = await cardService.UpdateAsync(id, loggedUser, updateCard);

                return StatusCode(StatusCodes.Status200OK, updatedCard);
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
        public async Task<IActionResult> DeleteCardAsync(int id)
        {
            try
            {
                User loggedUser = await FindLoggedUserAsync();
                var isDeleted = await this.cardService.DeleteAsync(id, loggedUser);

                return StatusCode(StatusCodes.Status200OK);
            }
            catch (UnauthorizedOperationException e)
            {
                return StatusCode(StatusCodes.Status401Unauthorized, e.Message);
            }
            catch (EntityNotFoundException e)
            {
                return StatusCode(StatusCodes.Status404NotFound, e.Message);
            }
        }
        private async Task<User> FindLoggedUserAsync()
        {
            var loggedUsersUsername = User.Claims.FirstOrDefault(claim => claim.Type == "Username").Value;
            var loggedUser = await authManager.TryGetUserByUsernameAsync(loggedUsersUsername);
            return loggedUser;
        }
        private async Task<int> FindLoggedUsersAccountAsync()
        {
            var loggedUsersAccountIdAsString = User.Claims.FirstOrDefault(claim => claim.Type == "UsersAccountId").Value;
            var accountId = int.Parse(loggedUsersAccountIdAsString);
            return accountId;
        }

    }
}