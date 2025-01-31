﻿using System.Data.Entity;
using Havit.Data.Entity.Tests.Infrastructure.Model;

namespace Havit.Data.Entity.Tests.Infrastructure;

// Kvůli záměru použití s nastavením databáze nedědí z TestDbContextBase
public class TwoContructorsDbContext : DbContext
{
	public TwoContructorsDbContext() : base("Havit.Data.Entity6.Tests.TwoContructorsDbContext1")
	{

	}

	public TwoContructorsDbContext(string nameOrConnectionString) : base(nameOrConnectionString)
	{

	}

	protected override void OnModelCreating(DbModelBuilder modelBuilder)
	{
		base.OnModelCreating(modelBuilder);

		modelBuilder.RegisterEntityType(typeof(Master));
		modelBuilder.RegisterEntityType(typeof(Child));
	}

}
