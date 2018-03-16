using System;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Havit.Data.Entity.Metadata.Builders
{
	/// <summary>
	/// ExtensionMetody k <see cref="EntityTypeBuilder{T}" />.
	/// </summary>
	public static class EntityTypeBuilderExtensions 	
    {
		/// <summary>
		/// Konfiguruje unikání index s danými vlastnostmi.
		/// Implementováno zřetězením metod HasIndex(...).IsUnique().
		/// </summary>
	    public static IndexBuilder HasUniqueIndex<TEntity>(this EntityTypeBuilder<TEntity> entityTypeBuilder, Expression<Func<TEntity, object>> indexExpression)
		    where TEntity : class
	    {
		    return entityTypeBuilder.HasIndex(indexExpression).IsUnique();
	    }

		/// <summary>
		/// Konfiguruje unikání index s danými vlastnostmi.
		/// Implementováno zřetězením metod HasIndex(...).IsUnique().
		/// </summary>
	    public static IndexBuilder HasUniqueIndex(this EntityTypeBuilder entityTypeBuilder, params string[] propertyNames)
	    {
		    return entityTypeBuilder.HasIndex(propertyNames).IsUnique();
	    }
    }
}
