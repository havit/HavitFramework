using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Havit.Diagnostics.Contracts;

namespace Havit.Data.EntityFrameworkCore.Patterns.DataLoaders.Internal
{
	internal class PropertiesSequenceExpressionVisitor : ExpressionVisitor
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
				Type propertyType = ((PropertyInfo)node.Member).PropertyType;
				Type enumerableInterfaceType = propertyType.GetInterfaces().FirstOrDefault(item => item.IsGenericType && item.GetGenericTypeDefinition() == typeof(IEnumerable<>));

				if (enumerableInterfaceType != null)
				{
					propertiesToLoad.Add(new PropertyToLoad
					{
						SourceType = node.Member.DeclaringType,
						PropertyName = node.Member.Name,
						OriginalPropertyName = node.Member.Name,
						TargetType = ((PropertyInfo)node.Member).PropertyType,
						OriginalTargetType = ((PropertyInfo)node.Member).PropertyType,
						CollectionItemType = enumerableInterfaceType.GetGenericArguments()[0]
					});

				}
				else
				{
					propertiesToLoad.Add(new PropertyToLoad
					{
						SourceType = node.Member.DeclaringType,
						PropertyName = node.Member.Name,
						OriginalPropertyName = node.Member.Name,
						TargetType = ((PropertyInfo)node.Member).PropertyType,
						OriginalTargetType = ((PropertyInfo)node.Member).PropertyType
					});
				}
			}
			return result;
		}
	}
}
