using System.Linq.Expressions;

namespace Havit.Data.EntityFrameworkCore.Patterns.DataSeeds.Internal;

internal static class PairExpressionWithCompilationExtensions
{
	public static List<PairExpressionWithCompilation<TEntity>> ToPairByExpressionsWithCompilations<TEntity>(this List<Expression<Func<TEntity, object>>> pairByExpressions)
		where TEntity : class
	{
		ArgumentNullException.ThrowIfNull(pairByExpressions);

		return pairByExpressions.Select(expression => new PairExpressionWithCompilation<TEntity>
		{
			Expression = expression,
			CompiledLambda = expression.Compile()
		}).ToList();
	}
}
