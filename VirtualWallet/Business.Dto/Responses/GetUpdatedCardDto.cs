using DataAccess.Models.ValidationAttributes;
using System.ComponentModel.DataAnnotations;

namespace Business.DTOs.Responses
{
    public class GetUpdatedCardDto
    {
        public string CardNumber { get; set; }
        public DateTime ExpirationDate { get; set; }
        public string CardHolder { get; set; }
        public string CheckNumber { get; set; }
        public string CardType { get; set; }
        public decimal CreditLimit { get; set; }
        public string CurrencyCode { get; set; }
    }
}
