using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;
using Havit.Data.EntityFrameworkCore.Patterns.Lookups;
using Havit.Services.Caching;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace Havit.EFCoreTests.BenchmarkApp;

// dotnet run -c Release

[ShortRunJob]
public class EntityLookupDataStorageBenchmark
{
	private const string StorageKey = "MyStorageKey";
	private readonly CacheEntityLookupDataStorage _cacheEntityLookupDataStorage;
	private readonly DictionaryEntityLookupDataStorage _dictionaryEntityLookupDataStorage = new DictionaryEntityLookupDataStorage();

	public EntityLookupDataStorageBenchmark()
	{
		_cacheEntityLookupDataStorage = new CacheEntityLookupDataStorage(new MemoryCacheService(new MemoryCache(Options.Create(new MemoryCacheOptions { }))));
		_cacheEntityLookupDataStorage.StoreEntityLookupData(StorageKey, new EntityLookupData<CultureInfo, int, string>(new Dictionary<string, int>(), new Dictionary<int, string>(), ci => ci.Name, _ => true));

		_dictionaryEntityLookupDataStorage = new DictionaryEntityLookupDataStorage();
		_dictionaryEntityLookupDataStorage.StoreEntityLookupData(StorageKey, new EntityLookupData<CultureInfo, int, string>(new Dictionary<string, int>(), new Dictionary<int, string>(), ci => ci.Name, _ => true));

	}

	[Benchmark]
	public object CacheEntityLookupDataStorage_GetEntityLookupData()
	{
		return _cacheEntityLookupDataStorage.GetEntityLookupData<CultureInfo, int, string>(StorageKey);
	}

	[Benchmark(Baseline = true)]
	public object DictionaryEntityLookupDataStorage_GetEntityLookupData()
	{
		return _dictionaryEntityLookupDataStorage.GetEntityLookupData<CultureInfo, int, string>(StorageKey);
	}

}
