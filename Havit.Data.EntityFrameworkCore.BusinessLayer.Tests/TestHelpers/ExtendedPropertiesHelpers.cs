using System.Collections.Generic;
using Havit.Data.EntityFrameworkCore.BusinessLayer.ExtendedProperties;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Havit.Data.EntityFrameworkCore.BusinessLayer.Tests.TestHelpers
{
	public static class ExtendedPropertiesHelpers
	{
		public static EntityTypeBuilder AddExtendedProperty(this EntityTypeBuilder entityTypeBuilder, string name, string value)
		{
			entityTypeBuilder.Metadata.AddExtendedProperties(new Dictionary<string, string> { { name, value } });
			return entityTypeBuilder;
		}

		public static PropertyBuilder AddExtendedProperty(this PropertyBuilder propertyBuilder, string name, string value)
		{
			propertyBuilder.Metadata.AddExtendedProperties(new Dictionary<string, string> { { name, value } });
			return propertyBuilder;
		}
	}
}