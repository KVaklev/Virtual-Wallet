using AutoMapper;
using Business.Dto;
using Business.Exceptions;
using Business.QueryParameters;
using Business.Services.Contracts;
using Business.Services.Models;
using DataAccess.Models.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Presentation.Helpers;

namespace VirtualWallet.Controllers.API
{
    [ApiController]
    [Route("api/cards")]
    public class CardsApiController : ControllerBase
    {
        private readonly IMapper mapper;
        private readonly IAuthManager authManager;
        private readonly ICardService cardService;

        public CardsApiController(IMapper mapper, IAuthManager authManager, ICardService cardService)
        {
            this.mapper = mapper;
            this.authManager = authManager;
            this.cardService = cardService;
        }

        [HttpGet, Authorize]
        public IActionResult GetCards([FromQuery] CardQueryParameters cardQueryParameters)
        {
            try
            {
                List<Card> result = cardService.FilterBy(cardQueryParameters);

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
    }
}
