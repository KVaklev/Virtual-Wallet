using System.ComponentModel.DataAnnotations;

namespace Business.DTOs.Responses
{
    public class GetTransferDto
    {
        public string Username { get; set; }
        public DateTime DateCreated { get; set; }
        public string CurrencyCode { get; set; }
        public string CardNumber { get; set; }
        public string TransferType { get; set; }
        public double Amount { get; set; }
    }
}
