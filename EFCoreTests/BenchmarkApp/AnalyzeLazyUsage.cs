using System;
using System.Threading;
using BenchmarkDotNet.Attributes;

namespace Havit.EFCoreTests.BenchmarkApp;

// dotnet run -c Release

[ShortRunJob]
public class AnalyzeLazyUsage
{
	private readonly Lazy<object> _lazy = new Lazy<object>(() => new object(), LazyThreadSafetyMode.None);
	private object _object;

	public AnalyzeLazyUsage()
	{
	}

	//[Benchmark(Baseline = true)]
	public object Lazy()
	{
		return _lazy.Value;
	}

	//[Benchmark]
	public object NonLazy()
	{
		return (_object ??= new object());
	}

}
