using Business.DTOs.Requests;
using DataAccess.Models.Models;

namespace Business.ViewModels.CardViewModels
{
    public class CreateCardViewModel
    {
        public CreateCardDto CreateCardDto { get; set; }

        public Account Account { get; set; }
    }
}
