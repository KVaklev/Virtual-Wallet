using DataAccess.Models.Enums;
using DataAccess.Models.Models;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

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
                     Password = new byte[] {0x65, 0x23, 0x25, 0x55},
                     PhoneNumber = "1234567890",
                     AccountId = 1,
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
                     Password = new byte[] {0x65, 0x23, 0x25, 0x55},
                     PhoneNumber = "1134567890",
                     AccountId = 2,
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
                     Password = new byte[] {0x65, 0x23, 0x25, 0x55},
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
                     Password = new byte[] {0x65, 0x23, 0x25, 0x55},
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
                     Password = new byte[] {0x65, 0x23, 0x25, 0x55},
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
                     Password = new byte[] {0x65, 0x23, 0x25, 0x55},
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
                     Password = new byte[] {0x65, 0x23, 0x25, 0x55},
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
                     Password = new byte[] {0x65, 0x23, 0x25, 0x55},
                     PhoneNumber = "1234527890",
                     IsAdmin = false,
                     IsBlocked = false,
                     AccountId=2
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
                new Account()
                {
                   Id = 1,
                   UserId = 1,
                   Balance = 850,
                   DateCreated = DateTime.Now,
                   CurrencyId = 1
                },

                new Account()
                {
                    Id = 2,
                    UserId = 2,
                    Balance = 1000,
                    DateCreated = DateTime.Now.AddMonths(1),
                    CurrencyId=2
                }
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
                new Currency()
                {
                Id=1,
                Name = "Български лев",
                Abbreviation="BGN",
                IsDeleted=false
                },

                new Currency()
                {
                Id = 2,
                Name = "Американски долар",
                Abbreviation = "USD",
                IsDeleted=false
                }
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
                 new Card
                {
                     Id = 1,
                     CardNumber = "1234567891011121",
                     CardHolder = "Ivancho Draganchov",
                     CheckNumber = "005",
                     CurrencyId = 1,
                     ExpirationDate = DateTime.Now.AddMonths(7),
                     CardType = CardType.Debit,
                     AccountId = 1,
                     Balance = 750
                 },

                 new Card
                {
                     Id = 2,
                     CardNumber = "2232567891011121",
                     CardHolder = "Mariq Andreeva",
                     CheckNumber = "015",
                     CurrencyId = 2,
                     ExpirationDate = DateTime.Now.AddMonths(6),
                     CardType = CardType.Debit,
                     AccountId = 2,
                     Balance = 1000
                },

                 new Card
                {
                     Id = 3,
                     CardNumber = "2232565891011121",
                     CardHolder = "Mariq Andreeva",
                     CheckNumber = "025",
                     CurrencyId = 2,
                     ExpirationDate = DateTime.Now.AddMonths(6),
                     CardType = CardType.Credit,
                     AccountId = 2,
                     Balance = 800,
                     CreditLimit = 1000
                }
            };

            modelBuilder.Entity<Card>().HasData(cards);
            modelBuilder.Entity<Card>()
                .HasOne(a => a.Currency)
                .WithMany(a => a.Cards)
                .HasForeignKey(c => c.CurrencyId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Card>()
               .HasOne(a => a.Account)
               .WithMany(a => a.Cards)
               .HasForeignKey(c => c.AccountId)
               .OnDelete(DeleteBehavior.NoAction);

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