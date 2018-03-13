using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Havit.Data.EFCore.Tests.Infrastructure.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Havit.Data.EFCore.Tests.Infrastructure.Configurations
{
	internal class ModelClassConfiguration : IEntityTypeConfiguration<ModelClass>
	{
		public void Configure(EntityTypeBuilder<ModelClass> builder)
		{
			// NOOP
		}
	}
}
