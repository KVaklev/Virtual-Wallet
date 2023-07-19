using DataAccess.Models.Contracts;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Models.Models
{
    public class Transaction

    {
        public int Id { get; set; }

        public int SenderId { get; set; } //FK

        public IUser Sender { get; set; }

        public int RecipientId { get; set;} // FK

        public IUser Recipient { get; set; }

        [MinLength(2, ErrorMessage = "The {0} must be \"in\" or \"out\".")]
        [MaxLength(3, ErrorMessage = "The {0} must be \"in\" or \"out\".")]
        public string Direction { get; set; }

        public decimal Amount { get; set; }

        public DateTime date { get; set; }

        public int CurrencyId { get; set; } //FK

        public ICurrency Currency { get; set; }
    }
}
