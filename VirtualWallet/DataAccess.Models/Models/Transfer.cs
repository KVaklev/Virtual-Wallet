using System.ComponentModel.DataAnnotations;

namespace DataAccess.Models.Models
{
    public class Transfer
    {

        public int Id { get; set; }
        public DateTime Date { get; set; }
        public int? AccountId { get; set; }
        public Account Account { get; set; }
        public int CurrencyId { get; set; }
        public Currency Currency { get; set; }
        public decimal Amount { get; set; }
        public int CardId { get; set; }
        public Card Card { get; set; }
        public bool IsTransferred { get; set; }


    }
}
