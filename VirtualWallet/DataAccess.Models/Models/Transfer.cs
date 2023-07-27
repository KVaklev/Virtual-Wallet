using DataAccess.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace DataAccess.Models.Models
{
    public class Transfer
    {

        public int Id { get; set; }
        public DateTime DateCreated { get; set; }
        public int? AccountId { get; set; }
        public Account Account { get; set; }
        public int CurrencyId { get; set; }
        public Currency Currency { get; set; }
        public decimal Amount { get; set; }
        public TransferDirection TransferType { get; set; }
        public int CardId { get; set; }
        public Card Card { get; set; }
        public bool IsConfirmed { get; set; }
        public bool IsCancelled { get; set; }


    }
}
