using Business.DTOs.Responses;
using DataAccess.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.ViewModels
{
    public class ExecuteTransferViewModel
    {
        public GetTransferDto GetTransferDto { get; set; }

        public List<GetCreatedCardDto> Cards { get; set; }

        public TransferDirection TransferDirection { get; set; }
    }
}
