using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Havit.Data.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore;

namespace Havit.Data.EntityFrameworkCore.Conventions
{
	public class ManyToManyKeyDiscoverConvention : IModelConvention
	{
		public void Apply(ModelBuilder modelBuilder)
		{
			foreach (var manyToManyCandidateEntityType in modelBuilder.Model
				.GetApplicationEntityTypes()
				.WhereNotConventionSuppressed(typeof(ManyToManyKeyDiscoverConvention)) // testujeme entity types
				.Where(entityType => entityType.FindPrimaryKey() == null)
				.Where(entityType => entityType.HasExactlyTwoPropertiesWhichAreAlsoForeignKeys()))
			{
				manyToManyCandidateEntityType.SetPrimaryKey(manyToManyCandidateEntityType.GetProperties().ToList().AsReadOnly());
			}
		}
	}
}
