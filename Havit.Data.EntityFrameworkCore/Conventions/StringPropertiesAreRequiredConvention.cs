using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace Havit.Data.EntityFrameworkCore.Conventions
{
	/// <summary>
	/// Všechny stringové vlastnosti jsou označeny jako povinné s výchozí hodnotou <see cref="string.Empty"/>.
	/// </summary>
    public class StringPropertiesAreRequiredConvention : IModelConvention
    {
	    /// <summary>
	    /// Aplikuje konvenci.
	    /// </summary>
		public void Apply(ModelBuilder modelBuilder)
	    {
		    var stringProperties = modelBuilder.Model.GetEntityTypes().SelectMany(e => e.GetDeclaredProperties().Where(prop => prop.ClrType == typeof(String))).ToList();

		    foreach (IMutableProperty stringProperty in stringProperties)
		    {
			    stringProperty.IsNullable = false;

				if ((stringProperty.Relational().DefaultValue == null) && String.IsNullOrEmpty(stringProperty.Relational().DefaultValueSql))
			    {
				    stringProperty.Relational().DefaultValue = "";
			    }
		    }
	    }
    }
}
