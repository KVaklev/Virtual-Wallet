using Business.DTOs.Responses;
using DataAccess.Models.Models;

namespace Business.ViewModels.TransactionViewModels
{
    public class ConfirmTransactionViewModel
    {
        public GetTransactionDto GetTransactionDto { get; set; }
        public User Recipient { get; set; }
        public decimal RecipientGetsAmount { get; set; }
    }
}
