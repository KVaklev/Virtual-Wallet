using Business.DTOs.Requests;
using Business.DTOs.Responses;
using Business.QueryParameters;
using DataAccess.Models.Models;

namespace Business.ViewModels.CardViewModels
{
    public class CardViewModel
    {
        public CardQueryParameters CardQueryParameters { get; set; }

        public Response<PaginatedList<GetCreatedCardDto>> Cards { get; set; }

        public User Owner { get; set; }

        public CreateCardDto CreateCardDto { get; set; }

        public UpdateCardDto UpdateCardDto { get; set; }

    }
}
