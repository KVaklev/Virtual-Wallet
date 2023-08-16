using Business.DTOs.Responses;
using DataAccess.Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.ViewModels.TransactionViewModels
{
    public class ConfirmTransactionViewModel
    {
        public GetTransactionDto GetTransactionDto { get; set; }
        public User Recipient { get; set; }

        public decimal RecipientGetsAmount { get; set; }

    }
}
