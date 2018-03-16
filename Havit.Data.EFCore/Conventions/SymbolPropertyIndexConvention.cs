using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Metadata.Conventions.Internal;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace Havit.Data.Entity.Conventions
{
	/// <summary>
	/// Pokud obsahuje třída vlastnost Symbol, je k ní automaticky vytvořen unikátní index.
	/// </summary>
    public class SymbolPropertyIndexConvention : IPropertyAddedConvention
    {
		/// <inheritdoc />
	    public InternalPropertyBuilder Apply(InternalPropertyBuilder propertyBuilder)
	    {
			// TODO JK: Suppress

		    if (propertyBuilder.Metadata.Name == "Symbol")
		    {
			    propertyBuilder.Metadata.DeclaringEntityType.Builder.HasIndex(new List<Property> { propertyBuilder.Metadata }, ConfigurationSource.Convention);
		    }

		    return propertyBuilder;
	    }
    }
}
