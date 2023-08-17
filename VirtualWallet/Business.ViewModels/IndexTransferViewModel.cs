using Business.DTOs.Responses;
using Business.QueryParameters;
using DataAccess.Models.Models;

namespace Business.ViewModels
{
    public class IndexTransferViewModel
    {
        public PaginatedList<GetTransferDto> TransferDtos { get; set; }
        public TransferQueryParameters TransferQueryParameters { get; set; } = new TransferQueryParameters();
        public User User { get; set; }
    }
}
