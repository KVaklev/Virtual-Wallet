using Business.Exceptions;
using Business.QueryParameters;
using DataAccess.Models.Models;
using DataAccess.Repositories.Contracts;
using DataAccess.Repositories.Data;
using DataAccess.Repositories.Helpers;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Repositories.Models
{
    public class AccountRepository : IAccountRepository
    {
        private readonly ApplicationContext context;
        private readonly ICardRepository cardRepository;
        private readonly IUserRepository userRepository;

        public AccountRepository(ApplicationContext context, ICardRepository cardRepository, IUserRepository userRepository)
        {
            this.context = context;
            this.cardRepository = cardRepository;
            this.userRepository = userRepository;
        }

        public IQueryable <Account> GetAll()
        {
            IQueryable<Account> result = context.Accounts
                .Where(a => a.IsDeleted == false)
                .Include(a => a.User)
                .Include(a => a.Cards)
                .Include(a => a.Currency);

            return result ?? throw new EntityNotFoundException($"There are no accounts");
        }

        public async Task<Account> CreateAsync(Account account, User user)
        {
            account.DateCreated = DateTime.Now;
            account.User = user;
            account.UserId = user.Id;
            account.Balance = 0;
            context.Add(account);
            await context.SaveChangesAsync();

            return account;
        }

        public async Task <bool> DeleteAsync(int id)
        {
            var accountToDelete = await this.GetByIdAsync(id);

            accountToDelete.IsDeleted = true;

            if(accountToDelete.Cards.Count==0) 
            {
                throw new EntityNotFoundException("There are no cards lineked to this account");
            }

            foreach (var card in accountToDelete.Cards)
            {
                this.cardRepository.DeleteAsync(card.Id);
            }

            await context.SaveChangesAsync();

            return accountToDelete.IsDeleted;
        }
               

        public async Task <bool> AddCardAsync(int id, Card card)
        {
            var accountToAddCard = await this.context.Accounts.FirstOrDefaultAsync(a => a.Id == id);

            if (accountToAddCard == null)
            {
                throw new EntityNotFoundException($"Account with ID = {id} does not exist.");
            }

            if (!CardExists(card.CardNumber))
            {
                accountToAddCard.Cards.Add(card);

                await context.SaveChangesAsync();

                return true;
            }

            else
            {
                throw new EntityNotFoundException($"Card with number = {card.CardNumber} already exists.");
            }
        }

        public async Task <bool> RemoveCardAsync(int id, Card card)
        {
            var accountToRemoveCard = await this.context.Accounts.FirstOrDefaultAsync(a => a.Id == id);


            if (accountToRemoveCard == null)
            {
                throw new EntityNotFoundException($"Account with ID = {id} does not exist.");
            }

            if (CardExists(card.CardNumber))
            {
                accountToRemoveCard.Cards.Remove(card);

                await context.SaveChangesAsync();

                return true;
            }
            else
            {
                throw new EntityNotFoundException($"Card with ID = {card.Id} does not exist.");
            }
        }

        public async Task <Account> GetByIdAsync(int id)
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

        public async Task<Account> GetByUsernameAsync(string username)
        {
            Account account = await context.Accounts
                .Include(a => a.User)
                .Include(a => a.Cards)
                .Include(a => a.Currency)
                .Where(a => a.IsDeleted == false)
                .Where(a => a.User.Username == username)
                .FirstOrDefaultAsync();

            return account ?? throw new EntityNotFoundException($"Account with username = {username} does not exist.");
        }

        public async Task <Account> IncreaseBalanceAsync(int id, decimal amount)
        {
            Account accountToDepositTo = await this.GetByIdAsync(id);

            accountToDepositTo.Balance += amount;

            await context.SaveChangesAsync();

            return accountToDepositTo;
        }

        public async Task <Account> DecreaseBalanceAsync(int id, decimal amount)
        {
            Account accountToWithdrawFrom = await this.GetByIdAsync(id);

            accountToWithdrawFrom.Balance -= amount;

            await context.SaveChangesAsync();

            return accountToWithdrawFrom;
        }


        public async Task<bool> HasEnoughBalanceAsync(int id, decimal amount)
        {
            Account accountToCheck = await this.GetByIdAsync(id);

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

        public async Task<bool> ConfirmRegistrationAsync(int userId, string token)
        {
            var user = await this.userRepository.GetByIdAsync(userId);
            user.IsVerified = true;

            await this.context.SaveChangesAsync();
            return true;
        }
    }
}
