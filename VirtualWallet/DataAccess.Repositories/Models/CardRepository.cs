using Business.Exceptions;
using Business.QueryParameters;
using DataAccess.Models.Enums;
using DataAccess.Models.Models;
using DataAccess.Repositories.Contracts;
using DataAccess.Repositories.Data;
using DataAccess.Repositories.Helpers;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Repositories.Models
{
    public class CardRepository : ICardRepository
    {
        private readonly ApplicationContext context;

        public CardRepository(ApplicationContext context)
        {
            this.context = context;
        }

        public IQueryable<Card> GetAll()
        {
            var cards = this.context.Cards
               .Where(c=>c.IsDeleted==false)
               .Include(c=>c.Account)
               .ThenInclude(c=>c.User)
               .AsQueryable();

            return cards;
        }

        public async Task<Card> GetByIdAsync(int id)
        {
            var card = await this.context.Cards
                .Where(c => c.IsDeleted == false)
                .Include(c => c.Account)
                .ThenInclude(a => a.User)
                .FirstOrDefaultAsync(c => c.Id == id);
                
            return card;
        }

        public IQueryable<Card> GetByAccountId(int accountId)
        {
            var cards = context.Cards
                .Where(c=>c.IsDeleted==false)
                .Where(card => card.AccountId == accountId)
                .AsQueryable();

            return cards;
        }

        public async Task<PaginatedList<Card>> FilterByAsync(CardQueryParameters filterParameters, User loggeedUser)
        {
            IQueryable<Card> result = this.GetAll();

            result = await FilterByExpirationDateAsync(result, filterParameters.ExpirationDate);
            result = await FilterByCardTypeAsync(result, filterParameters.CardType);
            result = await FilterByBalanceAsync(result, filterParameters.Balance);
            result = await SortByAsync(result, filterParameters.SortBy);
            result = await SortOrderAsync(result, filterParameters.SortOrder);

            int totalPages = (result.Count() + filterParameters.PageSize - 1) / filterParameters.PageSize;
            result = await Common<Card>.PaginateAsync(result, filterParameters.PageNumber, filterParameters.PageSize);

            return new PaginatedList<Card>(result.ToList(), totalPages, filterParameters.PageNumber);
        }

        public async Task<Card> CreateAsync(int accountId, Card card)
        {
            context.Cards.Add(card);
            await context.SaveChangesAsync();

            return card;
        }

        public async Task<Card> UpdateAsync(int id, Card cardToUpdate)
        {
            await context.SaveChangesAsync();
            return cardToUpdate;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var cardToDelete = await this.GetByIdAsync(id);
            cardToDelete.IsDeleted = true;

            await context.SaveChangesAsync();
            return cardToDelete.IsDeleted;
        }

        public async Task<bool> CardNumberExistsAsync(string cardNumber)
        {
            return await context.Cards.AnyAsync(u => u.CardNumber == cardNumber);
        }

        public async Task<bool> SaveChangesAsync()
        {
            await context.SaveChangesAsync();
            return true;
        }

        //private async Task<IQueryable<Card>> FilterByUsernameAsync(IQueryable<Card> result, string username)
        //{
        //    result = result
        //        .Include(c => c.Account)
        //        .ThenInclude(a => a.User);

        //    if (!string.IsNullOrEmpty(username))
        //    {
        //        return await Task.FromResult(result.Where(card => card.Account.User.Username.Contains(username.ToUpper())));
        //    }

        //    return await Task.FromResult(result);
        //}
        private async Task<IQueryable<Card>> FilterByExpirationDateAsync(IQueryable<Card> result, string expirationDate)
        {
            DateTime? date = !string.IsNullOrEmpty(expirationDate) ? DateTime.Parse(expirationDate) : null;
            return await Task.FromResult(result.Where(t => !date.HasValue || t.ExpirationDate <= date));
        }
        private async Task<IQueryable<Card>> FilterByCardTypeAsync(IQueryable<Card> result, string cardTypeString)
        {
            if (!string.IsNullOrEmpty(cardTypeString) && Enum.TryParse(cardTypeString, true, out CardType cardType))
            {
                result = result.Where(card => card.CardType == cardType);
            }

            return await Task.FromResult(result);
        }
        private async Task<IQueryable<Card>> FilterByBalanceAsync(IQueryable<Card> result, decimal? balance)
        {
            if (balance.HasValue)
            {
                result = result.Where(card => card.Balance <= balance);
            }

            return await Task.FromResult(result);
        }
        private async Task<IQueryable<Card>> SortByAsync(IQueryable<Card> result, string sortCriteria)
        {
            switch (sortCriteria)
            {
                case "username":
                    return await Task.FromResult(result.OrderBy(card => card.Account.User.Username));
                case "expirationDate":
                    return await Task.FromResult(result.OrderBy(card => card.ExpirationDate));
                case "cardType":
                    return await Task.FromResult(result.OrderBy(card => card.CardType));
                case "balance":
                    return await Task.FromResult(result.OrderBy(card => card.Account.Balance));
                default:
                    return await Task.FromResult(result);
            }
        }
        private async Task<IQueryable<Card>> SortOrderAsync(IQueryable<Card> result, string sortOrder)
        {
            switch (sortOrder)
            {
                case "desc":
                    return await Task.FromResult(result.Reverse());
                default:
                    return await Task.FromResult(result);
            }
        }
    }
}
