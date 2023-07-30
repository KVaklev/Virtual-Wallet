﻿using Business.Dto;
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
        IQueryable<Account> GetAll();
        Task <Account> GetByIdAsync(int id, User user);
        Task <Account> GetByUsernameAsync(int id, User user);
        Task <Account> CreateAsync(string currencyCode, User user);
        Task <bool> AddCardAsync(int id, Card card, User user);
        Task <bool> RemoveCardAsync(int id,  Card card, User user);
        Task <bool> DeleteAsync(int id, User loggedUser);
    }
}
