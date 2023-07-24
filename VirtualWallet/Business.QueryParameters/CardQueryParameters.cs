namespace Business.QueryParameters
{
    public class CardQueryParameters
    {
        public string? Username {  get; set; }
        public string? CardNumber { get; set; }
        public DateTime? ExpirationDate { get; set; }
        public string? CardType { get; set; }
        public string? CardHolder { get; set; }
        public string? CheckNumber { get; set; }
        public int? UserId { get; set;}
        public int? AccountId { get; set; }
        public string? SortBy { get; set; }
        public string? SortOrder { get; set; }
        public decimal? Balance { get; set; }
    }
}
