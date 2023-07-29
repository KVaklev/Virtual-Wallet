using AutoMapper;
using Business.DTOs;
using Business.Exceptions;
using Business.QueryParameters;
using Business.Services.Contracts;
using Business.Services.Helpers;
using DataAccess.Models.Models;
using DataAccess.Repositories.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        public async Task<IQueryable<Account>> GetAll()
        {
            return await accountRepository.GetAll();
        }


        public async Task<Account> Create(CreateAccountDto accountDto, User user)
        {
            Account account = new Account();
            account.Currency = await currencyRepository.GetByАbbreviationAsync(accountDto.Abbreviation);
            account.CurrencyId = account.Currency.Id;
            
            Account accountToCreate = await this.accountRepository.CreateAsync(account, user);

            return accountToCreate;
        }

        public async Task <bool> DeleteAsync(int id, User loggedUser)
        { 
            return await this.accountRepository.DeleteAsync(id);
        }

        public Account GetById(int id, User user)
        {
            if (!IsUserAuthorized(id, user))
            {
                throw new UnauthorizedOperationException(Constants.ModifyAccountErrorMessage);
            }
            return this.accountRepository.GetById(id);
        }

        public Account GetByUsername(int id, User user)
        {
            if (!IsUserAuthorized(id, user))
            {
                throw new UnauthorizedOperationException(Constants.ModifyAccountErrorMessage);
            }
            return accountRepository.GetByUsername(user.Username);
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
