using System;
using Microsoft.EntityFrameworkCore;

namespace Havit.Data.EntityFrameworkCore.BusinessLayer.Tests
{
	internal class EndToEndDbContext<TEntity> : EndToEndDbContext
		where TEntity : class
	{
		public EndToEndDbContext(Action<ModelBuilder> onModelCreating = default)
			: base(onModelCreating)
		{ }

		public DbSet<TEntity> Entities { get; }
	}
}
