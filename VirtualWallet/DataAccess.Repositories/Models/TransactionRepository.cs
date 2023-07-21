using Business.Exceptions;
using DataAccess.Models.Models;
using DataAccess.Models.Contracts;
using DataAccess.Repositories.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using DataAccess.Repositories.Contracts;

namespace DataAccess.Repositories.Models
{
    public class TransactionRepository : ITransactionRepository
    {
        private readonly ApplicationContext context;

        public TransactionRepository(ApplicationContext context)
        {
            this.context = context;
        }

        public Transaction Create(Transaction trasaction)
        {
            trasaction.Date = DateTime.Now;
            trasaction.IsExecute = false;
            context.Add(trasaction);
            context.SaveChanges();

            return trasaction;
        }
        
        public ITransaction GetById(int id) 
        {
            ITransaction transaction = context.Transactions
                .Include(s =>s.Sender)
                .Include(r =>r.Recipient)
                .Include(c =>c.Currency)
                .FirstOrDefault(t => t.Id == id);

            return transaction ?? throw new EntityNotFoundException($"Transaction with ID = {id} doesn't exist.");
        }


        public ITransaction Update(int id, Transaction transaction)
        {
            var transactionToUpdate = GetById(id);
            transactionToUpdate.RecipientId = transaction.RecipientId;
            transactionToUpdate.Amount = transaction.Amount;
            transactionToUpdate.CurrencyId = transaction.CurrencyId;
            transactionToUpdate.Date = transaction.Date;

            context.SaveChanges();
            return transactionToUpdate;
        }

        public bool Delete(int id)
        {
            //TODO
            return true;
        }

        public List<ITransaction> GetAll()
        {
            List<ITransaction> result = context.Transactions
                    .Include(s =>s.Sender)
                    .Include(r =>r.Recipient)
                    .Include(c => c.Currency)
                    .ToList();
            return result ?? throw new EntityNotFoundException("There are any transactions!");
        }
    }
}
