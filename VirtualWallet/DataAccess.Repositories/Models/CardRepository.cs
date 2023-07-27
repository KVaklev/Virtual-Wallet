using Business.Exceptions;
using Business.QueryParameters;
using DataAccess.Models.Enums;
using DataAccess.Models.Models;
using DataAccess.Repositories.Contracts;
using DataAccess.Repositories.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders.Physical;

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

        public List<Card> GetAll()
        {
            return this.context.Cards
               .Include(c=>c.Account)
               .ThenInclude(c=>c.User)
               .ToList();
        }

        public Card GetById(int id)
        {
            var card = this.context.Cards
                .Include(c => c.Account)
                .ThenInclude(a => a.User)
                .FirstOrDefault(c => c.Id == id)
                ?? throw new EntityNotFoundException($"Card with ID = {id} doesn't exist.");

            return card;
        }
        public List<Card> GetByAccountId(int accountId)
        {
            List<Card> cards = context.Cards
                .Where(card => card.AccountId == accountId)
                .ToList();

            return cards ?? throw new EntityNotFoundException($"Account with ID = {accountId} doesn't have any cards.");
        }
        public List<Card> FilterBy(CardQueryParameters filterParameters)
        {
            IQueryable<Card> result = context.Cards
                .Include(c => c.Account)
                .ThenInclude(a => a.User);

            result = FilterByUsername(result, filterParameters.Username);
            result = FilterByExpirationDate(result, filterParameters.ExpirationDate);
            result = FilterByCardType(result, filterParameters.CardType);
            result = FilterByBalance(result, filterParameters.Balance);
            result = SortBy(result, filterParameters.SortBy);
            result = SortOrder(result, filterParameters.SortOrder);

            List<Card> filteredAndSortedCards = result.ToList();

            if (filteredAndSortedCards.Count == 0)
            {
                throw new EntityNotFoundException("No cards match the specified filter criteria.");
            }

            return filteredAndSortedCards;
        }
        public Card Create(int accountId, Card card)
        {
            var carToCreate = new Card();
            var currency = currencyRepository.GetByАbbreviation(card.Currency.Abbreviation);

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
            context.SaveChanges();

            return carToCreate;
        }
        public Card Update(int id, Card card)
        {
            var cardToUpdate = this.GetById(id);
            var currency = currencyRepository.GetByАbbreviation(card.Currency.Abbreviation);

            cardToUpdate.CardNumber = card.CardNumber ?? cardToUpdate.CardNumber;
            cardToUpdate.CheckNumber = card.CheckNumber ?? cardToUpdate.CheckNumber;
            cardToUpdate.CardHolder = card.CardHolder ?? cardToUpdate.CardHolder;
            cardToUpdate.CreditLimit = card.CreditLimit ?? cardToUpdate.CreditLimit;
            UpdateCardType(card, cardToUpdate);
            UpdateExpirationDate(card, cardToUpdate);
            UpdateCurrency(card, cardToUpdate, currency);

            context.SaveChanges();
            return cardToUpdate;
        }

        private static void UpdateCurrency(Card card, Card cardToUpdate, Currency currency)
        {
            if (card.Currency != null)
            {
                cardToUpdate.CurrencyId = currency.Id;
            }
        }

        private static IQueryable<Card> FilterByUsername(IQueryable<Card> result, string username)
        {
            result = result
                .Include(c => c.Account)
                .ThenInclude(a => a.User);

            if (!string.IsNullOrEmpty(username))
            {
                return result.Where(card => card.Account.User.Username.Contains(username.ToUpper()));
            }

            return result;
        }
        private static IQueryable<Card> FilterByExpirationDate(IQueryable<Card> result, string expirationDate)
        {
            DateTime? date = !string.IsNullOrEmpty(expirationDate) ? DateTime.Parse(expirationDate) : null;
            return result.Where(t => !date.HasValue || t.ExpirationDate <= date);
        }
        private static IQueryable<Card> FilterByCardType(IQueryable<Card> result, string cardTypeString)
        {
            if (!string.IsNullOrEmpty(cardTypeString) && Enum.TryParse(cardTypeString, true, out CardType cardType))
            {
                result = result.Where(card => card.CardType == cardType);
            }

            return result;
        }
        private static IQueryable<Card> FilterByBalance(IQueryable<Card> result, decimal? balance)
        {
            if (balance.HasValue)
            {
                result = result.Where(card => card.Balance <= balance);
            }

            return result;
        }
        private static IQueryable<Card> SortBy(IQueryable<Card> result, string sortCriteria)
        {
            switch (sortCriteria)
            {
                case "username":
                    return result.OrderBy(card => card.Account.User.Username);
                case "expirationDate":
                    return result.OrderBy(card => card.ExpirationDate);
                case "cardType":
                    return result.OrderBy(card => card.CardType);
                case "balance":
                    return result.OrderBy(card => card.Account.Balance);
                default:
                    return result;
            }
        }
        private static IQueryable<Card> SortOrder(IQueryable<Card> result, string sortOrder)
        {
            switch (sortOrder)
            {
                case "desc":
                    return result.AsEnumerable().Reverse().AsQueryable();
                default:
                    return result;
            }
        }
        private void UpdateExpirationDate(Card card, Card cardToUpdate)
        {
            if (card.ExpirationDate != null)
            {
                cardToUpdate.ExpirationDate = card.ExpirationDate;
            }
        }
        private void UpdateCardType(Card card, Card cardToUpdate)
        {
            if (card.CardType != null)
            {
                cardToUpdate.CardType = card.CardType;
            }
        }
        public bool CardNumberExists(string cardNumber)
        {
            return context.Cards.Any(u => u.CardNumber == cardNumber);
        }
        public Card IncreaseBalance(int id, decimal amount)
        {
            Card card = this.GetById(id);
            card.Balance += amount;
            context.SaveChanges();

            return card;
        }
        public Card DecreaseBalance(int id, decimal amount)
        {
            Card card = this.GetById(id);
            card.Balance -= amount;
            context.SaveChanges();

            return card;
        }

    }
}
