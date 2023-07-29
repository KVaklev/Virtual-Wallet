using Business.Exceptions;
using Business.QueryParameters;
using DataAccess.Models.Models;
using DataAccess.Repositories.Contracts;
using DataAccess.Repositories.Data;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Repositories.Models
{
    public class AccountRepository : IAccountRepository
    {
        private readonly ApplicationContext context;
        private readonly ICardRepository cardRepository;

        public AccountRepository(ApplicationContext context, ICardRepository cardRepository)
        {
            this.context = context;
            this.cardRepository = cardRepository;
        }

        public Task<IQueryable<Account>> GetAll()
        {
            IQueryable<Account> result = context.Accounts
                .Where(a => a.IsDeleted == false)
                .Include(a => a.User)
                .Include(a => a.Cards)
                .Include(a => a.Currency);

            return Task.FromResult(result ?? throw new EntityNotFoundException($"There are no accounts"));
        }

        public Task<Account> Create(Account account, User user)
        {
            account.DateCreated = DateTime.Now;
            account.User = user;
            account.UserId = user.Id;
            account.Balance = 0;
            context.Add(account);
            context.SaveChanges();

            return Task.FromResult(account);
        }

        public bool Delete(int id)
        {
            var accountToDelete = this.GetById(id);
            accountToDelete.IsDeleted = true;

            foreach (var card in accountToDelete.Cards)
            {
                this.cardRepository.Delete(card.Id);
            }
            context.SaveChanges();
            return accountToDelete.IsDeleted;
        }

        public bool AddCard(int id, Card card)
        {
            var accountToAddCard = this.context.Accounts.FirstOrDefault(a => a.Id == id);

            if (accountToAddCard == null)
            {
                throw new EntityNotFoundException($"Account with ID = {id} does not exist.");
            }

            if (!CardExists(card.CardNumber))
            {
                accountToAddCard.Cards.Add(card);

                context.SaveChanges();

                return true;
            }

            else
            {
                throw new EntityNotFoundException($"Card with number = {card.CardNumber} already exists.");
            }
        }

        public bool RemoveCard(int id, Card card)
        {
            var accountToRemoveCard = this.context.Accounts.FirstOrDefault(a => a.Id == id);


            if (accountToRemoveCard == null)
            {
                throw new EntityNotFoundException($"Account with ID = {id} does not exist.");
            }

            if (CardExists(card.CardNumber))
            {
                accountToRemoveCard.Cards.Remove(card);

                context.SaveChanges();

                return true;
            }

            else
            {
                throw new EntityNotFoundException($"Card with ID = {card.Id} does not exist.");
            }
        }

        public Account GetById(int id)
        {
            Account account = await context.Accounts
                .Where(a => a.IsDeleted == false)
                .Where(a => a.Id == id)
                .Include(a => a.User)
                .Include(a => a.Cards)
                .Include(a => a.Currency)
                .FirstOrDefaultAsync();

            return account ?? throw new EntityNotFoundException($"Account with ID ={id} does not exist.");
        }

        public Account GetByUsername(string username)
        {
            Account account = context.Accounts
                .Include(a => a.User)
                .Include(a => a.Cards)
                .Include(a => a.Currency)
                .Where(a => a.IsDeleted == false)
                .Where(a => a.User.Username == username)
                .FirstOrDefault();

            return account ?? throw new EntityNotFoundException($"Account with username = {username} does not exist.");
        }

        public Account IncreaseBalance(int id, decimal amount)
        {
            Account accountToDepositTo = this.GetById(id);

            accountToDepositTo.Balance += amount;

            context.SaveChanges();

            return accountToDepositTo;
        }

        public Account DecreaseBalance(int id, decimal amount)
        {
            Account accountToWithdrawFrom = this.GetById(id);

            accountToWithdrawFrom.Balance -= amount;

            context.SaveChanges();

            return accountToWithdrawFrom;
        }

        public bool CheckBalance(int id, decimal amount)
        {
            Account accountToCheck = this.GetById(id);

            if (accountToCheck.Balance < amount)
            {
                return false;
            }
            return true;
        }

        
        public bool CardExists(string cardNumber)
        {
            return context.Cards
                .Where(a => a.IsDeleted == false)
                .Any(card => card.CardNumber == cardNumber);

        }

        public bool AccountExists(int id)
        {
            return context.Accounts
                .Where(a => a.IsDeleted == false)
                .Any(account => account.Id == id);
        }
    }
}
