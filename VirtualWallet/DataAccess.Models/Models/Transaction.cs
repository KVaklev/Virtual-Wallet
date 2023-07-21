using DataAccess.Models.Enums;

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

        public DateTime Date { get; set; }

        public int CurrencyId { get; set; } //FK

        public Currency Currency { get; set; }

        public bool IsExecuted { get; set; }
    }
}
