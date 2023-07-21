using DataAccess.Models.Models;

namespace Presentation.Helpers
{
    public interface IAuthManager
    {
        User TryGetUser(string credentials);
        User TryGetUserByUsername(string username);
        User TryGetUser(string username, string password);
    }
}
