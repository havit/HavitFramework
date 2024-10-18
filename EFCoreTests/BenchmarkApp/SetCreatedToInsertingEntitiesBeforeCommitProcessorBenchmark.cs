using System;
using System.Collections.Concurrent;
using System.Reflection;
using BenchmarkDotNet.Attributes;

namespace Havit.EFCoreTests.BenchmarkApp;

// dotnet run -c Release

//| Method                     | Mean      | Error      | StdDev    | Ratio | RatioSD |
//|--------------------------- |----------:|-----------:|----------:|------:|--------:|
//| WithCreated_GetProperty    | 18.361 ns | 13.8941 ns | 0.7616 ns |  1.40 |    0.06 |
//| WithoutCreated_GetProperty | 13.095 ns |  1.4308 ns | 0.0784 ns |  1.00 |    0.00 |
//| WithoutCreated_Dictionary  |  8.389 ns |  1.2569 ns | 0.0689 ns |  0.64 |    0.00 |
//| WithCreated_Dictionary     |  8.394 ns |  0.2687 ns | 0.0147 ns |  0.64 |    0.00 |

[ShortRunJob]
public class SetCreatedToInsertingEntitiesBeforeCommitProcessorBenchmark
{
	private WithCreated withCreated;
	private WithoutCreated withoutCreated;
	private ConcurrentDictionary<Type, PropertyInfo> dictionary;

	public SetCreatedToInsertingEntitiesBeforeCommitProcessorBenchmark()
	{
		withCreated = new WithCreated();
		withoutCreated = new WithoutCreated();
		dictionary = new ConcurrentDictionary<Type, PropertyInfo>();
		dictionary.TryAdd(typeof(WithoutCreated), null);
		dictionary.TryAdd(typeof(WithCreated), typeof(WithCreated).GetProperty("Created"));
	}

	[Benchmark]
	public void WithCreated_GetProperty()
	{
		PropertyInfo pi = withCreated.GetType().GetProperty("Created");
		if (pi != null && pi.PropertyType != typeof(DateTime))
		{
			pi = null;
		}
	}

	[Benchmark(Baseline = true)]
	public void WithoutCreated_GetProperty()
	{
		PropertyInfo pi = withoutCreated.GetType().GetProperty("Created");
		if (pi != null && pi.PropertyType != typeof(DateTime))
		{
			pi = null;
		}
	}

	[Benchmark]
	public void WithoutCreated_Dictionary()
	{
		var pi = dictionary.GetOrAdd(withoutCreated.GetType(), (type) => throw new ApplicationException());
	}

	[Benchmark]
	public void WithCreated_Dictionary()
	{
		var pi = dictionary.GetOrAdd(withCreated.GetType(), (type) => throw new ApplicationException());
	}


	public class WithCreated
	{
		public DateTime Created { get; set; }
	}

	public class WithoutCreated
	{
	}
}
