using System;
using System.Collections.Generic;

namespace Havit.Data.EntityFrameworkCore.BusinessLayer
{
	internal static class DictionaryExtensions
	{
		public static Dictionary<string, string> AddIfNotDefault<TValue>(this Dictionary<string, string> dictionary, string key, TValue value, TValue businessLayerGeneratorDefaultValue)
		{
			if (dictionary == null)
			{
				throw new ArgumentNullException(nameof(dictionary));
			}

			if (key == null)
			{
				throw new ArgumentNullException(nameof(key));
			}

			// operator== is undefined for generic T; EqualityComparer solves this
			if (!EqualityComparer<TValue>.Default.Equals(value, businessLayerGeneratorDefaultValue))
			{
				dictionary.Add(key, value.ToString());
			}

			return dictionary;
		}
	}
}