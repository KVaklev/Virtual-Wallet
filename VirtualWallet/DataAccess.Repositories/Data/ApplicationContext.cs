using DataAccess.Models.Models;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Repositories.Data
{
    public class ApplicationContext : DbContext
    {
        public ApplicationContext(DbContextOptions<ApplicationContext> options)
            : base(options)
        {

        }
        public ApplicationContext()
        {

        }

        //Configure DB tables 
        public DbSet<User> Users { get; set; }
        public DbSet<Currency> Currencies { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<Transfer> Transfers { get; set; }
        public DbSet<Card> Cards { get; set; }
        public DbSet<Account> Accounts { get; set; }
        public DbSet<History> History { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(this.GetType().Assembly);
            modelBuilder.SeedData();
        }
    }
}