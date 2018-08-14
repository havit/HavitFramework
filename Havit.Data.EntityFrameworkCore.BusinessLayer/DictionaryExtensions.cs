using System;
using System.Collections.Generic;
using Havit.Diagnostics.Contracts;
using Microsoft.EntityFrameworkCore.Internal;

namespace Havit.Data.EntityFrameworkCore.BusinessLayer
{
	internal static class DictionaryExtensions
	{
		public static Dictionary<TKey, TValue> AddIfNotDefault<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, TKey key, TValue value)
		{
			Contract.Requires<ArgumentNullException>(dictionary != null);
			Contract.Requires<ArgumentNullException>(key != null);

			if (value != null)
			{
				dictionary.Add(key, value);
			}

			return dictionary;
		}

		public static Dictionary<string, string> AddIfNotDefault(this Dictionary<string, string> dictionary, string key, object value)
		{
			Contract.Requires<ArgumentNullException>(dictionary != null);
			Contract.Requires<ArgumentNullException>(key != null);

			if (value != null && !value.GetType().IsDefaultValue(value))
			{
				dictionary.Add(key, value.ToString());
			}

			return dictionary;
		}

		public static Dictionary<string, string> AddIfNotDefault(this Dictionary<string, string> dictionary, string key, object value, object defaultValue)
		{
			Contract.Requires<ArgumentNullException>(dictionary != null);
			Contract.Requires<ArgumentNullException>(key != null);

			if (value != null && !value.Equals(defaultValue))
			{
				dictionary.Add(key, value.ToString());
			}

			return dictionary;
		}
	}
}