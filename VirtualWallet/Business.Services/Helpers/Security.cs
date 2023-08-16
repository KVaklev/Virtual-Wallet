using Business.DTOs.Requests;
using Business.Services.Contracts;
using DataAccess.Models.Enums;
using DataAccess.Models.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using static Business.Services.Helpers.Constants;

namespace Business.Services.Helpers
{
    public class Security : ISecurityService
    {
        public async Task<Response<string>> CreateApiTokenAsync(User loggedUser)
        {
            var result = new Response<string>();

            var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("This is my secret testing key"));
            var signinCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);
            var jwtSecurityToken = new JwtSecurityToken(
                    issuer: "VirtualWallet",
                    audience: "Where is that audience",
                    claims: new[] {
                new Claim(ClaimTypes.NameIdentifier, loggedUser.Id.ToString()),
                new Claim(ClaimTypes.Name, loggedUser.Username),
                new Claim("IsAdmin", loggedUser.IsAdmin.ToString()),
                new Claim("IsBlocked", loggedUser.IsBlocked.ToString()),
                new Claim("UsersAccountId", loggedUser.Account.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, loggedUser.Email),
                new Claim(JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString())
        },
            expires: DateTime.Now.AddMinutes(15),
                    signingCredentials: signinCredentials
                );

            string resultToken = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
            if (resultToken == null)
            {
                result.IsSuccessful = false;
                result.Message = GenerateTokenErrorMessage;
                return result;
            }
            result.Data = resultToken;

            return await Task.FromResult(result);
        }

        public async Task<bool> IsAdminAsync(User loggedUser)
        {
            if (!loggedUser.IsAdmin)
            {
                return await Task.FromResult(false);
            }
            return await Task.FromResult(true);
        }

        public async Task<Response<User>> AuthenticateAsync(User loggedUser, string password)
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


        public async Task<User> ComputePasswordHashAsync<T>(object dto, User user)
        {
            byte[] passwordHash, passwordKey;
            string password = string.Empty;

            if (dto is CreateUserModel createDto)
            {

                password = createDto.Password;
                using (var hmac = new HMACSHA512())
                {
                    passwordKey = hmac.Key;
                    passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
                }

                user.Password = passwordHash;
                user.PasswordKey = passwordKey;
            }
            else if (dto is UpdateUserDto updateDto)
            {
                if (updateDto.Password!=null)
                {
                    password = updateDto.Password;
                    using (var hmac = new HMACSHA512())
                    {
                        passwordKey = hmac.Key;
                        passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
                    }
                    user.Password = passwordHash;
                    user.PasswordKey = passwordKey;
                }  
            }

            return await Task.FromResult(user);
        }

        public async Task<bool> CanModifyTransactionAsync(Transaction transaction)
        {
            var canExecuteTransaction = true;
            if (transaction.IsConfirmed
                    || transaction.Direction == DirectionType.In
                    || transaction.IsDeleted)
            {
                canExecuteTransaction = false;
            }
            return await Task.FromResult(canExecuteTransaction);
        }

        public async Task<bool> IsHistoryOwnerAsync(History history, User user)
        {
            bool isHistoryOwner = true;

            if (history.AccountId != user.AccountId)
            {
                isHistoryOwner = false;
            }
            return await Task.FromResult(isHistoryOwner);
        }

        public async Task<bool> IsTransactionSenderAsync(Transaction transaction, int userId)
        {
            bool isTransactionSender = true;

            if (transaction.AccountSenderId != userId)
            {
                isTransactionSender = false;
            }
            return await Task.FromResult(isTransactionSender);
        }

        public async Task<bool> IsUserAuthorizedAsync(Transfer transfer, User user)
        {
            bool IsUserAuthorized = true;

            if (transfer.Account.UserId != user.Id)
            {
                IsUserAuthorized = false;
            }

            return await Task.FromResult(IsUserAuthorized);
        }

        public async Task<bool> IsAuthorizedAsync(User user, User loggedUser)
        {
            bool isAuthorized = false;

            if (user.Id == loggedUser.Id || loggedUser.IsAdmin)
            {
                isAuthorized = true;
            }
            return await Task.FromResult(isAuthorized);
        }

        public async Task<bool> IsAuthorizedAsync(Card card, User loggedUser)
        {
            bool isAuthorized = false;

            if (card.Account.User.Id == loggedUser.Id || loggedUser.IsAdmin)
            {
                isAuthorized = true;
            }
            return await Task.FromResult(isAuthorized);
        }

        public async Task<bool> IsUserAuthorized(int accountId, User user)
        {
            bool isUserAccountOwnerOrAdminId = false;

            if (accountId == user.Id || user.IsAdmin)
            {
                isUserAccountOwnerOrAdminId = true;
            }
            return await Task.FromResult(isUserAccountOwnerOrAdminId);
        }
        private async Task<bool> IsEmailConfirmedAsync(User loggedUser)
        {
            if (!loggedUser.IsVerified)
            {
                return await Task.FromResult(false);
            }
            return await Task.FromResult(true);
        }
        private async Task<bool> IsPasswordHashMatchedAsync(string passwordFilled, byte[] password, byte[]? passwordKey)
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

     
    }
}
