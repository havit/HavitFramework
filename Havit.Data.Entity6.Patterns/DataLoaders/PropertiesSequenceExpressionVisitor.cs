using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Havit.Diagnostics.Contracts;

namespace Havit.Data.Entity.Patterns.DataLoaders
{
	internal class PropertiesSequenceExpressionVisitor : ExpressionVisitor
	{
		private bool collectionUnwrapRequired = false;
		private string propertyPathString;
		private List<PropertyToLoad> propertiesToLoad;
		private readonly MethodInfo unwrapCollectionMethodInfo = typeof(EnumerableExtensions).GetMethod(nameof(EnumerableExtensions.Unwrap), BindingFlags.Static | BindingFlags.Public);

		public PropertyToLoad[] GetPropertiesToLoad<TEntity, TProperty>(Expression<Func<TEntity, TProperty>> propertyPath)
			where TEntity : class
		{
			Contract.Requires(propertyPath != null);

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
				case ExpressionType.Call:
				case ExpressionType.MemberAccess:
					return base.Visit(node);

				default:
					throw new NotSupportedException($"There is unsupported node \"{node.NodeType}\" in the expression \"{propertyPathString}\".");
            }
		}

		protected override Expression VisitMethodCall(MethodCallExpression node)
		{
			if (!node.Method.IsGenericMethod || node.Method.GetGenericMethodDefinition() != unwrapCollectionMethodInfo)
			{
				throw new NotSupportedException($"There is unsupported method call \"{node.Method.Name}\" in the expression \"{propertyPathString}\".");
			}

			collectionUnwrapRequired = true;

			return base.VisitMethodCall(node);
		}

		protected override Expression VisitMember(MemberExpression node)
		{
			bool collectionUnwrapThisMember = this.collectionUnwrapRequired;
			collectionUnwrapRequired = false;

			Expression result = base.VisitMember(node);

			if (node.NodeType == ExpressionType.MemberAccess)
			{
				Type propertyType = ((PropertyInfo)node.Member).PropertyType;
				Type enumerableInterfaceType = propertyType.GetInterfaces().FirstOrDefault(item => item.IsGenericType && item.GetGenericTypeDefinition() == typeof(IEnumerable<>));

				if (enumerableInterfaceType != null)
				{
					propertiesToLoad.Add(new PropertyToLoad
					{
						SourceType = node.Member.DeclaringType,
						PropertyName = node.Member.Name,
						TargetType = ((PropertyInfo)node.Member).PropertyType,
						CollectionItemType = enumerableInterfaceType.GetGenericArguments()[0],
						CollectionUnwrapped = collectionUnwrapThisMember
					});

				}
				else
				{
					propertiesToLoad.Add(new PropertyToLoad
					{
						SourceType = node.Member.DeclaringType,
						TargetType = ((PropertyInfo)node.Member).PropertyType,
						PropertyName = node.Member.Name
					});
				}
			}
			return result;
		}
	}
}
