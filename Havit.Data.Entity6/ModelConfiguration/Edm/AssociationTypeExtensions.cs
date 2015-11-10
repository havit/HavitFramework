using System;
using System.Data.Entity.Core.Metadata.Edm;
using System.Reflection;

namespace Havit.Data.Entity.ModelConfiguration.Edm
{
	internal static class AssociationTypeExtensions
	{
		public static AssociationEndMember GetSourceEnd(this AssociationType associationType)
		{
			return (AssociationEndMember)typeof(AssociationType).GetProperty("SourceEnd", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(associationType);
		}

		public static AssociationEndMember GetTargetEnd(this AssociationType associationType)
		{
			return (AssociationEndMember)typeof(AssociationType).GetProperty("TargetEnd", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(associationType);
		}

		public static bool IsManyToMany(this AssociationType associationType)
		{
			object result = Type.GetType("System.Data.Entity.ModelConfiguration.Edm.AssociationTypeExtensions, EntityFramework")
				.GetMethod("IsManyToMany", BindingFlags.Static | BindingFlags.Public)
				.Invoke(null, new object[] { associationType });
			return (bool)result;
		}
	}
}
