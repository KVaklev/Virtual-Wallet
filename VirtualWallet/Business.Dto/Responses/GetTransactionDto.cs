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
        public string Direction { get; set; }
        public bool IsExecuted { get; set; }
        public bool IsDeleted { get; set; }
    }
}
