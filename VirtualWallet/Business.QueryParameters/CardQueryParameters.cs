namespace Business.QueryParameters
{
    public class CardQueryParameters
    {
        public string? ExpirationDate { get; set; }
        public string? CardType { get; set; }
        public string? SortBy { get; set; }
        public string? SortOrder { get; set; }
        public decimal? Balance { get; set; }
        public int PageSize { get; set; } = 5;
        public int PageNumber { get; set; } = 1;
    }
}
