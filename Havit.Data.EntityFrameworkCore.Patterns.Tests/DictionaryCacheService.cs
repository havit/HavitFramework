using Havit.Services.Caching;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Havit.Data.EntityFrameworkCore.Patterns.Tests
{
	public class DictionaryCacheService : ICacheService
	{
		private Dictionary<string, object> values = new Dictionary<string, object>();

		public bool SupportsCacheDependencies => false;

		public void Add(string key, object value, CacheOptions options = null)
		{
			values[key] = value;
		}

		public void Clear()
		{
			values = new Dictionary<string, object>();
		}

		public bool Contains(string key)
		{
			return values.ContainsKey(key);
		}

		public void Remove(string key)
		{
			values.Remove(key);
		}

		public bool TryGet(string key, out object result)
		{
			return values.TryGetValue(key, out result);
		}
	}

}
