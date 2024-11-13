using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using BenchmarkDotNet.Attributes;

namespace Havit.EFCoreTests.BenchmarkApp;

// dotnet run -c Release

//| Method                    | Mean      | Error     | StdDev    | Ratio | RatioSD |
//|-------------------------- |----------:|----------:|----------:|------:|--------:|
//| NET8 CompareByInstance    | 0.1716 ns | 0.1594 ns | 0.0087 ns |  1.00 |    0.00 |
//| NET8 CompareByICollection | 2.7006 ns | 0.5945 ns | 0.0326 ns | 15.76 |    0.67 |
//|-------------------------- |----------:|----------:|----------:|------:|--------:|
//| NET9 CompareByInstance    | 0.1614 ns | 0.1626 ns | 0.0089 ns |  1.00 |    0.00 |
//| NET9 CompareByICollection | 1.9937 ns | 0.0324 ns | 0.0018 ns | 12.38 |    0.68 |

[ShortRunJob]
public class EmptyCheckBenckmark
{
	private IEnumerable<string> emptyEnumerable = Enumerable.Empty<string>();

	[Benchmark(Baseline = true)]
	public bool CompareByInstance()
	{
		return emptyEnumerable == Enumerable.Empty<string>();
	}

	[Benchmark]
	public bool CompareByICollection()
	{
		return (emptyEnumerable is ICollection<string> ic) && (ic.Count == 0);
	}
}
