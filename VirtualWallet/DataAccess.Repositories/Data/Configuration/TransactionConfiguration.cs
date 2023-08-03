using DataAccess.Models.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataAccess.Repositories.Data.Configuration
{
    public class TransactionConfiguration : IEntityTypeConfiguration<Transaction>
    {
        public void Configure(EntityTypeBuilder<Transaction> builder)
        {
            builder
                .HasOne(c => c.AccountSender)
                .WithMany(u => u.TransactionsSender)
                .HasForeignKey(c => c.AccountSenderId)
                .OnDelete(DeleteBehavior.NoAction);

            builder
                .HasOne(c => c.AccountRecipient)
                .WithMany(u => u.TransactionsRecipient)
                .HasForeignKey(c => c.AccountRecepientId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.Property(t => t.Amount).HasColumnType("decimal(18, 2)");

        }
    }
}
