
using Business.DTOs.Requests;
using Microsoft.AspNetCore.Mvc.Rendering;


namespace Business.ViewModels
{
    public class CreateTransactionViewModel
    {
        public CreateTransactionDto CreateTransactionDto { get; set; }

        public IQueryable<CreateCurrencyDto> Currencies { get; set; }
    }
}
