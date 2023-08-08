﻿using Business.DTOs.Requests;
using DataAccess.Models.Enums;
using DataAccess.Models.Models;
using System.Security.Cryptography;
using System.Text;
using static Business.Services.Helpers.Constants;

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


        public async static Task<Response<bool>> CheckForNullEntryAsync(string username, string password)
        {
            var result = new Response<bool>();

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                result.IsSuccessful = false;
                result.Message = CredentialsErrorMessage;
                result.Error = new Error(PropertyName.Credentials);
            }

            return await Task.FromResult(result);
        }

        public static async Task<Response<User>> AuthenticateAsync(User loggedUser, string password)
        {
            var result = new Response<User>();

            if (!await IsPasswordHashMatchedAsync(password, loggedUser.Password, loggedUser.PasswordKey))
            {
                result.IsSuccessful = false;
                result.Message = FailedLoginAtemptErrorMessage;
                result.Error = new Error(PropertyName.PasswordHashMatch);
                return result;
            }

            if (!await IsEmailConfirmedAsync(loggedUser))
            {
                result.IsSuccessful = false;
                result.Message = NotConfirmedEmailErrorMessage;
                result.Error = new Error(PropertyName.NotConfirmedEmail);
                return result;
            }

            result.Data = loggedUser;

            return result;
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
            byte[] passwordHash, passwordKey;
            string password = string.Empty;

            if (dto is CreateUserModel createDto)
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
                return await Task.FromResult(false);
            }
            return await Task.FromResult(true);
        }

        public static async Task<bool> HasEnoughCardBalanceAsync(Card card, decimal amount)
        {
            if (card.Balance < amount)
            {
                return await Task.FromResult(false);
            }
            return await Task.FromResult(true);
        }

        public static async Task<bool> IsTransactionSenderAsync(Transaction transaction, int userId)
        {
            bool isTransactionSender = true;

            if (transaction.AccountSenderId != userId)
            {
                isTransactionSender = false;
            }
            return await Task.FromResult(isTransactionSender);
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
            return await Task.FromResult(canExecuteTransaction);
        }

        public static async Task<bool> IsHistoryOwnerAsync(History history, User user)
        {
            bool isHistoryOwner = true;

            if (history.AccountId != user.AccountId)
            {
                isHistoryOwner = false;
            }
            return await Task.FromResult(isHistoryOwner);
        }

        //ToDo-Check if we can combine and use only one with generic
        public static async Task<bool> IsUserAuthorizedAsync(Transfer transfer, User user)
        {
            bool IsUserAuthorized = true;

            if (transfer.Account.UserId != user.Id)
            {
                IsUserAuthorized = false;
            }

            return await Task.FromResult(IsUserAuthorized);
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

            if (accountId == user.Id || user.IsAdmin)
            {
                isUserAccountOwnerOrAdminId = true;
            }
            return await Task.FromResult(isUserAccountOwnerOrAdminId);
        }
    }
}
