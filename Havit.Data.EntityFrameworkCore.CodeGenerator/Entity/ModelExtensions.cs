using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Havit.Data.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Havit.Data.EntityFrameworkCore.CodeGenerator.Entity
{
    public static class ModelExtensions
    {
	    public static IEnumerable<IEntityType> GetApplicationEntityTypes(this IModel model)
	    {
		    return model
			    .GetEntityTypes()
			    .Where(entityType => !entityType.IsSystemEntity())
			    .Where(entityType => !entityType.IsManyToManyEntity());
	    }
    }
}
