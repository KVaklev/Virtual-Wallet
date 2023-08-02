using DataAccess.Models.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Reflection.Emit;

namespace DataAccess.Repositories.Data.Configuration
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder
               .HasOne(u => u.Account)
               .WithOne(a => a.User)
               .HasForeignKey<Account>(a => a.UserId)
               .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
