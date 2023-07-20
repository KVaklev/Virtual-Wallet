using DataAccess.Models.Models;
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

        public User Sender { get; set; }

        public int RecipientId { get; set; }

        public User Recipient { get; set; }

        public string Direction { get; set; }

        public double Amount { get; set; }

        public DateTime date { get; set; }

        public int CurrencyId { get; set; }

        public Currency  Currency {get; set;}
    }
}
