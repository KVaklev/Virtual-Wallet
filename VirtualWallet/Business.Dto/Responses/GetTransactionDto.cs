using System.ComponentModel.DataAnnotations;

namespace Business.DTOs.Responses
{
    public class GetTransactionDto
    {
        public int Id { get; set; }
        public string SenderUsername { get; set; }
        public string RecipientUsername { get; set; }
        public DateTime Date { get; set; }
        public string CurrencyCode { get; set; }
        public decimal Amount { get; set; }
        public decimal AmountExchange { get; set; }
        public decimal ExchangeRate { get; set; }
        public string Direction { get; set; }
        public string Description { get; set; }
        public bool IsConfirmed { get; set; }
        public bool IsDeleted { get; set; }
    }
}
