using System;
using Microsoft.EntityFrameworkCore;

namespace Havit.Data.EntityFrameworkCore.BusinessLayer.Tests;

public class TestDbContext<T> : TestDbContext
	where T : class
{
	public TestDbContext(Action<ModelBuilder> onModelCreating = default)
		: base(onModelCreating)
	{
	}

	protected override void CustomizeModelCreating(ModelBuilder modelBuilder)
	{
		base.CustomizeModelCreating(modelBuilder);
		modelBuilder.Entity<T>();
	}
}