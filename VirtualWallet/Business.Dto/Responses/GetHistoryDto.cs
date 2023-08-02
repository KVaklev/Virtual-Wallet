using System.ComponentModel.DataAnnotations;

namespace Business.DTOs.Responses
{
    public class GetHistoryDto
    {
        [Required]
        public string NameOperation { get; set; }

        [Required]
        public string EventTime { get; set; }

        [Required]
        public string From { get; set; }

        [Required]
        public string To { get; set; }

        [Required]
        public decimal Amount { get; set; }

        [Required]
        public string CurrencyCode { get; set; }

        [Required]
        public string Direction { get; set; }
    }
}
