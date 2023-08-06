namespace Business.DTOs.Responses
{
    public class GetAccountDto
    {
        public string Username { get; set; }
        public string CurrencyCode { get; set; }
        public string DateCreated { get; set; }
        public decimal Balance { get; set; }
    }
}

