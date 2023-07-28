using DataAccess.Models.Models;

namespace Presentation.Helpers
{
    public interface IAuthManager
    {
        Task<User> TryGetUserAsync(string credentials);
        Task<User> TryGetUserByUsernameAsync(string username);
        Task<User> TryGetUserAsync(string username, string password);
    }
}
