using System.Threading;
using System;
using BenchmarkDotNet.Attributes;
using Havit.Data.EntityFrameworkCore;
using Havit.EFCoreTests.Entity;
using Havit.EFCoreTests.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Havit.EFCoreTests.BenchmarkApp;

// dotnet run -c Release

[ShortRunJob]
#pragma warning disable EF1001 // Internal EF Core API usage.
public class AnalyzeFindEntityMethods
{
	private readonly ApplicationDbContext _dbContext;
	private IKey _primaryKey;
	private IStateManager _stateManager;

	public AnalyzeFindEntityMethods()
	{
		DbContextOptionsBuilder optionsBuilder = new DbContextOptionsBuilder();
		optionsBuilder.UseInMemoryDatabase(nameof(AnalyzeFindEntityMethods));

		var dbContext = _dbContext = new ApplicationDbContext(optionsBuilder.Options);
		for (int i = 1; i <= 100; i++)
		{
			_dbContext.Set<User>().Attach(new User { Id = i });
		}
		_dbContext.ChangeTracker.DetectChanges();

		_primaryKey = _dbContext.Model.FindEntityType(typeof(User)).FindPrimaryKey();
		_stateManager = _dbContext.GetService<IStateManager>();
	}

	[Benchmark]
	public object LocalView_FindEntry()
	{
		try
		{
			_dbContext.ChangeTracker.AutoDetectChangesEnabled = false;
			return _dbContext.Set<User>().Local.FindEntry<int>(5)?.Entity;
		}
		finally
		{
			_dbContext.ChangeTracker.AutoDetectChangesEnabled = true;
		}
	}

	[Benchmark]
	public object Set_FindTracked()
	{
		return ((IDbContext)_dbContext).Set<User>().FindTracked([5]);
	}

	[Benchmark]
	public object LocalView_FindEntryUntyped()
	{
		try
		{
			_dbContext.ChangeTracker.AutoDetectChangesEnabled = false;
			return _dbContext.Set<User>().Local.FindEntryUntyped([5])?.Entity;
		}
		finally
		{
			_dbContext.ChangeTracker.AutoDetectChangesEnabled = true;
		}
	}

	[Benchmark(Baseline = true)]
	public object StateManeger_TryGetEntry()
	{
		_dbContext.Set<User>();
		return _stateManager.TryGetEntry(_primaryKey, [5])?.Entity;
	}

#pragma warning restore EF1001 // Internal EF Core API usage.
}
