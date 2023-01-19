using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Havit.Data.EntityFrameworkCore.Migrations.ModelExtensions
{
	/// <summary>
	/// Base implementation of <see cref="IModelExtensionAnnotationProvider"/> that provides strongly-typed implementation of methods for inherited classes to use.
	/// </summary>
	/// <typeparam name="T">Type of <see cref="IModelExtension"/> that this <see cref="IModelExtensionAnnotationProvider"/> implementation should handle.</typeparam>
	public abstract class ModelExtensionAnnotationProvider<T> : IModelExtensionAnnotationProvider
		where T : IModelExtension
	{
		List<IAnnotation> IModelExtensionAnnotationProvider.GetAnnotations(IModelExtension dbAnnotation, MemberInfo memberInfo) =>
			dbAnnotation is T dbAnnotationT ? GetAnnotations(dbAnnotationT, memberInfo) : new List<IAnnotation>();

		List<IModelExtension> IModelExtensionAnnotationProvider.GetModelExtensions(List<IAnnotation> annotations) =>
			GetModelExtensions(annotations).Cast<IModelExtension>().ToList();

		/// <summary>
		/// Translates <see cref="IModelExtension"/> to a list of <see cref="IAnnotation"/>s.
		/// </summary>
		/// <param name="dbAnnotation"><see cref="IModelExtension"/> of type <typeparamref name="T"/> to translate.</param>
		/// <param name="memberInfo"><see cref="MemberInfo"/> from which <see cref="IModelExtension"/> was created.</param>
		/// <returns>A list of <see cref="IAnnotation"/>s.</returns>
		protected abstract List<IAnnotation> GetAnnotations(T dbAnnotation, MemberInfo memberInfo);

		/// <summary>
		/// Translates a list of <see cref="IAnnotation"/> to a list of <see cref="IModelExtension"/>s. Implementation should ignore unknown <see cref="IAnnotation"/>s.
		/// </summary>
		/// <param name="annotations">A list of <see cref="IAnnotation"/> to translate.</param>
		/// <returns>A list of <see cref="IModelExtension"/>s of type <typeparamref name="T"/>. Cannot be null.</returns>
		protected abstract List<T> GetModelExtensions(List<IAnnotation> annotations);
	}
}