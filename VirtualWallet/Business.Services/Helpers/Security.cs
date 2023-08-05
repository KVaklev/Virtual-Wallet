using Business.DTOs.Requests;
using Business.Exceptions;
using Business.Services.Contracts;
using DataAccess.Models.Enums;
using DataAccess.Models.Models;
using System.Security.Cryptography;
using System.Text;

namespace Business.Services.Helpers
{
    public static class Security
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

        public static async Task<bool> IsUserAuthorized(int accountId, User user)
        {
            bool isUserAccountOwnerOrAdminId = false;

            //Account accountToGet = await this.accountRepository.GetByIdAsync(id);

            if (accountId == user.Id || user.IsAdmin)
            {
                isUserAccountOwnerOrAdminId = true;
            }

            return await Task.FromResult(isUserAccountOwnerOrAdminId);
        }
        public static Task CheckForNullEntryAsync(string username, string password)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                throw new ArgumentException("Username and/or Password not specified");
            }
            return Task.FromResult(true);
        }

        public static async Task<User> AuthenticateAsync(User loggedUser, string username, string password)
        {
            if (!await IsPasswordHashMatchedAsync(password, loggedUser.Password, loggedUser.PasswordKey))
            {
                throw new UnauthorizedOperationException("Nice try! Invalid credentials!");
            }

            if (!await IsEmailConfirmedAsync(loggedUser))
            {
                throw new UnauthorizedOperationException("Your email is not confirmed, please check your inbox folder and follow the link!");
            }

            return loggedUser;
        }

        private static async Task<bool> IsPasswordHashMatchedAsync(string passwordFilled, byte[] password, byte[]? passwordKey)
        {
            using (var hmac = new HMACSHA512(passwordKey))
            {
                var passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(passwordFilled));
                for (var i = 0; i < passwordHash.Length; i++)
                {
                    if (passwordHash[i] != password[i])
                    {
                        return await Task.FromResult(false);
                    }
                }
                return await Task.FromResult(true);
            }
        }
        private static async Task<bool> IsEmailConfirmedAsync(User loggedUser)
        {
            if (!loggedUser.IsVerified)
            {
                return await Task.FromResult(false);
            }
            return await Task.FromResult(true);
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
        public static async Task<bool> HasEnoughBalanceAsync(Account account, decimal amount)
        {
            if (account.Balance < amount)
            {
                return false;
            }
            return true;
        }

        public static async Task<bool> HasEnoughCardBalanceAsync(Card card, decimal amount)
        {
            if (card.Balance < amount)
            {
                return false;
            }
            return true;
        }

        public static async Task<bool> IsTransactionSenderAsync(Transaction transaction, int userId)
        {
            bool isTransactionSender = true;

            if (transaction.AccountSender.User.Id != userId)
            {
                isTransactionSender = false;
            }
            return isTransactionSender;
        }

        public static async Task<bool> IsUserAuthorizedAsync(Transfer transfer, User user)
        {
            bool IsUserAuthorized = true;

            if (transfer.Account.UserId != user.Id)
            {
                IsUserAuthorized = false;
            }

            return IsUserAuthorized;

        }

        public static async Task<bool> CanModifyTransactionAsync(Transaction transaction)
        {
            var canExecuteTransaction = true;
            if (transaction.IsExecuted
                    || transaction.Direction == DirectionType.In
                    || transaction.IsDeleted)
            {
                canExecuteTransaction = false;
            }
            return canExecuteTransaction;
        }

        public static async Task<bool> IsHistoryOwnerAsync(History history, User user)
        {
            bool isHistoryOwner = true;

            if (history.AccountId != user.AccountId)
            {
                isHistoryOwner = false;
            }
            return isHistoryOwner;
        }
    }
}
