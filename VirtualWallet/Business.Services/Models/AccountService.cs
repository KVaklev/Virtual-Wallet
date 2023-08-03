using AutoMapper;
using Business.DTOs.Requests;
using Business.Exceptions;
using Business.Mappers;
using Business.Services.Additional;
using Business.Services.Contracts;
using Business.Services.Helpers;
using DataAccess.Models.Models;
using DataAccess.Repositories.Contracts;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Business.Services.Models
{
    public class AccountService : IAccountService
    {
        private readonly IAccountRepository accountRepository;
        private readonly IUserRepository userRepository;
        private readonly ICardService cardService;
        private readonly ICurrencyService currencyService;
        private readonly IUserService userService;

        public AccountService(
            IAccountRepository accountRepository,  
            IUserRepository userRepository,
            ICardService cardService,
            ICurrencyService currencyService,
            IUserService userService)
        {
            this.accountRepository = accountRepository;
            this.userRepository = userRepository;
            this.cardService = cardService;
            this.currencyService = currencyService;
            this.userService = userService;
        }
        public IQueryable<Account> GetAll()
        {
            return accountRepository.GetAll();
        }
        public async Task<Account> GetByIdAsync(int id, User user)
        {
            if (!await Security.IsUserAuthorized(id, user))
            {
                throw new UnauthorizedOperationException(Constants.ModifyAccountErrorMessage);
            }
            return await this.accountRepository.GetByIdAsync(id);
        }
        public async Task<Account> GetByUsernameAsync(int id, User user)
        {
            if (!await Security.IsUserAuthorized(id, user))
            {
                throw new UnauthorizedOperationException(Constants.ModifyAccountErrorMessage);
            }
            return await accountRepository.GetByUsernameAsync(user.Username);
        }
        public async Task<Account> CreateAsync(string currencyCode, User user)
        {
            var accountToCreate = new Account();
            var currency = await currencyService.GetByCurrencyCodeAsync(currencyCode);
            accountToCreate = await AccountsMapper.MapCreateDtoToAccountAsync(accountToCreate, currency, user);
            
            user.AccountId = user.Id;
            accountToCreate = await this.accountRepository.CreateAsync(accountToCreate);

            return accountToCreate;
        }
        public async Task<bool> DeleteAsync(int id, User loggedUser)
        { 
            var accountToDelete = await this.GetByIdAsync(id, loggedUser);

            if (!await Security.IsAdminAsync(loggedUser))
            {
                throw new UnauthorizedOperationException(Constants.ModifyUserErrorMessage);
            }

            if (accountToDelete.Cards.Count != 0)
            {
                foreach (var card in accountToDelete.Cards)
                {
                    await this.cardService.DeleteAsync(card.Id, loggedUser);
                }
            }          
            return await this.accountRepository.DeleteAsync(id);
        }
        public async Task<bool> AddCardAsync(int id, Card card, User user)
        {
            if (!await Security.IsUserAuthorized(id, user))
            {
                throw new UnauthorizedOperationException(Constants.ModifyAccountCardErrorMessage);
            }

            if (!await this.cardService.CardNumberExistsAsync(card.CardNumber))
            {
                return await this.accountRepository.AddCardAsync(id, card);
            }
            return true;
        }
        public async Task<bool> RemoveCardAsync(int id, Card card, User user)
        {
            if (!await Security.IsUserAuthorized(id, user))
            {
                throw new UnauthorizedOperationException(Constants.ModifyAccountCardErrorMessage);
            }
            if (!await this.cardService.CardNumberExistsAsync(card.CardNumber))
            {
                return await this.accountRepository.RemoveCardAsync(id, card);
            }
            return true;
        }
        public async Task<string> CreateApiTokenAsync(User loggedUser)
        {
            var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("This is my secret testing key"));
            var signinCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);
            var jwtSecurityToken = new JwtSecurityToken(
                    issuer: "VirtualWallet",
                    audience: "Where is that audience",
                    claims: new[] {
                new Claim("LoggedUserId", loggedUser.Id.ToString()),
                new Claim("Username", loggedUser.Username),
                new Claim("IsAdmin", loggedUser.IsAdmin.ToString()),
                new Claim("UsersAccountId", loggedUser.Account.Id.ToString()),//null check
                new Claim(JwtRegisteredClaimNames.Email, loggedUser.Email),
                new Claim(JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString())
        },
            expires: DateTime.Now.AddMinutes(30),
                    signingCredentials: signinCredentials
                );
            string token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
         
            return await Task.FromResult(token);
        }
        public async Task<string> GenerateTokenAsync(int id)
        {
            var user = await this.userRepository.GetByIdAsync(id);
            var token = (DateTime.Now.ToString() + user.Username).ComputeSha256Hash();
            return token;
        }
        public async Task<bool> ConfirmRegistrationAsync(int id, string token)
        {
            return await this.accountRepository.ConfirmRegistrationAsync(id, token);
        }
        public async Task<User> LoginAsync(string username, string password)
        {
            await Security.CheckForNullEntryAsync(username, password);
            var loggedUser = await this.userService.GetByUsernameAsync(username);
            var authenticatedUser = await Security.AuthenticateAsync(loggedUser, username, password);

            return authenticatedUser;
        }

        public async Task<Account> IncreaseBalanceAsync(int id, decimal amount, User loggedUser)
        {
            Account accountToDepositTo = await this.GetByIdAsync(id, loggedUser);
            accountToDepositTo.Balance += amount;

            return await this.accountRepository.IncreaseBalanceAsync(id, amount);
        }

        public async Task<Account> DecreaseBalanceAsync(int id, decimal amount, User loggedUser)
        {
            Account accountToWithdrawFrom = await this.GetByIdAsync(id, loggedUser);
            accountToWithdrawFrom.Balance -= amount;

            return await this.accountRepository.DecreaseBalanceAsync(id, amount);
        }
    }
}
