using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Presentation.Helpers;

namespace VirtualWallet.Controllers.API
{
    [ApiController]
    [Route("api/cards")]
    public class CardsApiController : Controller
    {
        private readonly IMapper mapper;
        private readonly IAuthManager authManager;

        public CardsApiController(IMapper mapper, IAuthManager authManager)
        {
            this.mapper = mapper;
            this.authManager = authManager;
        }


    }
}
