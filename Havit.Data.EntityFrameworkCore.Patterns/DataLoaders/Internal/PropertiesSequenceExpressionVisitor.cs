using System.Linq.Expressions;
using System.Reflection;

namespace Havit.Data.EntityFrameworkCore.Patterns.DataLoaders.Internal;

internal class PropertiesSequenceExpressionVisitor : ExpressionVisitor
{
	private string propertyPathString;
	private List<PropertyToLoad> propertiesToLoad;

	public PropertyToLoad[] GetPropertiesToLoad<TEntity, TProperty>(Expression<Func<TEntity, TProperty>> propertyPath)
		where TEntity : class
	{
		ArgumentNullException.ThrowIfNull(propertyPath);

		propertyPathString = propertyPath.ToString();

		propertiesToLoad = new List<PropertyToLoad>();
		Visit(propertyPath);
		return propertiesToLoad.ToArray();
	}

	public override Expression Visit(Expression node)
	{
		if (node == null)
		{
			return null;
		}

		switch (node.NodeType)
		{
			case ExpressionType.Parameter:
			case ExpressionType.Lambda:
			case ExpressionType.MemberAccess:
				return base.Visit(node);

			default:
				throw new NotSupportedException($"There is unsupported node \"{node.NodeType}\" in the expression \"{propertyPathString}\".");
		}
	}

	protected override Expression VisitMember(MemberExpression node)
	{
		Expression result = base.VisitMember(node);

		if (node.NodeType == ExpressionType.MemberAccess)
		{
			PropertyInfo property = GetPropertyToLoad(node);

			Type propertyType = property.PropertyType;
			// string a byte[] sice implementují IEnumerable<>, ale nejsou to navigace na kolekce - nesmí být klasifikovány jako kolekce.
			Type enumerableInterfaceType = ((propertyType != typeof(string)) && (propertyType != typeof(byte[])))
				? propertyType.GetInterfaces().FirstOrDefault(item => item.IsGenericType && item.GetGenericTypeDefinition() == typeof(IEnumerable<>))
				: null;

			propertiesToLoad.Add(new PropertyToLoad
			{
				SourceType = property.DeclaringType,
				PropertyName = property.Name,
				OriginalPropertyName = property.Name,
				TargetType = propertyType,
				OriginalTargetType = propertyType,
				CollectionItemType = enumerableInterfaceType?.GetGenericArguments()[0]
			});
		}
		return result;
	}

	/// <summary>
	/// Vrací vlastnost, která má být pro daný member access načtena.
	/// Pokud je v expression tree vlastnost navázána na interface (lambda vzniklá v generické metodě s type parametrem omezeným interfacem),
	/// remapuje ji na stejnojmennou public vlastnost typu, na kterém k přístupu dochází (předpokládá se implicitní implementace).
	/// SourceType tak není interface, se kterým by DataLoader dále pracovat neuměl.
	/// </summary>
	/// <exception cref="InvalidOperationException">Pokud na typu, na kterém k přístupu dochází, stejnojmenná public vlastnost není (např. explicitní implementace interface).</exception>
	private PropertyInfo GetPropertyToLoad(MemberExpression node)
	{
		PropertyInfo property = (PropertyInfo)node.Member;

		if (property.DeclaringType.IsInterface && !node.Expression.Type.IsInterface)
		{
			property = node.Expression.Type.GetProperty(property.Name, BindingFlags.Public | BindingFlags.Instance)
				?? throw new InvalidOperationException($"DataLoader cannot load property {property.Name} declared on interface {property.DeclaringType.FullName} while there is no public property of the same name on type {node.Expression.Type.FullName}. The property is required to be implemented implicitly (as a public property with the same name).");
		}

		return property;
	}
}
