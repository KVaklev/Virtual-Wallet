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
        public int Id { get; set; }

        public DateTime Date { get; set; }

        public double BalanceAmount { get; set; }

        public int UserId { get; set; }

        public int CurrencyId { get; set; }

        public double DepositAmount { get; set; }

        public double WithdrawalAmount { get; set; }
    }
}
