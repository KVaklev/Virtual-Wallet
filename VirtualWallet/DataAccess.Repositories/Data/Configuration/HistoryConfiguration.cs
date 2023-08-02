using DataAccess.Models.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Reflection.Emit;

namespace DataAccess.Repositories.Data.Configuration
{
    public class HistoryConfiguration : IEntityTypeConfiguration<History>
    {
        public void Configure(EntityTypeBuilder<History> builder)
        {
            builder
               .HasOne(a => a.Account)
               .WithMany()
               .HasForeignKey(a => a.AccountId)
               .OnDelete(DeleteBehavior.NoAction);

            builder
                .HasOne(a => a.Transfer)
                .WithMany()
                .HasForeignKey(a => a.TransferId)
                .OnDelete(DeleteBehavior.NoAction);

            builder
                .HasOne(a => a.Transaction)
                .WithMany()
                .HasForeignKey(a => a.TransactionId)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
