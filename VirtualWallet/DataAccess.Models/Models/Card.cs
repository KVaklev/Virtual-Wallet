using DataAccess.Models.Contracts;
using DataAccess.Models.Enums;

namespace DataAccess.Models.Models
{
    public class Card : ICard
    {
        public int Id { get; set; }
        public int Number { get; set; }
        public DateTime ExpirationDate { get; set; }
        public int CheckNumber { get; set; }
        public string CardHolder { get; set; }
        public ICardType CardType { get; set; }

        public int UserId { get; set; }
    }
}
