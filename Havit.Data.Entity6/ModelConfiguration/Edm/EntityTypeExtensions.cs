using System.Data.Entity.Core.Metadata.Edm;
using System.Reflection;

namespace Havit.Data.Entity.ModelConfiguration.Edm;

internal static class EntityTypeExtensions
{
	public static Type GetClrType(this EntityType entityType)
	{
		object result = Type.GetType("System.Data.Entity.ModelConfiguration.Edm.EntityTypeExtensions, EntityFramework")
			.GetMethod("GetClrType", BindingFlags.Static | BindingFlags.Public)
			.Invoke(null, new object[] { entityType });
		return (Type)result;
	}
}
