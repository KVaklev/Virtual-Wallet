using Business.DTOs.Responses;
using Business.QueryParameters;
using DataAccess.Models.Models;

namespace Business.ViewModels
{
    public class IndexHistoryViewModel
    {
        public PaginatedList<GetHistoryDto> GetHistoryDtos { get; set; }
        public HistoryQueryParameters HistoryQueryParameters { get; set; }
        public User LoggedUser { get; set; }
    }
}
