using Business.QueryParameters;
using DataAccess.Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repositories.Contracts
{
    public interface IAccountRepository
    {
        IQueryable<Account> GetAll();

        Task <Account> GetByIdAsync(int id);

        Task <Account> GetByUsernameAsync(string username);

        Task<Account> CreateAsync(Account account, User user);

        Task <bool> AddCardAsync(int id, Card card);

        Task <bool> RemoveCardAsync(int id, Card card);

        Task <bool> DeleteAsync(int id);

        Task <Account> IncreaseBalanceAsync(int id, decimal amount);

        Task <Account> DecreaseBalanceAsync(int id, decimal amount);

        Task<bool> HasEnoughBalanceAsync(int id, decimal amount);

        public bool CardExists(string cardNumber);

        public bool AccountExists(int id);

        Task<bool> ConfirmRegistrationAsync(int id, string token);

        // bool AccountExists(int id);


    }
}
