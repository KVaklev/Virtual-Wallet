using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.QueryParameters
{
    public class TransactionQueryParameters
    {
        public string? ResipientUsername { get; set; }
        public string? FromDate { get; set; }
        public string? ToDate { get; set; }
        public string? Direction { get; set; }
        public string? SortBy { get; set; }
        public int PageSize { get; set; } = 6;
        public int PageNumber { get; set; } = 1;
    }
}
