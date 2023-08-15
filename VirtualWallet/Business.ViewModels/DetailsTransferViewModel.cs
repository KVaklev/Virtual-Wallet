using Business.DTOs.Responses;
using DataAccess.Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.ViewModels
{
    public class DetailsTransferViewModel
    {
        public GetTransferDto GetTransferDto { get; set; }

        public User LoggedUser { get; set; }
    }
}
