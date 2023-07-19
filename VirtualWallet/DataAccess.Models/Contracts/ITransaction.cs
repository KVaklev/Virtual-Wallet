using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Models.Contracts
{
    internal interface ITransaction
    {
        public int Id { get; set; }

        public int SenderId { get; set; }

        public IUser Sender { get; set; }

        public int RecipientId { get; set; }

        public IUser Recipient { get; set; }

        public string Direction { get; set; }

        public decimal Amount { get; set; }

        public DateTime date { get; set; }

        public int CurrencyId { get; set; }

        public ICurrency  Currency {get; set;}
    }
}
