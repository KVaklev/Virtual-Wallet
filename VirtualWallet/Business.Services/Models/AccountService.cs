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
        private readonly IMapper mapper;
        private readonly ICurrencyRepository currencyRepository;

        public AccountService(IAccountRepository accountRepository, ICardRepository cardRepository, IMapper mapper, ICurrencyRepository currencyRepository)
        {
            this.accountRepository = accountRepository;
            this.cardRepository = cardRepository;
            this.mapper = mapper;
            this.currencyRepository = currencyRepository;
        }
        public async Task<IQueryable<Account>> GetAll()
        {
            return await accountRepository.GetAll();
        }

        public PaginatedList<Account> FilterBy(AccountQueryParameters filterParameters)
        {
            return this.accountRepository.FilterBy(filterParameters);
        }

        public async Task<Account> Create(CreateAccountDto accountDto, User user)
        {
            Account account = mapper.Map<Account>(accountDto);
            account.CurrencyId = currencyRepository.GetByАbbreviationAsync(accountDto.Abbreviation).Id;

            Account accountToCreate = await this.accountRepository.Create(account, user);

            return accountToCreate;
        }

        public bool Delete(int id, User loggedUser)
        {
            if (!IsUserAuthorized(id, loggedUser))
            {
                throw new UnauthorizedOperationException(Constants.ModifyAccountErrorMessage);
            }

            return this.accountRepository.Delete(id);
        }

        public Account GetById(int id, User user)
        {
            if (!IsUserAuthorized(id, user))
            {
                throw new UnauthorizedOperationException(Constants.ModifyAccountErrorMessage);
            }
            return this.accountRepository.GetById(id);
        }

        public async Task <Account> GetByUsernameAsync(int id, User user)
        {
            if (!IsUserAuthorized(id, user))
            {
                throw new UnauthorizedOperationException(Constants.ModifyAccountErrorMessage);
            }
            return await accountRepository.GetByUsernameAsync(user.Username);
        }

        public bool AddCard(int id, Card card, User user)
        {
            if (!IsUserAuthorized(id, user))
            {
                throw new UnauthorizedOperationException(Constants.ModifyAccountCardErrorMessage);
            }
            return this.accountRepository.AddCard(id, card);
        }

        public bool RemoveCard(int id, Card card, User user)
        {
            if (!IsUserAuthorized(id, user))
            {
                throw new UnauthorizedOperationException(Constants.ModifyAccountCardErrorMessage);
            }
            return this.accountRepository.RemoveCard(id, card);
        }

        public bool IsUserAuthorized(int id, User user)
        {
            bool IsUserAccountOwnerOrAdminId = false;

            Account accountToGet = this.accountRepository.GetById(id);

            if (accountToGet.UserId == user.Id || user.IsAdmin)
            {
                IsUserAccountOwnerOrAdminId = true;
            }

            return IsUserAccountOwnerOrAdminId;

        }




    }
}
