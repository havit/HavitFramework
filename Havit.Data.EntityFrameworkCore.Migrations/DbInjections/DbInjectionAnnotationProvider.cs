using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Havit.Data.EntityFrameworkCore.Migrations.DbInjections
{
	/// <summary>
	/// Base implementation of <see cref="IDbInjectionAnnotationProvider"/> that provides strongly-typed implementation of methods for inherited classes to use.
	/// </summary>
	/// <typeparam name="T">Type of <see cref="IDbInjection"/> that this <see cref="IDbInjectionAnnotationProvider"/> implementation should handle.</typeparam>
	public abstract class DbInjectionAnnotationProvider<T> : IDbInjectionAnnotationProvider
		where T : IDbInjection
	{
        List<IAnnotation> IDbInjectionAnnotationProvider.GetAnnotations(IDbInjection dbAnnotation, MemberInfo memberInfo) => 
			dbAnnotation is T dbAnnotationT ? GetAnnotations(dbAnnotationT, memberInfo) : new List<IAnnotation>();

        List<IDbInjection> IDbInjectionAnnotationProvider.GetDbInjections(List<IAnnotation> annotations) => 
			GetDbInjections(annotations).Cast<IDbInjection>().ToList();

        /// <summary>
        /// Translates <see cref="IDbInjection"/> to a list of <see cref="IAnnotation"/>s.
        /// </summary>
        /// <param name="dbAnnotation"><see cref="IDbInjection"/> of type <typeparamref name="T"/> to translate.</param>
        /// <param name="memberInfo"><see cref="MemberInfo"/> from which <see cref="IDbInjection"/> was created.</param>
        /// <returns>A list of <see cref="IAnnotation"/>s.</returns>
        protected abstract List<IAnnotation> GetAnnotations(T dbAnnotation, MemberInfo memberInfo);

        /// <summary>
        /// Translates a list of <see cref="IAnnotation"/> to a list of <see cref="IDbInjection"/>s. Implementation should ignore unknown <see cref="IAnnotation"/>s.
        /// </summary>
        /// <param name="annotations">A list of <see cref="IAnnotation"/> to translate.</param>
        /// <returns>A list of <see cref="IDbInjection"/>s of type <typeparamref name="T"/>. Cannot be null.</returns>
        protected abstract List<T> GetDbInjections(List<IAnnotation> annotations);
	}
}