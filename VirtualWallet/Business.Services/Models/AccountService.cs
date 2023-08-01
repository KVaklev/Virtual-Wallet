using AutoMapper;
using Business.DTOs;
using Business.Exceptions;
using Business.QueryParameters;
using Business.Services.Contracts;
using Business.Services.Helpers;
using DataAccess.Models.Models;
using DataAccess.Repositories.Contracts;

namespace Business.Services.Models
{
    public class AccountService : IAccountService
    {
        private readonly IAccountRepository accountRepository;
        private readonly ICardRepository cardRepository;
        private readonly ICurrencyRepository currencyRepository;

        public AccountService(IAccountRepository accountRepository, ICardRepository cardRepository, IMapper mapper, ICurrencyRepository currencyRepository)
        {
            this.accountRepository = accountRepository;
            this.cardRepository = cardRepository;
            this.currencyRepository = currencyRepository;
        }
        public IQueryable<Account> GetAll()
        {
            return accountRepository.GetAll();
        }


        public async Task<Account> CreateAsync(CreateAccountDto accountDto, User user)
        {
            Account account = new Account();
            account.Currency = await currencyRepository.GetByCurrencyCodeAsync(accountDto.currencyCode);
            account.CurrencyId = account.Currency.Id;
            
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
    }
}
