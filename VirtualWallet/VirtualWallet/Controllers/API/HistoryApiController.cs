using AutoMapper;
using Business.DTOs;
using Business.QueryParameters;
using Business.Services.Contracts;
using DataAccess.Models.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Presentation.Helpers;

namespace VirtualWallet.Controllers.API
{
    [ApiController]
    [Route("api/hisrories")]
    public class HistoryApiController : ControllerBase
    {
        private readonly IHistoryService historyService;
        private readonly IAuthManager authManager;
        private readonly IMapper mapper;

        public HistoryApiController(
            IHistoryService historyService,
            IAuthManager authManager,
            IMapper mapper)
        {
            this.historyService = historyService;
            this.authManager = authManager;
            this.mapper = mapper;
        }

        [HttpGet("{id}"), Authorize]
        public IActionResult GetbyId(int id)
        {
            var loggedUser = FindLoggedUser();
            var history = this.historyService.GetById(id, loggedUser);
            var historyDto = this.mapper.Map<GetHistoryDto>(history);
            return StatusCode(StatusCodes.Status200OK, history);
        }

        [HttpGet, Authorize]
        public IActionResult GetHistory([FromQuery] HistoryQueryParameters historyQueryParameters) 
        {
            var loggedUser = FindLoggedUser();
            var history = this.historyService.FilterBy(historyQueryParameters, loggedUser);
            var historyDto = this.mapper.Map<GetHistoryDto>(history);
            return StatusCode(StatusCodes.Status200OK, history);
        }

        private User FindLoggedUser()
        {
            var loggedUsersUsername = User.Claims.FirstOrDefault(claim => claim.Type == "Username").Value;
            var loggedUser = authManager.TryGetUserByUsername(loggedUsersUsername);
            return loggedUser;
        }
    }
}
