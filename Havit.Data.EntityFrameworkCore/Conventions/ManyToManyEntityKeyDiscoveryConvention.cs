using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Havit.Data.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore;

namespace Havit.Data.EntityFrameworkCore.Conventions
{
	/// <summary>
	/// Konvence vyhledá kandidáty na entity reprezentující vztah ManyToMany bez nastaveného primárního klíče a nastaví složený primární klíč.
	/// </summary>
	public class ManyToManyEntityKeyDiscoveryConvention : IModelConvention
	{
		/// <inheritdoc />
		public void Apply(ModelBuilder modelBuilder)
		{
			foreach (var manyToManyCandidateEntityType in modelBuilder.Model
				.GetApplicationEntityTypes()
				.WhereNotConventionSuppressed(typeof(ManyToManyEntityKeyDiscoveryConvention)) // testujeme entity types
				.Where(entityType => entityType.FindPrimaryKey() == null)
				.Where(entityType => entityType.HasExactlyTwoPropertiesWhichAreAlsoForeignKeys()))
			{
				manyToManyCandidateEntityType.SetPrimaryKey(manyToManyCandidateEntityType.GetProperties().ToList().AsReadOnly());
			}
		}
	}
}
