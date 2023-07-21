using DataAccess.Models.Contracts;
using DataAccess.Models.Enums;
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
        public int Id { get; set; }
        public DateTime Date { get; set; }

        [Range(1, double.MaxValue)]
        public int UserId { get; set; }
        public IUser User { get; set; }

        [Range(1, double.MaxValue)]
        public int CurrencyId { get; set; }
        public ICurrency Currency { get; set; }

        [Range(0, double.MaxValue)]
        public double DepositAmount { get; set; }

        [Range(0, double.MaxValue)]
        public double WithdrawalAmount { get; set; }

        public Card Card { get; set; }

        public bool IsTransferred { get; set; }
    }
}
