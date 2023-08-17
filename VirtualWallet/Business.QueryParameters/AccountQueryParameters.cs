﻿
namespace Business.QueryParameters
{
    public class AccountQueryParameters
    {
        public string? Username { get; set; }
        public string? FromDate { get; set; }
        public string? ToDate { get; set; }
        public string? Currencyabbrev { get; set; }
        public string? SortBy { get; set; }
        public int PageSize { get; set; } = 5;
        public int PageNumber { get; set; } = 1;
    }
}
