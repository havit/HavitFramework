using BenchmarkDotNet.Attributes;
using Havit.Data.EntityFrameworkCore.Patterns.Benchmarks.Infrastructure;
using Havit.Data.EntityFrameworkCore.Patterns.SoftDeletes;

namespace Havit.Data.EntityFrameworkCore.Patterns.Benchmarks;

// Previous = implementace s dynamic a Contract.Requires s String.Format (alokace = closure, boxing DateTime?, String.Format).
//| Method              | Mean      | Error     | StdDev    | Ratio | Gen0   | Allocated |
//|-------------------- |----------:|----------:|----------:|------:|-------:|----------:|
//| SetDeleted          |  7.141 ns | 1.2145 ns | 0.0666 ns |  1.00 |      - |         - |
//| SetDeleted_Previous | 53.693 ns | 6.6830 ns | 0.3663 ns |  7.52 | 0.0186 |     312 B |

// ShortRun job (vč. workaroundu pro multi-target závislosti) je nastaven globálně v Program.cs.
[MemoryDiagnoser]
public class SoftDeleteManagerSetDeletedBenchmark
{
	private SoftDeleteManager _softDeleteManager;
	private PreviousSoftDeleteManager _previousSoftDeleteManager;
	private BenchmarkEntity _entity;

	[GlobalSetup]
	public void Setup()
	{
		_softDeleteManager = new SoftDeleteManager(new FixedTimeService());
		_previousSoftDeleteManager = new PreviousSoftDeleteManager(new FixedTimeService());
		_entity = new BenchmarkEntity { Id = 1, Name = "Test" };

		// naplnění cache, měříme opakované volání
		_softDeleteManager.SetDeleted(_entity);
		_previousSoftDeleteManager.SetDeleted(_entity);
	}

	// Reset příznaku v těle benchmarku zajišťuje, že se vždy měří i nastavení hodnoty (nikoliv jen přečtení již nastaveného příznaku).
	// Cena resetu je v obou variantách stejná.

	[Benchmark(Baseline = true)]
	public void SetDeleted()
	{
		_entity.Deleted = null;
		_softDeleteManager.SetDeleted(_entity);
	}

	[Benchmark]
	public void SetDeleted_Previous()
	{
		_entity.Deleted = null;
		_previousSoftDeleteManager.SetDeleted(_entity);
	}
}
