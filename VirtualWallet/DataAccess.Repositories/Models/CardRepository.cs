using Business.Exceptions;
using Business.QueryParameters;
using DataAccess.Models.Enums;
using DataAccess.Models.Models;
using DataAccess.Repositories.Contracts;
using DataAccess.Repositories.Data;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Repositories.Models
{
    public class CardRepository : ICardRepository
    {
        private readonly ApplicationContext context;
        private readonly ICurrencyRepository currencyRepository;

        public CardRepository(ApplicationContext context, ICurrencyRepository currencyRepository)
        {
            this.context = context;
            this.currencyRepository = currencyRepository;
        }

        public async Task<List<Card>> GetAllAsync()
        {
            return await this.context.Cards
               .Where(c=>c.IsDeleted==false)
               .Include(c=>c.Account)
               .ThenInclude(c=>c.User)
               .ToListAsync();
        }

        public async Task<Card> GetByIdAsync(int id)
        {
            var card = await this.context.Cards
                .Where(c => c.IsDeleted == false)
                .Include(c => c.Account)
                .ThenInclude(a => a.User)
                .FirstOrDefaultAsync(c => c.Id == id)
                ?? throw new EntityNotFoundException($"Card with ID = {id} doesn't exist.");

            return card;
        }

        public async Task<List<Card>> GetByAccountIdAsync(int accountId)
        {
            List<Card> cards = await context.Cards
                .Where(c=>c.IsDeleted==false)
                .Where(card => card.AccountId == accountId)
                .ToListAsync();

            return cards ?? throw new EntityNotFoundException($"Account with ID = {accountId} doesn't have any cards.");
        }

        public async Task<PaginatedList<Card>> FilterByAsync(CardQueryParameters filterParameters)
        {
            IQueryable<Card> result = context.Cards
                .Where(c=>c.IsDeleted==false)
                .Include(c => c.Account)
                .ThenInclude(a => a.User);

            result = await FilterByUsernameAsync(result, filterParameters.Username);
            result = await FilterByExpirationDateAsync(result, filterParameters.ExpirationDate);
            result = await FilterByCardTypeAsync(result, filterParameters.CardType);
            result = await FilterByBalanceAsync(result, filterParameters.Balance);
            result = await SortByAsync(result, filterParameters.SortBy);
            result = await SortOrderAsync(result, filterParameters.SortOrder);

            int totalItems = await result.CountAsync();
            if (totalItems == 0)
            {
                throw new EntityNotFoundException("No cards match the specified filter criteria.");
            }

            int totalPages = (result.Count() + filterParameters.PageSize - 1) / filterParameters.PageSize;
            result = await PaginateAsync(result, filterParameters.PageNumber, filterParameters.PageSize);

            return new PaginatedList<Card>(result.ToList(), totalPages, filterParameters.PageNumber);
        }
        public async Task<Card> CreateAsync(int accountId, Card card)
        {
            var carToCreate = new Card();
            var currency = await currencyRepository.GetByАbbreviationAsync(card.Currency.Abbreviation);

            carToCreate.AccountId = accountId;
            carToCreate.CardNumber = card.CardNumber;
            carToCreate.CardType = card.CardType;
            carToCreate.Balance = card.Balance;
            carToCreate.CardHolder = card.CardHolder;
            carToCreate.ExpirationDate = card.ExpirationDate;
            carToCreate.CheckNumber = card.CheckNumber;
            carToCreate.CreditLimit = card.CreditLimit;
            carToCreate.CurrencyId = currency.Id;

            context.Cards.Add(carToCreate);
            await context.SaveChangesAsync();

            return carToCreate;
        }

        public async Task<Card> UpdateAsync(int id, Card card)
        {
            var cardToUpdate = await this.GetByIdAsync(id);
            var currency = await currencyRepository.GetByАbbreviationAsync(card.Currency.Abbreviation);

            cardToUpdate.CardNumber = card.CardNumber ?? cardToUpdate.CardNumber;
            cardToUpdate.CheckNumber = card.CheckNumber ?? cardToUpdate.CheckNumber;
            cardToUpdate.CardHolder = card.CardHolder ?? cardToUpdate.CardHolder;
            cardToUpdate.CreditLimit = card.CreditLimit ?? cardToUpdate.CreditLimit;
            await UpdateCardTypeAsync(card, cardToUpdate);
            await UpdateExpirationDateAsync(card, cardToUpdate);
            await UpdateCurrencyAsync(card, cardToUpdate, currency);

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
            return context.Cards.Any(u => u.CardNumber == cardNumber);
        }

        public async Task<Card> IncreaseBalanceAsync(int id, decimal amount)
        {
            Card card = await this.GetByIdAsync(id);
            card.Balance += amount;
            await context.SaveChangesAsync();

            return card;
        }

        public async Task<Card> DecreaseBalanceAsync(int id, decimal amount)
        {
            Card card = await this.GetByIdAsync(id);
            card.Balance -= amount;
            await context.SaveChangesAsync();

            return card;
        }

        public async static Task<IQueryable<Card>> PaginateAsync(IQueryable<Card> result, int pageNumber, int pageSize)
        {
            return await Task.FromResult(result
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize));
        }

        private async Task UpdateExpirationDateAsync(Card card, Card cardToUpdate)
        {
            if (card.ExpirationDate != null)
            {
                cardToUpdate.ExpirationDate = card.ExpirationDate;
            }
        }
        private async Task UpdateCardTypeAsync(Card card, Card cardToUpdate)
        {
            if (card.CardType != null)
            {
                cardToUpdate.CardType = card.CardType;
            }
        }
        private async Task UpdateCurrencyAsync(Card card, Card cardToUpdate, Currency currency)
        {
            if (card.Currency != null)
            {
                cardToUpdate.CurrencyId = currency.Id;
            }
        }
        private async Task<IQueryable<Card>> FilterByUsernameAsync(IQueryable<Card> result, string username)
        {
            result = result
                .Include(c => c.Account)
                .ThenInclude(a => a.User);

            if (!string.IsNullOrEmpty(username))
            {
                return await Task.FromResult(result.Where(card => card.Account.User.Username.Contains(username.ToUpper())));
            }

            return await Task.FromResult(result);
        }
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
