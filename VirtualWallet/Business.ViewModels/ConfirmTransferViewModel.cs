using Business.DTOs.Responses;
using DataAccess.Models.Models;

namespace Business.ViewModels
{
    public class ConfirmTransferViewModel
    {
        public GetTransferDto GetTransferDto { get; set; }
        public User UserDetails { get; set; }
    }
}
