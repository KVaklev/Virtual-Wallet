using Business.DTOs.Requests;
using Business.DTOs.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.ViewModels
{
    public class UpdateTransactionViewModel
    {
        public GetTransactionDto GetTransactionDto { get; set; }

        public List<CreateCurrencyDto> Currencies { get; set; }
    }
}
