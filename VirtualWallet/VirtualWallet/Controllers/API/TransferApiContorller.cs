using Business.Services.Contracts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace VirtualWallet.Controllers.API
{
    [ApiController]
    [Route("api/transfer")]
    public class TransferApiContorller : ControllerBase
    {
        private readonly ITransferService transferService;

        public TransferApiContorller(ITransferService transferservice)
        {
            this.transferService = transferservice;
        }
        [HttpGet]
        public IActionResult Withdraw(int id)
        {

            var transfers = transferService.GetAll()
                .Where(u => u.Id == id)
                                .ToList();

            var json = JsonSerializer.Serialize(transfers);

            return Ok(json);

        }

    }
}
