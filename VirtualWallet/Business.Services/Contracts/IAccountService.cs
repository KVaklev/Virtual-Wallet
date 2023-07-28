using Business.Dto;
using Business.DTOs;
using Business.QueryParameters;
using DataAccess.Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Services.Contracts
{
    public interface IAccountService
    {
        Task<IQueryable<Account>> GetAll();
        PaginatedList<Account> FilterBy(AccountQueryParameters accountQueryParameters);
        Account GetById(int id, User user);
        Account GetByUsername(int id, User user);
        Task <Account> Create(CreateAccountDto accountDto, User user);
        bool AddCard(int id, Card card, User user);
        bool RemoveCard(int id,  Card card, User user);
        bool Delete(int id, User loggedUser);

       // bool AccountExists(int id);



    }
}
