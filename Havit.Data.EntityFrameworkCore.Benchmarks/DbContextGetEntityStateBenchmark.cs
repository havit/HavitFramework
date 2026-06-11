using BenchmarkDotNet.Attributes;
using Havit.Data.EntityFrameworkCore.Benchmarks.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace Havit.Data.EntityFrameworkCore.Benchmarks;

// Do věcí vstupuje neznámý element, při otočení implementací (alternative se stane produkční variantou a naopak),
// se alternative opět stane lehce rychlejší.
//| Method                     | Mean     | Error    | StdDev    | Ratio | RatioSD | Allocated | Alloc Ratio |
//|--------------------------- |---------:|---------:|----------:|------:|--------:|----------:|------------:|
//| GetEntityState             | 9.779 ns | 5.213 ns | 0.2858 ns |  1.00 |    0.04 |         - |          NA |
//| GetEntityState_Alternative | 7.098 ns | 1.927 ns | 0.1056 ns |  0.73 |    0.02 |         - |          NA |

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

	[Benchmark(Baseline = true)]
	public EntityState GetEntityState()
	{
		return ((IDbContext)_dbContext).GetEntityState(_trackedEntity);
	}

	[Benchmark]
	public EntityState GetEntityState_Alternative()
	{
		return _dbContext.GetEntry(_trackedEntity, suppressDetectChanges: true).State;
	}
}
