using Business.Exceptions;
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

        public Account Delete(int id)
        {
            Account accountToDelete = this.GetById(id);
            context.Accounts.Remove(accountToDelete);
            context.SaveChanges();

            return accountToDelete;
        }

        public Account Update(int id, Account account)
        {
            var accountToUpdate = this.GetById(id);
            accountToUpdate.Balance = account.Balance;
            accountToUpdate.Currency = account.Currency;
            accountToUpdate.DailyLimit = account.DailyLimit;
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

        public Account DepositToBalance(Account account, int amount)
        {
            Account accountToDepositTo = this.GetById(account.Id);

            accountToDepositTo.Balance += amount;

            return accountToDepositTo;
        }


        public Account WithdrawalFromBalance(Account account, int amount)
        {
            Account accountToWithdrawFrom = this.GetById(account.Id);

            accountToWithdrawFrom.Balance -= amount;

            return accountToWithdrawFrom;
        }

        public bool CheckBalance(Account account, int amount)
        {
            Account accountToCheck = this.GetById(account.Id);

            if (accountToCheck.Balance < amount)
            {
                return false;
            }
            return true;
        }

        
    }
}
