using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.QueryParameters
{
    public class HistoryQueryParameters
    {
        public string? Username { get; set; }
        public string? FromDate { get; set; }
        public string? ToDate { get; set; }
        public int PageSize { get; set; } = 5;
        public int PageNumber { get; set; } = 1;
    }
}
