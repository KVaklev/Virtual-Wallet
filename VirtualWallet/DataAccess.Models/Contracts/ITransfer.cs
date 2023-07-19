using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Models.Contracts
{
    public interface ITransfer
    {
        public int ID { get; set; }

        public DateTime Date { get; set; }

        public decimal BalanceAmount { get; set; }

        public int UserID { get; set; }

        public int CurrencyID { get; set; }

        public decimal DepositAmount { get; set; }

        public decimal WithdrawalAmount { get; set; }
    }
}
