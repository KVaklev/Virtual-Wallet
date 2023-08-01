using AutoMapper;
using Business.Exceptions;
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
        private readonly ICardRepository cardRepository;
        private readonly ICurrencyRepository currencyRepository;
        private readonly IUserRepository userRepository;

        public AccountService(IAccountRepository accountRepository, ICardRepository cardRepository, IMapper mapper, ICurrencyRepository currencyRepository, IUserRepository userRepository)
        {
            this.accountRepository = accountRepository;
            this.cardRepository = cardRepository;
            this.currencyRepository = currencyRepository;
            this.userRepository = userRepository;
        }
        public IQueryable<Account> GetAll()
        {
            return accountRepository.GetAll();
        }

        public async Task<Account> CreateAsync(string currencyCode, User user)
        {
            Account account = new Account();
            account.Currency = await currencyRepository.GetByCurrencyCodeAsync(accountDto.currencyCode);
            account.CurrencyId = account.Currency.Id;

            user.AccountId = user.Id;

            Account accountToCreate = await this.accountRepository.CreateAsync(account, user);

            return accountToCreate;
        }

        public async Task <bool> DeleteAsync(int id, User loggedUser)
        { 
            return await this.accountRepository.DeleteAsync(id);
        }

        public async Task <Account> GetByIdAsync(int id, User user)
        {
            if (!(await IsUserAuthorized(id, user)))
            {
                throw new UnauthorizedOperationException(Constants.ModifyAccountErrorMessage);
            }
            return await this.accountRepository.GetByIdAsync(id);
        }

        public async Task <Account> GetByUsernameAsync(int id, User user)
        {
            if (!(await IsUserAuthorized(id, user)))
            {
                throw new UnauthorizedOperationException(Constants.ModifyAccountErrorMessage);
            }
            return await accountRepository.GetByUsernameAsync(user.Username);
        }

        public async Task <bool> AddCardAsync(int id, Card card, User user)
        {
            if (!(await IsUserAuthorized(id, user)))
            {
                throw new UnauthorizedOperationException(Constants.ModifyAccountCardErrorMessage);
            }
            return await this.accountRepository.AddCardAsync(id, card);
        }

        public async Task <bool> RemoveCardAsync(int id, Card card, User user)
        {
            if (!(await IsUserAuthorized(id, user)))
            {
                throw new UnauthorizedOperationException(Constants.ModifyAccountCardErrorMessage);
            }
            return await this.accountRepository.RemoveCardAsync(id, card);
        }

        public async Task <bool> IsUserAuthorized(int id, User user)
        {
            bool IsUserAccountOwnerOrAdminId = false;

            Account accountToGet = await this.accountRepository.GetByIdAsync(id);

            if (accountToGet.UserId == user.Id || user.IsAdmin)
            {
                IsUserAccountOwnerOrAdminId = true;
            }

            return IsUserAccountOwnerOrAdminId;
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

    }
}
