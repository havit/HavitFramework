using BenchmarkDotNet.Attributes;
using Havit.Data.EntityFrameworkCore.Benchmarks.Infrastructure;

namespace Havit.Data.EntityFrameworkCore.Benchmarks;

//| Method           | Mean     | Error     | StdDev   | Ratio | RatioSD | Gen0   | Allocated | Alloc Ratio |
//|----------------- |---------:|----------:|---------:|------:|--------:|-------:|----------:|------------:|
//| FindTracked      | 17.28 ns | 10.943 ns | 0.600 ns |  1.00 |    0.04 | 0.0067 |      56 B |        1.00 |
//| FindTrackedTyped | 16.13 ns |  6.545 ns | 0.359 ns |  0.93 |    0.03 |      - |         - |        0.00 |

[MemoryDiagnoser]
[ShortRunJob]
public class DbSetFindTrackedBenchmark
{
	private BenchmarkDbContext _dbContext;
	private IDbSet<BenchmarkEntity> _dbSet;

	[GlobalSetup]
	public void Setup()
	{
		_dbContext = new BenchmarkDbContext();
		_dbContext.Add(new BenchmarkEntity { Id = 1, Name = "Test" });
		_dbContext.SaveChanges();
		_dbSet = ((IDbContext)_dbContext).Set<BenchmarkEntity>();
	}

	[GlobalCleanup]
	public void Cleanup()
	{
		_dbContext.Dispose();
	}

	[Benchmark(Baseline = true)]
	public BenchmarkEntity FindTracked()
	{
		return _dbSet.FindTracked(1);
	}

	[Benchmark]
	public BenchmarkEntity FindTrackedTyped()
	{
		return _dbSet.FindTrackedTyped<int>(1);
	}
}
