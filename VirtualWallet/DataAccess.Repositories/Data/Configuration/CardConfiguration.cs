using DataAccess.Models.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataAccess.Repositories.Data.Configuration
{
    public class CardConfiguration : IEntityTypeConfiguration<Card>
    {
        public void Configure(EntityTypeBuilder<Card> builder)
        {
            builder
               .HasOne(a => a.Currency)
               .WithMany(a => a.Cards)
               .HasForeignKey(c => c.CurrencyId)
               .OnDelete(DeleteBehavior.NoAction);

            builder
               .HasOne(a => a.Account)
               .WithMany(a => a.Cards)
               .HasForeignKey(c => c.AccountId)
               .OnDelete(DeleteBehavior.NoAction);

            builder.Property(t => t.Balance).HasColumnType("decimal(18, 2)");
            builder.Property(t => t.CreditLimit).HasColumnType("decimal(18, 2)");

        }
    }
}
