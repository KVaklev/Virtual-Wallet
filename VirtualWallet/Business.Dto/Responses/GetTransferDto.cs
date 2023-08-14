using DataAccess.Models.Models;
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
        public decimal AmountExchange { get; set; }
        public decimal ExchangeRate { get; set; }

        public Card Card { get; set; }

        public User User { get; set; }

        public Account Account { get; set; }
        public string TransferType { get; set; }
        public decimal Amount { get; set; }

        public bool IsConfirmed { get; set; }

        public bool IsCancelled { get; set; }
    }
}
