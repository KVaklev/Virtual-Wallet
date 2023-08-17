using Business.DTOs.Responses;
using DataAccess.Models.Enums;

namespace Business.ViewModels
{
    public class ExecuteTransferViewModel
    {
        public GetTransferDto GetTransferDto { get; set; }
        public List<GetCreatedCardDto> Cards { get; set; }
        public TransferDirection TransferDirection { get; set; }
    }
}
