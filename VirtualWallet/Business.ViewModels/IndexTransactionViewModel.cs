using Business.DTOs.Responses;
using Business.QueryParameters;
using DataAccess.Models.Models;

namespace Business.ViewModels
{
    public class IndexTransactionViewModel
    {
        public PaginatedList<GetTransactionDto> TransactionDtos { get; set; }

        public TransactionQueryParameters TransactionQueryParameters { get; set; } = new TransactionQueryParameters();

        public User User { get; set; }
    }
}
