using Business.DTOs.Requests;
using Business.DTOs.Responses;

namespace Business.ViewModels.UserViewModels
{
    public class UpdateTransactionViewModel
    {
        public GetTransactionDto GetTransactionDto { get; set; }

        public List<CreateCurrencyDto> Currencies { get; set; }
    }
}
