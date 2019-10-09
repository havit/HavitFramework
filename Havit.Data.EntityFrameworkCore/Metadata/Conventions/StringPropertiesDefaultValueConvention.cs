using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Havit.Data.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Havit.Data.EntityFrameworkCore.Metadata.Conventions.Infrastructure;

namespace Havit.Data.EntityFrameworkCore.Metadata.Conventions
{
	/// <summary>
	/// Všechny stringové vlastnosti jsou označeny jako povinné s výchozí hodnotou <see cref="string.Empty"/>.
	/// </summary>
    public class StringPropertiesDefaultValueConvention : IPropertyAddedConvention
    {
		/// <inheritdoc />
		public void ProcessPropertyAdded(IConventionPropertyBuilder propertyBuilder, IConventionContext<IConventionPropertyBuilder> context)
		{
			IConventionProperty property = propertyBuilder.Metadata;
			
			// Systémové tabulky nechceme změnit.
			if (property.DeclaringEntityType.IsSystemType())
			{
				return;
			}

			if (property.DeclaringEntityType.IsConventionSuppressed<StringPropertiesDefaultValueConvention>()
				|| (property.IsConventionSuppressed<StringPropertiesDefaultValueConvention>()))
			{
				return;
			}

			if (property.ClrType == typeof(String))
			{				
				if ((property.GetDefaultValue() == null) && String.IsNullOrEmpty(property.GetDefaultValueSql()))
				{
					// fromDataAnnotation: false - není definováno v modelu
					propertyBuilder.HasDefaultValue(String.Empty, fromDataAnnotation: false);
					propertyBuilder.ValueGenerated(Microsoft.EntityFrameworkCore.Metadata.ValueGenerated.Never, fromDataAnnotation: false); // https://stackoverflow.com/questions/40655968/how-to-force-default-values-in-an-insert-with-entityframework-core
				}
			}
		}
	}
}
