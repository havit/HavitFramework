using System;
using Microsoft.EntityFrameworkCore.Metadata.Conventions.Internal;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace Havit.Data.Entity.Conventions
{
	/// <summary>
	/// Všechny stringové vlastnosti jsou označeny jako povinné s výchozí hodnotou <see cref="string.Empty"/>.
	/// </summary>
    public class StringPropertiesAreRequiredConvention : IPropertyAddedConvention
    {
		/// <inheritdoc />
	    public InternalPropertyBuilder Apply(InternalPropertyBuilder propertyBuilder)
	    {
		    if (propertyBuilder.Metadata.ClrType == typeof(string))
		    {
			    propertyBuilder.IsRequired(true, ConfigurationSource.Convention);
			    propertyBuilder.Relational(ConfigurationSource.Convention).HasDefaultValue(String.Empty);
		    }

		    return propertyBuilder;
	    }
    }
}
