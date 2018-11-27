using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Havit.Data.EntityFrameworkCore.Patterns.DataSeeds.Internal
{
	internal static class DataWithPairByValuesExtensions
	{
		public static void ThrowIfContainsDuplicates<TEntity>(this List<DataWithPairByValues<TEntity>> dataWithPairByValues, string message)
			where TEntity : class
		{
			var duplicateDataWithPairByValues = dataWithPairByValues.ToLookup(item => item.PairByValues /* overrides Equals*/ ).Where(itemGroup => itemGroup.Count() > 1).Select(item => item.Key).ToList();
			if (duplicateDataWithPairByValues.Count > 0)
			{
				string duplicates = String.Join(", ", duplicateDataWithPairByValues /* overrides ToString */);
				throw new InvalidOperationException(message + $" Duplicates: {duplicates}.");
			}
		}
	}
}
