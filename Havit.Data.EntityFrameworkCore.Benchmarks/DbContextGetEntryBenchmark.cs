using BenchmarkDotNet.Attributes;
using Havit.Data.EntityFrameworkCore.Benchmarks.Infrastructure;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Havit.Data.EntityFrameworkCore.Benchmarks;

[MemoryDiagnoser]
[ShortRunJob]
public class DbContextGetEntryBenchmark
{
	private BenchmarkDbContext _dbContext;
	private BenchmarkEntity _trackedEntity;

	[GlobalSetup]
	public void Setup()
	{
		_dbContext = new BenchmarkDbContext();
		_trackedEntity = new BenchmarkEntity { Id = -1, Name = "Test" };
		_dbContext.Add(_trackedEntity);
		_dbContext.SaveChanges();
	}

	[GlobalCleanup]
	public void Cleanup()
	{
		_dbContext.Dispose();
	}

	[Benchmark(Baseline = true)]
	public EntityEntry GetEntry_SuppressDetectChanges_True()
	{
		return _dbContext.GetEntry(_trackedEntity, suppressDetectChanges: true);
	}

	[Benchmark]
	public EntityEntry GetEntry_SuppressDetectChanges_False()
	{
		return _dbContext.GetEntry(_trackedEntity, suppressDetectChanges: false);
	}
}
