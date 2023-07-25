using DataAccess.Models.Models;
using DataAccess.Repositories.Data;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccess.Repositories.Contracts;

namespace DataAccess.Repositories.Models
{
    public class HistoryRepository: DataAccess.Repositories.Contracts.IHistoryRepository
    {
        private readonly ApplicationContext context;

        public HistoryRepository(ApplicationContext context) 
        {
             this.context=context;
        }

        public History Ctraete(History history)
        {
            this.context.Add(history);
            this.context.SaveChanges();

            return history;
        }
    }
}
