using Business.DTOs.Responses;
using DataAccess.Models.Models;

namespace Business.ViewModels
{
    public class DetailsTransferViewModel
    {
        public GetTransferDto GetTransferDto { get; set; }
        public User LoggedUser { get; set; }
    }
}
