using Business.DTOs.Requests;
using DataAccess.Models.Models;
using Microsoft.AspNetCore.Mvc.Rendering;


namespace Business.ViewModels.TransactionViewModels
{
    public class CreateTransactionViewModel
    {
        public CreateTransactionDto CreateTransactionDto { get; set; }

    }
}
