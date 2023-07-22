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
                     IsAdmin = false,
                     IsBlocked = false
                },
            };
            modelBuilder.Entity<User>().HasData(users);
            modelBuilder.Entity<User>()
                .HasMany(u => u.TransactionsSender)
                .WithOne(t => t.Sender)
                .HasForeignKey(t => t.SenderId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<User>()
                .HasMany(u => u.TransactionsRecipient)
                .WithOne(t => t.Recipient)
                .HasForeignKey(t => t.RecipientId)
                .OnDelete(DeleteBehavior.NoAction);

            List<Account> accounts = new List<Account>()
            {
            };
            modelBuilder.Entity<Account>().HasData(accounts);

            List<Currency> currencies = new List<Currency>()
            {
            };
            modelBuilder.Entity<Currency>().HasData(currencies);

            List<Transaction> transactions = new List<Transaction>()
            {
            };
            modelBuilder.Entity<Transaction>().HasData(transactions);

            modelBuilder.Entity<Transaction>()
            .HasOne(c => c.Sender)
            .WithMany(u => u.TransactionsSender)
            .HasForeignKey(c => c.SenderId)
            .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Transaction>()
                .HasOne(c => c.Recipient)
                .WithMany(u => u.TransactionsRecipient)
                .HasForeignKey(c => c.RecipientId)
                .OnDelete(DeleteBehavior.NoAction);

            List<Transfer> transfers = new List<Transfer>()
            {
            };
            modelBuilder.Entity<Transfer>().HasData(transfers);

            modelBuilder.Entity<Transfer>()
                .HasOne(u=>u.User)
                .WithMany(t=>t.Transfers)
                .HasForeignKey(u=>u.UserId)
                .OnDelete(DeleteBehavior.NoAction);
                    


            List<Card> cards = new List<Card>()
            {
            };
            modelBuilder.Entity<Card>().HasData(cards);

            List<History> history = new List<History>()
            {
            };
            modelBuilder.Entity<History>().HasData(history);

            modelBuilder.Entity<History>()
                .HasOne(c => c.Transfer)
                .WithMany(u => u.TransferHistories)
                .HasForeignKey(c => c.TransferId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<History>()
                .HasOne(c => c.Transaction)
                .WithMany(u => u.TransactionHistories)
                .HasForeignKey(c => c.TransactionId)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}