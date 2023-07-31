using DataAccess.Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Dto
{
    public class GetTransferDto
    {
        public string Username { get; set; }

        public DateTime DateCreated { get; set; }

        public string Abbreviation { get; set; }

        public string CardNumber { get; set; }

        public string TransferType { get; set; }

        public double Amount { get; set; }
    }
}
