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

        public AccountService(IAccountRepository accountRepository, ICardRepository cardRepository)
        {
            this.accountRepository = accountRepository;
            this.cardRepository = cardRepository;   
        }
        public IQueryable<Account> GetAll()
        {
            return accountRepository.GetAll();
        }

        public PaginatedList<Account> FilterBy(AccountQueryParameters filterParameters)
        {
            return this.accountRepository.FilterBy(filterParameters);
        }
                
        public Account Create(Account account, User user)
        {
            Account accountToCreate = this.accountRepository.Create(account, user);

            accountToCreate.User = user;

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



        //public bool AccountExists(int id)
        //{
        //    return this.accountRepository.AccountExists(id);
        //}


    }
}
