using Business.Exceptions;
using Business.QueryParameters;
using DataAccess.Models.Models;
using DataAccess.Repositories.Contracts;
using DataAccess.Repositories.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repositories.Models
{
    public class AccountRepository : IAccountRepository
    {
        private readonly ApplicationContext context;

        public AccountRepository(ApplicationContext context)
        {
            this.context = context;
        }

        public List<Account> GetAll()
        {
            List<Account> result = context.Accounts
                 .Include(a => a.User)
                 .Include(a => a.Balance)
                 .Include(a => a.Currency)
                 .ToList();

            return result ?? throw new EntityNotFoundException($"There are no accounts");
        }

        public Account Create(Account account, User user)
        {
            account.DateCreated = DateTime.Now;
            account.User = user;
            context.Add(account);
            context.SaveChanges();

            return account;
        }

        public bool Delete(int id)
        {
            Account accountToDelete = this.GetById(id);

            if (accountToDelete != null)
            {
                context.Accounts.Remove(accountToDelete);
                context.SaveChanges();

                return true;
            }
            return false;
        }

        public Account Update(int id, Account account)
        {
            var accountToUpdate = this.GetById(id);
            accountToUpdate.Balance = account.Balance;
            accountToUpdate.Currency = account.Currency;
            context.SaveChanges();
            return accountToUpdate;
        }

        public Account GetById(int id)
        {
            Account account = context.Accounts
                .Where(a => a.Id == id)
                .FirstOrDefault();

            return account ?? throw new EntityNotFoundException($"Account with ID ={id} does not exist.");
        }

        public Account GetByUserId(int id)
        {
            Account account = context.Accounts
                .Where(a => a.UserId == id)
                .FirstOrDefault();

            return account ?? throw new EntityNotFoundException($"Account with UserID = {id} does not exist.");
        }

        public Account GetByUsername(string username)
        {
            Account account = context.Accounts
                .Where(a => a.User.Username == username)
                .FirstOrDefault();

            return account ?? throw new EntityNotFoundException($"Account with Username = {username} does not exist.");
        }

        public Account IncreaseBalance(int id, int amount)
        {
            Account accountToDepositTo = this.GetById(id);

            accountToDepositTo.Balance += amount;

            return accountToDepositTo;
        }


        public Account DecreaseBalance(int id, int amount)
        {
            Account accountToWithdrawFrom = this.GetById(id);

            accountToWithdrawFrom.Balance -= amount;

            return accountToWithdrawFrom;
        }

        public bool CheckBalance(int id, int amount)
        {
            Account accountToCheck = this.GetById(id);

            if (accountToCheck.Balance < amount)
            {
                return false;
            }
            return true;
        }

        public PaginatedList<Transaction> FilterBy(TransactionQueryParameters filterParameters)
        {
            throw new NotImplementedException();
        }
    }
}
