using DataAccess.Models.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Reflection.Emit;

namespace DataAccess.Repositories.Data.Configuration
{
    public class TransferConfiguration : IEntityTypeConfiguration<Transfer>
    {
        public void Configure(EntityTypeBuilder<Transfer> builder)
        {
            builder
              .HasOne(a => a.Account)
              .WithMany(t => t.Transfers)
              .HasForeignKey(a => a.AccountId)
              .OnDelete(DeleteBehavior.NoAction);

            builder
                .HasOne(t => t.Currency)
                .WithMany()
                .HasForeignKey(t => t.CurrencyId)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
