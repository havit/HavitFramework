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
			string tableName = ParseTableName(dbAnnotation.ProcedureName);
			if (!String.IsNullOrEmpty(tableName))
			{
				return new List<IAnnotation>
				{
					new Annotation($"ExtendedProperty:PROCEDURE:{dbAnnotation.ProcedureName}:Attach", tableName)
				};
			}

			return new List<IAnnotation>();
		}

		protected override List<StoredProcedureDbInjection> GetDbInjections(List<IAnnotation> annotations)
		{
			return new List<StoredProcedureDbInjection>();
		}

		private string ParseTableName(string procedureName)
		{
			var split = procedureName.Split('_');
			return split.Length == 1 ? null : split[0];
		}
	}
}