using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Havit.Diagnostics.Contracts;

namespace Havit.Data.Entity.Patterns.DataLoaders.Internal
{
	public class PropertiesSequenceExpressionVisitor : ExpressionVisitor
	{
		private string propertyPathString;
		private List<PropertyToLoad> propertiesToLoad;

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
			// podmínka vypadá "šíleně jednoduše", ale je opsána z EF (System.Data.Entity.Internal.DbHelpers.TryParsePath).
			// viz DbExtensionsIncludeTest
			if ((node.Method.Name == "Select") && (node.Arguments.Count == 2))
			{
				Visit(node.Arguments[0]);
				Visit(node.Arguments[1]);

				// NO BASE CALL! return base.VisitMethodCall(node);
				return node;
			}
			else
			{
				throw new NotSupportedException($"There is an unsupported method call \"{node.Method.Name}\" in the expression \"{propertyPathString}\".");
			}
		}

		protected override Expression VisitMember(MemberExpression node)
		{
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
						CollectionItemType = enumerableInterfaceType.GetGenericArguments()[0]
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
