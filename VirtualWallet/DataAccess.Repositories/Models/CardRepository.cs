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

        public CardRepository(ApplicationContext context)
        {
            this.context = context;
        }

        public List<Card> GetAll()
        {
            return this.context.Cards
               .Include(c=>c.Account)
               .ThenInclude(c=>c.User)
               .ToList();
        }

        public List<Card> GetByAccountId(int accountId)
        {
            List<Card> cards = context.Cards
                .Where(card => card.AccountId == accountId)
                .ToList();

            return cards ?? throw new EntityNotFoundException($"Account with ID = {accountId} doesn't have any cards.");
        }
        public Card GetById(int id)
        {
            var card = this.context.Cards
                .FirstOrDefault(c => c.Id == id)
                ?? throw new EntityNotFoundException($"Card with ID = {id} doesn't exist.");

            return card;
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
        //public Card Add(int userId, int accountId, Card card)
        //{
        //    User user = context.Users
        //        .FirstOrDefault(u => u.Id == userId)
        //        ?? throw new EntityNotFoundException($"User with ID = {userId} doesn't exist.");
        //    Account account = context.Accounts
        //        .FirstOrDefault(a => a.Id == accountId && a.UserId == userId)
        //        ?? throw new EntityNotFoundException($"Account with ID = {accountId} doesn't exist or doesn't belong to the user.");

        //    card.AccountId = accountId;
        //    card.UserId = userId;
        //    context.Cards.Add(card);
        //    context.SaveChanges();

        //    return card;
        //}

        //public Card Update(int id, Card card)
        //{
        //    Card cardToUpdate = context.Cards
        //        .FirstOrDefault(c => c.Id == id)
        //        ?? throw new EntityNotFoundException($"Card with ID = {id} doesn't exist.");

        //    cardToUpdate.CardNumber = card.CardNumber ?? cardToUpdate.CardNumber;
        //    cardToUpdate.CardHolder = card.CardHolder ?? cardToUpdate.CardHolder;
        //    cardToUpdate.ExpirationDate = card.ExpirationDate;
        //    cardToUpdate.CheckNumber = card.CheckNumber;
        //    cardToUpdate.CardType = card.CardType;
        //    cardToUpdate.AccountId = card.AccountId;
        //    //cardToUpdate.UserId = card.UserId;

        //    context.SaveChanges();
        //    return cardToUpdate;
        //}
        //public void Delete(int id)
        //{
        //    Card cardToDelete = this.GetById(id);
        //    context.Cards.Remove(cardToDelete);
        //    context.SaveChanges();
        //}

        private static IQueryable<Card> FilterByUsername(IQueryable<Card> result, string username)
        {
            if (!string.IsNullOrEmpty(username))
            {
                result = result
                    .Include(card => card.Account)
                    .ThenInclude(account => account.User)
                    .Where(card =>
                        card.Account != null && card.Account.User != null &&
                        card.Account.User.Username != null &&
                        card.Account.User.Username.ToUpper().Contains(username.ToUpper())
                    );
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
                result = result.Where(card => card.Account.Balance != null
                                   && card.Account.Balance == balance.Value);
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
                    return result.Reverse();
                default:
                    return result;
            }
        }

    }
}
