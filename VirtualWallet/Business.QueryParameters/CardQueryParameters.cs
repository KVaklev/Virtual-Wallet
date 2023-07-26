namespace Business.QueryParameters
{
    public class CardQueryParameters
    {
        public string? Username {  get; set; }
        public string? ExpirationDate { get; set; }
        public string? CardType { get; set; }
        public string? SortBy { get; set; }
        public string? SortOrder { get; set; }
        public decimal? Balance { get; set; }
    }
}
