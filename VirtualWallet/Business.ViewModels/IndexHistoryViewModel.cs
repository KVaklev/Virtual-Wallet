using Business.DTOs.Responses;
using DataAccess.Models.Models;

namespace Business.ViewModels
{
    public class IndexHistoryViewModel
    {
        public PaginatedList<GetHistoryDto> GetHistoryDtos { get; set; }
    }
}
