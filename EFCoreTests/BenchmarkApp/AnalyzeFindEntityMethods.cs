using BenchmarkDotNet.Attributes;
using Havit.Data.EntityFrameworkCore;
using Havit.Data.EntityFrameworkCore.Internal;
using Havit.EFCoreTests.Entity;
using Havit.EFCoreTests.Model;
using Microsoft.EntityFrameworkCore;

namespace Havit.EFCoreTests.BenchmarkApp;

// dotnet run -c Release

[ShortRunJob]
#pragma warning disable EF1001 // Internal EF Core API usage.
public class AnalyzeFindEntityMethods
{
	private readonly DbSetInternal<User> _dbSet;

	public AnalyzeFindEntityMethods()
	{
		DbContextOptionsBuilder optionsBuilder = new DbContextOptionsBuilder();
		optionsBuilder.UseInMemoryDatabase(nameof(AnalyzeFindEntityMethods));

		var dbContext = new ApplicationDbContext(optionsBuilder.Options);
		for (int i = 1; i <= 100; i++)
		{
			dbContext.Set<User>().Attach(new User { Id = i });
		}
		dbContext.ChangeTracker.DetectChanges();
		_dbSet = (DbSetInternal<User>)((IDbContext)dbContext).Set<User>();
	}

	[Benchmark(Baseline = true)]
	public object Set_FindTracked()
	{
		return _dbSet.FindTracked(5);
	}

#if BENCHMARKING
	[Benchmark]
	public object UsingLocal_FindEntry()
	{
		return _dbSet.UsingLocal_FindEntry(5);
	}

	[Benchmark]
	public object UsingLocal_FindEntryUntyped()
	{
		return _dbSet.UsingLocal_FindEntryUntyped(5);
	}

#endif

#pragma warning restore EF1001 // Internal EF Core API usage.
}
