using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Havit.Business.CodeMigrations.DbInjections.ExtendedProperties.Attributes;
using Havit.Business.CodeMigrations.DbInjections.StoredProcedures;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Havit.Business.CodeMigrations.DbInjections.ExtendedProperties
{
	public class StoredProcedureAttachPropertyAnnotationProvider : DbInjectionAnnotationProvider<StoredProcedureDbInjection>
	{
		protected override List<IAnnotation> GetAnnotations(StoredProcedureDbInjection dbAnnotation, MemberInfo memberInfo)
		{
			string attachedEntityName = GetAttachedEntityName(memberInfo);
			if (attachedEntityName != null)
			{
				return new List<IAnnotation>
				{
					new Annotation($"ExtendedProperty:PROCEDURE:{dbAnnotation.ProcedureName}:Attach", attachedEntityName)
				};
			}

			return new List<IAnnotation>();
		}

		protected override List<StoredProcedureDbInjection> GetDbInjections(List<IAnnotation> annotations)
		{
			return new List<StoredProcedureDbInjection>();
		}

		private static string GetAttachedEntityName(MemberInfo method)
		{
			AttachAttribute attachAttribute = method.DeclaringType.GetCustomAttributes<AttachAttribute>().FirstOrDefault();

			return attachAttribute?.EntityName;
		}
	}
}