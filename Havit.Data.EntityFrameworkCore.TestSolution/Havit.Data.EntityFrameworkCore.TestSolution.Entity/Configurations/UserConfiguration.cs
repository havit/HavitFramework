using Havit.Data.EntityFrameworkCore.TestSolution.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Havit.Data.EntityFrameworkCore.TestSolution.Entity.Configurations
{
	public class UserConfiguration : IEntityTypeConfiguration<User>
	{
		public void Configure(EntityTypeBuilder<User> builder)
		{
			builder.HasMany(u => u.Roles).WithOne(ur => ur.User).HasForeignKey(ur => ur.UserId);
		}
	}
}
