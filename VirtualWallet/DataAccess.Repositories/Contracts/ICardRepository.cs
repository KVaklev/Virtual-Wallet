﻿using Business.QueryParameters;
using DataAccess.Models.Models;

namespace DataAccess.Repositories.Contracts
{
    public interface ICardRepository
    {
        List<Card> GetAll();
        Card GetById(int id);
        List<Card> GetByAccountId(int accountId);
        List<Card> FilterBy(CardQueryParameters filterParameters);
        Card CreateCard(int accountId, Card card);
        bool CardNumberExists(string cardNumber);

        //Card Update(int id, Card card);
        //void Delete(int id);
        //PaginatedList<Card> FilterBy(UserQueryParameters filterParameters);

    }
}
