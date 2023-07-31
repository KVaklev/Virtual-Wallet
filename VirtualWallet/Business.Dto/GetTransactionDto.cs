using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.DTOs
{
    public class GetTransactionDto
    {
        public string RecipientUsername { get; set; }

        public DateTime Date { get; set;}

        public string CurrencyCode { get; set;}

        public decimal Amount { get; set;}

        public string Direction { get; set;}

    }
}
