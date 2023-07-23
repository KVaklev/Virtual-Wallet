using DataAccess.Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Dto
{
    public class TransferWithdrawalDto
    {
        public DateTime Date { get; set; }

        public User User { get; set; }

        public Currency Currency { get; set; }

        public double WithdrawalAmount { get; set; }
    }
}
