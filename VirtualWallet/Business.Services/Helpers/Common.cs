using Business.DTOs.Requests;
using DataAccess.Models.Models;
using DataAccess.Repositories.Contracts;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;

namespace Business.Services.Helpers
{
    public static class Common
    {
        public static async Task<bool> IsAdminAsync(User loggedUser)
        {
            if (!loggedUser.IsAdmin)
            {
                return await Task.FromResult(false);
            }
            return await Task.FromResult(true);
        }

        public static async Task<bool> IsAuthorizedAsync(User user, User loggedUser)
        {
            bool isAuthorized = false;

            if (user.Id == loggedUser.Id || loggedUser.IsAdmin)
            {
                isAuthorized = true;
            }
            return await Task.FromResult(isAuthorized);
        }

        public static async Task<bool> IsAuthorizedAsync(Card card, User loggedUser)
        {
            bool isAuthorized = false;

            if (card.Account.User.Id == loggedUser.Id || loggedUser.IsAdmin)
            {
                isAuthorized = true;
            }
            return await Task.FromResult(isAuthorized);
        }

        public static async Task<User> ComputePasswordHashAsync<T>(object dto, User user)
        {
            //Benefit one of using saltkey on hash - if users enter equal password - they are stored different in the db
            //Benefit of using SHA512 - it is not easily decrypted online over the internet available dictionaries

            byte[] passwordHash, passwordKey;
            string password = string.Empty;

            if (dto is CreateUserDto createDto)
            {
                password = createDto.Password;
            }
            else if (dto is UpdateUserDto updateDto)
            {
                password = updateDto.Password;
            }
            using (var hmac = new HMACSHA512())
            {
                passwordKey = hmac.Key;
                passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
            }

            user.Password = passwordHash;
            user.PasswordKey = passwordKey;

            return await Task.FromResult(user);
        }

    }
}
