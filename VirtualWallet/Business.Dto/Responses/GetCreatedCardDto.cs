using System.ComponentModel.DataAnnotations;

namespace Business.DTOs.Responses
{
    public class GetCreatedCardDto
    {
        public int Id { get; set; }
        public string CardNumber { get; set; }
        public DateTime ExpirationDate { get; set; }
        public string CardHolder { get; set; }
        public string CheckNumber { get; set; }
        public string CardType { get; set; }
        public string Username { get; set; }
        public decimal Balance { get; set; }
        public decimal CreditLimit { get; set; }
    }
}
