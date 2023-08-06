using System.ComponentModel.DataAnnotations;

namespace Business.DTOs.Responses
{
    public class GetHistoryDto
    {
        public string NameOperation { get; set; }
        public string EventTime { get; set; }
        public string From { get; set; }
        public string To { get; set; }
        public decimal Amount { get; set; }
        public string CurrencyCode { get; set; }
        public string Direction { get; set; }
    }
}
