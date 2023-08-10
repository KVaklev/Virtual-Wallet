using Business.DTOs.Responses;
using Business.QueryParameters;
using DataAccess.Models.Models;

namespace Business.ViewModels
{
    public class UserSearchModel
    { 
        public UserQueryParameters UserQueryParameters { get; set; }

        public Response<PaginatedList<GetCreatedUserDto>> Users { get; set; }

        public UserChangeStatusViewModel UserChangeStatusViewModel { get; set; }
    }
}
