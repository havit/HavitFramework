using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Havit.Data.EFCore.Metadata.Builders
{
    public static class EntityTypeBuilderExtensions 	
    {
	    public static IndexBuilder HasUniqueIndex<TEntity>(this EntityTypeBuilder<TEntity> entityTypeBuilder, Expression<Func<TEntity, object>> indexExpression)
		    where TEntity : class
	    {
		    return entityTypeBuilder.HasIndex(indexExpression).IsUnique();
	    }

	    public static IndexBuilder HasUniqueIndex(this EntityTypeBuilder entityTypeBuilder, params string[] propertyNames)
	    {
		    return entityTypeBuilder.HasIndex(propertyNames).IsUnique();
	    }
    }
}
