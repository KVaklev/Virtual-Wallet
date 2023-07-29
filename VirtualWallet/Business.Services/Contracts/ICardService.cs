﻿using Business.QueryParameters;
using DataAccess.Models.Models;

namespace Business.Services.Contracts
{
    public interface ICardService
    {
        IQueryable<Card> GetAll();
        Task<PaginatedList<Card>> FilterByAsync(CardQueryParameters queryParameters);
        Task<Card> GetByIdAsync(int id);
        IQueryable<Card> GetByAccountId(int accountId);
        Task<Card> CreateAsync(int accountId, Card card);
        Task<Card> UpdateAsync(int id, User loggedUser, Card card);
        Task<bool> DeleteAsync(int id, User loggedUser);
        Task<bool> CardNumberExistsAsync(string cardNumber);
       
    }
}
