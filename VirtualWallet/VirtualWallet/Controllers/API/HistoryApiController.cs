using Business.QueryParameters;
using Business.Services.Contracts;
using Business.Services.Helpers;
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

        [HttpGet, Authorize]
        public async Task<IActionResult> GetHistoryAsync([FromQuery] HistoryQueryParameters historyQueryParameters) 
        {
           
           var loggedUserResult = await userService.FindLoggedUserAsync(User.FindFirst(ClaimTypes.Name)?.Value!);
            if (!loggedUserResult.IsSuccessful)
           {
                return StatusCode(StatusCodes.Status401Unauthorized, loggedUserResult.Message);
           }

           var result = await this.historyService.FilterByAsync(historyQueryParameters, loggedUserResult.Data);
           if (!result.IsSuccessful)
           {
                if (result.Message == Constants.NoRecordsFound)
                {
                    return StatusCode(StatusCodes.Status404NotFound, result.Message);
                }
                return BadRequest(result.Message);
           }

           return StatusCode(StatusCodes.Status200OK, result.Data);   
        }
    }
}
