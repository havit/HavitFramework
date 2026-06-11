using BenchmarkDotNet.Attributes;
using Havit.Data.EntityFrameworkCore.Benchmarks.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace Havit.Data.EntityFrameworkCore.Benchmarks;

//| Method                      | Mean     | Error     | StdDev   | Ratio | RatioSD | Gen0   | Allocated | Alloc Ratio |
//|---------------------------- |---------:|----------:|---------:|------:|--------:|-------:|----------:|------------:|
//| IsNavigationLoaded          | 27.17 ns |  1.353 ns | 0.074 ns |  1.00 |    0.00 |      - |         - |          NA |
//| IsNavigationLoaded_Previous | 89.94 ns | 18.013 ns | 0.987 ns |  3.31 |    0.03 | 0.0181 |     152 B |          NA

[MemoryDiagnoser]
[ShortRunJob]
public class DbContextIsNavigationLoadedBenchmark
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

	[Benchmark(Baseline = true)]
	public bool IsNavigationLoaded()
	{
		return ((IDbContext)_dbContext).IsNavigationLoaded(_trackedEntity, nameof(BenchmarkEntity.Children));
	}

	[Benchmark]
	public bool IsNavigationLoaded_Previous()
	{
		// Předchozí implementace však používala též jinou implementaci GetEntry, to zde neuvažujeme.
		return _dbContext.GetEntry(_trackedEntity, suppressDetectChanges: true).Navigation(nameof(BenchmarkEntity.Children)).IsLoaded;
	}
}
