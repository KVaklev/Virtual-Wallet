using DataAccess.Models.Contracts;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Models.Models
{
    public class Transfer : ITransfer
    {

        [Range(1, double.MaxValue)]
        public int Id { get ; set ; }
        public DateTime Date { get ; set ; }

        [Range(0, double.MaxValue)]
        public decimal BalanceAmount { get ; set ; }

        [Range(1, double.MaxValue)]
        public int UserId { get ; set ; }

        [Range(1, double.MaxValue)]
        public int CurrencyId { get ; set ; }

        [Range(0, double.MaxValue)]
        public decimal DepositAmount { get ; set ; }

        [Range(0, double.MaxValue)]
        public decimal WithdrawalAmount { get ; set ; }
    }
}
