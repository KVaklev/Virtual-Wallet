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

        //Seed database
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            //Seed users

            List<User> users = new List<User>()
            {
            };
            modelBuilder.Entity<User>().HasData(users);

            List<Currency> currencies = new List<Currency>()
            {
            };
            modelBuilder.Entity<Currency>().HasData(currencies);

            List<Transaction> transactions = new List<Transaction>()
            {
            };
            modelBuilder.Entity<Transaction>().HasData(transactions);
            //modelBuilder.Entity<Transaction>()
            //.HasOne(c => c.Sender)
            //.WithMany(u => u.TransactionsSender)
            //.HasForeignKey(c => c.SenderId)
            //.OnDelete(DeleteBehavior.NoAction);

            //modelBuilder.Entity<Transaction>()
            //    .HasOne(c => c.Recipient)
            //    .WithMany(u => u.TransactionsRecipiend)
            //    .HasForeignKey(c => c.RecipientId)
            //    .OnDelete(DeleteBehavior.NoAction);

            List<Transfer> transfers = new List<Transfer>()
            {
            };
            modelBuilder.Entity<Transfer>().HasData(transfers);

            List<Card> cards = new List<Card>()
            {
            };
            modelBuilder.Entity<Card>().HasData(cards);
        }
    }
}