using System.Collections.Generic;
using System.Reflection;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Havit.Data.EntityFrameworkCore.Migrations.ModelExtensions
{
	/// <summary>
	/// Service for translating <see cref="IModelExtension"/> to <see cref="IAnnotation"/> and vice-versa.
	/// </summary>
	public interface IModelExtensionAnnotationProvider
	{
		/// <summary>
		/// Translates <see cref="IModelExtension"/> to a list of <see cref="IAnnotation"/>s.
		/// </summary>
		/// <param name="dbAnnotation"><see cref="IModelExtension"/> to translate.</param>
		/// <param name="memberInfo"><see cref="MemberInfo"/> from which <see cref="IModelExtension"/> was created.</param>
		/// <returns>A list of <see cref="IAnnotation"/>s.</returns>
		List<IAnnotation> GetAnnotations(IModelExtension dbAnnotation, MemberInfo memberInfo);

		/// <summary>
		/// Translates a list of <see cref="IAnnotation"/> to a list of <see cref="IModelExtension"/>s. Implementation should ignore unknown <see cref="IAnnotation"/>s.
		/// </summary>
		/// <param name="annotations">A list of <see cref="IAnnotation"/> to translate.</param>
		/// <returns>A list of <see cref="IModelExtension"/>s. Cannot be null.</returns>
		List<IModelExtension> GetModelExtensions(List<IAnnotation> annotations);
	}
}