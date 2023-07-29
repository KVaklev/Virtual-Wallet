using Business.Exceptions;
using Business.Services.Contracts;
using DataAccess.Models.Models;
using System.Text;

namespace Presentation.Helpers
{
    public class AuthManager : IAuthManager
    {
        private readonly IUserService userService;
        public AuthManager(IUserService userService)
        {
            this.userService = userService;
        }
        public async Task<User> TryGetUserAsync(string credentials)
        {
            string[] credentialsArray = credentials.Split(':');
            string username = credentialsArray[0];
            string password = credentialsArray[1];

            string encodedPassword = Convert.ToBase64String(Encoding.UTF8.GetBytes(password));

            try
            {
                var user = await this.userService.GetByUsernameAsync(username);
                if (user.Password == encodedPassword)
                {
                    return user;
                }
                throw new UnauthenticatedOperationException("Invalid credentials");
            }
            catch (EntityNotFoundException)
            {
                throw new UnauthorizedOperationException("Invalid username!");
            }
        }

        public async Task<User> TryGetUserAsync(string username, string password)
        {
            var user = await TryGetUserAsync(username + ":" + password);
            return user;
        }

        public async Task<User> TryGetUserByUsernameAsync(string username)
        {
            try
            {
                return await this.userService.GetByUsernameAsync(username);
            }
            catch (EntityNotFoundException)
            {
                throw new UnauthorizedOperationException("Invalid username!");
            }
        }

    }
}
