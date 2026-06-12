using BenchmarkDotNet.Attributes;
using Havit.Data.EntityFrameworkCore.Patterns.Benchmarks.Infrastructure;
using Havit.Data.EntityFrameworkCore.Patterns.SoftDeletes;

namespace Havit.Data.EntityFrameworkCore.Patterns.Benchmarks;

// Previous = implementace s ConcurrentDictionary<Type, bool> (alokace = closure nad entityType v GetOrAdd).
//| Method                         | Mean     | Error     | StdDev    | Ratio | Gen0   | Allocated |
//|------------------------------- |---------:|----------:|----------:|------:|-------:|----------:|
//| IsSoftDeleteSupported          | 3.484 ns | 0.8435 ns | 0.0462 ns |  1.00 |      - |         - |
//| IsSoftDeleteSupported_Previous | 3.612 ns | 1.0181 ns | 0.0558 ns |  1.04 | 0.0014 |      24 B |

// ShortRun job (vč. workaroundu pro multi-target závislosti) je nastaven globálně v Program.cs.
[MemoryDiagnoser]
public class SoftDeleteManagerIsSoftDeleteSupportedBenchmark
{
	private SoftDeleteManager _softDeleteManager;
	private PreviousSoftDeleteManager _previousSoftDeleteManager;

	[GlobalSetup]
	public void Setup()
	{
		_softDeleteManager = new SoftDeleteManager(new FixedTimeService());
		_previousSoftDeleteManager = new PreviousSoftDeleteManager(new FixedTimeService());

		// naplnění cache, měříme opakované volání
		_softDeleteManager.IsSoftDeleteSupported<BenchmarkEntity>();
		_previousSoftDeleteManager.IsSoftDeleteSupported<BenchmarkEntity>();
	}

	[Benchmark(Baseline = true)]
	public bool IsSoftDeleteSupported()
	{
		return _softDeleteManager.IsSoftDeleteSupported<BenchmarkEntity>();
	}

	[Benchmark]
	public bool IsSoftDeleteSupported_Previous()
	{
		return _previousSoftDeleteManager.IsSoftDeleteSupported<BenchmarkEntity>();
	}
}
