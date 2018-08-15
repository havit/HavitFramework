using System;
using System.Collections.Generic;

namespace Havit.Data.EntityFrameworkCore.BusinessLayer
{
	internal static class DictionaryExtensions
	{
		public static Dictionary<string, string> AddIfNotDefault<TValue>(this Dictionary<string, string> dictionary, string key, TValue value)
		{
			if (dictionary == null)
			{
				throw new ArgumentNullException(nameof(dictionary));
			}

			if (key == null)
			{
				throw new ArgumentNullException(nameof(key));
			}

			if ((value != null) && (!EqualityComparer<TValue>.Default.Equals(value, default(TValue))))
			{
				dictionary.Add(key, value.ToString());
			}

			return dictionary;
		}
	}
}