using Business.Exceptions;
using Business.Services.Contracts;
using DataAccess.Models.Models;
using System.Security.Cryptography;
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
        //public async Task<User> TryGetUserByUsernameAsync(string username)
        //{
        //    try
        //    {
        //        return await this.userService.GetByUsernameAsync(username);
        //    }
        //    catch (EntityNotFoundException)
        //    {
        //        throw new UnauthorizedOperationException("Invalid username!");
        //    }
        //}

        //public Task CheckForNullEntryAsync(string username, string password)
        //{
        //    if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
        //    {
        //        throw new ArgumentException("Username and/or Password not specified");
        //    }
        //    return Task.FromResult(true);
        //}

        //public async Task<User> AuthenticateAsync(User loggedUser, string username, string password)
        //{
        //    if (!await IsPasswordHashMatchedAsync(password, loggedUser.Password, loggedUser.PasswordKey))
        //    {
        //        throw new UnauthorizedOperationException("Nice try! Invalid credentials!");
        //    }

        //    if (!await IsEmailConfirmedAsync(loggedUser))
        //    {
        //        throw new UnauthorizedOperationException("Your email is not confirmed, please check your inbox folder and follow the link!");
        //    }

        //    return loggedUser;
        //}

        //private async Task<bool> IsPasswordHashMatchedAsync(string passwordFilled, byte[] password, byte[]? passwordKey)
        //{
        //    using (var hmac = new HMACSHA512(passwordKey))
        //    {
        //        var passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(passwordFilled));
        //        for (var i = 0; i < passwordHash.Length; i++)
        //        {
        //            if (passwordHash[i] != password[i])
        //            {
        //                return await Task.FromResult(false);
        //            }
        //        }
        //        return await Task.FromResult(true);
        //    }
        //}
        //private async Task<bool> IsEmailConfirmedAsync(User loggedUser)
        //{
        //    if (!loggedUser.IsVerified)
        //    {
        //        return await Task.FromResult(false);
        //    }
        //    return await Task.FromResult(true);
        //}


        //public async Task<User> TryGetUserAsync(string credentials)
        //{
        //    string[] credentialsArray = credentials.Split(':');
        //    string username = credentialsArray[0];
        //    string password = credentialsArray[1];

        //    string encodedPassword = Convert.ToBase64String(Encoding.UTF8.GetBytes(password));

        //    try
        //    {
        //        var user = await this.userService.GetByUsernameAsync(username);
        //        //if (user.Password == encodedPassword)
        //        //{
        //            return user;
        //        //}
        //        throw new UnauthenticatedOperationException("Invalid credentials");
        //    }
        //    catch (EntityNotFoundException)
        //    {
        //        throw new UnauthorizedOperationException("Invalid username!");
        //    }
        //}

        //public async Task<User> TryGetUserAsync(string username, string password)
        //{
        //    var user = await TryGetUserAsync(username + ":" + password);
        //    return user;
        //}

    }
}
