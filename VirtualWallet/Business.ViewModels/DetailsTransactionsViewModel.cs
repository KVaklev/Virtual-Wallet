using Business.DTOs.Responses;
using DataAccess.Models.Models;

namespace Business.ViewModels
{
    public class DetailsTransactionsViewModel
    {
        public GetTransactionDto GetTransactionDto { get; set; }

        public User SenderUser { get; set; }
        
        public User RecipientUser { get; set; }
        public User LoggedUser { get; set; }
    }
}
