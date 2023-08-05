using Business.DTOs;
using Business.DTOs.Requests;
using Business.DTOs.Responses;
using Business.QueryParameters;
using DataAccess.Models.Enums;
using DataAccess.Models.Models;

namespace Business.Services.Contracts
{
    public interface IUserService
    {
        Response<IQueryable<GetUserDto>> GetAll();
        Task<PaginatedList<User>> FilterByAsync(UserQueryParameters queryParameters);
        Task<Response<GetUserDto>> GetByIdAsync(int id, User loggedUser);
        Task<Response<GetUserDto>> GetByUsernameAsync(string username);
        Task<Response<GetCreatedUserDto>> CreateAsync(CreateUserModel createUserDto);
        Task<Response<GetUpdatedUserDto>> UpdateAsync(int id, UpdateUserDto updateUserDto, User loggedUser);
        Task<Response<bool>> DeleteAsync(int id, User loggedUser);
        Task<Response<GetUserDto>> PromoteAsync(int id, User loggedUser);
        Task<Response<GetUserDto>> BlockUserAsync(int id, User loggedUser);
        Task<Response<GetUserDto>> UnblockUserAsync(int id, User loggedUser);
        Task<Response<User>> LoginAsync(string username, string password);
    }
}
