using Havit.Diagnostics.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Havit.Data.EntityFrameworkCore.Patterns.DataSeeds.Internal
{
	internal static class PairExpressionWithCompilationExtensions
	{
		public static List<PairExpressionWithCompilation<TEntity>> ToPairByExpressionsWithCompilations<TEntity>(this List<Expression<Func<TEntity, object>>> pairByExpressions)
			where TEntity : class
		{
			Contract.Assert(pairByExpressions != null);

			return pairByExpressions.Select(expression => new PairExpressionWithCompilation<TEntity>
			{
				Expression = expression,
				CompiledLambda = expression.Compile()
			}).ToList();
		}
	}
}
