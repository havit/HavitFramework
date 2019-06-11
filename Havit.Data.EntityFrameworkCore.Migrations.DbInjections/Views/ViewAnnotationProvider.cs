using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Havit.Data.EntityFrameworkCore.Migrations.DbInjections.Views
{
	public class ViewAnnotationProvider : DbInjectionAnnotationProvider<ViewDbInjection>
	{
		private const string AnnotationPrefix = "View:";

		protected override List<IAnnotation> GetAnnotations(ViewDbInjection dbInjection, MemberInfo memberInfo)
		{
			return new List<IAnnotation>
			{
				new Annotation($"{AnnotationPrefix}{memberInfo.Name}:{dbInjection.ViewName}", dbInjection.CreateSql)
			};
		}

		protected override List<ViewDbInjection> GetDbInjections(List<IAnnotation> annotations)
		{
			var spAnnotations = annotations.Where(annotation => annotation.Name.StartsWith(AnnotationPrefix));

			return spAnnotations.Select(annotation => new ViewDbInjection
			{
				CreateSql = (string)annotation.Value,
				ViewName = ParseProcedureName(annotation)
			}).ToList();
		}

		private string ParseProcedureName(IAnnotation annotation)
		{
			return annotation.Name.Split(':').Last();
		}
	}
}