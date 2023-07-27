using DataAccess.Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Dto
{
    public class TransferDto
    {
        public DateTime Date { get; set; }

        public string Username { get; set; }

        public string CurrencyAbbreviation { get; set; }

        public string CardNumber { get; set; }

        public string TransferType { get; set; }

        public double Amount { get; set; }
    }
}
