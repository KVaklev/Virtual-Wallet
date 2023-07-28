

namespace Business.DTOs
{
    public class GetHistoryDto
    {
        public string From { get; set; }
        public string To { get; set; }
        public string Date { get; set; }
        public decimal Amount { get; set; }
        public string Аbbreviation { get; set; }
        public string NameOperation { get; set; }
        public string Direction { get; set; }
    }
}
