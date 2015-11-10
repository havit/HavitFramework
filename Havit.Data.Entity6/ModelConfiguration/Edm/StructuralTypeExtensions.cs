using System;
using System.Data.Entity.Core.Metadata.Edm;
using Havit.Data.Entity.Annotations;

namespace Havit.Data.Entity.ModelConfiguration.Edm
{
	internal static class StructuralTypeExtensions
	{
		internal static bool IsConventionSuppressed(this StructuralType structuralType, Type conventionType)
		{
			return SuppressConventionAnnotation.IsConventionSuppressed(structuralType, conventionType);
		}
	}
}
