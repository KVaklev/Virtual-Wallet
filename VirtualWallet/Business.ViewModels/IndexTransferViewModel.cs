using Business.DTOs.Responses;
using Business.QueryParameters;
using DataAccess.Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.ViewModels
{
    public class IndexTransferViewModel
    {
        public PaginatedList<GetTransferDto> TransferDtos { get; set; }

        public TransferQueryParameters TransferQueryParameters { get; set; } = new TransferQueryParameters();
    }
}
