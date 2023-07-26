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

        public AccountService(IAccountRepository accountRepository)
        {
            this.accountRepository = accountRepository;
        }

        public IQueryable<Account> GetAll()
        {
            return accountRepository.GetAll();
        }

        public PaginatedList<Account> FilterBy(AccountQueryParameters accountQueryParameters)
        {
            throw new NotImplementedException();
        }
        public Account Create(Account account, User user)
        {
            Account accountToCreate = this.accountRepository.Create(account, user);

            accountToCreate.User = user;

            return accountToCreate;
        }

        public bool Delete(int id, User user)
        {
            var accounToDelete = this.accountRepository.GetById(id);

            if (accounToDelete.UserId != user.Id || !user.IsAdmin)
            {
                throw new UnauthorizedOperationException(Constants.ModifyAccountErrorMessage);
            }

            return this.accountRepository.Delete(id);
        }

        public Account GetById(int id)
        {
            return this.accountRepository.GetById(id);
        }

        public Account GetByUsername(string username)
        {
            return accountRepository.GetByUsername(username);
        }

        public bool AddCard(int id, Card card) 
        {

          return this.accountRepository.AddCard(id, card);
        }

        public bool RemoveCard(int id, Card card) 
        {
            return this.accountRepository.RemoveCard(id, card);
        }


        //public bool AccountExists(int id)
        //{
        //    return this.accountRepository.AccountExists(id);
        //}


    }
}
