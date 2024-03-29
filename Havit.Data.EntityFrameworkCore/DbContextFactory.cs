﻿using Microsoft.EntityFrameworkCore;

namespace Havit.Data.EntityFrameworkCore;

/// <inheritdoc />
public class DbContextFactory<TDbContext> : IDbContextFactory
	where TDbContext : Havit.Data.EntityFrameworkCore.DbContext
{
	private readonly IDbContextFactory<TDbContext> dbContextFactory;

	/// <summary>
	/// Konstruktor.
	/// </summary>
	public DbContextFactory(IDbContextFactory<TDbContext> dbContextFactory)
	{
		this.dbContextFactory = dbContextFactory;
	}

	/// <inheritdoc />
	public IDbContext CreateDbContext() => dbContextFactory.CreateDbContext();
}
