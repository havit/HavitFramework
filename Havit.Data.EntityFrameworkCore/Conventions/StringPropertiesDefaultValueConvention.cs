using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Havit.Data.EntityFrameworkCore.Metadata;

namespace Havit.Data.EntityFrameworkCore.Conventions
{
	/// <summary>
	/// Všechny stringové vlastnosti jsou označeny jako povinné s výchozí hodnotou <see cref="string.Empty"/>.
	/// </summary>
    public class StringPropertiesDefaultValueConvention : IModelConvention
    {
	    /// <summary>
	    /// Aplikuje konvenci.
	    /// </summary>
		public void Apply(ModelBuilder modelBuilder)
	    {
		    var stringProperties = modelBuilder.Model
				.GetApplicationEntityTypes()
				.WhereNotConventionSuppressed(this) // testujeme entity types
				.SelectMany(entityType => entityType.GetDeclaredProperties()
				.WhereNotConventionSuppressed(this) // testujeme properties
				.Where(prop => prop.ClrType == typeof(String))).ToList();

		    foreach (IMutableProperty stringProperty in stringProperties)
		    {
				if ((stringProperty.GetDefaultValue() == null) && String.IsNullOrEmpty(stringProperty.GetDefaultValueSql()))
			    {
				    stringProperty.SetDefaultValue(String.Empty);
					stringProperty.ValueGenerated = Microsoft.EntityFrameworkCore.Metadata.ValueGenerated.Never; // https://stackoverflow.com/questions/40655968/how-to-force-default-values-in-an-insert-with-entityframework-core
				}
			}
	    }
    }
}
