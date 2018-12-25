using System;
using Havit.Data.EntityFrameworkCore.BusinessLayer.Tests.ExtendedProperties;
using Microsoft.EntityFrameworkCore;

namespace Havit.Data.EntityFrameworkCore.BusinessLayer.Tests
{
	internal class EndToEndDbContext : TestDbContext
	{
		private readonly Action<ModelBuilder> onModelCreating;

		public EndToEndDbContext(Action<ModelBuilder> onModelCreating = default)
		{
			this.onModelCreating = onModelCreating;
		}

		protected override void CustomizeModelCreating(ModelBuilder modelBuilder)
		{
			base.CustomizeModelCreating(modelBuilder);
			onModelCreating?.Invoke(modelBuilder);
		}
	}
}