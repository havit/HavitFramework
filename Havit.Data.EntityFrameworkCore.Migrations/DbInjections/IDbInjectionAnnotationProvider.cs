using System.Collections.Generic;
using System.Reflection;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Havit.Data.EntityFrameworkCore.Migrations.DbInjections
{
    /// <summary>
    /// Service for translating <see cref="IDbInjection"/> to <see cref="IAnnotation"/> and vice-versa.
    /// </summary>
    public interface IDbInjectionAnnotationProvider
    {
        /// <summary>
        /// Translates <see cref="IDbInjection"/> to a list of <see cref="IAnnotation"/>s.
        /// </summary>
        /// <param name="dbAnnotation"><see cref="IDbInjection"/> to translate.</param>
        /// <param name="memberInfo"><see cref="MemberInfo"/> from which <see cref="IDbInjection"/> was created.</param>
        /// <returns>A list of <see cref="IAnnotation"/>s.</returns>
        List<IAnnotation> GetAnnotations(IDbInjection dbAnnotation, MemberInfo memberInfo);

        /// <summary>
        /// Translates a list of <see cref="IAnnotation"/> to a list of <see cref="IDbInjection"/>s. Implementation should ignore unknown <see cref="IAnnotation"/>s.
        /// </summary>
        /// <param name="annotations">A list of <see cref="IAnnotation"/> to translate.</param>
        /// <returns>A list of <see cref="IDbInjection"/>s. Cannot be null.</returns>
        List<IDbInjection> GetDbInjections(List<IAnnotation> annotations);
    }
}