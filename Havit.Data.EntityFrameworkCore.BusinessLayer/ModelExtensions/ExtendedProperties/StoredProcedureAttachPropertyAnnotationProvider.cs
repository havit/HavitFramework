using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Havit.Data.EntityFrameworkCore.BusinessLayer.ExtendedProperties;
using Havit.Data.EntityFrameworkCore.BusinessLayer.ModelExtensions.ExtendedProperties.Attributes;
using Havit.Data.EntityFrameworkCore.Migrations.ModelExtensions;
using Havit.Data.EntityFrameworkCore.Migrations.ModelExtensions.StoredProcedures;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Havit.Data.EntityFrameworkCore.BusinessLayer.ModelExtensions.ExtendedProperties;

public class StoredProcedureAttachPropertyAnnotationProvider : ModelExtensionAnnotationProvider<StoredProcedureModelExtension>
{
	protected override List<IAnnotation> GetAnnotations(StoredProcedureModelExtension dbAnnotation, MemberInfo memberInfo)
	{
		string attachedEntityName = GetAttachedEntityName(memberInfo);
		if (attachedEntityName != null)
		{
			return ExtendedPropertiesForExtraDatabaseObjectsBuilder.ForProcedure(new Dictionary<string, string>()
			{
				{ "Attach", attachedEntityName }
			}, dbAnnotation.ProcedureName).ToList();
		}

		return new List<IAnnotation>();
	}

	protected override List<StoredProcedureModelExtension> GetModelExtensions(List<IAnnotation> annotations)
	{
		return new List<StoredProcedureModelExtension>();
	}

	private static string GetAttachedEntityName(MemberInfo method)
	{
		AttachAttribute attachAttribute = method.DeclaringType.GetCustomAttributes<AttachAttribute>().FirstOrDefault();

		return attachAttribute?.EntityName;
	}
}