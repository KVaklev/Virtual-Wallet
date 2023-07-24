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

        //Seed database
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            //Seed users

            List<User> users = new List<User>()
            {
                new User
                {
                     Id = 1,
                     FirstName = "Ivan",
                     LastName = "Draganov",
                     Email = "i.draganov@gmail.com",
                     Username = "ivanchoDraganchov",
                     Password = "MTIz",
                     PhoneNumber = "1234567890",
                     IsAdmin = true,
                     IsBlocked = false
                },

                new User
                {
                     Id = 2,
                     FirstName = "Mariq",
                     LastName = "Petrova",
                     Email = "m.petrova@gmail.com",
                     Username = "mariicheto",
                     Password = "MTIz",
                     PhoneNumber = "1134567890",
                     IsAdmin = false,
                     IsBlocked = false
                },

                new User
                {
                     Id=3,
                     FirstName = "Mara",
                     LastName = "Dobreva",
                     Email = "m.dobreva@gmail.com",
                     Username = "marcheto",
                     Password = "MTIz",
                     PhoneNumber = "1214567890",
                     IsAdmin = false,
                     IsBlocked = true
                },

                new User
                {
                     Id = 4,
                     FirstName = "Andrei",
                     LastName = "Sokolov",
                     Email = "a.sokolov@gmail.com",
                     Username = "sokolov",
                     Password = "MTIz",
                     PhoneNumber = "1234167890",
                     IsAdmin = true,
                     IsBlocked = false
                },
                new User
                {
                     Id = 5,
                     FirstName = "Margarita",
                     LastName = "Ivanova",
                     Email = "marg89@gmail.com",
                     Username = "margIvanova",
                     Password = "MTIz",
                     PhoneNumber = "1234561890",
                     IsAdmin = false,
                     IsBlocked = false
                },
                new User
                {
                     Id = 6,
                     FirstName = "Dimitar",
                     LastName = "Peev",
                     Email = "dim@gmail.com",
                     Username = "dimitarDimitrov",
                     Password = "MTIz",
                     PhoneNumber = "1234567810",
                     IsAdmin = false,
                     IsBlocked = false
                },
                new User
                {
                     Id = 7,
                     FirstName = "Ivan",
                     LastName = "Apostolov",
                     Email = "Apostolche@gmail.com",
                     Username = "IApostolov99",
                     Password = "MTIz",
                     PhoneNumber = "2234567890",
                     IsAdmin = true,
                     IsBlocked = false
                },
                 new User
                {
                     Id = 8,
                     FirstName = "Ivan",
                     LastName = "Atanasov",
                     Email = "AtansovGerey@gmail.com",
                     Username = "DreamerTillX",
                     Password = "MTIz",
                     PhoneNumber = "1234527890",
                     IsAdmin = false,
                     IsBlocked = false
                },
            };

            modelBuilder.Entity<User>().HasData(users);


            modelBuilder.Entity<User>()
                .HasOne(u => u.Account)
                .WithOne(a => a.User)
                .HasForeignKey<Account>(a => a.UserId)
                .OnDelete(DeleteBehavior.NoAction);

            //Seed accounts

            List<Account> accounts = new List<Account>()
            {
            };

            modelBuilder.Entity<Account>().HasData(accounts);
            modelBuilder.Entity<Account>()
                .HasMany(u => u.TransactionsSender)
                .WithOne(t => t.AccountSender)
                .HasForeignKey(t => t.AccountSenderId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Account>()
                .HasMany(u => u.TransactionsRecipient)
                .WithOne(t => t.AccountRecepient)
                .HasForeignKey(t => t.AccountRecepientId)
                .OnDelete(DeleteBehavior.NoAction);

            //Seed currencies

            List<Currency> currencies = new List<Currency>()
            {
            };

            modelBuilder.Entity<Currency>().HasData(currencies);

            //Seed transactions

            List<Transaction> transactions = new List<Transaction>()
            {
            };

            modelBuilder.Entity<Transaction>().HasData(transactions);

            modelBuilder.Entity<Transaction>()
            .HasOne(c => c.AccountSender)
            .WithMany(u => u.TransactionsSender)
            .HasForeignKey(c => c.AccountSenderId)
            .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Transaction>()
                .HasOne(c => c.AccountRecepient)
                .WithMany(u => u.TransactionsRecipient)
                .HasForeignKey(c => c.AccountRecepientId)
                .OnDelete(DeleteBehavior.NoAction);

            //Seed transfers

            List<Transfer> transfers = new List<Transfer>()
            {
            };

            modelBuilder.Entity<Transfer>().HasData(transfers);

            modelBuilder.Entity<Transfer>()
                .HasOne(a => a.Account)
                .WithMany(t => t.Transfers)
                .HasForeignKey(a => a.AccountId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Transfer>()
                .HasOne(t => t.Currency)
                .WithMany()
                .HasForeignKey(t => t.CurrencyId)
                .OnDelete(DeleteBehavior.NoAction);

            //Seed cards

            List<Card> cards = new List<Card>()
            {
            };

            modelBuilder.Entity<Card>().HasData(cards);

            //Seed history

            List<History> history = new List<History>()
            {
            };

            modelBuilder.Entity<History>().HasData(history);
            modelBuilder.Entity<History>()
                .HasOne(a => a.Account)
                .WithMany()
                .HasForeignKey(a => a.AccountId)
            .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<History>()
                .HasOne(a => a.Transfer)
                .WithMany()
                .HasForeignKey(a => a.TransferId)
            .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<History>()
                .HasOne(a => a.Transaction)
                .WithMany()
                .HasForeignKey(a => a.TransactionId)
            .OnDelete(DeleteBehavior.NoAction);


        }
    }
}