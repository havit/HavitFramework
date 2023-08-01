using System.Linq.Expressions;

namespace Havit.Data.EntityFrameworkCore.Patterns.DataLoaders.Internal;

/// <summary>
/// Poskytuje seznam vlastností k načtení.
/// </summary>
public class PropertyLoadSequenceResolver : IPropertyLoadSequenceResolver
{
	/// <summary>
	/// Vrací z expression tree seznam vlastností, které mají být DataLoaderem načteny.
	/// </summary>
	public virtual PropertyToLoad[] GetPropertiesToLoad<TEntity, TProperty>(Expression<Func<TEntity, TProperty>> propertyPath)
		where TEntity : class
	{
		PropertiesSequenceExpressionVisitor visitor = new PropertiesSequenceExpressionVisitor();
		return visitor.GetPropertiesToLoad(propertyPath);
	}
}