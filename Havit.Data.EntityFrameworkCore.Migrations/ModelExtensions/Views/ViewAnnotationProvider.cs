using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Havit.Data.EntityFrameworkCore.Migrations.ModelExtensions.Views
{
	/// <summary>
	/// Implementation of <see cref="IModelExtensionAnnotationProvider"/>, that handles <see cref="ViewModelExtension"/>s.
	/// </summary>
	public class ViewAnnotationProvider : ModelExtensionAnnotationProvider<ViewModelExtension>
	{
		private const string AnnotationPrefix = "View:";

        /// <inheritdoc />
        protected override List<IAnnotation> GetAnnotations(ViewModelExtension modelExtension, MemberInfo memberInfo)
		{
			return new List<IAnnotation>
			{
				new Annotation($"{AnnotationPrefix}{memberInfo.Name}:{modelExtension.ViewName}", modelExtension.CreateSql)
			};
		}

        /// <inheritdoc />
        protected override List<ViewModelExtension> GetModelExtensions(List<IAnnotation> annotations)
		{
			var spAnnotations = annotations.Where(annotation => annotation.Name.StartsWith(AnnotationPrefix));

			return spAnnotations.Select(annotation => new ViewModelExtension
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