using Business.DTOs.Responses;
using DataAccess.Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.ViewModels
{
    public class IndexHistoryViewModel
    {

        public PaginatedList<GetHistoryDto> GetHistoryDtos { get; set; }
    }
}
