using Havit.Diagnostics.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Havit.Data.EntityFrameworkCore.Patterns.DataLoaders.Internal
{
	internal static class QueryHelpers
	{
		internal static Expression<Func<TEntity, bool>> ContainsEffective<TEntity>(this List<int> values, Expression<Func<TEntity, int>> propertyAccessor)
		{
			Contract.Requires(values != null);

			// žádný hodnota -> item => false;
			if (values.Count == 0)
			{
				return (Expression<Func<TEntity, bool>>)Expression.Lambda(Expression.Constant(false), propertyAccessor.Parameters);
			}

			// jediný záznam - testujeme na rovnost
			if (values.Count == 1)
			{								
				// item => item == [0]
				return (Expression<Func<TEntity, bool>>)Expression.Lambda(
					Expression.Equal(
						propertyAccessor.Body,
						Expression.Constant(values.Single())),
					propertyAccessor.Parameters);
			}

			// více záznamů
			// pokud jde o řadu IDček (1, 2, 3, 4) bez přeskakování, pak použijeme porovnání >= a  <=.
			int[] sortedKeysToQuery = values.OrderBy(item => item).Distinct().ToArray();

			//pro pole: 1, 2, 3, 4
			// if 1 + 4 - 1 (4) == 4
			if ((sortedKeysToQuery[0] + sortedKeysToQuery.Length - 1) == sortedKeysToQuery[sortedKeysToQuery.Length - 1]) // testujeme, zda jde o posloupnost IDček
			{
				return (Expression<Func<TEntity, bool>>)Expression.Lambda(
					Expression.AndAlso(
						Expression.GreaterThanOrEqual(propertyAccessor.Body, Expression.Constant(sortedKeysToQuery.First())),
						Expression.LessThanOrEqual(propertyAccessor.Body, Expression.Constant(sortedKeysToQuery.Last()))),
					propertyAccessor.Parameters);
			}

			// v obecném případě hledáme přes IN (...)
			return (Expression<Func<TEntity, bool>>)Expression.Lambda(
				Expression.Call(
					Expression.Constant(values),
					typeof(List<int>).GetMethod("Contains"),
					new List<Expression> { propertyAccessor.Body }),
				propertyAccessor.Parameters);

		}
	}
}
