using System.ComponentModel.DataAnnotations;

namespace Business.DTOs.Responses
{
    public class GetTransferDto
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public DateTime DateCreated { get; set; }
        public string CurrencyCode { get; set; }
        public string CardNumber { get; set; }
        public string TransferType { get; set; }
        public decimal Amount { get; set; }
    }
}
