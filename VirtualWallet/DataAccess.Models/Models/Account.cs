using System.Security.Principal;

namespace DataAccess.Models.Models
{
    public class Account
    {
        public int Id { get; set; }
        public DateTime DateCreated { get; set; }
        public int? UserId { get; set; }
        public User? User { get; set; }
        public decimal Balance { get; set; }
        public int? CurrencyId { get; set; }
        public Currency? Currency { get; set; }
        public List<Card> Cards { get; set; } = new List<Card>();
        public List<Transfer> Transfers { get; set; } = new List<Transfer>();
        public List<Transaction> TransactionsSender { get; set; } = new List<Transaction>();
        public List<Transaction> TransactionsRecipient { get; set; } = new List<Transaction>();

        //public string InvitationCode { get; set; }
    }
}
