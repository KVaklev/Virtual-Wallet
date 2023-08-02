using DataAccess.Models.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataAccess.Repositories.Data.Configuration
{
    public class AccountConfiguration : IEntityTypeConfiguration<Account>
    {
        public void Configure(EntityTypeBuilder<Account> builder)
        {
            builder
                .HasMany(u => u.TransactionsSender)
                .WithOne(t => t.AccountSender)
                .HasForeignKey(t => t.AccountSenderId)
                .OnDelete(DeleteBehavior.NoAction);

            builder
                .HasMany(u => u.TransactionsRecipient)
                .WithOne(t => t.AccountRecepient)
                .HasForeignKey(t => t.AccountRecepientId)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
