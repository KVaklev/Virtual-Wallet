using Business.DTOs.Requests;
using Business.DTOs.Responses;
using DataAccess.Models.Enums;
using DataAccess.Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.ViewModels
{
    public class CreateTransferDepositViewModel
    {
        public CreateTransferDto CreateTransferDto { get; set; }

       // public User User { get; set; }

        public PaginatedList<GetCreatedCardDto> Cards { get; set; }

        public TransferDirection TransferDirection { get; set; }
    }
}
