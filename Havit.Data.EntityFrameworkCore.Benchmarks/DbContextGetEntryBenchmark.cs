using BenchmarkDotNet.Attributes;
using Havit.Data.EntityFrameworkCore.Benchmarks.Infrastructure;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Havit.Data.EntityFrameworkCore.Benchmarks;

//| Method                                        | Mean      | Error      | StdDev   | Ratio | RatioSD | Gen0   | Allocated | Alloc Ratio |
//|---------------------------------------------- |----------:|-----------:|---------:|------:|--------:|-------:|----------:|------------:|
//| GetEntry_SuppressDetectChanges_True           |  10.62 ns |  10.320 ns | 0.566 ns |  1.00 |    0.06 | 0.0038 |      32 B |        1.00 |
//| GetEntry_SuppressDetectChanges_False          | 397.05 ns |  15.300 ns | 0.839 ns | 37.45 |    1.69 | 0.0687 |     576 B |       18.00 |
//| GetEntry_Previous_SuppressDetectChanges_False | 412.09 ns | 116.592 ns | 6.391 ns | 38.87 |    1.83 | 0.0687 |     576 B |       18.00 |
//| GetEntry_Previous_SuppressDetectChanges_True  |  19.59 ns |   6.609 ns | 0.362 ns |  1.85 |    0.09 | 0.0076 |      64 B |        2.00 |

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
		//_dbContext.AddRange(Enumerable.Range(1, 1000000).Select(i => new BenchmarkEntity { Id = i, Name = $"Test {i}" }));
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

	[Benchmark]
	public EntityEntry GetEntry_Previous_SuppressDetectChanges_False()
	{
		return GetEntry_Previous(_dbContext, _trackedEntity, suppressDetectChanges: false);
	}

	[Benchmark]
	public EntityEntry GetEntry_Previous_SuppressDetectChanges_True()
	{
		return GetEntry_Previous(_dbContext, _trackedEntity, suppressDetectChanges: true);
	}

	/// <summary>
	/// Vrací EntityEntry pro danou entitu.
	/// </summary>
	private static EntityEntry GetEntry_Previous(DbContext dbContext, object entity, bool suppressDetectChanges)
	{
		return suppressDetectChanges
			? dbContext.ExecuteWithoutAutoDetectChanges(() => dbContext.Entry(entity))
			: dbContext.Entry(entity);
	}
}
