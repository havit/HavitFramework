using System.Linq.Expressions;

namespace Havit.Data.Entity.Patterns.DataLoaders.Internal;

/// <summary>
/// Lambda expression jako expression tree a zkompilovaný.
/// </summary>
public class PropertyLambdaExpression<TEntity, TProperty>
{
	/// <summary>
	/// Lambda expression jako expression tree.
	/// </summary>
	public Expression<Func<TEntity, TProperty>> LambdaExpression { get; }

	/// <summary>
	/// Zkompilovaný lambda expression .
	/// </summary>
	public Func<TEntity, TProperty> LambdaCompiled { get; }

	/// <summary>
	/// Konstruktor.
	/// </summary>
	public PropertyLambdaExpression(Expression<Func<TEntity, TProperty>> lambdaExpression, Func<TEntity, TProperty> lambdaCompiled)
	{
		this.LambdaExpression = lambdaExpression;
		this.LambdaCompiled = lambdaCompiled;
	}
}
