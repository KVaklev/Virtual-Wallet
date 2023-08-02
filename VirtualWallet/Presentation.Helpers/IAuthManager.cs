using DataAccess.Models.Models;

namespace Presentation.Helpers
{
    public interface IAuthManager
    {
        Task<User> TryGetUserByUsernameAsync(string username);
        Task<User> AuthenticateAsync(User loggedUser, string username, string password);
        void CheckForNullEntry(string username, string password);

        //Task<User> TryGetUserAsync(string credentials);
        //Task<User> TryGetUserAsync(string username, string password);
    }
}
