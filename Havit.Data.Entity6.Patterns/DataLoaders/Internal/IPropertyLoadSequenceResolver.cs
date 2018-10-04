using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Havit.Data.Entity.Patterns.DataLoaders.Internal
{
	/// <summary>
	/// Poskytuje seznam vlastností k načtení.
	/// </summary>
	public interface IPropertyLoadSequenceResolver
	{
		/// <summary>
		/// Vrací z expression tree seznam vlastností, které mají být DataLoaderem načteny.
		/// </summary>
		PropertyToLoad[] GetPropertiesToLoad<TEntity, TProperty>(Expression<Func<TEntity, TProperty>> propertyPath)
			where TEntity : class;
	}
}
