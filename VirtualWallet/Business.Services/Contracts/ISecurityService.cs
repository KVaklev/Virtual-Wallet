using DataAccess.Models.Models;

namespace Business.Services.Contracts
{
    public interface ISecurityService
    {
        Task<Response<string>> CreateApiTokenAsync(User loggedUser);
        Task<bool> IsAdminAsync(User loggedUser);
        Task<Response<User>> AuthenticateAsync(User loggedUser, string password);
        Task<User> ComputePasswordHashAsync<T>(object dto, User user);
        Task<bool> CanModifyTransactionAsync(Transaction transaction);
        Task<bool> IsHistoryOwnerAsync(History history, User user);
        Task<bool> IsTransactionSenderAsync(Transaction transaction, int userId);
        Task<bool> IsUserAuthorizedAsync(Transfer transfer, User user);
        Task<bool> IsAuthorizedAsync(User user, User loggedUser);
        Task<bool> IsAuthorizedAsync(Card card, User loggedUser);
        Task<bool> IsUserAuthorized(int accountId, User user);
    }
}
