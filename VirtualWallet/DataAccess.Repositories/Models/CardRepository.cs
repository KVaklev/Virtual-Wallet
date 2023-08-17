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

        public IQueryable<Card> GetAll()
        {
            var cards = this.context.Cards
               .Where(c=>c.IsDeleted==false)
               .Include(c=>c.Currency)
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
    }
}
