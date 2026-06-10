using BenchmarkDotNet.Attributes;
using Havit.Data.EntityFrameworkCore.Benchmarks.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace Havit.Data.EntityFrameworkCore.Benchmarks;

[MemoryDiagnoser]
[ShortRunJob]
public class DbContextGetEntityStateBenchmark
{
	private BenchmarkDbContext _dbContext;
	private BenchmarkEntity _trackedEntity;

	[GlobalSetup]
	public void Setup()
	{
		_dbContext = new BenchmarkDbContext();
		_trackedEntity = new BenchmarkEntity { Id = 1, Name = "Test" };
		_dbContext.Add(_trackedEntity);
		_dbContext.SaveChanges();
	}

	[GlobalCleanup]
	public void Cleanup()
	{
		_dbContext.Dispose();
	}

	[Benchmark]
	public EntityState GetEntityState()
	{
		return ((IDbContext)_dbContext).GetEntityState(_trackedEntity);
	}
}
