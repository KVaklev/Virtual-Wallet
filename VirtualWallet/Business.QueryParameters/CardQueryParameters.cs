namespace Business.QueryParameters
{
    public class CardQueryParameters
    {
        public string? Username {  get; set; }
        public string? CardNumber { get; set; }
        public DateTime? ExpirationDate { get; set; }
        public string? CardType { get; set; }
        public string? CardHolder { get; set; }
        public int? CheckNumber { get; set; }
        public string? UserId { get; set;}
        public string? SortBy { get; set; }
        public string? SortOrder { get; set; }
    }
}
