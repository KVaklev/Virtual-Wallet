using Business.DTOs.Responses;
using DataAccess.Models.Models;

namespace Business.ViewModels
{
    public class IndexTransactionViewModel
    {
        public PaginatedList<GetTransactionDto> TransactionDtos { get; set; }
    }
}
