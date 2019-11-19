using System;
using Havit.Data.EntityFrameworkCore.BusinessLayer.Tests.ModelExtensions.Fakes;
using Havit.Data.EntityFrameworkCore.Migrations.ModelExtensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Havit.Data.EntityFrameworkCore.BusinessLayer.Tests
{
    public class EndToEndTestDbContext<TEntity> : EndToEndTestDbContext
		where TEntity : class
	{
		public EndToEndTestDbContext(Action<ModelBuilder> onModelCreating = default)
			: base(onModelCreating)
		{ }

        public DbSet<TEntity> Entities { get; }
	}
}
