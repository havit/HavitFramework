using System;
using System.Collections.Generic;
using System.Reflection;
using Havit.Business.CodeMigrations.DbInjections.StoredProcedures;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Havit.Business.CodeMigrations.DbInjections.ExtendedProperties
{
	public class StoredProcedureAttachPropertyAnnotationProvider : DbInjectionAnnotationProvider<StoredProcedureDbInjection>
	{
		protected override List<IAnnotation> GetAnnotations(StoredProcedureDbInjection dbAnnotation, MemberInfo memberInfo)
		{
			Type attachedType = GetAttachedType(memberInfo);
			if (attachedType != null)
			{
				return new List<IAnnotation>
				{
					new Annotation($"ExtendedProperty:PROCEDURE:{dbAnnotation.ProcedureName}:Attach", attachedType.Name)
				};
			}

			return new List<IAnnotation>();
		}

		protected override List<StoredProcedureDbInjection> GetDbInjections(List<IAnnotation> annotations)
		{
			return new List<StoredProcedureDbInjection>();
		}

		private static Type GetAttachedType(MemberInfo method)
		{
			Type baseType = method.DeclaringType.BaseType;
			if (baseType?.GetGenericTypeDefinition() != typeof(StoredProcedureDbInjector<>))
			{
				return null;
			}

			return baseType.GetGenericArguments()[0];
		}
	}
}