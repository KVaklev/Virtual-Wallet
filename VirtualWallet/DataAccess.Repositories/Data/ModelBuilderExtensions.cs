using DataAccess.Models.Enums;
using DataAccess.Models.Models;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Repositories.Data
{
    public static class ModelBuilderExtensions
    {
        public static void SeedData(this ModelBuilder modelBuilder)
        {
            var users = new List<User>()
            {
                new User { Id = 1, FirstName = "Ivan", LastName = "Draganov", Email = "i.draganov@gmail.com", Username = "ivanchoDraganchov",
                           Password = new byte[] {0x65, 0x23, 0x25, 0x55}, PhoneNumber = "0878558547", AccountId = 1, IsAdmin = true, IsBlocked = false    },
                new User { Id = 2, FirstName = "Mariq", LastName = "Petrova", Email = "m.petrova@gmail.com", Username = "mariicheto",
                           Password = new byte[] { 0x65, 0x23, 0x25, 0x55 }, PhoneNumber = "0898568569", AccountId = 2, IsAdmin = false, IsBlocked = false },
                new User { Id = 3, FirstName = "Mara",  LastName = "Dobreva", Email = "m.dobreva@gmail.com", Username = "marcheto",
                           Password = new byte[] { 0x65, 0x23, 0x25, 0x55 }, PhoneNumber = "0888524585", AccountId = 3, IsAdmin = false, IsBlocked = true  },
                new User { Id = 4, FirstName = "Andrei", LastName = "Sokolov", Email = "a.sokolov@gmail.com", Username = "sokolov",
                           Password = new byte[] { 0x65, 0x23, 0x25, 0x55 }, PhoneNumber = "0898652365", AccountId = 4, IsAdmin = true, IsBlocked = false   },
                new User { Id = 5, FirstName = "Margarita", LastName = "Ivanova", Email = "marg89@gmail.com", Username = "margIvanova",
                           Password = new byte[] { 0x65, 0x23, 0x25, 0x55 }, PhoneNumber = "0885659856",AccountId = 5, IsAdmin = false, IsBlocked = false   },
                new User { Id = 6, FirstName = "Dimitar", LastName = "Peev", Email = "dim@gmail.com", Username = "dimitarDimitrov",
                           Password = new byte[] { 0x65, 0x23, 0x25, 0x55 }, PhoneNumber = "0887885778",AccountId = 6, IsAdmin = false, IsBlocked = false   },
                new User { Id = 7, FirstName = "Ivan", LastName = "Apostolov", Email = "Apostolche@gmail.com", Username = "IApostolov99",
                           Password = new byte[] { 0x65, 0x23, 0x25, 0x55 }, PhoneNumber = "0898658856", AccountId = 7, IsAdmin = true, IsBlocked = false   },
                new User { Id = 8, FirstName = "Ivan", LastName = "Atanasov", Email = "AtansovGerey@gmail.com", Username = "DreamerTillX",
                           Password = new byte[] { 0x65, 0x23, 0x25, 0x55 }, PhoneNumber = "0889566547", AccountId = 8, IsAdmin = false, IsBlocked = false, }
            };

            var accounts = new List<Account>()
            {
               new Account() { Id = 1, UserId = 1, Balance = 850, DateCreated = DateTime.Now, CurrencyId = 1 },
               new Account() { Id = 2, UserId = 2, Balance = 1000, DateCreated = DateTime.Now, CurrencyId=2  },
               new Account() { Id = 3, UserId = 3, Balance = 1500, DateCreated = DateTime.Now, CurrencyId=1  },
               new Account() { Id = 4, UserId = 4, Balance = 5000, DateCreated = DateTime.Now, CurrencyId=1  },
               new Account() { Id = 5, UserId = 5, Balance = 900, DateCreated = DateTime.Now, CurrencyId=2  },
               new Account() { Id = 6, UserId = 6, Balance = 650, DateCreated = DateTime.Now, CurrencyId=1  },
               new Account() { Id = 7, UserId = 7, Balance = 800, DateCreated = DateTime.Now, CurrencyId=1  },
               new Account() { Id = 8, UserId = 8, Balance = 2500, DateCreated = DateTime.Now, CurrencyId=2  },
            };

            var currencies = new List<Currency>()
            {
               new Currency() { Id=1, Name = "Български лев", CurrencyCode="BGN" },
               new Currency() { Id = 2, Name = "Американски долар", CurrencyCode = "USD" }
            };

            var cards = new List<Card>()
            {
               new Card { Id = 1, CardNumber = "1234567891011121", CardHolder = "Ivancho Draganchov", CheckNumber = "005", CurrencyId = 1,
                          ExpirationDate = DateTime.Now.AddMonths(7), CardType = CardType.Debit, AccountId = 1, Balance = 750  },
               new Card { Id = 2, CardNumber = "2232567891011121", CardHolder = "Mariq Andreeva", CheckNumber = "015", CurrencyId = 2,
                          ExpirationDate = DateTime.Now.AddMonths(6), CardType = CardType.Debit, AccountId = 2, Balance = 1000 },
               new Card { Id = 3, CardNumber = "2232565891011121", CardHolder = "Mariq Andreeva", CheckNumber = "025", CurrencyId = 2,
                          ExpirationDate = DateTime.Now.AddMonths(6), CardType = CardType.Credit, AccountId = 2, Balance = 800, CreditLimit = 1000 }
            };

            var transactions = new List<Transaction>()
            {
            };

            var transfers = new List<Transfer>()
            {
            };

            var history = new List<History>()
            {
            };

            modelBuilder.Entity<User>()
                .HasData(users);

            modelBuilder.Entity<Account>()
               .HasData(accounts);

            modelBuilder.Entity<Card>()
               .HasData(cards);

            modelBuilder.Entity<Transaction>()
               .HasData(transactions);

            modelBuilder.Entity<Transfer>()
               .HasData(transfers);

            modelBuilder.Entity<Currency>()
               .HasData(currencies);

            modelBuilder.Entity<History>()
               .HasData(history);

        }
    }
}