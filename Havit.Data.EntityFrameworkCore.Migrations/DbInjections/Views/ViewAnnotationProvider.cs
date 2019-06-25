using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Havit.Data.EntityFrameworkCore.Migrations.DbInjections.Views
{
	/// <summary>
	/// Implementation of <see cref="IDbInjectionAnnotationProvider"/>, that handles <see cref="ViewDbInjection"/>s.
	/// </summary>
	public class ViewAnnotationProvider : DbInjectionAnnotationProvider<ViewDbInjection>
	{
		private const string AnnotationPrefix = "View:";

        /// <inheritdoc />
        protected override List<IAnnotation> GetAnnotations(ViewDbInjection dbInjection, MemberInfo memberInfo)
		{
			return new List<IAnnotation>
			{
				new Annotation($"{AnnotationPrefix}{memberInfo.Name}:{dbInjection.ViewName}", dbInjection.CreateSql)
			};
		}

        /// <inheritdoc />
        protected override List<ViewDbInjection> GetDbInjections(List<IAnnotation> annotations)
		{
			var spAnnotations = annotations.Where(annotation => annotation.Name.StartsWith(AnnotationPrefix));

			return spAnnotations.Select(annotation => new ViewDbInjection
			{
				CreateSql = (string)annotation.Value,
				ViewName = ParseViewName(annotation)
			}).ToList();
		}

		private string ParseViewName(IAnnotation annotation)
		{
			return annotation.Name.Split(':').Last();
		}
	}
}