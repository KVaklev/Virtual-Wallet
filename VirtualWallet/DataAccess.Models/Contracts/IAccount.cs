using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Models.Contracts
{
    public interface IAccount
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        public IUser User { get; set; }

        public decimal Balance { get; set; }

        public decimal DailyLimit { get; set; }

        public int CurrencyId { get; set; }

        public ICurrency Currency { get; set; }
    }
}
