using Business.DTOs.Responses;
using Business.QueryParameters;
using DataAccess.Models.Models;

namespace Business.ViewModels.CardViewModels
{
    public class CardsAllUsersViewModel
    {
        public CardQueryParameters CardQueryParameters { get; set; }
        public Response<PaginatedList<GetCreatedCardDto>> Cards { get; set; }

    }
}
