namespace Business.DTOs.Responses
{
    public class GetCardDto
    {
        public string CardNumber { get; set; }
        public DateTime ExpirationDate { get; set; }
        public string CardHolder { get; set; }
        public string CheckNumber { get; set; }
        public string CardType { get; set; }
        public int AccountId { get; set; }
        public string Username { get; set; }
        public decimal Balance { get; set; }
    }
}
