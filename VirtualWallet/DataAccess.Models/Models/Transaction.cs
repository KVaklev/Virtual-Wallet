using DataAccess.Models.Contracts;
using DataAccess.Models.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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

        public User Sender { get; set; }

        public int RecipientId { get; set;} // FK

        public User Recipient { get; set; }

        public DirectionType Direction { get; set; }

        public double Amount { get; set; }

        public DateTime date { get; set; }

        public int CurrencyId { get; set; } //FK

        public Currency Currency { get; set; }
    }
}
