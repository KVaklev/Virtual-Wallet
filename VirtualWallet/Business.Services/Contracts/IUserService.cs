using Business.DTOs.Requests;
using Business.DTOs.Responses;
using Business.QueryParameters;
using DataAccess.Models.Models;

namespace Business.Services.Contracts
{
    public interface IUserService
    {
        IQueryable<User> GetAll();
        Task<PaginatedList<User>> FilterByAsync(UserQueryParameters queryParameters);
        Task<User> GetByIdAsync(int id);
        Task<User> GetByUsernameAsync(string username);
        Task<User> GetByEmailAsync(string email);
        Task<User> GetByPhoneNumberAsync(string phoneNumber);
        Task<GetCreatedUserDto> CreateAsync(CreateUserDto createUserDto);
        Task<GetUpdatedUserDto> UpdateAsync(int id, UpdateUserDto updateUserDto, User loggedUser);
        Task<bool> DeleteAsync(int id, User loggedUser);
        Task<User> PromoteAsync(int id, User loggedUser);
        Task<User> BlockUserAsync(int id, User loggedUser);
        Task<User> UnblockUserAsync(int id, User loggedUser);
    }
}
