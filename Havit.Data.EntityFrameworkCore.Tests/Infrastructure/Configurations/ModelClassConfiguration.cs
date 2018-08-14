using Havit.Data.Entity.Tests.Infrastructure.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Havit.Data.Entity.Tests.Infrastructure.Configurations
{
	internal class ModelClassConfiguration : IEntityTypeConfiguration<ModelClass>
	{
		public void Configure(EntityTypeBuilder<ModelClass> builder)
		{
			// NOOP
		}
	}
}
