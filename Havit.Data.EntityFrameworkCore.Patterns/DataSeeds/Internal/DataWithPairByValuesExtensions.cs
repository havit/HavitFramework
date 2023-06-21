using System;
using System.Collections.Generic;
using System.Linq;
using Havit.Linq.Expressions;

namespace Havit.Data.EntityFrameworkCore.Patterns.DataSeeds.Internal;

internal static class DataWithPairByValuesExtensions
{
	public static void ThrowIfContainsDuplicates<TEntity>(this List<DataWithPairByValues<TEntity>> dataWithPairByValues, string message, List<PairExpressionWithCompilation<TEntity>> pairByExpressions)
		where TEntity : class
	{
		var duplicateDataWithPairByValues = dataWithPairByValues.ToLookup(item => item.PairByValues /* overrides Equals*/ ).Where(itemGroup => itemGroup.Count() > 1).Select(item => item.First()).ToList();
		if (duplicateDataWithPairByValues.Count > 0)
		{
			string duplicates = String.Join(", ", duplicateDataWithPairByValues.Select(item => FormatDuplicateItem(item, pairByExpressions)));
			throw new InvalidOperationException(message + " " + duplicates + ".");
		}
	}

	private static string FormatDuplicateItem<TEntity>(DataWithPairByValues<TEntity> dataWithPairByValues, List<PairExpressionWithCompilation<TEntity>> pairByExpressions)
		where TEntity : class
	{
		return "("
			+ String.Join(", ", Enumerable.Range(0, dataWithPairByValues.PairByValues.Data.Length).Select(index => FormatDuplicateItemProperty<TEntity>(dataWithPairByValues, pairByExpressions, index)))
			+ ")";
	}

	private static string FormatDuplicateItemProperty<TEntity>(DataWithPairByValues<TEntity> dataWithPairByValues, List<PairExpressionWithCompilation<TEntity>> pairByExpressions, int index)
		where TEntity : class
	{
		string propertyNamePrefix = "";

		try
		{
			propertyNamePrefix = ExpressionExt.GetMemberAccessMemberName(pairByExpressions[index].Expression) + ": ";
		}
		catch (InvalidOperationException)
		{
			// NOOP
		}

		// závorky - pozor na prioritu operátorů
		return propertyNamePrefix + (dataWithPairByValues.PairByValues.Data[index]?.ToString() ?? "null");
	}
}
