using DataAccess.Models.Enums;

namespace DataAccess.Models.Models
{
    public class Card 
    {
        public int Id { get; set; }
        public int Number { get; set; }
        public DateTime ExpirationDate { get; set; }
        public int CheckNumber { get; set; }
        public string CardHolder { get; set; }
        public CardType CardType { get; set; }

        public int UserId { get; set; }
    }
}
